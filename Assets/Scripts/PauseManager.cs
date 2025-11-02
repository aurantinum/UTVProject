using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [SerializeField] FirstPersonController controller;
    [SerializeField] GameObject PauseMenu;

    public UnityEvent OnPause = new();
    public UnityEvent OnResume = new();

    public static bool paused = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(!paused)
                PauseGame();
            else 
                ResumeGame();

            paused = !paused;
        }
    }

    public void PauseGame()
    {
        Debug.Log("PAUSE");

        controller.enabled = false;
        Cursor.lockState = CursorLockMode.None;
        
        Time.timeScale = 0f;
        Cursor.visible = true;
        PauseMenu.SetActive(true);
    }

    public void ResumeGame()
    {
        Debug.Log("RESUME");

        controller.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;

        Time.timeScale = 1f;
        Cursor.visible = false;
        PauseMenu.SetActive(false);
    }

    public void Restart()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentScene);
        ResumeGame();
        paused = false;
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }
}
