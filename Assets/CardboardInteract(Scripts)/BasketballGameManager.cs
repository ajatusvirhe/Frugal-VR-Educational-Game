using UnityEngine;
using TMPro;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class BasketballGameManager : MonoBehaviour
{
    [Header("Tekstielementit")]
    public TMP_Text questionText;
    public TMP_Text[] basketTexts;

    [Header("Valoasetukset")]
    public Light feedbackLight;
    public Color normalColor = Color.white;
    public Color correctColor = Color.green;
    public Color wrongColor = Color.red;

    [Header("Visuaaliefektit")]
    public ParticleSystem confettiEffect;

    [Header("Peli läpi -efektit")]
    public ParticleSystem completionEffect1;
    public ParticleSystem completionEffect2;

    [Header("Ääniefektit")]
    public AudioClip correctSound;
    public AudioClip wrongSound;

    [Header("Pelin asetukset")]
    public int targetScore = 5;
    public float feedbackDuration = 2f;

    [Header("Peli läpi")]
    public UnityEvent onGameCompleted;

    // Score is private to prevent accidental external modification
    private int score;
    private int correctAnswer;
    private bool isProcessing;
    private bool gameCompleted;
    private AudioSource audioSource;

    // Multiplier rotation for Medium (1-5) and Hard (6-10)
    private int _mediumRound;
    private int _hardRound;
    private readonly List<int> _mediumMultipliers = new List<int>();
    private readonly List<int> _hardMultipliers = new List<int>();

    // Excluded second-factors per multiplier, to keep questions non-trivial
    private static readonly Dictionary<int, List<int>> MultiplierExclusions = new Dictionary<int, List<int>>
    {
        { 1,  new List<int> { 1, 2, 3, 10 } },
        { 2,  new List<int> { 1, 2, 10 } },
        { 3,  new List<int> { 1, 2, 3, 10 } },
        { 4,  new List<int> { 1, 2, 4, 10 } },
        { 5,  new List<int> { 1, 2, 5, 10 } },
        { 6,  new List<int> { 1, 2, 6, 10 } },
        { 7,  new List<int> { 1, 2, 7, 10 } },
        { 8,  new List<int> { 1, 2, 8, 10 } },
        { 9,  new List<int> { 1, 2, 9, 10 } },
        { 10, new List<int> { 1, 2, 5, 10 } },
    };

    // -------------------------------------------------------------------------
    // Unity lifecycle
    // -------------------------------------------------------------------------

    void Start()
    {
        // GetComponent first - avoids stacking AudioSources if Start runs again
        audioSource = gameObject.GetComponent<AudioSource>()
                      ?? gameObject.AddComponent<AudioSource>();

        if (feedbackLight != null)
            feedbackLight.color = normalColor;

        GenerateNewQuestion();
    }

    // -------------------------------------------------------------------------
    // Answer handling
    // -------------------------------------------------------------------------

    public void CheckAnswer(int submittedAnswer)
    {
        if (isProcessing || gameCompleted) return;

        if (submittedAnswer == correctAnswer)
        {
            score++;
            OnCorrectAnswer();

            if (score >= targetScore)
            {
                gameCompleted = true;
                StartCoroutine(TriggerGameCompleted());
            }

        }
        else
        {
            OnWrongAnswer();
        }
    }

    private void OnCorrectAnswer()
    {
        if (confettiEffect != null) confettiEffect.Play();
        if (audioSource != null && correctSound != null) audioSource.PlayOneShot(correctSound);

        string msg = LanguageManager.CurrentLanguage == "FI" ? "Oikein!" : "Correct!";
        StartCoroutine(ShowFeedback(msg, correctColor));
    }

    private void OnWrongAnswer()
    {
        if (audioSource != null && wrongSound != null) audioSource.PlayOneShot(wrongSound);

        string msg = LanguageManager.CurrentLanguage == "FI"
            ? "Väärin, oikea vastaus on " + correctAnswer
            : "Wrong, the correct answer is " + correctAnswer;

        StartCoroutine(ShowFeedback(msg, wrongColor));
    }

    // -------------------------------------------------------------------------
    // Feedback coroutine
    // -------------------------------------------------------------------------

    private IEnumerator ShowFeedback(string message, Color color)
    {
        isProcessing = true;

        questionText.text = message;
        questionText.color = color;

        if (feedbackLight != null) feedbackLight.color = color;

        yield return new WaitForSeconds(feedbackDuration);

        if (feedbackLight != null) feedbackLight.color = normalColor;

        isProcessing = false;

        if (!gameCompleted)
            GenerateNewQuestion();
    }

    // -------------------------------------------------------------------------
    // Game completion
    // -------------------------------------------------------------------------

    // Waits for ShowFeedback to finish, then fires all completion behaviour
    private IEnumerator TriggerGameCompleted()
    {
        yield return new WaitUntil(() => !isProcessing);

        onGameCompleted.Invoke();
        StartCompletionEffects();
        ShowCompletionText();
    }

    private void ShowCompletionText()
    {
        questionText.text = LanguageManager.CurrentLanguage == "FI"
            ? "Voit jatkaa seuraavaan huoneeseen"
            : "You can continue to the next room";
        questionText.color = Color.yellow;
        if (feedbackLight != null) feedbackLight.color = Color.yellow;
    }

    private void StartCompletionEffects()
    {
        if (completionEffect1 != null) completionEffect1.Play();
        if (completionEffect2 != null) completionEffect2.Play();
    }

    // -------------------------------------------------------------------------
    // Question generation
    // -------------------------------------------------------------------------

    public void GenerateNewQuestion()
    {
        GenerateQuestion(CurrentDifficulty(), out string questionString, out correctAnswer);
        questionText.text = questionString;
        questionText.color = Color.white;

        List<int> answers = BuildAnswerList(correctAnswer);

        for (int i = 0; i < basketTexts.Length; i++)
        {
            if (basketTexts[i] != null)
                basketTexts[i].text = i < answers.Count ? answers[i].ToString() : "?";
        }
    }

    private Difficulty CurrentDifficulty()
    {
        return DifficultyManager.Instance != null
            ? DifficultyManager.Instance.CurrentDifficulty
            : Difficulty.Easy;
    }

    // Builds a shuffled list of [correctAnswer, wrongAnswer1, wrongAnswer2]
    private List<int> BuildAnswerList(int correct)
    {
        List<int> answers = new List<int> { correct };
        answers.AddRange(GetWrongAnswers(correct, needed: 2));
        ShuffleList(answers);
        return answers;
    }

    // Generates distractor answers without touching round counters
    private List<int> GetWrongAnswers(int correct, int needed)
    {
        List<int> result = new List<int>();
        int attempts = 0;

        while (result.Count < needed && attempts < 200)
        {
            attempts++;
            int candidate = GenerateRandomAnswer(CurrentDifficulty());
            if (candidate != correct && !result.Contains(candidate))
                result.Add(candidate);
        }

        return result;
    }

    // Generates a plausible answer for the given difficulty (used only for distractors)
    private int GenerateRandomAnswer(Difficulty difficulty)
    {
        switch (difficulty)
        {
            case Difficulty.Easy:
                return Random.Range(1, 11) + Random.Range(1, 11);

            case Difficulty.Medium:
            {
                int m = Random.Range(1, 6);
                List<int> pool = BuildPool(m);
                return m * pool[Random.Range(0, pool.Count)];
            }

            case Difficulty.Hard:
            default:
            {
                int m = Random.Range(6, 11);
                List<int> pool = BuildPool(m);
                return m * pool[Random.Range(0, pool.Count)];
            }
        }
    }

    private void GenerateQuestion(Difficulty difficulty, out string questionString, out int answer)
    {
        int a, b;

        switch (difficulty)
        {
            case Difficulty.Easy:
                a = Random.Range(1, 11);
                b = Random.Range(1, 11);
                answer = a + b;
                questionString = $"{a} + {b} = ?";
                break;

            case Difficulty.Medium:
                GenerateMultiplicationQuestion(_mediumMultipliers, ref _mediumRound, 1, 5, out a, out b);
                answer = a * b;
                questionString = $"{a} x {b} = ?";
                break;

            case Difficulty.Hard:
            default:
                GenerateMultiplicationQuestion(_hardMultipliers, ref _hardRound, 6, 10, out a, out b);
                answer = a * b;
                questionString = $"{a} x {b} = ?";
                break;
        }
    }

    // Shared logic for Medium and Hard multiplication questions
    private void GenerateMultiplicationQuestion(
        List<int> multiplierList, ref int round,
        int rangeMin, int rangeMax,
        out int a, out int b)
    {
        if (round >= multiplierList.Count || multiplierList.Count == 0)
        {
            multiplierList.Clear();
            for (int n = rangeMin; n <= rangeMax; n++) multiplierList.Add(n);
            ShuffleList(multiplierList);
            round = 0;
        }

        a = multiplierList[round];
        round++;

        List<int> pool = BuildPool(a);
        b = pool[Random.Range(0, pool.Count)];
    }

    // Builds the allowed second-factor pool for a given multiplier
    private static List<int> BuildPool(int multiplier)
    {
        List<int> excluded = MultiplierExclusions.ContainsKey(multiplier)
            ? MultiplierExclusions[multiplier]
            : new List<int> { 1, 10 };

        List<int> pool = new List<int>();
        for (int n = 1; n <= 10; n++)
            if (!excluded.Contains(n))
                pool.Add(n);

        return pool;
    }

    // -------------------------------------------------------------------------
    // Utilities
    // -------------------------------------------------------------------------

    private static void ShuffleList<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            T tmp = list[i];
            list[i] = list[j];
            list[j] = tmp;
        }
    }
}