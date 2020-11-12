using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour {
	[Header("Number of room cells wide this room is; for special rooms or sequences.")]
	public int NumUnits = 1;
	[Space]
	public Collectable_SlowLava[] LavaCollectables;
}
