using System.Collections;
using System.Collections.Generic;
using TMPro;
//using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

enum PlayerDir {
	u, d, l, r,
};

public class PlayerController : MonoBehaviour {
	public Transform MovePoint = null;

	public LayerMask wallLayer;

	public float MoveSpd = 3f;

	public float FireDurartion = 1f;

	public float InputCooldown = .2f;
	private float _cooldownTimer = 0f;

	public TMP_Text ResetText;
	public TMP_Text LoseText;
	public Color TextColor;
	public Color WinTextColor;
	public GameObject DeathPanel;
	public GameObject WinPanel;
	public float DeathFadeSpd = .6f;
	public float DeathFadeDelay = 1f;

	private SpriteRenderer _sprite;
	private FireController _fire;
	private Animator _anim;

	// input stuff
	int _horz = 0;
	int _vert = 0;
	bool _firePressed = false, _fireHeld = false;

	// player's direction
	private PlayerDir _dir = PlayerDir.r;

	private bool _isDead = false;
	private bool _canMove = true;
	private bool _canInput = true;
	private bool _doneMoving = true;


	// audio Manager
	public AudioClip ResetSound;
	public AudioClip BumpSound;
	public AudioClip MoveSound;
	public AudioClip IgniteSound;
	public AudioClip WinTrack;
	public AudioClip LoseTrack;
	private AudioManager _audio;

	// AI
	private AIManager _aiManager;

	// Start is called before the first frame update
	void Start() {
		_audio = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
		_sprite = GetComponentInChildren<SpriteRenderer>();
		_anim = GetComponentInChildren<Animator>();
		_aiManager = GameObject.FindGameObjectWithTag("AIManager").GetComponent<AIManager>();

		// death/win screen setup
		DeathPanel.GetComponent<CanvasRenderer>().SetAlpha(0);
		WinPanel.GetComponent<CanvasRenderer>().SetAlpha(0);
		ResetText.GetComponent<CanvasRenderer>().SetAlpha(0);
		LoseText.GetComponent<CanvasRenderer>().SetAlpha(0);
		ResetText.color = TextColor;
		LoseText.color = TextColor;

		// set up our fire with our values
		_fire = GetComponentInChildren<FireController>();
		_fire.PlayersFire = true;
		_fire.LifeTime = FireDurartion;
		_fire.gameObject.SetActive(false);

		// move movepoint to world space
		MovePoint.parent = null;
		var temp = MovePoint.position;
		temp.z = transform.position.z;
		MovePoint.position = temp;
	}

	// Update is called once per frame
	void Update() {
		if (!_isDead && _canMove && _canInput && Input.GetKeyDown(KeyCode.R)) {
			_canMove = false;
			_sprite.enabled = false;
			_fire.transform.position = transform.position;
			_fire.gameObject.SetActive(true);
			_audio.SFX.PlayOneShot(IgniteSound, 0.8f);
			_audio.SFX.PlayOneShot(ResetSound, 0.6f);
		}

		PollInput();

		if (_isDead || !_canMove)
			return;

		// set new move command or fire command if not moving already
		var distSqr = Vector3.SqrMagnitude(transform.position - MovePoint.position);
		DoFire(distSqr);
		Turn(distSqr);
		Move(distSqr);
	}

