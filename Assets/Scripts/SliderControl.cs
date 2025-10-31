using UnityEngine;
using UnityEngine.UI; // Required for UI elements

public class SliderController : MonoBehaviour
{
    public Slider fovSlider;
    public Camera mainCamera; // Or reference your specific camera

    void Start()
    {
        // Set the slider's initial value to the camera's current FOV
        if (mainCamera != null && fovSlider != null)
        {
            fovSlider.value = mainCamera.fieldOfView;
            // Set min/max values for the slider if not already done in the Inspector
            fovSlider.minValue = 20f; // Example minimum FOV
            fovSlider.maxValue = 100f; // Example maximum FOV
        }
    }

    // This method will be called when the slider's value changes
    public void OnFOVChanged(float newFOV)
    {
        if (mainCamera != null)
        {
            mainCamera.fieldOfView = newFOV;
        }
    }
}
