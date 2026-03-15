using UnityEngine;

public class DoorController : MonoBehaviour
{
    [SerializeField] private bool isLocked = true;

    // Tätä metodia kutsutaan kun pelaaja ratkaisee tehtävän, joka avaa oven
    public void UnlockDoor()
    {
        isLocked = false;
        Debug.Log("Oven lukitus avattu!");     
    }

    // Tätä käytetään Teleport-skriptissä tarkistamaan, voiko pelaaja teleportata oven läpi
    public bool CanTeleport()
    {
        return !isLocked;
    }
}