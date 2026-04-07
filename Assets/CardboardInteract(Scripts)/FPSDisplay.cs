using UnityEngine;
using TMPro;

public class FPSDisplay : MonoBehaviour
{
    public TextMeshProUGUI fpsText;

    void Update()
    {
        float fps = 1f / Time.unscaledDeltaTime;
        float frameTime = Time.unscaledDeltaTime * 1000f;

        fpsText.text = $"FPS: {fps:F0}\nFrametime: {frameTime:F1} ms\nGyro: {Input.gyro.rotationRate}";
    }
}