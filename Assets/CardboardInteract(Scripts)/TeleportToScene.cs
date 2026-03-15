using UnityEngine;
using UnityEngine.SceneManagement;

public class TeleportToScene : Interactive
{
    [SerializeField] private string sceneToLoad; 
    [SerializeField] private DoorController doorController; // Viittaus DoorControlleriin

    public new void Interact()
    {
        // Kysytään vain ovelta, pääseekö tästä läpi
        if (doorController != null && doorController.CanTeleport())
        {
            if (!string.IsNullOrEmpty(sceneToLoad))
            {
                SceneManager.LoadScene(sceneToLoad);
            }
        }
        else
        {
            Debug.Log("Ovi on vielä lukossa!");
        }
    }
}