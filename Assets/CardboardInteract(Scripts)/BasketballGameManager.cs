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

    [Header("Ääniefektit")]
    public AudioClip correctSound;
    public AudioClip wrongSound;
    private AudioSource audioSource;

    [Header("Pelin asetukset")]
    public int score = 0;
    public int targetScore = 10;
    public float feedbackDuration = 2f;

    [Header("Peli läpi")]
    public UnityEvent onGameCompleted;

    private int correctAnswer;
    private bool isProcessing = false;

    // Medium-tason kierrostenhallinta
    private int _mediumRound = 0;
    private List<int> _mediumMultipliers = new List<int>();

    // Hard-tason kierrostenhallinta
    private int _hardRound = 0;
    private List<int> _hardMultipliers = new List<int>();

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        if (feedbackLight != null) feedbackLight.color = normalColor;
        GenerateNewQuestion();
    }

    private void ShuffleMediumMultipliers()
    {
        _mediumMultipliers = new List<int> { 1, 2, 3, 4, 5 };
        for (int i = _mediumMultipliers.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            int tmp = _mediumMultipliers[i];
            _mediumMultipliers[i] = _mediumMultipliers[j];
            _mediumMultipliers[j] = tmp;
        }
        _mediumRound = 0;
    }

    private void ShuffleHardMultipliers()
    {
        _hardMultipliers = new List<int> { 6, 7, 8, 9, 10 };
        for (int i = _hardMultipliers.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            int tmp = _hardMultipliers[i];
            _hardMultipliers[i] = _hardMultipliers[j];
            _hardMultipliers[j] = tmp;
        }
        _hardRound = 0;
    }

    public void CheckAnswer(int submittedAnswer)
    {
        if (isProcessing) return;

        if (submittedAnswer == correctAnswer)
        {
            score++;
            if (score >= targetScore)
            {
                onGameCompleted.Invoke();
            }
            OnCorrectAnswer();
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

    IEnumerator ShowFeedback(string message, Color color)
    {
        isProcessing = true;
        questionText.text = message;
        questionText.color = color;

        if (feedbackLight != null) feedbackLight.color = color;

        yield return new WaitForSeconds(feedbackDuration);

        if (feedbackLight != null) feedbackLight.color = normalColor;

        isProcessing = false;
        GenerateNewQuestion();
    }

    public void GenerateNewQuestion()
    {
        if (score >= targetScore)
        {
            questionText.text = LanguageManager.CurrentLanguage == "FI"
                ? "Tehtävä valmis. Voit jatkaa seuraavaan huoneeseen"
                : "Task complete. You can continue to the next room";
            questionText.color = Color.yellow;
            if (feedbackLight != null) feedbackLight.color = Color.yellow;
            return;
        }

        Difficulty difficulty = DifficultyManager.Instance != null
            ? DifficultyManager.Instance.CurrentDifficulty
            : Difficulty.Easy;

        string questionString;
        GenerateQuestion(difficulty, out questionString, out correctAnswer);

        questionText.text = questionString;
        questionText.color = Color.white;

        List<int> answers = new List<int>();
        answers.Add(correctAnswer);

        int attempts = 0;
        while (answers.Count < 3 && attempts < 100)
        {
            attempts++;
            string dummy;
            int wrongAnswer;
            GenerateQuestion(difficulty, out dummy, out wrongAnswer);
            if (!answers.Contains(wrongAnswer)) answers.Add(wrongAnswer);
        }

        for (int i = 0; i < answers.Count; i++)
        {
            int temp = answers[i];
            int randomIndex = Random.Range(i, answers.Count);
            answers[i] = answers[randomIndex];
            answers[randomIndex] = temp;
        }

        for (int i = 0; i < basketTexts.Length; i++)
        {
            if (basketTexts[i] != null)
                basketTexts[i].text = answers[i].ToString();
        }
    }

    private void GenerateQuestion(Difficulty difficulty, out string questionString, out int answer)
    {
        int a, b;

        switch (difficulty)
        {
            // EASY: Yhteenlasku, luvut 1–10
            case Difficulty.Easy:
                a = Random.Range(1, 11);
                b = Random.Range(1, 11);
                answer = a + b;
                questionString = a + " + " + b + " = ?";
                break;

            // MEDIUM: Kertotaulu 1–5, sekoitettu järjestys, vaikeutetut luvut
            case Difficulty.Medium:
            {
                if (_mediumRound >= _mediumMultipliers.Count || _mediumMultipliers.Count == 0)
                    ShuffleMediumMultipliers();

                int multiplier = _mediumMultipliers[_mediumRound];
                _mediumRound++;

                List<int> excluded;
                switch (multiplier)
                {
                    case 1:  excluded = new List<int> { 1, 2, 3, 10 }; break;
                    case 2:  excluded = new List<int> { 1, 2, 10 };    break;
                    case 3:  excluded = new List<int> { 1, 2, 3, 10 }; break;
                    case 4:  excluded = new List<int> { 1, 2, 4, 10 }; break;
                    case 5:  excluded = new List<int> { 1, 2, 5, 10 }; break;
                    default: excluded = new List<int> { 1, 10 };       break;
                }

                List<int> pool = new List<int>();
                for (int n = 1; n <= 10; n++)
                    if (!excluded.Contains(n))
                        pool.Add(n);

                a = multiplier;
                b = pool[Random.Range(0, pool.Count)];
                answer = a * b;
                questionString = a + " x " + b + " = ?";
                break;
            }

            // HARD: Kertotaulu 6–10, sekoitettu järjestys, vaikeutetut luvut
            case Difficulty.Hard:
            default:
            {
                if (_hardRound >= _hardMultipliers.Count || _hardMultipliers.Count == 0)
                    ShuffleHardMultipliers();

                int multiplier = _hardMultipliers[_hardRound];
                _hardRound++;

                List<int> excluded;
                switch (multiplier)
                {
                    case 6:  excluded = new List<int> { 1, 2, 6, 10 };  break;
                    case 7:  excluded = new List<int> { 1, 2, 7, 10 };  break;
                    case 8:  excluded = new List<int> { 1, 2, 8, 10 };  break;
                    case 9:  excluded = new List<int> { 1, 2, 9, 10 };  break;
                    case 10: excluded = new List<int> { 1, 2, 5, 10 };  break;
                    default: excluded = new List<int> { 1, 10 };        break;
                }

                List<int> pool = new List<int>();
                for (int n = 1; n <= 10; n++)
                    if (!excluded.Contains(n))
                        pool.Add(n);

                a = multiplier;
                b = pool[Random.Range(0, pool.Count)];
                answer = a * b;
                questionString = a + " x " + b + " = ?";
                break;
            }
        }
    }
}