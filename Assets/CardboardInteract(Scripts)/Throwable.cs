using UnityEngine;

public class Throwable : Interactive
{
    [SerializeField] float grabSpeed = 8f; 
    [SerializeField] float throwForce = 15f; 
    
    [Header("Pito-asetukset")]
    [SerializeField] float holdDistance = 3.5f; // Pallo pysyy kauempana (kokeile 3-4)
    [SerializeField] float verticalOffset = -0.5f; // Pallo laskeutuu hieman alemmas näkökentästä
    
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
            grabbed = transform;
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
        // Lisätään heittoon pieni yläviisto, jos pallo on alempana
        rb.AddForce((forceDirection + Vector3.up * 0.1f) * throwForce, ForceMode.Impulse);
    }

    void Update()
    {
        if (!cam && Camera.main)
            cam = Camera.main.transform;

        rb.useGravity = (grabbed != transform) && useGravity;

        if (grabbed == transform)
        {
            // Lasketaan paikka: Kamera + Suunta * Etäisyys + Pieni pudotus alaspäin
            Vector3 targetPoint = cam.position + (cam.forward * holdDistance);
            
            rb.linearVelocity = (targetPoint - transform.position) * grabSpeed;
        }
    }
}