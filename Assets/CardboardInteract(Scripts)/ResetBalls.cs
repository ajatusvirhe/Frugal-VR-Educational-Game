using UnityEngine;

public class ResetBalls : Interactive
{
    [Header("Audio")]
    public AudioClip blingSound;

    private AudioSource audioSource;
    private bool isResetting;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    public new void Interact()
    {
        base.Interact(); // optional: logs "Interacted with ..."

        if (isResetting)
            return;

        StartCoroutine(ResetAfterDelay());
    }

    private System.Collections.IEnumerator ResetAfterDelay()
    {
        isResetting = true;

        if (audioSource != null && blingSound != null)
            audioSource.PlayOneShot(blingSound);

        yield return new WaitForSeconds(1f);

        Debug.Log("Reloading scene while keeping correct-answer progress.");
        BasketWeightChecker.ReloadActiveSceneKeepingProgress();
    }
}