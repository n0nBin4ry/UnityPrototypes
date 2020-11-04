using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AndyController : MonoBehaviour {
	public GameObject SpelunkyText;
	public GameObject PodcastText;

	public GameObject WhyText;
	public GameObject NeverText;
	public GameObject EggplantText;

	public GameObject DudeText;
	public GameObject OneText;

	public GameObject ProjectilePrefab;
	public GameObject FirePrefab;

	public AudioClip ErrorSound;

	private Rigidbody2D _rb;

	private bool _hasShot = false;

    // Start is called before the first frame update
    void Start() {
		SpelunkyText.SetActive(true);
		PodcastText.SetActive(true);
		WhyText.SetActive(false);
		NeverText.SetActive(false);
		EggplantText.SetActive(false);
		DudeText.SetActive(false);
		OneText.SetActive(false);
		
		_rb = GetComponent<Rigidbody2D>();
    }
	
	private void OnTriggerEnter2D(Collider2D collision) {
		if (collision.tag == "Fire") {
			collision.GetComponent<SpriteRenderer>().enabled = false;
			collision.GetComponent<BoxCollider2D>().enabled = false;
			collision.GetComponent<FireController>().enabled = false;
			GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>().GetComponent<AudioManager>().SFX.PlayOneShot(ErrorSound, .4f);
			IgnitePlayer();
		}
	}

	private void IgnitePlayer() {
		SpelunkyText.SetActive(false);
		PodcastText.SetActive(false);
		DudeText.SetActive(true);
		OneText.SetActive(true);
		var player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
		player.LoseText.text = "Can't take the critique? Get out of the kitchen.";
		player.Die();
		Camera.main.GetComponent<HorizontalShake>().DoShake();
		GameObject.Instantiate(FirePrefab, player.transform.position, Quaternion.identity);
	}

	public void ShootPlayer() {
		// only shoot one;
		if (_hasShot)
			return;
		_hasShot = true;

		SpelunkyText.SetActive(false);
		PodcastText.SetActive(false);
		WhyText.SetActive(true);
		NeverText.SetActive(true);
		EggplantText.SetActive(true);
		var bullet = GameObject.Instantiate(ProjectilePrefab, transform.position, Quaternion.identity);
		bullet.GetComponent<AndyBulletController>().Init(GameObject.FindGameObjectWithTag("Player"));
	}
}
