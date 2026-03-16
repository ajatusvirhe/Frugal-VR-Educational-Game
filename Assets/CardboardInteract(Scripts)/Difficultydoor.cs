// DifficultyDoor.cs
// =================
// Lisää tämä scripti jokaisen lobby-oven Interactive-objektiin.
// Aseta Inspectorissa:
//   - Difficulty       → Easy / Medium / Hard
//   - Scene To Load    → Pelin scenen nimi (string)
//   - Door Controller  → Viittaus DoorController-komponenttiin (valinnainen lukitus)
//

using UnityEngine;
using UnityEngine.SceneManagement;

public class DifficultyDoor : Interactive
{
    [SerializeField] private Difficulty difficulty;
    [SerializeField] private string sceneToLoad;
    [SerializeField] private DoorController doorController; // Valinnainen — jätä tyhjäksi jos ovi ei ole lukossa

    public new void Interact()
    {
        // Tarkista lukko, jos DoorController on asetettu
        if (doorController != null && !doorController.CanTeleport())
        {
            Debug.Log("Ovi on lukossa!");
            return;
        }

        // Tallenna vaikeustaso ennen scenen vaihtoa
        if (DifficultyManager.Instance != null)
        {
            DifficultyManager.Instance.SetDifficulty(difficulty);
        }
        else
        {
            Debug.LogWarning("DifficultyManager ei löydy scenestä! Lisää se lobby-sceneen.");
        }

        // Vaihda sceneen
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogWarning("DifficultyDoor: sceneToLoad on tyhjä!");
        }
    }
}
