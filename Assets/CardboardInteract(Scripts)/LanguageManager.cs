using UnityEngine;
using UnityEngine.SceneManagement;

public static class LanguageManager
{
    public static string CurrentLanguage { get; private set; }
    public static event System.Action OnLanguageChanged;

    [RuntimeInitializeOnLoadMethod]
    static void Init()
    {
        CurrentLanguage = PlayerPrefs.GetString("language", "FI");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (PlayerPrefs.GetInt("hasPlayed", 0) == 1)
        {
            var menu = GameObject.FindWithTag("MainMenu");
            if (menu != null) menu.SetActive(false);
        }

        OnLanguageChanged?.Invoke();
    }

    public static void SetLanguage(string lang)
    {
        CurrentLanguage = lang;
        PlayerPrefs.SetString("language", lang);
        OnLanguageChanged?.Invoke();
    }
}