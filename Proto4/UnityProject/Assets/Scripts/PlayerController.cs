using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
	[Header("Jump")]
	private float m_currJumpCharge = 4f;
	[SerializeField] private float m_jumpChargeRate = 5f;
	[SerializeField] private float m_maxJumpForce = 30f;
	[SerializeField] private float m_minJumpForce = 4f;
	[Header("Move")]
	[SerializeField] private float m_currMoveSpd = 6f;
	[SerializeField] private float m_moveChangeRate = 2f;
	[SerializeField] private float m_maxMoveSpd = 16f;
	[SerializeField] private float m_minMoveSpd = 2f;
	[Header("Attack")]
	private float m_attackRange = 1.5f;
	[SerializeField] private float m_maxAttackRange = 1.5f;
	[SerializeField] private float m_minAttackRange = 0.2f;
	[SerializeField] private float m_attackRangeRate = .1f;

	[Header("VFX Values")]
	[SerializeField] private float defaultLevelHeight = 8.5f;
	[SerializeField] private float defaultplayerHeight = -4.37f;
	[SerializeField] private float terminalVelocity = 30f;
	[SerializeField] private Color safeColor = new Color(56f / 255f, 241f / 255f, 60f / 255f);
	[SerializeField] private Color dangerColor = new Color(204f / 255f, 32f / 255f, 32f / 255f);

	// vars for state and other book-keeping
	bool m_inAir = true;
	bool m_jumpPressed = false;
	bool m_attackPressed = false;
	bool m_attackHeld = false;
	bool m_attackReleased = false;
	bool m_rightHeld = false;
	bool m_leftHeld = false;
	bool canMoveRight = true;
	bool canMoveLeft = true;
	bool dead = false;
	bool _didKill = false;

	// component references
	Rigidbody2D m_rb = null;
	BoxCollider2D m_coll = null;
	GameObject m_sword = null;
	SpriteRenderer m_renderer = null;

	//SFX
	[Header("SFX Values")]
	[SerializeField] private AudioSource attackSFX;
	[SerializeField] private AudioSource deathSFX;
	[SerializeField] private AudioSource highjumpSFX;
	[SerializeField] private AudioSource lowjumpSFX;
	[SerializeField] private AudioSource killSFX;
	[SerializeField] private AudioSource speedUpSFX;
	[SerializeField] private AudioSource slowDownSFX;

	// particle effects
	[Header("Particle Effects")]
	[SerializeField] ParticleSystem _speedUpParticles;
	[SerializeField] ParticleSystem _slowDownParticles;
	[SerializeField] ParticleSystem _rangeUpParticles;
	[SerializeField] ParticleSystem _rangeDownParticles;

	// Start is called before the first frame update
    void Start() {
		m_rb = GetComponent<Rigidbody2D>();
		m_coll = GetComponent<BoxCollider2D>();
		m_sword = GameObject.FindGameObjectWithTag("Sword");
		m_renderer = GetComponent<SpriteRenderer>();
		m_attackRange = m_maxAttackRange;
    }

    // Update is called once per frame
    void Update() {
		if (dead)
			return;

		// increase jump charge
		m_currJumpCharge = Mathf.Clamp(m_currJumpCharge + Time.deltaTime * m_jumpChargeRate,
										m_minJumpForce, m_maxJumpForce);

		// draw charge indicator
		drawJumpIndicator();

		// check if walking off platform (no jumping in air)
		if (!m_inAir && m_rb.velocity.y < 0)
			m_inAir = true;

		// poll player input
		pollInput();

		// process jump
        processJump();

		// process attack
		processAttack();

		// process movement
		processMovement();

		// set sword scale based on the attack range
		m_sword.transform.localScale = new Vector3(m_sword.transform.localScale.x, m_attackRange, m_sword.transform.localScale.z);
	}

	// polls player input
	void pollInput() {
		m_jumpPressed = (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W));
		m_rightHeld = (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D));
		m_leftHeld = (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A));
		m_attackHeld = (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.RightShift));
		m_attackPressed = (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.RightShift));
		m_attackReleased = (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.RightShift));
	}

	// make player jump with height based on jump charge
	void processJump() {
		// only jump if input pressed while on ground
		if (m_inAir || !m_jumpPressed)
			return;

		//SFX
		if(m_currJumpCharge >= 20f)
        {
			if (highjumpSFX)
				highjumpSFX.Play();
        }
        else
        {
			if (lowjumpSFX)
				lowjumpSFX.Play();
        }
		// apply jump force
		m_rb.velocity = new Vector2(m_rb.velocity.x, m_currJumpCharge);
		// mark self off ground
		m_inAir = true;
		m_jumpPressed = false;
		// reset jump charge
		m_currJumpCharge = m_minJumpForce;

		// if we arent at lowest range, decrease range
		if (m_attackRange > m_minAttackRange) {
			// show sword decrease range effect
			if (_rangeDownParticles)
			_rangeDownParticles.Play();
			// reduce the attack range
			m_attackRange = Mathf.Clamp(m_attackRange - m_attackRangeRate, m_minAttackRange, m_attackRange);
			//m_sword.transform.localScale = new Vector3(m_sword.transform.localScale.x, m_attackRange, m_sword.transform.localScale.z);
		}
	}

	// make player attack and affect other stats
	void processAttack() {
		// move sword up when not active
		if (!m_attackHeld) {
			m_sword.transform.rotation = Quaternion.identity;
			m_sword.transform.position = transform.position + new Vector3(0, m_coll.bounds.extents.y, 0);
		}
		// when sword pressed, move it on front of player
		if (m_attackHeld) {
			m_sword.transform.rotation = Quaternion.AngleAxis(-90f * Mathf.Sign(transform.localScale.x), Vector3.forward);
			m_sword.transform.position = transform.position + new Vector3(m_coll.bounds.extents.x * Mathf.Sign(transform.localScale.x), 0, 0);
		}
		// when pressed, the jump height resets, and speed reduces
		if (m_attackPressed) {
			// reset kill check
			_didKill = false;
			if (attackSFX) {
				attackSFX.Play();
            }
			/*if (_slowDownParticles) {
				_slowDownParticles.Play();
			}
			m_currMoveSpd = Mathf.Clamp(m_currMoveSpd - m_moveChangeRate, m_minMoveSpd, m_maxMoveSpd);*/
			m_currJumpCharge = m_minJumpForce;
		}
		// when attack released, apply slowdown if no enemies killed
		if (m_attackReleased && !_didKill) {
			if (_slowDownParticles) {
				_slowDownParticles.Play();
			}
			m_currMoveSpd = Mathf.Clamp(m_currMoveSpd - m_moveChangeRate, m_minMoveSpd, m_maxMoveSpd);
			// TODO: add slow-down sound effect
			if (slowDownSFX)
				slowDownSFX.Play();
		}
	}

	// make player move
	void processMovement() {
		short temp = 0;
		if (m_rightHeld)
			temp++;
		if (m_leftHeld)
			temp--;
		
		// turn facing based on input
		// if turning left
		if (temp < 0 && Mathf.Sign(transform.localScale.x) > 0) {
			transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
		}
		// if turning right
		else if (temp > 0 && Mathf.Sign(transform.localScale.x) < 0) {
			transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
		}

		// can't move if attacking
		if (m_attackHeld)
			return;

		// if moving left but were blocked, check if we are free now
		RaycastHit2D[] hits = new RaycastHit2D[1];
		if (temp < 0 && !canMoveLeft && m_coll.Cast(Vector2.left, hits, .1f) < 1)
			canMoveLeft = true;
		// if moving right but were blocked, check if we are free now
		if (temp > 0 && !canMoveRight && m_coll.Cast(Vector2.right, hits, .1f) < 1)
			canMoveRight = true;

		// get input based on if we can move left/right
		temp = 0;
		if (m_rightHeld && canMoveRight)
			temp++;
		if (m_leftHeld && canMoveLeft)
			temp--;

		// allow right-ward movement if was blocked
		if (temp < 0) {
			canMoveRight = true;
		}
		// allow left-ward movement if was blocked
		else if (temp > 0) {
			canMoveLeft = true;
		}

		// move left-right based on move speed and input
		m_rb.velocity = new Vector2(temp * m_currMoveSpd, m_rb.velocity.y);
	}

	// draw's indicator of how dangerous jump charge is
	void drawJumpIndicator() {
		float percentage = m_currJumpCharge / terminalVelocity;
		m_renderer.color = new Color((safeColor.r * (1 - percentage)) + (dangerColor.r * percentage),
									(safeColor.g * (1 - percentage)) + (dangerColor.g * percentage),
									(safeColor.b * (1 - percentage)) + (dangerColor.b * percentage));
	}

	// process special collision events
	private void OnCollisionEnter2D(Collision2D collision) {
		// if we collide with our sword, ignore it
		if (collision.gameObject.tag == "Sword")
			return;

		// check if we die
		if (collision.gameObject.tag == "Hazard" || collision.gameObject.tag == "Enemy") {
			if(deathSFX)
            {
				deathSFX.Play();
            }
			die();
			return;
		}


		var contact = collision.contacts[0];
		// check if we land on top of something; if we do, mark on ground and update terminal velocity
		if (m_inAir && contact.normal == Vector2.up) {
			m_inAir = false;
			setNewTerminalVel();
		}

		// if we are colliding with the ground, no horrizontal checks
		if (collision.gameObject.tag == "Ground")
			return;
		else if (m_rightHeld && canMoveRight && contact.normal == Vector2.left) {
			m_rb.velocity = new Vector2(0, m_rb.velocity.y);
			canMoveRight = false;
		}
		else if (m_leftHeld && canMoveLeft && contact.normal == Vector2.right) {
			m_rb.velocity = new Vector2(0, m_rb.velocity.y);
			canMoveLeft = false;
		}
	}

	// sets new terminal velocity based on where player is standing
	void setNewTerminalVel() {
		terminalVelocity = m_maxJumpForce - (m_maxJumpForce * (transform.position.y - defaultplayerHeight) / defaultLevelHeight);
	}

	// changes stats when a kill is done; to be called by the victims
	public void processKill() {
		// show sword effect of reseting range if the range is not max yet
		if (_rangeUpParticles && m_attackRange != m_maxAttackRange)
			_rangeUpParticles.Play();
		// reset attack range
		m_attackRange = m_maxAttackRange;
		//m_sword.transform.localScale = new Vector3(m_sword.transform.localScale.x, m_attackRange, m_sword.transform.localScale.z);
		// increase speed
		//m_currMoveSpd = Mathf.Clamp(m_currMoveSpd + (m_moveChangeRate * 2), m_currMoveSpd, m_maxMoveSpd);
		m_currMoveSpd = Mathf.Clamp(m_currMoveSpd + m_moveChangeRate, m_currMoveSpd, m_maxMoveSpd);
		// maek that we killed so that we don't slow down
		_didKill = true;
		// play sound
		if (killSFX)
			killSFX.Play();
		// show particle effects for speeding up
		if (!dead && _speedUpParticles) {
			if (_slowDownParticles && _slowDownParticles.isPlaying) {
				_slowDownParticles.Stop();
			}
			_speedUpParticles.Play();
			// play sound effect too
			if (speedUpSFX)
				speedUpSFX.Play();
		}
	}

	// die
	public void die() {
		// die; DIE!
		Debug.Log("Dead");
		Camera.main.GetComponent<CameraController>().playerDead = true;
		if (deathSFX)
			deathSFX.Play();
		Camera.main.GetComponent<CameraController>().showFinalScore();
		//GameObject.Destroy(gameObject);
		m_coll.enabled = false;
		m_rb.simulated = false;
		m_renderer.enabled = false;
		m_sword.GetComponent<SpriteRenderer>().enabled = false;
		dead = true;
		// turn off particles
		if (_rangeDownParticles) Destroy(_rangeDownParticles);
		if (_rangeUpParticles) Destroy(_rangeUpParticles);
		if (_speedUpParticles) Destroy(_speedUpParticles);
		if (_slowDownParticles) Destroy(_slowDownParticles);
	}

	// accessors
	//public bool isInAir() { return m_inAir; }
}
