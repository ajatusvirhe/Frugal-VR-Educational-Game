using UnityEngine;

public class MenuController : MonoBehaviour
{
    public GameObject menuCanvas;
    public CameraInteract cameraInteract;

    void Start()
    {
        bool hasPlayed = PlayerPrefs.GetInt("hasPlayed", 0) == 1;

        if (hasPlayed)
        {
            // Palataan alkusceneen pelin jälkeen – ei valikkoa, kaikki interaktoitavissa
            menuCanvas.SetActive(false);
            cameraInteract.SetNormalLayer();
        }
        else
        {
            // Ensimmäinen kerta – valikko näkyviin, vain napit toimivat
            menuCanvas.SetActive(true);
            cameraInteract.SetMenuLayer();
        }
    }
}