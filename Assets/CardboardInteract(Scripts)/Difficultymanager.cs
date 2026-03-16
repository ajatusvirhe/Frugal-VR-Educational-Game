// DifficultyManager.cs
// =====================
// Singleton joka säilyy scenien välissä.
// Aseta vaikeustaso lobby-scenessä, lue se mistä tahansa muusta scriptistä näin:
//   Difficulty level = DifficultyManager.Instance.CurrentDifficulty;
//
// Enum Difficulty on määritelty tässä tiedostossa, joten se on käytettävissä kaikkialla.

using UnityEngine;

public enum Difficulty
{
    Easy,   // Level 1: Yhteenlasku, luvut 1–10
    Medium, // Level 2: Kertotaulu 1–5 sekä kertotaulu x10
    Hard    // Level 3: Kertotaulu 1–10
}

public class DifficultyManager : MonoBehaviour
{
    // Staattinen instanssi — käytä näin muista scripteistä: DifficultyManager.Instance
    public static DifficultyManager Instance { get; private set; }

    public Difficulty CurrentDifficulty { get; private set; } = Difficulty.Easy;

    private void Awake()
    {
        // Singleton-logiikka: vain yksi instanssi saa olla olemassa
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Säilyy scenien välissä
    }

    // Lobby-oven kutsuma metodi
    public void SetDifficulty(Difficulty difficulty)
    {
        CurrentDifficulty = difficulty;
        Debug.Log("Vaikeustaso asetettu: " + difficulty);
    }
}