using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuButton : Interactive
{
    public enum ButtonAction { HideMenu, SetLanguageFI, SetLanguageEN }
    public ButtonAction action;
    public GameObject menuCanvas;
    public CameraInteract cameraInteract;

    [Header("Kielikorostus")]
    public string thisLanguage; // kirjoita "FI" tai "EN" Inspectorissa
    public Color selectedColor = Color.yellow;
    public Color normalColor = Color.white;

    private TextMeshProUGUI buttonText;

    void OnEnable()
    {
        buttonText = GetComponentInChildren<TextMeshProUGUI>();
        LanguageManager.OnLanguageChanged += UpdateHighlight;
        UpdateHighlight();
    }

    void OnDisable()
    {
        LanguageManager.OnLanguageChanged -= UpdateHighlight;
    }

    void UpdateHighlight()
    {
        if (buttonText == null || string.IsNullOrEmpty(thisLanguage)) return;

        if (LanguageManager.CurrentLanguage == thisLanguage)
        {
            buttonText.color = selectedColor;
            buttonText.fontStyle = FontStyles.Bold;
            transform.localScale = Vector3.one * 1.2f;
        }
        else
        {
            buttonText.color = normalColor;
            buttonText.fontStyle = FontStyles.Normal;
            transform.localScale = Vector3.one;
        }
    }

    public new void Interact()
    {
        switch (action)
        {
            case ButtonAction.HideMenu:
                menuCanvas.SetActive(false);
                if (cameraInteract != null)
                    cameraInteract.SetNormalLayer();
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