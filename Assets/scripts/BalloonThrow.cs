using UnityEngine;

public class BalloonThrow : Interactive
{
    [Tooltip("Desired apex height (meters) above the current position.")]
    [SerializeField]
    private float _apexHeight = 1.5f;

    [Tooltip("If true the Rigidbody will be kinematic until thrown.")]
    [SerializeField]
    private bool _startKinematic = true;

    private Rigidbody _rb;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.useGravity = true;

        if (_startKinematic)
        {
            // keep the balloon in place until first interact/throw
            //_rb.isKinematic = true;
        }
    }

    public new void Interact()
    {
        // make sure physics will move the object
        if (_rb.isKinematic)
        {
            _rb.isKinematic = false;
        }

        // compute initial upward velocity to reach desired apex:
        // v = sqrt(2 * g * h), where g is gravity magnitude
        float g = -Physics.gravity.y;
        if (g <= 0.0001f)
        {
            g = 9.81f;
        }
        float initialVelocity = Mathf.Sqrt(2f * g * Mathf.Max(0f, _apexHeight));

        // set upward velocity (preserves any existing horizontal velocity)
        Vector3 vel = _rb.linearVelocity;
        vel.y = initialVelocity;
        _rb.linearVelocity = vel;
    }
}
