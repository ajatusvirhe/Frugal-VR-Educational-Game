using UnityEngine;

public class MenuController : MonoBehaviour
{
    public GameObject menuCanvas;
    public CameraInteract cameraInteract;

    // static muuttuja – nollautuu kun peli käynnistetään uudelleen
    // mutta pysyy kun siirrytään scenestä toiseen
    private static bool hasPlayed = false;

    void Start()
    {
        if (hasPlayed)
        {
            menuCanvas.SetActive(false);
            cameraInteract.SetNormalLayer();
        }
        else
        {
            menuCanvas.SetActive(true);
            cameraInteract.SetMenuLayer();
        }
    }

    public static void SetPlayed()
    {
        hasPlayed = true;
    }
}