using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AstarData {
	// distance from start
	public int G = 0;
	// distance from goal
	public int H = 0;
	// bools to keep track of what list they are in
	public bool Closed = false;
	// reference to cell it came from; doubles as marking self as open when not null
	public AstarData Parent = null;

	public Vector2Int coord = Vector2Int.zero;

	public int GetF() { return G + H; }
}
