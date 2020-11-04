using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class AISpawnTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    void Start() {
		var aiManager = GameObject.FindGameObjectWithTag("AIManager").GetComponent<AIManager>();
		transform.parent = aiManager.transform;
		aiManager.DoSpawn = true;
    }
}
