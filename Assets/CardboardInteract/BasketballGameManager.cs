using UnityEngine;
using TMPro;
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
            OnCorrectAnswer();
        }
        else
        {
            OnWrongAnswer();
        }
    }

    private void OnCorrectAnswer()
    {
        if (confettiEffect != null)
        {
            confettiEffect.Play();
        }

        StartCoroutine(ShowFeedback("Correct!", correctColor));
    }

    private void OnWrongAnswer()
    {
        StartCoroutine(ShowFeedback("Incorrect, correct answer is " + correctAnswer, wrongColor));
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
            questionText.text = "You win! Game over.";
            questionText.color = Color.yellow;
            if (feedbackLight != null) feedbackLight.color = Color.yellow;
            return;
        }

        int luku1 = Random.Range(1, 11);
        int luku2 = Random.Range(1, 11);
        correctAnswer = luku1 * luku2;
        
        questionText.text = luku1 + " x " + luku2 + " = ?";
        questionText.color = Color.white;

        List<int> answers = new List<int>();
        answers.Add(correctAnswer);

        while (answers.Count < 3)
        {
            int vaaraLuku1 = Random.Range(1, 11);
            int vaaraLuku2 = Random.Range(1, 11);
            int wrongAnswer = vaaraLuku1 * vaaraLuku2;
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
}