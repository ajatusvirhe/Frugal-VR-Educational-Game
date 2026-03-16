using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class BasketWeightChecker : MonoBehaviour
{
    [Header("Target generation (multiplication table up to 10)")]
    public int minFactor = 1;
    public int maxFactor = 10;

    [Header("References")]
    public Light basketLight;
    public TextMeshPro targetText;
    public TextMeshPro currentText;

    [Header("Behaviour")]
    public float successDelay = 2f;
    public float tolerance = 0.1f;
    public float correctTextDuration = 3f;

    [Header("Peli läpi")]
    private int roundsCompleted = 0;
    public UnityEvent onGameCompleted; // Tämä tapahtuma voidaan asettaa Unityn editorissa, esimerkiksi näyttämään onnitteluteksti tai siirtymään seuraavaan kohtaukseen pelin päätyttyä.

    private int targetWeight;
    private int lastFactorA;
    private int lastFactorB;

    private List<Rigidbody> objectsInBasket = new List<Rigidbody>();

    void Start()
    {
        GenerateNewTarget();
    }

    void GenerateNewTarget()
    {
        lastFactorA = Random.Range(minFactor, maxFactor + 1);
        lastFactorB = Random.Range(minFactor, maxFactor + 1);
        targetWeight = lastFactorA * lastFactorB;

        if (targetText != null)
            targetText.text = $"Tavoite paino: {lastFactorA} × {lastFactorB}";

        if (currentText != null)
            currentText.text = "Tämän hetkinen paino: 0";

        if (basketLight != null)
            basketLight.enabled = false;
    }

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
            totalWeight += rb.mass;

        int roundedWeight = Mathf.RoundToInt(totalWeight);
        
        if (currentText != null)
            currentText.text = "Tämän hetkinen paino: " + roundedWeight;

        if (Mathf.Abs(totalWeight - targetWeight) < tolerance)
        {
            if (basketLight != null)
            {
                basketLight.enabled = true;
                basketLight.color = Color.green;
            }

            roundsCompleted++;

            if (roundsCompleted >= 5) // Oletetaan, että peli vaatii 5 onnistunutta tehtävää voittoon
            {
                if (targetText != null)
                    targetText.text = $"Onnea! Olet saavuttanut tavoiteen!";
                if (currentText != null)
                    currentText.text = $"Voit nyt edetä!";
                winGame();
                return; // Lopeta metodin suoritus, jotta ei generoida uutta tavoitetta pelin päätyttyä
            }

            if (currentText != null)
                currentText.text = $"Oikein!";

            CancelInvoke(nameof(StartNextRound));
            Invoke(nameof(StartNextRound), correctTextDuration);
        }
        else
        {
            if (basketLight != null)
            {
                basketLight.enabled = true;
                basketLight.color = Color.red;
            }
        }
    }

    void ResetAllBalls()
    {
        Ball[] balls = FindObjectsOfType<Ball>();

        foreach (Ball ball in balls)
        {
            ball.ResetPosition();

            Grabbable grab = ball.GetComponent<Grabbable>();
            if (grab != null)
                grab.ResetBallState();
        }

        Debug.Log("All balls reset after correct answer");
    }

    void winGame()
    {
        Debug.Log("Peli voitettu! Kaikki tehtävät suoritettu.");
        onGameCompleted.Invoke(); // Kutsu tapahtuma, kun pelaaja saavuttaa tavoitepisteet
    }

    void StartNextRound()
{
    // RESET ALL BALLS
    ResetAllBalls();

    // CLEAR basket list
    objectsInBasket.Clear();

    GenerateNewTarget();
}
}