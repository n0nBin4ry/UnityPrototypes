using JetBrains.Annotations;
using Microsoft.Unity.VisualStudio.Editor;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AIManager : MonoBehaviour {
	public Tilemap AIGrid;
	public int GridHeight = 32;
	public int GridWidth = 23;

	public GameObject DebugSprite;

	[HideInInspector] public List<AIController> AIMinions = new List<AIController>();

	private Dictionary<Vector2Int, AstarData> _board;
	private GameObject _player;

	// vars for spawning mechanism
	[SerializeField] private GameObject _minionPrefab;
	public int MaxMinions = 3;
	public int SpawnRate = 3;
	private int _spawnTimer = 0;
	[HideInInspector] public bool DoSpawn = false;
	[HideInInspector] public bool BossDead = false;
	[SerializeField] private Transform _spawnPoint;

	void Start() {
		// find player
		_player = GameObject.FindGameObjectWithTag("Player");

		// set up board for A*
		InitBoard();
	}
	
	// update all the minions to do their next move
	public void TickMinions(bool playerMoved) { 
		// tick any spawned minions
		foreach (var minion in AIMinions) {
			minion.Tick(playerMoved);
		}

		// spawn new minons if needed
		if (AIMinions.Count < MaxMinions && !BossDead) {
			if (!DoSpawn) {
				_spawnTimer = 0;
				return;
			}
			_spawnTimer++;
			if (_spawnTimer >= SpawnRate) {
				GameObject.Instantiate(_minionPrefab, _spawnPoint.position, UnityEngine.Quaternion.identity);
				_spawnTimer = 0;
			}
		}
	}

	public Stack<Vector2Int> GetPathToPlayer(UnityEngine.Vector3 start) {
		return GetPath(start, _player.transform.position);
	}

	// return a list of grid positions that are a path from given start pos (world) to end pos (world)
	public Stack<Vector2Int> GetPath(UnityEngine.Vector3 start, UnityEngine.Vector3 end) {
		start.z = AIGrid.transform.position.z; end.z = AIGrid.transform.position.z;

		// reset board for new pathfinding
		ResetBoard();

		Vector3Int temp = AIGrid.WorldToCell(start);
		Vector2Int currCell = new Vector2Int(temp.x, temp.y);
		temp = AIGrid.WorldToCell(end);
		Vector2Int goalCell = new Vector2Int(temp.x, temp.y);
		AstarData currData = null;
		_board.TryGetValue(currCell, out currData);
		if (currData == null) {
			Debug.Log("AI Error: AI path starting on a space not on _board!");
			return null;
		}

		Vector2Int neighborCell = new Vector2Int();
		AstarData neighborData;
		while (currCell != goalCell) {
			// get current data
			_board.TryGetValue(currCell, out currData);
			if (currData == null) {
				Debug.Assert(currData != null, "AI Fatal Error: Pathfinding used a cell that is not on _board!");
				return null;
			}

			// add top neightbor
			neighborCell.x = currCell.x;
			neighborCell.y = currCell.y + 1;
			// if this cell is on the _board
			if (_board.TryGetValue(neighborCell, out neighborData)) {
				// if there is no parent then this was never opened; check closed as well in case it is start node
				if (neighborData.Parent == null && !neighborData.Closed) {
					neighborData.Parent = currData;
					neighborData.G = currData.G + 1;
					neighborData.H = Mathf.Abs(currCell.x - goalCell.x) + Mathf.Abs(currCell.y - goalCell.y);
				}
				// if opened, check if current cell is a better way to get to neighbor
				else if (neighborData.G > currData.G + 1) {
					neighborData.G = currData.G + 1;
					neighborData.Parent = currData;
				}
			}

			// add bottom neighbor
			neighborCell.x = currCell.x;
			neighborCell.y = currCell.y - 1;
			// if this cell is on the _board
			if (_board.TryGetValue(neighborCell, out neighborData)) {
				// if there is no parent then this was never opened; check closed as well in case it is start node
				if (neighborData.Parent == null && !neighborData.Closed) {
					neighborData.Parent = currData;
					neighborData.G = currData.G + 1;
					neighborData.H = Mathf.Abs(currCell.x - goalCell.x) + Mathf.Abs(currCell.y - goalCell.y);
				}
				// if opened, check if current cell is a better way to get to neighbor
				else if (neighborData.G > currData.G + 1) {
					neighborData.G = currData.G + 1;
					neighborData.Parent = currData;
				}
			}

			// add right neighbor
			neighborCell.x = currCell.x + 1;
			neighborCell.y = currCell.y;
			// if this cell is on the _board
			if (_board.TryGetValue(neighborCell, out neighborData)) {
				// if there is no parent then this was never opened; check closed as well in case it is start node
				if (neighborData.Parent == null && !neighborData.Closed) {
					neighborData.Parent = currData;
					neighborData.G = currData.G + 1;
					neighborData.H = Mathf.Abs(currCell.x - goalCell.x) + Mathf.Abs(currCell.y - goalCell.y);
				}
				// if opened, check if current cell is a better way to get to neighbor
				else if (neighborData.G > currData.G + 1) {
					neighborData.G = currData.G + 1;
					neighborData.Parent = currData;
				}
			}

			// add left neighbor
			neighborCell.x = currCell.x - 1;
			neighborCell.y = currCell.y;
			// if this cell is on the _board
			if (_board.TryGetValue(neighborCell, out neighborData)) {
				// if there is no parent then this was never opened; check closed as well in case it is start node
				if (neighborData.Parent == null && !neighborData.Closed) {
					neighborData.Parent = currData;
					neighborData.G = currData.G + 1;
					neighborData.H = Mathf.Abs(currCell.x - goalCell.x) + Mathf.Abs(currCell.y - goalCell.y);
				}
				// if opened, check if current cell is a better way to get to neighbor
				else if (neighborData.G > currData.G + 1) {
					neighborData.G = currData.G + 1;
					neighborData.Parent = currData;
				}
			}

			// NOTE: should do this with a priority queue when you learn how to make custom comparators in C#
			// add current cell to the closed list
			currData.Closed = true;
			// get the next current cell by searching the open cells for lowest F value
			int minF = int.MaxValue;
			foreach (var entry in _board) { 
				if (!entry.Value.Closed && entry.Value.Parent != null && entry.Value.GetF() < minF) {
					minF = entry.Value.GetF();
					currCell = entry.Key;
				}
			}
		}


		_board.TryGetValue(currCell, out currData);
		if (currData == null) {
			Debug.Assert(currData != null, "AI Fatal Error: Pathfinding used a cell that is not on _board!");
			return null;
		}

		// make path to return
		Stack<Vector2Int> path = new Stack<Vector2Int>();
		while (currData.Parent != null) {
			path.Push(currData.coord);
			currData = currData.Parent;
		}
		return path;
	}

	// resets thew _board's A* data
	private void ResetBoard() {
		foreach (var entry in _board) {
			entry.Value.Closed = false;
			entry.Value.G = 0;
			entry.Value.H = 0;
			entry.Value.Parent = null;
		}
	}

	// set up an initial set of tiles that the AI will use for A* pathfinding off of the tilemap grid given
	private void InitBoard() {
		_board = new Dictionary<Vector2Int, AstarData>();
		var center = AIGrid.WorldToCell(AIGrid.transform.position);
		for (int i = 0; i < GridHeight; i++) {
			for (int j = 0; j < GridWidth; j++) {
				var tilePos = new Vector3Int(center.x - (GridHeight / 2) + j, center.y - (GridWidth / 2) + i, center.z);
				if (AIGrid.GetTile(tilePos) != null) {
					AstarData data = new AstarData();
					data.coord = new Vector2Int(tilePos.x, tilePos.y);
					_board.Add(data.coord, data);
					if (DebugSprite != null) {
						GameObject.Instantiate(DebugSprite, AIGrid.CellToWorld(tilePos), UnityEngine.Quaternion.identity);
					}
				}
			}
		}
		Debug.Log("AI tiles done, " + _board.Count + " tiles to walk on.");
	}

	// if player leaves collision then set spawning according to what side they are on
	// if player on left, do spawning, if playeron right, hold off on spawning
	private void OnTriggerExit2D(Collider2D collision) {
		if (collision.tag == "Player") {
			if (collision.transform.position.x < transform.position.x)
				DoSpawn = true;
			else 
				DoSpawn = false;
		}
	}
}

