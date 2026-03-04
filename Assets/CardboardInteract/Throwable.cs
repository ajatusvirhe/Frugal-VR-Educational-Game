using UnityEngine;

public class Throwable : Interactive
{
    [SerializeField] float grabSpeed = 5f;
    [SerializeField] float throwForce = 15f; // Heittovoiman suuruus
    public bool useGravity = true;
    
    static Transform grabbed = null;
    static Transform cam = null;
    Rigidbody rb;
    float grabDistance = 0f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public new void Interact()
    {
        // Jos esine ei ole vielä kädessä, poimi se
        if (grabbed != transform)
        {
            grabbed = transform;
            grabDistance = Vector3.Distance(cam.position, transform.position);
            
            // Nollataan liike-energia poimiessa, ettei esine "sinkoa" heti
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        else // JOS esine on jo kädessä -> HEITÄ
        {
            grabbed = null;
            ThrowObject();
        }
    }

    void ThrowObject()
    {
        // Varmistetaan, että painovoima palaa heiton yhteydessä
        rb.useGravity = useGravity;
        
        // Lasketaan suunta (kamerasta eteenpäin) ja lisätään voima
        Vector3 forceDirection = cam.forward;
        rb.AddForce(forceDirection * throwForce, ForceMode.Impulse);
        
        Debug.Log("Heitettiin esine: " + gameObject.name);
    }

    void Update()
    {
        if (!cam && Camera.main)
            cam = Camera.main.transform;

        // Painovoima on pois päältä vain silloin, kun esine on kädessä
        rb.useGravity = grabbed != transform && useGravity;

        if (grabbed == transform)
        {
            Vector3 targetPoint = cam.position + cam.forward * grabDistance;
            rb.linearVelocity = (targetPoint - transform.position) * grabSpeed;
        }
    }
}
