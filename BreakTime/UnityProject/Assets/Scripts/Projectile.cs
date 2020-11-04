using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed;
    public Vector3 direction;
    public GameObject vfx;

    Rigidbody2D rb;
    // Start is called before the first frame update
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        direction = transform.right;
        Destroy(gameObject, 4f);
    }
    private void Update()
    {
        rb.velocity = direction * speed;
    }
    private void OnDestroy()
    {
        Instantiate(vfx, transform.position, Quaternion.identity);
    }
    private void ApplyDamage(Collision2D collision) {
        var c = collision.transform.GetComponent<Crate>();
        if (c != null)
        {
            c.Damage();
        }
	}
	private void OnCollisionEnter2D(Collision2D collision) {
		ApplyDamage(collision);
		Destroy(gameObject);
	}
}
