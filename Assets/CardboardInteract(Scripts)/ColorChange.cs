using UnityEngine;

public class ColorChange : Interactive
{
    private Renderer objRenderer;

    void Start()
    {
        // Haetaan 3D-objektin renderer
        objRenderer = GetComponent<Renderer>();
    }

    public new void Interact()
    {
        Debug.Log("Interacting with 3D Cube!");
        
        // Muutetaan materiaalin väriä
        objRenderer.material.color = new Color(Random.value, Random.value, Random.value);
    }
}