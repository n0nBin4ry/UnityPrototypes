using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KickBack : MonoBehaviour
{
    public float kick = 5;
    private Rigidbody2D rb;
    private Vector3 velocity_h;

    [Range (0f, 1f)]
    public float damp = 0.4f;
    [Range (1f, 5f)]
    public float vel = 1f;

    public void Kick(int back=1)
    {
        rb.velocity = new Vector3(-kick*back, 0, 0);
    }

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 pos = transform.localPosition;
        pos.x =
           Mathf.MoveTowards(pos.x, 0, vel * Time.deltaTime);
        transform.localPosition = pos;
        transform.localPosition *= (1-damp);
        rb.velocity *= (1-damp);
    }
}
