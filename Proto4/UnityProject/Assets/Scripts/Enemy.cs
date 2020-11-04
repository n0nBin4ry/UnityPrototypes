using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    Rigidbody2D rb2D;
    public Vector2 velocity;
    Vector2 currVelocity;
    public int health = 1;
    bool inAir = false;
    public AudioSource deathSFX;

    // Start is called before the first frame update
    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();

        //Make them go left
        currVelocity = velocity;

        rb2D.velocity = currVelocity;
    }

    // Update is called once per frame
    void Update()
    {
        rb2D.velocity = currVelocity;

        //PROBABLY NOT CHECKING FOR THIS ANYMORE
        if (!inAir)
        {
            //rb2D.velocity = currVelocity;
        }
        else
        {
            //rb2D.velocity = currVelocity + Physics2D.gravity * Time.deltaTime * 20f;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
		// if hit by sword; reduce health
		if (collision.gameObject.tag == "Sword") {
			Debug.Log("hit by sword");
			health -= 1;
			// if dead, update player's stats and die
			if (health <= 0) {
				collision.gameObject.GetComponentInParent<PlayerController>().processKill();
				die();
				return;
			}
		}

		Vector2 normal = collision.contacts[0].normal;
        if (normal.x > 0f || normal.x < 0f || collision.gameObject.tag == "Enemy")
        {
            currVelocity.x *= -1f;
            rb2D.velocity = currVelocity;
        }
        else if(collision.gameObject.tag == "Ground" || normal.y > 0f)
        {
            inAir = false;
        }
    }

	// handle the death; sounds and/or animations here
	public void die() {
		// TODO
        if(deathSFX)
        {
            deathSFX.Play();
        }
		Destroy(gameObject);
	}

    private void OnCollisionExit2D(Collision2D collision)
    {
        /*Vector2 normal = collision.contacts[0].normal;
        if (collision.gameObject.tag == "Ground" || collision.gameObject.tag == "HorizontalObstacle")
        {
            inAir = true;
        }*/
    }

}
