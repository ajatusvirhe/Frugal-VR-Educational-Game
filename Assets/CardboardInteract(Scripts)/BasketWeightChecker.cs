using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class BasketWeightChecker : MonoBehaviour
{
    [Header("Hard difficulty multiplication range")]
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
    private bool isAdditionQuestion;

    private List<Rigidbody> objectsInBasket = new List<Rigidbody>();

    void Start()
    {
        LogCurrentDifficulty();
        GenerateNewTarget();
    }

    void LogCurrentDifficulty()
    {
        Difficulty activeDifficulty = DifficultyManager.Instance != null
            ? DifficultyManager.Instance.CurrentDifficulty
            : Difficulty.Hard;

        Debug.Log($"BasketWeightChecker difficulty: {activeDifficulty}");
    }

    void GenerateNewTarget()
    {
        Difficulty activeDifficulty = DifficultyManager.Instance != null
            ? DifficultyManager.Instance.CurrentDifficulty
            : Difficulty.Hard;

        isAdditionQuestion = false;

        switch (activeDifficulty)
        {
            case Difficulty.Easy:
                // Easy: addition with numbers 1-10.
                lastFactorA = Random.Range(1, 11);
                lastFactorB = Random.Range(1, 11);
                targetWeight = lastFactorA + lastFactorB;
                isAdditionQuestion = true;
                break;

            case Difficulty.Medium:
                // Medium: multiplication tables 1-5 and 10.
                int[] mediumTables = { 1, 2, 3, 4, 5, 10 };
                lastFactorA = mediumTables[Random.Range(0, mediumTables.Length)];
                lastFactorB = Random.Range(1, 11);
                targetWeight = lastFactorA * lastFactorB;
                break;

            case Difficulty.Hard:
            default:
                // Hard: multiplication 1-10 (existing behavior).
                lastFactorA = Random.Range(minFactor, maxFactor + 1);
                lastFactorB = Random.Range(minFactor, maxFactor + 1);
                targetWeight = lastFactorA * lastFactorB;
                break;
        }

        if (targetText != null)
        {
            string symbol = isAdditionQuestion ? "+" : "×";
            targetText.text = $"Tavoite paino: {lastFactorA} {symbol} {lastFactorB}";
        }

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