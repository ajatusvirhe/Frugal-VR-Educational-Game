using UnityEngine;
using TMPro;

public class Ball : MonoBehaviour
{
    public float weight = 1f;          // actual weight
    private Vector3 originalPosition;  // store initial position
    private Quaternion originalRotation; // store initial rotation
    private bool hasCachedStartTransform;

    private void Awake()
    {
        // Cache spawn transform as early as possible so reset is reliable even if called quickly.
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        hasCachedStartTransform = true;
    }

    private void Start()
    {
        if (!hasCachedStartTransform)
        {
            originalPosition = transform.position;
            originalRotation = transform.rotation;
            hasCachedStartTransform = true;
        }

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null) rb.mass = weight;

    }

    // Call this to reset ball to initial position
    public void ResetPosition()
    {
        if (!hasCachedStartTransform)
        {
            originalPosition = transform.position;
            originalRotation = transform.rotation;
            hasCachedStartTransform = true;
        }

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