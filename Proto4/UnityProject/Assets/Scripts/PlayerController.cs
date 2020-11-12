using System.Collections;
using System.Collections.Generic;
//using TreeEditor;
//using UnityEditor.Build;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	[Header("Jump")]

	[SerializeField] private GameObject m_JumpOverlayObject;
	[SerializeField] private JumpTrailObj[] m_JumpOverlayTrailObjs;
	
	[SerializeField] private float m_CurrentJumpForce = 4f;
	[SerializeField] private float m_JumpChargeRate = 6f;
	[SerializeField] private float m_MaxJumpForce = 30f;
	[SerializeField] private float m_MinJumpForce = 3f;
	[SerializeField] private float m_JumpRateIncrease = 5f;
	[SerializeField] private float m_JumpRateMaxIncrease = 30f;
	private float m_CurrentMaxJumpForce;
	private float m_InitialJumpChargeRate;

	[Header("Move")]
	
	[SerializeField] private float m_MaxSpeed = 6f;
	[SerializeField] private float m_TimeToReachMaxSpeed = 0.2f;
	//[SerializeField] private float m_MovementSpeedChangeRate = 5f;
	//[SerializeField] private float m_MaxMovementSpeed = 16f;
	//[SerializeField] private float m_MinMovementSpeed = 2f;
	
	[Header("Attack")]
	
	[SerializeField] private float m_AttackRange = 1.5f;
	[SerializeField] private float m_MaxAttackRange = 1.5f;
	[SerializeField] private float m_MinAttackRange = 0.2f;
	[SerializeField] private float m_AttackRangeRate = .1f;

	[Header("VFX Values")]
	
	[SerializeField] private float m_DefaultLevelHeight = 8.5f;
	[SerializeField] private float m_DefaultPlayerHeight = -4.37f;
	[SerializeField] private Color m_SafeColor = new Color(56f / 255f, 241f / 255f, 60f / 255f);
	[SerializeField] private Color m_DangerColor = new Color(204f / 255f, 32f / 255f, 32f / 255f);

    //SFX
    [Header("SFX Values")]

    [SerializeField] private AudioSource m_AttackSFX;
    [SerializeField] private AudioSource m_DeathSFX;
    [SerializeField] private AudioSource m_HighJumpSFX;
    [SerializeField] private AudioSource m_LowJumpSFX;
    [SerializeField] private AudioSource m_KillSFX;
    [SerializeField] private AudioSource m_SpeedUpSFX;
    [SerializeField] private AudioSource m_SlowDownSFX;

    // particle effects
    [Header("Particle Effects")]

    [SerializeField] private ParticleSystem m_SpeedUpParticles;
    [SerializeField] private ParticleSystem m_SlowDownParticles;
    [SerializeField] private ParticleSystem m_RangeUpParticles;
    [SerializeField] private ParticleSystem m_RangeDownParticles;

    // component references
    Rigidbody2D m_Rigidbody2D = null;
    //BoxCollider2D m_BoxCollider2D = null;
	CircleCollider2D m_CircleCollider2D = null;
	EdgeCollider2D m_EdgeCollider2D = null;
    GameObject m_SwordObject = null;
    SpriteRenderer m_SpriteRenderer = null;

	// vars for state and other book-keeping
	private bool m_InAir = true;
	private bool m_JumpPressed = false;
	private bool m_AttackPressed = false;
	private bool m_AttackHeld = false;
	private bool m_AttackReleased = false;
	private bool m_DownAttackPressed = false;
	private bool m_DownAttackHeld = false;
	private bool m_DownAttackReleased = false;
	private bool m_CanMoveRight = true;
	private bool m_CanMoveLeft = true;
	private bool m_IsDead = false;
	private bool m_DidKill = false;
	private bool m_ShouldMoveLeft = false;
	private bool m_ShouldMoveRight = false;

    //private float m_CurrentMaxJumpForce;
	//private float m_InitialJumpChargeRate;
	private float m_AccelerationRate;

	private const float MAX_Y_POSITION = 3.0f;
	private const float MIN_Y_POSITION = -3.38f;

	[SerializeField] SpriteRenderer m_LavaRenderer;
	[SerializeField] Material m_BurningMaterial;
	[SerializeField] Material m_DissolveMaterial;
	[SerializeField] float m_TimeTillDeath = .20f;
	float m_CurrDeathTime = 0f;
	bool m_IsDying = false;
	bool m_IsBurning = false;
	bool m_WasLookingLeft = false;
	Vector3 m_LastPositionAlive;

	// Start is called before the first frame update
	void Start() {
		m_Rigidbody2D = GetComponent<Rigidbody2D>();
		//m_BoxCollider2D = GetComponent<BoxCollider2D>();
		m_CircleCollider2D = GetComponent<CircleCollider2D>();
		m_EdgeCollider2D = GetComponent<EdgeCollider2D>();
		m_SwordObject = GameObject.FindGameObjectWithTag("Sword");
		m_SpriteRenderer = GetComponent<SpriteRenderer>();
		m_AttackRange = m_MaxAttackRange;
		m_CurrentMaxJumpForce = m_MaxJumpForce;
		m_InitialJumpChargeRate = m_JumpChargeRate;
		m_AccelerationRate = m_MaxSpeed / m_TimeToReachMaxSpeed;
    }

	private void FixedUpdate() {
		if (m_IsDead)
        {
			/*if(m_IsBurning)
            {
				transform.position = m_LastPositionAlive;
				m_CurrDeathTime += Time.deltaTime;
				float dissolveAmount = 0f;
				if(m_WasLookingLeft)
                {
					dissolveAmount = Mathf.Lerp(-2.5f, 3.5f, m_CurrDeathTime / m_TimeTillDeath);
				}
				else
                {
					dissolveAmount = Mathf.Lerp(3.5f, -2.5f, m_CurrDeathTime / m_TimeTillDeath);
				}
				//Debug.Log("dissolve amount: " + dissolveAmount);
				m_BurningMaterial.SetFloat("_DissolveAmount", dissolveAmount);
				if (m_CurrDeathTime >= m_TimeTillDeath)
				{
					m_CurrDeathTime = m_TimeTillDeath;
					m_SpriteRenderer.enabled = false;
					//Destroy(gameObject);
				}
			}*/
			return;
		}
		// checks for wall collisions
		UpdateAllowedMovementDirection();

		// check if walking off platform (no jumping in air)
		if (!m_InAir && m_Rigidbody2D.velocity.y < 0)
			m_InAir = true;
	}

	// Update is called once per frame
	void Update() {

		if (m_IsDead)
        {
			if (m_IsBurning)
			{
				transform.position = m_LastPositionAlive;
				m_CurrDeathTime += Time.deltaTime;
				float dissolveAmount = 0f;
				if (m_WasLookingLeft)
				{
					dissolveAmount = Mathf.Lerp(0f, 2.0f, m_CurrDeathTime / m_TimeTillDeath);
				}
				else
				{
					dissolveAmount = Mathf.Lerp(2.0f, 0f, m_CurrDeathTime / m_TimeTillDeath);
				}
				//Debug.Log("dissolve amount: " + dissolveAmount);
				m_BurningMaterial.SetFloat("_DissolveAmount", dissolveAmount);
				if (m_CurrDeathTime >= m_TimeTillDeath)
				{
					m_CurrDeathTime = m_TimeTillDeath;
					m_SpriteRenderer.enabled = false;
					//Destroy(gameObject);
				}
			}
			else
            {
				transform.position = m_LastPositionAlive;
				m_CurrDeathTime += Time.deltaTime;
				float dissolveAmount = Mathf.Lerp(0f, 0.8f, m_CurrDeathTime / m_TimeTillDeath);
				m_DissolveMaterial.SetFloat("_DissolveAmount", dissolveAmount);
				if (m_CurrDeathTime >= m_TimeTillDeath)
				{
					m_CurrDeathTime = m_TimeTillDeath;
					m_SpriteRenderer.enabled = false;
					//Destroy(gameObject);
				}
			}
			return;
		}

		// increase jump charge
		m_CurrentJumpForce = Mathf.Clamp(m_CurrentJumpForce + Time.deltaTime * m_JumpChargeRate,
										m_MinJumpForce, m_MaxJumpForce);

		// draw charge indicator
		DrawJumpIndicator();

		/*// check if walking off platform (no jumping in air)
		if (!m_InAir && m_Rigidbody2D.velocity.y < 0)
			m_InAir = true;*/

		CheckInput();
        ProcessJump();
		ProcessAttack();
		ProcessMovement();

		// set sword scale based on the attack range
		m_SwordObject.transform.localScale = new Vector3(m_SwordObject.transform.localScale.x, m_AttackRange, m_SwordObject.transform.localScale.z);
	}

	// polls player input
	void CheckInput() {

		m_JumpPressed = (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W));
		
		m_AttackHeld = (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.RightShift));
		m_AttackPressed = (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.RightShift));
		m_AttackReleased = (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.RightShift));

		m_DownAttackHeld = (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S));
		m_DownAttackPressed = (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S));
		m_DownAttackReleased = (Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.S));

		bool isRightCurrentlyHeld = (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D));
        bool isLeftCurrentlyHeld = (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A));

        m_ShouldMoveRight = isRightCurrentlyHeld && !isLeftCurrentlyHeld;
        m_ShouldMoveLeft = !isRightCurrentlyHeld && isLeftCurrentlyHeld;
    }

	private const float HIGH_JUMP_THRESHOLD = 20.0f;

	// make player jump with height based on jump charge
	void ProcessJump() {
		
		if (m_InAir || !m_JumpPressed)
			return;

		if(m_CurrentJumpForce >= HIGH_JUMP_THRESHOLD && m_HighJumpSFX) {
			m_HighJumpSFX.Play();
        } else if(m_LowJumpSFX) {
			m_LowJumpSFX.Play();
        }

		m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, m_CurrentJumpForce);
		
		m_InAir = true;
		m_JumpPressed = false;

		// reset jump charge
		m_CurrentJumpForce = m_MinJumpForce;

		// if we arent at lowest range, decrease range
		if (m_AttackRange > m_MinAttackRange) {

			// show sword decrease range effect
			if (m_RangeDownParticles)
				m_RangeDownParticles.Play();

			// reduce the attack range
			m_AttackRange = Mathf.Clamp(m_AttackRange - m_AttackRangeRate, m_MinAttackRange, m_AttackRange);
		}
	}

	// make player attack and affect other stats
	void ProcessAttack() {

		Quaternion swordRotation = (!m_AttackHeld)? Quaternion.identity: Quaternion.AngleAxis(-90f * Mathf.Sign(transform.localScale.x), Vector3.forward);
		m_SwordObject.transform.rotation = swordRotation;

		//Vector3 swordPositionOffset = (!m_AttackHeld)? new Vector3(0, m_BoxCollider2D.bounds.extents.y, 0): new Vector3(m_BoxCollider2D.bounds.extents.x * Mathf.Sign(transform.localScale.x), 0, 0);
		Vector3 swordPositionOffset = (!m_AttackHeld) ? new Vector3(0, m_CircleCollider2D.bounds.extents.y, 0) : new Vector3(m_CircleCollider2D.bounds.extents.x * Mathf.Sign(transform.localScale.x), 0, 0);
		m_SwordObject.transform.position = transform.position + swordPositionOffset;

		if (m_AttackPressed) { // when pressed, the jump height resets, and speed reduces

			m_DidKill = false; // reset kill check

			if (m_AttackSFX) {
				m_AttackSFX.Play();
            }
			
			m_CurrentJumpForce = m_MinJumpForce;
		
		} else if (m_AttackReleased && !m_DidKill && m_JumpChargeRate != m_JumpRateMaxIncrease) { // when attack released, apply increase charge rate if no kill
			m_JumpChargeRate = Mathf.Clamp(m_JumpChargeRate + m_JumpRateIncrease, m_JumpChargeRate, m_JumpRateMaxIncrease);

			// NOTE: test using speed-up sound to test increase jump effect
			if (m_SpeedUpSFX)
				m_SpeedUpSFX.Play();
			foreach (var trail in m_JumpOverlayTrailObjs)
				trail.ShowSpeedUp();

			// TODO: Probably should replace the sound effects at some point with something that works better for increasing jump charge rate

			/*if (m_SlowDownParticles)
				m_SlowDownParticles.Play();

			//m_MaxSpeed = Mathf.Clamp(m_MaxSpeed - m_MovementSpeedChangeRate, m_MinMovementSpeed, m_MaxMovementSpeed);

			if (m_SlowDownSFX)
				m_SlowDownSFX.Play();*/
		}
	}

	// make player move
	void ProcessMovement() {
		
		bool isCurrentlyLookingRight = Mathf.Sign(transform.localScale.x) > 0;
		bool isCurrentlyLookingLeft = Mathf.Sign(transform.localScale.x) < 0;

		bool shouldFlipXFacingDirection = (m_ShouldMoveRight && isCurrentlyLookingLeft) || (m_ShouldMoveLeft && isCurrentlyLookingRight);

		if (shouldFlipXFacingDirection) {
			transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
		}
		// the code above simply handles facing in the direction of input

		// can't move if attacking
		if (m_AttackHeld)
			return;

		//UpdateAllowedMovementDirection();

		// get input based on if we can move left/right
		short movementDirectionScalar = 0;
		if (m_ShouldMoveRight && m_CanMoveRight) {
			movementDirectionScalar = 1; m_CanMoveLeft = true;
		} else if (m_ShouldMoveLeft && m_CanMoveLeft) {
			movementDirectionScalar = -1; m_CanMoveRight = true;
		}

		Vector2 currentVelocity = m_Rigidbody2D.velocity;

		bool isAcceleratingRight = (movementDirectionScalar > 0 && currentVelocity.x < m_MaxSpeed);
		bool isDeacceleratingLeft = (movementDirectionScalar == 0 && currentVelocity.x < 0);

		bool shouldAccelerateRight = isAcceleratingRight || isDeacceleratingLeft;

		bool isAcceleratingLeft = (movementDirectionScalar < 0 && currentVelocity.x > -m_MaxSpeed);
		bool isDeacceleratingRight = (movementDirectionScalar == 0 && currentVelocity.x > 0);

		bool shouldAccelerateLeft = isAcceleratingLeft || isDeacceleratingRight;

		float powerVal = (isDeacceleratingRight || isDeacceleratingLeft)? 1.8f: 0.6f;

		if (shouldAccelerateRight) {
			currentVelocity.x += Mathf.Pow(m_AccelerationRate * Time.deltaTime, powerVal);
        } else if(shouldAccelerateLeft) {
			currentVelocity.x -= Mathf.Pow(m_AccelerationRate * Time.deltaTime, powerVal);
        }

		currentVelocity.x = Mathf.Clamp(currentVelocity.x, -m_MaxSpeed, m_MaxSpeed);

		// move left-right based on move speed and input
		m_Rigidbody2D.velocity = currentVelocity;
	}

	private void UpdateAllowedMovementDirection() {
        // if moving left but were blocked, check if we are free now
        RaycastHit2D[] hits = new RaycastHit2D[1];

		//if (m_Rigidbody2D.velocity.y >= 0) {
			//if (m_ShouldMoveLeft && !m_CanMoveLeft && m_BoxCollider2D.Cast(Vector2.left, hits, .1f) < 1)
			if (m_ShouldMoveLeft && !m_CanMoveLeft && m_CircleCollider2D.Cast(Vector2.left, hits, .1f) < 1)
				m_CanMoveLeft = true;
			else if (m_ShouldMoveLeft && !m_CanMoveLeft) { 
				foreach (var hit in hits) { 
					if (hit.transform.tag == "Hazard") {
						m_CanMoveLeft = true;
						break;
					}
				}
			} 
		/*}
		else {
			if (m_ShouldMoveLeft && !m_CanMoveLeft && m_EdgeCollider2D.Cast(Vector2.left, hits, .1f) < 1)
				m_CanMoveLeft = true;
			else if (m_ShouldMoveLeft && !m_CanMoveLeft) {
				foreach (var hit in hits) {
					if (hit.transform.tag == "Hazard") {
						m_CanMoveLeft = true;
						break;
					}
				}
			}
		}*/

		//if (m_Rigidbody2D.velocity.y >= 0) {
			//if (m_ShouldMoveRight && !m_CanMoveRight && m_BoxCollider2D.Cast(Vector2.right, hits, .1f) < 1)
			if (m_ShouldMoveRight && !m_CanMoveRight && m_CircleCollider2D.Cast(Vector2.right, hits, .1f) < 1)
				m_CanMoveRight = true;
			else if (m_ShouldMoveRight && !m_CanMoveRight) {
				foreach (var hit in hits) {
					if (hit.transform.tag == "Hazard") {
						m_CanMoveRight = true;
						break;
					}
				}
			}
		/*}
		else {
			if (m_ShouldMoveRight && !m_CanMoveRight && m_EdgeCollider2D.Cast(Vector2.right, hits, .1f) < 1)
				m_CanMoveRight = true;
			else if (m_ShouldMoveRight && !m_CanMoveRight) {
				foreach (var hit in hits) {
					if (hit.transform.tag == "Hazard") {
						m_CanMoveRight = true;
						break;
					}
				}
			}
		}*/

		// if considered in air and falling, check if we are actually landing on ground
		//if (m_InAir && m_Rigidbody2D.velocity.y <= 0 && m_BoxCollider2D.Cast(Vector2.down, hits, .1f) >= 1) {
		if (m_InAir && m_Rigidbody2D.velocity.y <= 0 && m_CircleCollider2D.Cast(Vector2.down, hits, .1f) >= 1) {
			foreach (var hit in hits) {
				if (hit.collider.gameObject.transform.position.y + hit.collider.bounds.extents.y > transform.position.y - m_CircleCollider2D.bounds.extents.y)
				//if (hit.transform.position.y > transform.position.y)
					continue;
				if (hit.transform.tag == "Ground" || hit.transform.tag == "Obstacle") {
					m_InAir = false;
					m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, 0);
					//float newDist = hit.point.y - (transform.position.y - m_CircleCollider2D.bounds.extents.y);
					//if (newDist > 0f)
						//transform.Translate(Vector2.up * newDist);
					break;
				}
			}
		}
	}

	// draw's indicator of how dangerous jump charge is
	void DrawJumpIndicator() {
		float percentage = m_CurrentJumpForce / m_CurrentMaxJumpForce;

		Color currentCubeColor = new Color((m_SafeColor.r * (1 - percentage)) + (m_DangerColor.r * percentage),
                                    (m_SafeColor.g * (1 - percentage)) + (m_DangerColor.g * percentage),
                                    (m_SafeColor.b * (1 - percentage)) + (m_DangerColor.b * percentage));

		m_SpriteRenderer.color = currentCubeColor;

		// NOTE: reducing the value because I realize that the predictive value over-estimates a little bit, leaving players feeling tricked
		float timeTillMaxJumpHeight = (-m_CurrentJumpForce / (Physics2D.gravity.y * m_Rigidbody2D.gravityScale)) * .75f;
		float peakJumpYPosition = transform.position.y + m_CurrentJumpForce * timeTillMaxJumpHeight + (0.5f * m_Rigidbody2D.gravityScale) * Physics2D.gravity.y * Mathf.Pow(timeTillMaxJumpHeight, 2.0f);

		peakJumpYPosition = Mathf.Clamp(peakJumpYPosition, MIN_Y_POSITION, MAX_Y_POSITION);

		Vector3 peakJumpPosition = new Vector3(transform.position.x, peakJumpYPosition, transform.position.z);

		m_JumpOverlayObject.transform.position = peakJumpPosition;

		SpriteRenderer jumpOverlaySpriteRenderer = m_JumpOverlayObject.GetComponent<SpriteRenderer>();
		currentCubeColor.a = jumpOverlaySpriteRenderer.color.a;
		jumpOverlaySpriteRenderer.color = currentCubeColor;
		foreach (var trail in m_JumpOverlayTrailObjs)
			trail.SetColor(currentCubeColor);
	}

	// process special collision events
	private void OnCollisionEnter2D(Collision2D collision) {
		// if we collide with our sword, ignore it
		if (collision.gameObject.tag == "Sword")
			return;

		bool playerShouldDie = collision.gameObject.tag == "Hazard";
		// only die from enemy if the enemy is not dead
		if (!playerShouldDie && collision.gameObject.tag == "Enemy" && !collision.gameObject.GetComponent<Enemy>().IsDead())
			playerShouldDie = true;

		if (playerShouldDie) {

			KillPlayer();
			
			if (m_DeathSFX)
				m_DeathSFX.Play();

			return;
		}


		// check if we land on top of something; if we do, mark on ground and update terminal velocity
		foreach (var contact in collision.contacts) {
				bool didPlayerLand = m_InAir && Vector2.Angle(contact.normal, Vector2.up) <= 60f;
				//bool didPlayerLand = m_InAir && contact.normal == Vector2.up;

				if (didPlayerLand) {
					SetCurrentMaxJumpForce(); m_InAir = false;
				}

			// if we are colliding with the ground, no horrizontal checks
			if (collision.gameObject.tag == "Ground")
				return;

			/*//bool isCollidingWithRightWall = m_ShouldMoveRight && m_CanMoveRight && contact.normal == Vector2.left;
			bool isCollidingWithRightWall = m_ShouldMoveRight && m_CanMoveRight && Vector2.Angle(contact.normal, Vector2.left) <= 60f;

			//bool isCollidingWithLeftWall = m_ShouldMoveLeft && m_CanMoveLeft && contact.normal == Vector2.right;
			bool isCollidingWithLeftWall = m_ShouldMoveLeft && m_CanMoveLeft && Vector2.Angle(contact.normal, Vector2.right) <= 60f;

			if (isCollidingWithLeftWall || isCollidingWithRightWall) {
            
				m_Rigidbody2D.velocity = new Vector2(0, m_Rigidbody2D.velocity.y);

				if (isCollidingWithLeftWall) {
					m_CanMoveLeft = false; 
				} else {
					m_CanMoveRight = false;
				}
			}*/

			// break out early if we got all we need
			//if (!m_InAir && (isCollidingWithLeftWall || isCollidingWithRightWall))
			if (!m_InAir)
				break;
		}
	}

	private void OnCollisionStay2D(Collision2D collision) {

		// only do side checks if we are in the the air
		if (!m_InAir)
			return;
		 foreach (var contact in collision.contacts) {
			bool isCollidingWithRightWall = m_ShouldMoveRight && m_CanMoveRight && Vector2.Angle(contact.normal, Vector2.left) <= 60f;

			bool isCollidingWithLeftWall = m_ShouldMoveLeft && m_CanMoveLeft && Vector2.Angle(contact.normal, Vector2.right) <= 60f;

			if (isCollidingWithLeftWall || isCollidingWithRightWall) {

				m_Rigidbody2D.velocity = new Vector2(0, m_Rigidbody2D.velocity.y);

				if (isCollidingWithLeftWall) {
					m_CanMoveLeft = false;
				}
				else {
					m_CanMoveRight = false;
				}
				break;
			}
		}
	}

	// sets new terminal velocity based on where player is standing
	void SetCurrentMaxJumpForce() {
		m_CurrentMaxJumpForce = m_MaxJumpForce - (m_MaxJumpForce * (transform.position.y - m_DefaultPlayerHeight) / m_DefaultLevelHeight);
	}

	// changes stats when a kill is done; to be called by the victims
	public void ProcessKill() {

		// show sword effect of reseting range if the range is not max yet
		if (m_RangeUpParticles && m_AttackRange != m_MaxAttackRange)
			m_RangeUpParticles.Play();
		
		// reset attack range
		m_AttackRange = m_MaxAttackRange;
		//m_MaxSpeed = Mathf.Clamp(m_MaxSpeed + m_MovementSpeedChangeRate, m_MaxSpeed, m_MaxMovementSpeed);
		
		// maek that we killed so that we don't slow down
		m_DidKill = true;
		
		// play sound
		if (m_KillSFX)
			m_KillSFX.Play();

		// NOTE: testing the speed down sound for reducing jump-charge speed
		if (m_JumpChargeRate != m_InitialJumpChargeRate) {
			if (m_SlowDownSFX)
				m_SlowDownSFX.Play();
			foreach (var trail in m_JumpOverlayTrailObjs)
				trail.ShowSlowDown();
			// reset jump charge rate
			m_JumpChargeRate = m_InitialJumpChargeRate;
		}

		/*// show particle effects for speeding up
		if (!m_IsDead && m_SpeedUpParticles) {

			if (m_SlowDownParticles && m_SlowDownParticles.isPlaying)
				m_SlowDownParticles.Stop();
			
			if(m_SpeedUpParticles)
				m_SpeedUpParticles.Play();
			
			if (m_SpeedUpSFX)
				m_SpeedUpSFX.Play();
		}*/
	}

	// called upon player death
	public void KillPlayer() {
		
		Debug.Log("Dead");
		
		if (m_DeathSFX)
			m_DeathSFX.Play();

        Camera.main.GetComponent<CameraController>().ShowFinalScore();

		float percentage = m_CurrentJumpForce / m_CurrentMaxJumpForce;
		Color currentCubeColor = new Color((m_SafeColor.r * (1 - percentage)) + (m_DangerColor.r * percentage),
									(m_SafeColor.g * (1 - percentage)) + (m_DangerColor.g * percentage),
									(m_SafeColor.b * (1 - percentage)) + (m_DangerColor.b * percentage));
		currentCubeColor.a = 0f;

		//m_BoxCollider2D.enabled = false;
		m_CircleCollider2D.enabled = false;
		m_Rigidbody2D.simulated = false;
		m_SpriteRenderer.material = m_DissolveMaterial;
		m_DissolveMaterial.SetColor("_MainColor", currentCubeColor);
		m_DissolveMaterial.SetFloat("_DissolveAmount", 0f);
		//m_SpriteRenderer.enabled = false;
		m_SwordObject.GetComponent<SpriteRenderer>().enabled = false;
		m_LastPositionAlive = transform.position;
		m_IsBurning = false;
		m_IsDead = true;

		if (m_JumpOverlayObject) m_JumpOverlayObject.SetActive(false);

		if (m_LavaRenderer) m_LavaRenderer.color = Color.black;
		
		// turn off particles
		if (m_RangeDownParticles) Destroy(m_RangeDownParticles);
		if (m_RangeUpParticles) Destroy(m_RangeUpParticles);
		if (m_SpeedUpParticles) Destroy(m_SpeedUpParticles);
		if (m_SlowDownParticles) Destroy(m_SlowDownParticles);

		foreach (var trail in m_JumpOverlayTrailObjs)
			Destroy(trail.gameObject);
	}

	public void Burn()
    {
		Debug.Log("burned");
		if (m_DeathSFX)
			m_DeathSFX.Play();

		Camera.main.GetComponent<CameraController>().ShowFinalScore();

		m_CircleCollider2D.enabled = false;
		//m_BoxCollider2D.enabled = false;
		m_Rigidbody2D.simulated = false;
		float percentage = m_CurrentJumpForce / m_CurrentMaxJumpForce;
		Color currentCubeColor = new Color((m_SafeColor.r * (1 - percentage)) + (m_DangerColor.r * percentage),
									(m_SafeColor.g * (1 - percentage)) + (m_DangerColor.g * percentage),
									(m_SafeColor.b * (1 - percentage)) + (m_DangerColor.b * percentage));
		currentCubeColor.a = 0f;
		m_SpriteRenderer.material = m_BurningMaterial;
		m_BurningMaterial.SetColor("_MainColor", currentCubeColor);
		//m_SpriteRenderer.enabled = false;
		m_SwordObject.GetComponent<SpriteRenderer>().enabled = false;
		m_LastPositionAlive = transform.position;
		m_IsBurning = true;
		m_IsDead = true;

		bool isCurrentlyLookingLeft = Mathf.Sign(transform.localScale.x) < 0;
		if(isCurrentlyLookingLeft)
        {
			m_BurningMaterial.SetFloat("_DissolveAmount", 0f);
			m_WasLookingLeft = true;
		}
		else
        {
			m_BurningMaterial.SetFloat("_DissolveAmount", 2.0f);
			m_WasLookingLeft = false;
		}

		if (m_JumpOverlayObject) m_JumpOverlayObject.SetActive(false);

		if (m_LavaRenderer) m_LavaRenderer.color = Color.black;

		if (m_RangeDownParticles) Destroy(m_RangeDownParticles);
		if (m_RangeUpParticles) Destroy(m_RangeUpParticles);
		if (m_SpeedUpParticles) Destroy(m_SpeedUpParticles);
		if (m_SlowDownParticles) Destroy(m_SlowDownParticles);
	}
}
