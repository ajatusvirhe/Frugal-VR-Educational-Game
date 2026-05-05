using UnityEngine;

public class MenuController : MonoBehaviour
{
    public GameObject menuCanvas;
    public CameraInteract cameraInteract;

    void Start()
    {
        menuCanvas.SetActive(true);
        cameraInteract.SetMenuLayer();
    }
}