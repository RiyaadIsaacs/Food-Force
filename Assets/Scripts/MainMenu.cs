using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public void StartGame()
    {
        SceneManager.LoadScene("Level Design");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void MainMenuButton()
    {
        SceneManager.LoadScene("Main Menu");
    }
    public void TutorialLoad()
    {
        SceneManager.LoadScene("Tutorial");
    }
    public void SettingsLoad()
    {
        SceneManager.LoadScene("Settings");
    }
}
