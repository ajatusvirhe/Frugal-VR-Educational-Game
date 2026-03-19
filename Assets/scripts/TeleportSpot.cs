using System.Collections;
using UnityEngine;

public class TeleportSpot : Interactive
{
    private float _teleportCooldown = 0.5f;
    private bool _isOnCooldown;
    public new void Interact()
    {
        if (_isOnCooldown)
        {
            return;
        }
        var player = CameraInteract.GetPlayerTransform();
        // place player at the spot position, offset by player height along the spot's up direction
        player.position = transform.position + transform.up * CameraInteract.GetPlayerHeight();

        StartCoroutine(TeleportCooldownRoutine());
    }

    private IEnumerator TeleportCooldownRoutine()
    {
        _isOnCooldown = true;
        yield return new WaitForSeconds(_teleportCooldown);
        _isOnCooldown = false;
    }
}
