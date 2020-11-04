using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLerp : MonoBehaviour
{
	public GameObject Character;
	public float LerpRate = 3f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		// if character moving left, lerp left
		if (Character.transform.localScale.x < 0) {
			Vector3 newPos = Vector3.Slerp(transform.position, Character.transform.position + new Vector3(1.46f, 0), LerpRate * Time.deltaTime);
			newPos.z = transform.position.z;
			transform.position = newPos;
		}
		// else right
		else if (Character.transform.localScale.x > 0) {
			Vector3 newPos = Vector3.Slerp(transform.position, Character.transform.position - new Vector3(1.46f, 0), LerpRate * Time.deltaTime);
			newPos.z = transform.position.z;
			transform.position = newPos;
		}
    }
}
