using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    void Update()
    {
        // Check if any button is pressed
        if (Input.anyKeyDown)
        {
            LoadNextScene();
        }
    }

    // Call this method to load the next scene
    public void LoadNextScene()
    {
        // Check if there are more scenes in the build settings
        if (SceneManager.sceneCountInBuildSettings > SceneManager.GetActiveScene().buildIndex + 1)
        {
            // Load the next scene based on the build index
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else
        {
            Debug.LogWarning("No more scenes in build settings to load.");
        }
    }
}