	void PollInput() {
		if (!_canInput) {
			_horz = 0;
			_vert = 0;
			_fireHeld = false;
			_firePressed = false;
			return;
		}

		var oldHorz = _horz;
		var oldVert = _vert;

		_horz = 0;
		_vert = 0;

		// have some time between inputs to give a more tactical feel
		if (_cooldownTimer > 0) {
			_cooldownTimer -= Time.deltaTime;
			return;
		}

		bool _downPressed = false, _upPressed = false, _rightPressed = false, _leftPressed = false;
		bool _downHeld = false, _upHeld = false, _rightHeld = false, _leftHeld = false;
		_firePressed = false; _fireHeld = false;

		if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
			_upPressed = true;
		if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
			_downPressed = true;
		if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
			_leftPressed = true;
		if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
			_rightPressed = true;

		if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
			_upHeld = true;
		if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
			_downHeld = true;
		if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
			_leftHeld = true;
		if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
			_rightHeld = true;

		if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift) || Input.GetKeyDown(KeyCode.Space))
			_firePressed = true;

		if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.Space))
			_fireHeld = true;

		// trying to make move input feel good; prioritizing most recent button pressed and ignoring opposite inputs
		// NOTE: doesn't work fully as intended but way better now
		if (_upPressed)
			_vert = 1;
		else if (_rightPressed)
			_horz = 1;
		else if (_downPressed)
			_vert = -1;
		else if (_leftPressed)
			_horz = -1;
		else {
			if (_rightHeld)
				_horz++;
			if (_leftHeld)
				_horz--;
			if (_upHeld)
				_vert++;
			if (_downHeld)
				_vert--;

			if (_vert != 0 && oldVert != 0) {
				_horz = 0;
			}
			else if (_horz != 0 && oldHorz != 0) {
				_vert = 0;
			}
			else if (_upHeld && _vert == 0 && oldVert != 0) {
				_horz = 0;
				_vert = oldVert;
			}
			else if (_rightHeld && _horz == 0 && oldHorz != 0) {
				_vert = 0;
				_horz = oldHorz;
			}
			else if (_horz != 0 && _vert != 0) {
				_horz = oldHorz;
				_vert = oldVert;
			}
		}
	}

	void Move(float distSqr) {
		// clamp the move-point to be safe
		var movePos = MovePoint.position;
		movePos.x = Mathf.Clamp(movePos.x, transform.position.x - 1f, transform.position.x + 1f);
		movePos.y = Mathf.Clamp(movePos.y, transform.position.y - 1f, transform.position.y + 1f);
		MovePoint.position = movePos;

		// move toward the move point if not on it
		if (distSqr >= 0.1 * 0.1 || _cooldownTimer > 0 || _fire.gameObject.activeSelf) {
			transform.position = Vector3.MoveTowards(transform.position, MovePoint.position, MoveSpd * Time.deltaTime);
			return;
		}

		// finish moving if close enough
		if (!_doneMoving) {
			_doneMoving = true;
			transform.position = MovePoint.position;
			// if movedtick AI manager to move all AIMinions; mark that we moved so that minions update path
			_aiManager.TickMinions(true);
		}
		

		string trigger = "";
		Vector2 possibleMove = transform.position;
		if (_vert > 0) {
			possibleMove += Vector2.up;
			trigger = "up";
		}
		else if (_vert < 0) {
			possibleMove += Vector2.down;
			trigger = "down";
		}
		else if (_horz > 0) {
			possibleMove += Vector2.right;
			trigger = "horizontal";
		}
		else if (_horz < 0) {
			possibleMove += Vector2.left;
			trigger = "horizontal";
		}

		if (possibleMove.x == transform.position.x && possibleMove.y == transform.position.y)
			return;

		// check if there is a wall in the way
		if (!Physics2D.OverlapCircle(possibleMove, .2f, wallLayer)) {
			// if not, then move and set animation
			MovePoint.position = new Vector3(possibleMove.x, possibleMove.y, MovePoint.position.z);
			_cooldownTimer = InputCooldown;
			_anim.SetTrigger(trigger);
			_audio.SFX.PlayOneShot(MoveSound);
			_doneMoving = false;
		}
		else {
			_cooldownTimer = InputCooldown * 2.5f;
			_audio.SFX.PlayOneShot(BumpSound);
			_anim.SetTrigger(trigger);

		}
	}

	void Turn(float distSqr) {
		// TODO: make like axis mechanism in the Move() function (along with priotity to recently-pressed key); update: might need more as I think about it

		if (distSqr >= 0.1 * 0.1 || _fire.gameObject.activeSelf || _cooldownTimer >= InputCooldown)
			return;

		// TODO (stretch): change to animated sprite
		if (_horz > 0 && _dir != PlayerDir.r) {
			//transform.position = _sprite.transform.right = Vector3.right;
			_anim.SetTrigger("horizontal");
			transform.localScale = Vector3.one;
			_dir = PlayerDir.r;
			_cooldownTimer = InputCooldown;
			_fire.transform.position = transform.position + Vector3.right;
			return;
		}
		else if (_horz < 0 && _dir != PlayerDir.l) {
			//_sprite.transform.right = Vector3.left;
			_anim.SetTrigger("horizontal");
			transform.localScale = new Vector3(-1, 1, 1);
			_dir = PlayerDir.l;
			_cooldownTimer = InputCooldown;
			_fire.transform.position = transform.position + Vector3.left;
			return;
		}
		else if (_vert > 0 && _dir != PlayerDir.u) {
			//_sprite.transform.right = Vector3.up;
			_anim.SetTrigger("up");
			transform.localScale = Vector3.one;
			_dir = PlayerDir.u;
			_cooldownTimer = InputCooldown;
			_fire.transform.position = transform.position + Vector3.up;
			return;
		}
		else if (_vert < 0 && _dir != PlayerDir.d) {
			//_sprite.transform.right = Vector3.down;
			_anim.SetTrigger("down");
			transform.localScale = Vector3.one;
			_dir = PlayerDir.d;
			_cooldownTimer = InputCooldown;
			_fire.transform.position = transform.position + Vector3.down;
			return;
		}
	}

	void DoFire(float distSqr) {
		// don't do fire if moving
		if (distSqr >= 0.1 * 0.1 || _cooldownTimer >= InputCooldown || _fire.gameObject.activeSelf)
			return;
		if (_fireHeld || _firePressed) {
			_fire.gameObject.SetActive(true);
			_cooldownTimer = InputCooldown;
			_audio.SFX.PlayOneShot(IgniteSound, 0.7f);
			// tick AI manager to move all AIMinions
			_aiManager.TickMinions(false);
		}
	}

	public void Die() {
		if (_isDead)
			return;
		_isDead = true;
		GetComponent<BoxCollider2D>().enabled = false;
		_sprite.enabled = false;
		_audio.Ambiance.Stop();
		_audio.Ambiance.clip = LoseTrack;
		_audio.Ambiance.volume = 1f;
		_audio.Ambiance.Play();
		StartCoroutine("CoFadeInDeathScreen");
	}

	IEnumerator CoFadeInDeathScreen() {
		var fade = 0f;
		var timer = 0f;

		while (timer < DeathFadeDelay) {
			timer += Time.deltaTime;
			yield return null;
		}

		var panelRender = DeathPanel.GetComponent<CanvasRenderer>();
		var resetRender = ResetText.GetComponent<CanvasRenderer>();
		var loseRender = LoseText.GetComponent<CanvasRenderer>();


		while (panelRender.GetAlpha() < 1f || loseRender.GetAlpha() < 1f || resetRender.GetAlpha() < 1f) {
			fade += Time.deltaTime * DeathFadeSpd;
			panelRender.SetAlpha(Mathf.Min(fade, 1f));
			loseRender.SetAlpha(Mathf.Min(fade, 1f));
			resetRender.SetAlpha(Mathf.Min(fade, 1f));
			yield return null;
		}

		yield break;
	}

	private void OnTriggerEnter2D(Collider2D collision) {
		if (collision.tag == "Fire") {
			_canMove = false;
		}
		else if (collision.tag == "Pitfall") {
			LoseText.text = "Why would you burn a bridge you're standing on??? You fell to your death.";
			Die();
		}
		else if (collision.tag == "Win") {
			LoseText.text = "You finally made it! Now just sit back and unwind.";
			ResetText.text = "Fire, monsters, Spelunky?! I just wanna chill for God's sake!";
			LoseText.color = WinTextColor;
			ResetText.color = WinTextColor;
			_canInput = false;
			DeathPanel = WinPanel;
			_audio.Ambiance.Stop();
			_audio.Ambiance.clip = WinTrack;
			_audio.Ambiance.volume = .5f;
			_audio.Ambiance.Play();
			StartCoroutine("CoFadeInDeathScreen");
		}
		else if (collision.tag == "SecretWarp") {
			// wanna finish any pending movement but not give any more input
			_canInput = false;
		}
	}
}
