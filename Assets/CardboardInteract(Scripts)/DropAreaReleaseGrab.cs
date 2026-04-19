using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DropAreaReleaseGrab : MonoBehaviour
{
    [Header("Release Damping")]
    [SerializeField, Range(0f, 1f)] float linearVelocityMultiplier = 0.2f;
    [SerializeField, Range(0f, 1f)] float angularVelocityMultiplier = 0.2f;
    [SerializeField] bool clampUpwardVelocity = true;
    [SerializeField] float maxUpwardVelocityAfterRelease = 0f;

    private void OnTriggerEnter(Collider other)
    {
        Rigidbody enteredBody = other.attachedRigidbody;
        if (enteredBody == null)
            return;

        Grabbable grabbable = enteredBody.GetComponent<Grabbable>();
        if (grabbable == null)
            grabbable = enteredBody.GetComponentInParent<Grabbable>();

        if (grabbable != null && grabbable.IsGrabbed)
            grabbable.ForceReleaseGrab(
                linearVelocityMultiplier,
                angularVelocityMultiplier,
                clampUpwardVelocity,
                maxUpwardVelocityAfterRelease
            );
    }
}
