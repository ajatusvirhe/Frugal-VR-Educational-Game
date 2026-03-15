using UnityEngine;

public class ReturnToPoint : MonoBehaviour
{
    [Header("Asetukset")]
    public Transform returnTarget; 
    
    private Vector3 startPosition;
    private Quaternion startRotation;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Tallennetaan paikka, jossa pallo on kun peli alkaa
        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    // Tämä funktio suoritetaan heti, kun pallo osuu johonkin
    private void OnCollisionEnter(Collision collision)
    {
        // Jos pallo osuu kohteeseen, jonka tägi on "Floor"
        if (collision.gameObject.CompareTag("Floor"))
        {
            Return();
        }
    }

    public void Return()
    {
        if (returnTarget != null)
        {
            transform.position = returnTarget.position;
            transform.rotation = returnTarget.rotation;
        }
        else
        {
            transform.position = startPosition;
            transform.rotation = startRotation;
        }

        // Nollataan fysiikat, ettei pallo liiku palautuksen jälkeen
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        Debug.Log("Pallo osui maahan ja palautettiin.");
    }
}