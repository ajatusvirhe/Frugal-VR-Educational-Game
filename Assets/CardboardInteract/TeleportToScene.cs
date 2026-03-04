using UnityEngine;
using UnityEngine.SceneManagement;

public class TeleportToScene : Interactive
{
    [SerializeField] private string sceneToLoad; 

    public new void Interact()
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.Log("Teleportataan sceneen: " + sceneToLoad);
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogError("Et ole asettanut scenen nimeä objektille: " + gameObject.name);
        }
    }
}