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

    [Header("Efektit")]
    public ParticleSystem confettiEffect;
    public ParticleSystem splashEffect;

    [Header("Ääniefektit")]
    public AudioClip correctSound;
    public AudioClip watersplashSound;
    private AudioSource audioSource;

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
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

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
                // Medium: fixed 5-question progression.
                switch (roundsCompleted)
                {
                    case 0:
                        // First question: (1 or 10) x (1-10)
                        lastFactorA = Random.value < 0.5f ? 1 : 10;
                        lastFactorB = Random.Range(1, 11);
                        break;

                    case 1:
                        // Second question: 2 x (2-9)
                        lastFactorA = 2;
                        lastFactorB = Random.Range(2, 10);
                        break;

                    case 2:
                        // Third question: 3 x (3-9)
                        lastFactorA = 3;
                        lastFactorB = Random.Range(3, 10);
                        break;

                    case 3:
                        // Fourth question: 4 x (3-9)
                        lastFactorA = 4;
                        lastFactorB = Random.Range(3, 10);
                        break;

                    case 4:
                        // Fifth question: 5 x (3-9)
                        lastFactorA = 5;
                        lastFactorB = Random.Range(3, 10);
                        break;

                    default:
                        // Fallback for any unexpected extra rounds.
                        lastFactorA = Random.Range(1, 11);
                        lastFactorB = Random.Range(1, 11);
                        break;
                }

                targetWeight = lastFactorA * lastFactorB;
                break;

            case Difficulty.Hard:
            default:
                // Hard: fixed 5-question progression.
                switch (roundsCompleted)
                {
                    case 0:
                        // First question: 6 x (3-9)
                        lastFactorA = 6;
                        lastFactorB = Random.Range(3, 10);
                        break;

                    case 1:
                        // Second question: 7 x (3-9)
                        lastFactorA = 7;
                        lastFactorB = Random.Range(3, 10);
                        break;

                    case 2:
                        // Third question: 8 x (3-9)
                        lastFactorA = 8;
                        lastFactorB = Random.Range(3, 10);
                        break;

                    case 3:
                        // Fourth question: 9 x (3-9)
                        lastFactorA = 9;
                        lastFactorB = Random.Range(3, 10);
                        break;

                    case 4:
                        // Fifth question: (6-9) x (3-9)
                        lastFactorA = Random.Range(6, 10);
                        lastFactorB = Random.Range(3, 10);
                        break;

                    default:
                        // Fallback for any unexpected extra rounds.
                        lastFactorA = Random.Range(minFactor, maxFactor + 1);
                        lastFactorB = Random.Range(minFactor, maxFactor + 1);
                        break;
                }

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
                    //läiskis efekti
            if (splashEffect != null)
            {
                splashEffect.transform.position = other.transform.position;
                splashEffect.Play();
            }

            if (audioSource != null && watersplashSound != null)
                audioSource.PlayOneShot(watersplashSound);

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
            if (audioSource != null && correctSound != null)
                audioSource.PlayOneShot(correctSound);

            if (basketLight != null)
            {
                basketLight.enabled = true;
                basketLight.color = Color.green;
                confettiEffect.Play();
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