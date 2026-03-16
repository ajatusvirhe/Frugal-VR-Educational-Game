using UnityEngine;

public class Grabbable : Interactive
{
    [SerializeField] float grabSpeed = 5f;
    public bool useGravity = true;

    static Transform grabbed = null;
    static Transform cam = null;

    Rigidbody rb;
    float grabDistance = 0f;

    bool gravityActivated = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public new void Interact()
    {
        if (!gravityActivated)
        {
            rb.useGravity = true;
            gravityActivated = true;
        }

        if (grabbed != transform)
        {
            grabbed = transform;
            grabDistance = Vector3.Distance(cam.position, transform.position);
        }
        else
            grabbed = null;
    }

    void Update()
    {
        if (!cam && Camera.main)
            cam = Camera.main.transform;

        if (grabbed == transform)
        {
            Vector3 targetPoint = cam.position + cam.forward * grabDistance;
            rb.linearVelocity = (targetPoint - transform.position) * grabSpeed;
        }
    }
    public void ResetBallState()
    {
        gravityActivated = false;
        rb.useGravity = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
}