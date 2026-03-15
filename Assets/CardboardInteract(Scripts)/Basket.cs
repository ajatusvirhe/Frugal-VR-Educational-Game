using UnityEngine;
using TMPro;

public class Basket : MonoBehaviour
{
    public BasketballGameManager gameManager;
    public TMP_Text myText; // Viittaus tämän korin päällä olevaan tekstiin

    private void OnTriggerEnter(Collider other)
    {
        // Tarkistetaan ensin, onko osuneella esineellä palautusskripti
        ReturnToPoint returnScript = other.GetComponent<ReturnToPoint>();

        // Jos esineessä on joko palautusskripti TAI se on merkattu palloksi
        if (returnScript != null || other.CompareTag("Ball") || other.GetComponent<Throwable>() != null)
        {
            // Luetaan vastaus korin tekstistä
            int myAnswer = int.Parse(myText.text);
            
            // Lähetetään vastaus managerille tarkistettavaksi
            gameManager.CheckAnswer(myAnswer);

            // Jos esineellä on palautusskripti, kutsutaan sen Return-metodia
            if (returnScript != null)
            {
                returnScript.Return();
            }
            else
            {
                Debug.LogWarning("Osuit koriin, mutta esineellä " + other.name + " ei ole ReturnToPoint-skriptiä.");
            }
        }
    }
}