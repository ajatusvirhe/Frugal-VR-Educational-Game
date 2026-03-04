using UnityEngine;
using TMPro;

public class Basket : MonoBehaviour
{
    public BasketballGameManager gameManager;
    public TextMeshPro myText; // Viittaus tämän korin päällä olevaan tekstiin

    private void OnTriggerEnter(Collider other)
    {
        // Tarkistetaan osuiko pallo (varmista että pallolla on tägi "Ball")
        if (other.CompareTag("Ball") || other.GetComponent<Throwable>() != null)
        {
            int myAnswer = int.Parse(myText.text);
            gameManager.CheckAnswer(myAnswer);
            
        }
    }
}