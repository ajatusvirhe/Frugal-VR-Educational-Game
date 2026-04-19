using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class BasketWeightChecker : MonoBehaviour
{
    private static int pendingRoundsCompleted = -1;

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
    public ParticleSystem BlingEffect;
    public ParticleSystem portalEffect;

    [Header("Ääniefektit")]
    public AudioClip correctSound;
    public AudioClip BlingSound;
    private AudioSource audioSource;

    [Header("Peli läpi")]
    private int roundsCompleted = 0;
    public UnityEvent onGameCompleted; // Tämä tapahtuma voidaan asettaa Unityn editorissa, esimerkiksi näyttämään onnitteluteksti tai siirtymään seuraavaan kohtaukseen pelin päätyttyä.

    [Header("Win Object Swap")]
    public GameObject objectToHideOnWin;
    public GameObject objectToShowOnWin;

    private int targetWeight;
    private int lastFactorA;
    private int lastFactorB;
    private bool isAdditionQuestion;

    private List<Rigidbody> objectsInBasket = new List<Rigidbody>();
    private bool isWinSequenceRunning = false;

    private bool IsFinnish => LanguageManager.CurrentLanguage == "FI";

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        if (pendingRoundsCompleted >= 0)
        {
            roundsCompleted = pendingRoundsCompleted;
            pendingRoundsCompleted = -1;
        }

        LogCurrentDifficulty();
        GenerateNewTarget();
    }

    public static void ReloadActiveSceneKeepingProgress()
    {
        BasketWeightChecker checker = FindObjectOfType<BasketWeightChecker>();
        pendingRoundsCompleted = checker != null ? checker.roundsCompleted : 0;

        Scene activeScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(activeScene.buildIndex);
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
            targetText.text = IsFinnish
                ? $"Tavoitepaino: {lastFactorA} {symbol} {lastFactorB}"
                : $"Target weight: {lastFactorA} {symbol} {lastFactorB}";
        }

        if (currentText != null)
            currentText.text = IsFinnish ? "Tämänhetkinen paino: 0" : "Current weight: 0";

        if (basketLight != null)
            basketLight.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        Rigidbody rb = other.attachedRigidbody;
        if (rb != null && !objectsInBasket.Contains(rb))
        {
            objectsInBasket.Add(rb);
            if (BlingEffect != null)
            {
                BlingEffect.transform.position = other.transform.position;
                BlingEffect.Play();
            }

            if (audioSource != null && BlingSound != null)
            audioSource.PlayOneShot(BlingSound);
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
        if (isWinSequenceRunning)
            return;

        float totalWeight = 0f;

        foreach (Rigidbody rb in objectsInBasket)
            totalWeight += rb.mass;

        int roundedWeight = Mathf.RoundToInt(totalWeight);
        
        if (currentText != null)
            currentText.text = (IsFinnish ? "Tämänhetkinen paino: " : "Current weight: ") + roundedWeight;

        if (Mathf.Abs(totalWeight - targetWeight) < tolerance)
        {
            if (basketLight != null)
            {
                basketLight.enabled = true;
                basketLight.color = Color.green;
                confettiEffect.Play();
            }

            roundsCompleted++;

            if (roundsCompleted >= 5) // Oletetaan, että peli vaatii 5 onnistunutta tehtävää voittoon
            {
                ResetAllBalls();
                objectsInBasket.Clear();

                if (targetText != null)
                    targetText.text = IsFinnish
                        ? "Onnea! Saavutit tavoitteen! Voit nyt edetä!"
                        : "Congratulations! You reached the target! You can now continue!";
                if (currentText != null)
                    currentText.text = $"";

                StartCoroutine(PlayWinEffectsThenComplete());
                return; // Lopeta metodin suoritus, jotta ei generoida uutta tavoitetta pelin päätyttyä
            }

            if (audioSource != null && correctSound != null)
                audioSource.PlayOneShot(correctSound);

            if (currentText != null)
                currentText.text = IsFinnish ? "Oikein!" : "Correct!";

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

    IEnumerator PlayWinEffectsThenComplete()
    {
        isWinSequenceRunning = true;

        if (audioSource != null && correctSound != null)
            audioSource.PlayOneShot(correctSound);

        if (portalEffect != null)
            portalEffect.Play();

        yield return new WaitForSeconds(1f);
        winGame();
    }

    void winGame()
    {
        if (objectToHideOnWin != null)
            objectToHideOnWin.SetActive(false);

        if (objectToShowOnWin != null)
            objectToShowOnWin.SetActive(true);

        Debug.Log("Peli voitettu! Kaikki tehtävät suoritettu.");
        onGameCompleted.Invoke(); // Kutsu tapahtuma, kun pelaaja saavuttaa tavoitepisteet
    }

    void StartNextRound()
{
    ReloadActiveSceneKeepingProgress();
}
}