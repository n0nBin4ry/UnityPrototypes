using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crate : MonoBehaviour
{
    public Sprite damaged;
    public int life = 2;
	private AudioManager _audio;

    public void Damage()
    {
        life--;
        if (life <= 0)
        {
			_audio.PlayCrateBreak();
            Destroy(gameObject);
        }
        if (life < 2)
        {
            GetComponent<SpriteRenderer>().sprite = damaged;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
		_audio = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
	}

    // Update is called once per frame
    void Update()
    {

    }
}
