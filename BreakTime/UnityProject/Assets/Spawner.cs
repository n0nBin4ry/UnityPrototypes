using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public float spawnPerSecond;
    public GameObject obj;
    public bool play=true;

    float lastSpawn=0;
    float spawnInterval => 1 / spawnPerSecond;
    
    private void Update()
    {
        if (!play) return;
        if (lastSpawn + spawnInterval < Time.time)
        {
            GameObject b =Instantiate(obj, transform.position, Quaternion.identity);
            b.transform.Rotate(0f, 0f, Random.Range(0f, 360f));
            lastSpawn = Time.time;
        }
    }
}
