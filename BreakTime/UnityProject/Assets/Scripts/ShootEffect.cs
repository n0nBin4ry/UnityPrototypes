using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class ShootEffect : MonoBehaviour
{
    int frame = 0;
    SpriteRenderer sprite;
    Light2D mLight;

    // Start is called before the first frame update
    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        mLight = GetComponent<Light2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (frame == 0)
        {
            sprite.enabled = false;
            mLight.enabled = false;
        }
        else
        {
            sprite.enabled = true;
            mLight.enabled = true;
            frame--;
        }
    }
    public void Play()
    {
        frame = 5;
        float scale = Random.Range(0.6f, 1.4f);
        transform.localScale = new Vector3(1, 1, 1) * scale;
    }
}
