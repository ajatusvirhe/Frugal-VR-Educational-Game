using UnityEngine;

public class Throwable : Interactive
{
    [SerializeField] float grabSpeed = 7f;      // Kuinka nopeasti esine seuraa pelaajaa, kun se on kädessä
    [SerializeField] float throwForce = 15f; 
    [SerializeField] float holdDistance = 2f;   // Etäisyys, jolla esine pysyy kamerasta, kun se on kädessä
    
    public bool useGravity = true;
    
    static Transform grabbed = null;
    static Transform cam = null;
    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public new void Interact()
    {
        if (grabbed != transform)
        {
            // Jos jokin muu on jo kädessä, se pitää pudottaa ensin
            grabbed = transform;
            
            // Nollataan fysiikat, jotta veto pelaajaa kohti on tasainen
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        else 
        {
            grabbed = null;
            ThrowObject();
        }
    }

    void ThrowObject()
    {
        rb.useGravity = useGravity;
        Vector3 forceDirection = cam.forward;
        rb.AddForce(forceDirection * throwForce, ForceMode.Impulse);
    }

    void Update()
    {
        if (!cam && Camera.main)
            cam = Camera.main.transform;

        rb.useGravity = (grabbed != transform) && useGravity;

        if (grabbed == transform)
        {
            // Nyt targetPoint on AINA tietyn matkan päässä kamerasta (holdDistance)
            Vector3 targetPoint = cam.position + cam.forward * holdDistance;
            
            rb.linearVelocity = (targetPoint - transform.position) * grabSpeed;
        }
    }
}
