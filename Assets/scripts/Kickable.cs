using UnityEngine;

public class Kickable : Interactive
{
    [SerializeField] float kickForce = 12f;
    [SerializeField] bool flattenDirection = true; // makes kick go along ground

    static Transform cam = null;

    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public new void Interact()
    {
        KickObject();
    }

    void KickObject()
    {
        if (!cam && Camera.main)
            cam = Camera.main.transform;

        Vector3 direction = cam.forward;

        // Optional: remove vertical component so ball rolls forward instead of flying
        if (flattenDirection)
        {
            direction.y = 0f;
            direction.Normalize();
        }

        rb.AddForce(direction * kickForce, ForceMode.Impulse);
    }
}