using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenuUI;
    private bool isPaused = false;

    //void Update()
    //{
    //    // Example: press Escape to toggle pause
    //    if (Input.GetKeyDown(KeyCode.Escape))
    //    {
    //        if (isPaused)
    //            ResumeGame();
    //        else
    //            PauseGame();
    //    }
    //}

    public void PauseGame()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;   // Freeze the game
        isPaused = true;
    }

    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;   // Resume time
        isPaused = false;
    }

    public void QuitGame()
    {
        // optional: return to main menu or quit
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}
