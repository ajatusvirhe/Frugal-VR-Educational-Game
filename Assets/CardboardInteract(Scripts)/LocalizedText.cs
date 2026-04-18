using UnityEngine;
using TMPro;

public class LocalizedText : MonoBehaviour
{
    [TextArea]
    public string textFI;
    [TextArea]
    public string textEN;

    void OnEnable()
    {
        LanguageManager.OnLanguageChanged += UpdateText;
        UpdateText();
    }

    void OnDisable()
    {
        LanguageManager.OnLanguageChanged -= UpdateText;
    }

    void UpdateText()
    {
        string text = LanguageManager.CurrentLanguage == "FI" ? textFI : textEN;

        // Kokeilee ensin UI-tekstiä, sitten 3D-tekstiä
        var tmpUI = GetComponent<TextMeshProUGUI>();
        if (tmpUI != null) { tmpUI.text = text; return; }

        var tmp3D = GetComponent<TextMeshPro>();
        if (tmp3D != null) { tmp3D.text = text; return; }
    }
}