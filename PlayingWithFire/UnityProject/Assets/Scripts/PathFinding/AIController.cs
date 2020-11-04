using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Numerics;
using System.Xml.Serialization;
using UnityEngine;

public class AIController : MonoBehaviour {

	private enum State {
		Idle,
		Moving,
		Primed,
		Exploding,
	};

	[SerializeField] private LayerMask _wallLayer;

	private State _state = State.Idle;

	private AIManager _manager = null;

	private Stack<Vector2Int> _path;
	private Vector2Int _targCoord;
	private float _movSpd = 0f;

    // Start is called before the first frame update
    void Start() {
        if (_manager == null) {
			_manager = GameObject.FindGameObjectWithTag("AIManager").GetComponent<AIManager>();
		}
		_manager.AIMinions.Add(this);

		// set move speed to the same as the player so we all move the same
		_movSpd = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().MoveSpd;
    }

    // Update is called once per frame
    void Update() {
		if (_state == State.Moving) {
			// get the target position from the grid coord
			UnityEngine.Vector3 targetPos = _manager.AIGrid.CellToWorld(new Vector3Int(_targCoord.x, _targCoord.y, 0));
			targetPos.z = transform.position.z;
			// if we are there, then set to idle to stop moving
			if (UnityEngine.Vector3.SqrMagnitude(targetPos - transform.position) < 0.1 * 0.1) {
				_state = State.Idle;
				transform.position = targetPos;
				// TODO: set idle animation
				return;
			}
			// move toward the target postion
			transform.position = UnityEngine.Vector3.MoveTowards(transform.position, targetPos, _movSpd * Time.deltaTime);
		}
    }

	public void Tick(bool playerMoved) { 
		// if idle, move
		if (_state == State.Idle) {
			if (_path == null)
				_path = _manager.GetPathToPlayer(transform.position);

			// check if player has moved or path is empty to get new path
			else if (_path.Count == 0 || playerMoved) {
				_path.Clear();
				_path = _manager.GetPathToPlayer(transform.position);
				// if path still empty then no path to player or already at player
				if (_path.Count == 0)
					return;
			}

			// get the next coordinate from path
			_targCoord = _path.Pop();

			// check if there is wall or enemy in way so that we can stay idle
			// TODO: also check if another minion is already going to step there, if yes then stay idle (may need a refactor on moving to use the move point mechanism like in player); IMPORTANT TODO
			if (Physics2D.OverlapCircle(_manager.AIGrid.CellToWorld(new Vector3Int(_targCoord.x, _targCoord.y, 0)), .2f, _wallLayer)) {
				_path.Push(_targCoord);
				_state = State.Idle;
				return;
			}

				_state = State.Moving;
			// TODO: set animation
		}
		// if primed, explode
		if (_state == State.Primed)
			StartCoroutine("CoExplode");
	}

	private IEnumerator CoExplode() {
		if (_state == State.Exploding) {
			yield break;
		}
		_state = State.Exploding;

		// screenshake
		Camera.main.GetComponent<VerticalShake>().DoShake();

		// turn off sprite
		var sprite = GetComponent<SpriteRenderer>();
		if (sprite != null)
			sprite.enabled = false;

		// TODO: create explosion on self and neighbor spaces


		// destroy self
		Destroy(gameObject);
		yield break;
	}

	private void OnTriggerEnter2D(Collider2D collision) {
		// only prime for explosion if not already primed
		if (_state == State.Primed || _state == State.Exploding)
			return;

		// get primed by getting lit on fire
		if (collision.tag == "Fire") {
			// only if flame is on us, not next to us
			var flamePosition = collision.transform.position;
			flamePosition.z = transform.position.z;
			if (UnityEngine.Vector3.SqrMagnitude(transform.position - flamePosition) < 0.1f) {
				_state = State.Primed;
				// TODO: set primed animation
			}
		}
	}

	private void OnDestroy() {
		// remove self from the ai manager
		_manager.AIMinions.Remove(this);
	}
}
