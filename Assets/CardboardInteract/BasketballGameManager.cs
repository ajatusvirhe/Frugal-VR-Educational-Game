using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class BasketballGameManager : MonoBehaviour
{
    [Header("Tekstielementit")]
    public TMP_Text questionText;
    public TMP_Text[] basketTexts;

    [Header("Pelin asetukset")]
    public int score = 0;
    public int targetScore = 10;
    public float feedbackDuration = 2f;

    private int correctAnswer;
    private bool isProcessing = false;

    void Start()
    {
        GenerateNewQuestion();
    }

    public void GenerateNewQuestion()
    {
        if (score >= targetScore)
        {
            questionText.text = "You win! Game over.";
            questionText.color = Color.yellow;
            return;
        }

        // Arvotaan molemmat luvut väliltä 1-10
        int luku1 = Random.Range(1, 11);
        int luku2 = Random.Range(1, 11);
        correctAnswer = luku1 * luku2;
        
        questionText.text = luku1 + " x " + luku2 + " = ?";
        questionText.color = Color.white;

        // Vastausvaihtoehtojen luominen
        List<int> answers = new List<int>();
        answers.Add(correctAnswer);

        // Luodaan kaksi uskottavaa väärää vastausta
        while (answers.Count < 3)
        {
            // Arvotaan satunnainen väärä vastaus, joka on lähellä oikeaa
            // tai muuten uskottava kertolaskun tulos
            int vaaraLuku1 = Random.Range(1, 11);
            int vaaraLuku2 = Random.Range(1, 11);
            int wrongAnswer = vaaraLuku1 * vaaraLuku2;

            if (!answers.Contains(wrongAnswer))
            {
                answers.Add(wrongAnswer);
            }
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

    public void CheckAnswer(int submittedAnswer)
    {
        if (isProcessing) return;

        if (submittedAnswer == correctAnswer)
        {
            score++;
            StartCoroutine(ShowFeedback("Correct!", Color.green));
        }
        else
        {
            // Näytetään pelaajalle myös oikea vastaus virheen sattuessa
            StartCoroutine(ShowFeedback("Incorrect, correct answer is " + correctAnswer, Color.red));
        }
    }

    IEnumerator ShowFeedback(string message, Color color)
    {
        isProcessing = true;
        
        questionText.text = message;
        questionText.color = color;

        yield return new WaitForSeconds(feedbackDuration);

        isProcessing = false;
        GenerateNewQuestion();
    }
}