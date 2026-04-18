using UnityEngine;

public class MenuButton : Interactive
{
    public enum ButtonAction { HideMenu, SetLanguageFI, SetLanguageEN }

    public ButtonAction action;
    public GameObject menuCanvas;
    public CameraInteract cameraInteract; // raahaa CameraInteract tähän

    public new void Interact()
    {
        switch (action)
        {
            case ButtonAction.HideMenu:
                menuCanvas.SetActive(false);
                if (cameraInteract != null)
                    cameraInteract.SetNormalLayer(); // vaihda layer
                break;
            case ButtonAction.SetLanguageFI:
                LanguageManager.SetLanguage("FI");
                break;
            case ButtonAction.SetLanguageEN:
                LanguageManager.SetLanguage("EN");
                break;
        }
    }
}