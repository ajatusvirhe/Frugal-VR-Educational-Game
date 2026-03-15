using UnityEngine;
using TMPro;

public class Ball : MonoBehaviour
{
    public float weight = 1f;          // actual weight
    private Vector3 originalPosition;  // store initial position
    private Quaternion originalRotation; // store initial rotation

    private void Start()
    {
        // Save original position and rotation
        originalPosition = transform.position;
        originalRotation = transform.rotation;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null) rb.mass = weight;

    }

    // Call this to reset ball to initial position
    public void ResetPosition()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        transform.position = originalPosition;
        transform.rotation = originalRotation;
    }
}