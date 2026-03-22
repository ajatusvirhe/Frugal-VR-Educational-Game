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

    [Header("Efektit")]
    public ParticleSystem confettiEffect;

    [Header("Pelin asetukset")]
    public int score = 0;
    public int targetScore = 10;
    public float feedbackDuration = 2f;

    [Header("Peli läpi")]
    public UnityEvent onGameCompleted;

    private int correctAnswer;
    private bool isProcessing = false;

    void Start()
    {
        if (feedbackLight != null) feedbackLight.color = normalColor;
        GenerateNewQuestion();
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
        StartCoroutine(ShowFeedback("Oikein!", correctColor));
    }

    private void OnWrongAnswer()
    {
        StartCoroutine(ShowFeedback("Väärin, oikea vastaus on " + correctAnswer, wrongColor));
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
            questionText.text = "Tehtävä valmis. Voit jatkaa seuraavaan huoneeseen";
            questionText.color = Color.yellow;
            if (feedbackLight != null) feedbackLight.color = Color.yellow;
            return;
        }

        // Luetaan vaikeustaso — jos manageria ei löydy, käytetään oletuksena Easy
        Difficulty difficulty = DifficultyManager.Instance != null
            ? DifficultyManager.Instance.CurrentDifficulty
            : Difficulty.Easy;

        string questionString;
        GenerateQuestion(difficulty, out questionString, out correctAnswer);

        questionText.text = questionString;
        questionText.color = Color.white;

        // Generoidaan väärät vastaukset
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

        // Sekoitetaan vastaukset
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

    // GenerateQuestion — kaikki vaikeustasologiikka on tässä metodissa
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

            // MEDIUM: Kertotaulu 1–5 sekä kertotaulu x10
            case Difficulty.Medium:
                // 50/50 mahdollisuus saada x10-kysymys tai 1–5-kertotaulukysymys
                if (Random.value > 0.5f)
                {
                    // Kertotaulu x10: a on 1–10, b on aina 10
                    a = Random.Range(1, 11);
                    b = 10;
                }
                else
                {
                    // Kertotaulu 1–5: molemmat luvut väliltä 1–5
                    a = Random.Range(1, 6);
                    b = Random.Range(1, 6);
                }
                answer = a * b;
                questionString = a + " x " + b + " = ?";
                break;

            // HARD: Kertotaulu 1–10 (alkuperäinen logiikka)
            case Difficulty.Hard:
            default:
                a = Random.Range(1, 11);
                b = Random.Range(1, 11);
                answer = a * b;
                questionString = a + " x " + b + " = ?";
                break;
        }
    }
}