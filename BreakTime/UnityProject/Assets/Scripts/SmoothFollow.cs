using UnityEngine;
using System.Collections;

public class SmoothFollow : MonoBehaviour
{
    public Transform target;
    public float smoothTime = 0.3F;
    private Vector3 velocity_v = Vector3.zero;
    private Vector3 diff;
    
    private void Start()
    {
        diff = transform.position - target.position;
    }
    
    void FixedUpdate()
    {
        // Define a target position above and behind the target transform
        Vector3 targetPosition = target.TransformPoint(diff);
        
        // Smoothly move the camera towards that target position
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity_v, smoothTime);
        var temp = transform.position;
        temp.x = targetPosition.x;
        transform.position = temp;
    }
}
