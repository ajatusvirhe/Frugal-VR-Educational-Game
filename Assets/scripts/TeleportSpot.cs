using UnityEngine;

public class TeleportSpot : Interactive
{
    public new void Interact()
    {
        var player = CameraInteract.GetPlayerTransform();
        // place player at the spot position, offset by player height along the spot's up direction
        player.position = transform.position + transform.up * CameraInteract.GetPlayerHeight();
    }
}
