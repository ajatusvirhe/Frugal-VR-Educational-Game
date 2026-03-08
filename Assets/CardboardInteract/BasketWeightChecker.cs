using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BasketWeightChecker : MonoBehaviour
{
    public float targetWeight = 10f;           // Target total weight
    public Light basketLight;                  // Assign your Unity Light here
    public TextMeshPro textDisplay;           // Assign TMP text UI

    private List<Rigidbody> objectsInBasket = new List<Rigidbody>();

    private void OnTriggerEnter(Collider other)
    {
        Rigidbody rb = other.attachedRigidbody;
        if (rb != null && !objectsInBasket.Contains(rb))
        {
            objectsInBasket.Add(rb);
            CheckWeight();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Rigidbody rb = other.attachedRigidbody;
        if (rb != null && objectsInBasket.Contains(rb))
        {
            objectsInBasket.Remove(rb);
            CheckWeight();
        }
    }

    void CheckWeight()
    {
        float totalWeight = 0f;
        foreach (Rigidbody rb in objectsInBasket)
        {
            totalWeight += rb.mass;
        }

        // Show weight only if not correct
        if (Mathf.Approximately(totalWeight, targetWeight))
        {
            basketLight.enabled = true;              // show the light
            basketLight.color = Color.green;         // green light
            textDisplay.gameObject.SetActive(true);  // show text
            textDisplay.text = "Congratulations!";
        }
        else
        {
            basketLight.enabled = true;              // show the light
            basketLight.color = Color.red;           // red light
            textDisplay.gameObject.SetActive(true);  // show weight
            textDisplay.text = "Weight: " + totalWeight.ToString("F2");
        }
    }
}