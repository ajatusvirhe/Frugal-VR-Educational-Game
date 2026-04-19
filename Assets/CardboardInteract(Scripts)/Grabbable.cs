using UnityEngine;

public class Grabbable : Interactive
{
    [SerializeField] float grabSpeed = 5f;
    public bool useGravity = true;

    int ballLayer;
    int defaultLayer;

    static Transform cam = null;

    Rigidbody rb;
    Collider col;

    float grabDistance = 0f;

    bool firstGrab = false;
    bool isGrabbed = false;

    public bool IsGrabbed => isGrabbed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        ballLayer = LayerMask.NameToLayer("Ball");
        defaultLayer = LayerMask.NameToLayer("Default");
    }

    private void Start()
    {
        if (ballLayer != -1 && defaultLayer != -1)
        {
            // Keep all Ball-layer objects from colliding with Default before grab.
            Physics.IgnoreLayerCollision(ballLayer, defaultLayer, true);
            gameObject.layer = ballLayer;
        }

        if (rb != null)
            rb.useGravity = false; // optional: start without gravity
    }

    public new void Interact()
    {
        if (!firstGrab)
        {
            firstGrab = true;

            // Switch to Default layer so physics collisions start working
            gameObject.layer = LayerMask.NameToLayer("Default");

            // Enable gravity on first grab if needed
            rb.useGravity = useGravity;
        }

        // Toggle grab state
        isGrabbed = !isGrabbed;

        if (isGrabbed && cam != null)
        {
            grabDistance = Vector3.Distance(cam.position, transform.position);
        }
    }

    void Update()
    {
        if (!cam && Camera.main)
            cam = Camera.main.transform;

        if (isGrabbed)
        {
            Vector3 targetPoint = cam.position + cam.forward * grabDistance;
            rb.linearVelocity = (targetPoint - transform.position) * grabSpeed;
        }
    }

    public void ForceReleaseGrab()
    {
        ForceReleaseGrab(0f, 0f, true, 0f);
    }

    public void ForceReleaseGrab(float linearVelocityMultiplier, float angularVelocityMultiplier, bool clampUpwardVelocity = true, float maxUpwardVelocity = 0f)
    {
        isGrabbed = false;

        if (rb == null)
            rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            Vector3 reducedLinearVelocity = rb.linearVelocity * Mathf.Clamp01(linearVelocityMultiplier);
            if (clampUpwardVelocity && reducedLinearVelocity.y > maxUpwardVelocity)
                reducedLinearVelocity.y = maxUpwardVelocity;

            rb.linearVelocity = reducedLinearVelocity;
            rb.angularVelocity *= Mathf.Clamp01(angularVelocityMultiplier);
        }
    }

    public void ResetBallState()
    {
        firstGrab = false;
        isGrabbed = false;

        if (rb == null)
            rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.useGravity = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // Put it back on Ball layer and ignore collisions
        if (ballLayer == -1)
            ballLayer = LayerMask.NameToLayer("Ball");

        if (ballLayer != -1)
            gameObject.layer = ballLayer;
    }
}