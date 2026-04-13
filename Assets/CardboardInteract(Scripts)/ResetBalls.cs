using UnityEngine;

public class ResetBalls : Interactive
{
    [Header("Audio")]
    public AudioClip blingSound;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    public new void Interact()
    {
        base.Interact(); // optional: logs "Interacted with ..."

        if (audioSource != null && blingSound != null)
            audioSource.PlayOneShot(blingSound);

        Debug.Log("Reloading scene while keeping correct-answer progress.");
        BasketWeightChecker.ReloadActiveSceneKeepingProgress();
    }
}