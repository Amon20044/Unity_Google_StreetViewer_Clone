using UnityEngine;

public class SkyboxTransition : MonoBehaviour
{
    public float transitionDuration = 1.0f; // Duration of the transition
    private Material transitionMaterial; // Material used for the transition effect

    void Start()
    {
        // Create a new material with the custom blur shader
        transitionMaterial = new Material(Shader.Find("Custom/SkyboxBlur"));
    }

    public void StartSkyboxTransition(Material newSkybox)
    {
        // Start the transition coroutine
        StartCoroutine(TransitionSkybox(newSkybox));
    }

    private System.Collections.IEnumerator TransitionSkybox(Material newSkybox)
    {
        // Get the current skybox material
        Material currentSkybox = RenderSettings.skybox;

        // Set the transition material as the current skybox
        RenderSettings.skybox = transitionMaterial;

        // Blend between the current and new skybox materials
        float elapsedTime = 0f;
        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float blend = Mathf.Clamp01(elapsedTime / transitionDuration);

            // Set the blur amount based on the blend factor
            transitionMaterial.SetFloat("_BlurAmount", 1.0f - blend);

            // Update the environment lighting
            DynamicGI.UpdateEnvironment();

            yield return null;
        }

        // Set the new skybox as the final material
        RenderSettings.skybox = newSkybox;

        // Update the environment lighting
        DynamicGI.UpdateEnvironment();
    }
}