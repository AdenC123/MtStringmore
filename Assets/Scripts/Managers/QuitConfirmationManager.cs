using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuitConfirmationManager : MonoBehaviour
{
    //Access to the QuitConfirmation panel
    [SerializeField] GameObject confirmationPanel;
    //Access to main menu scene name
    [SerializeField] private string mainMenuScene = "MainMenu";
    //Determines whether the "yes" confirmation should lead to mainMenu
    public bool goToMainMenu = false;

    private void Awake()
    {
        confirmationPanel.SetActive(false);
    }
    
    //Show the confirmation panel
    public void ShowConfirmation()
    {
        confirmationPanel.SetActive(true);
    }

    //Quitting out of the game
    public void ConfirmQuitGame()
    {
        PauseMenu pauseMenu = FindObjectOfType<PauseMenu>();
        
        if (goToMainMenu)
        {
            confirmationPanel.SetActive(false);
            //Resets the state of the pause menu (i.e. inactive)
            pauseMenu.Resume();
            Debug.Log("User is going to main menu!");
            SceneManager.LoadScene(mainMenuScene);
        }
        else
        {
            confirmationPanel.SetActive(false);
            //Resets the state of the pause menu (i.e. inactive)
            pauseMenu.Resume();
            Debug.Log("User has quit game!");
            Application.Quit();
        }
        
    }

    //Hides the confirmation panel when user pressed "No"
    public void CancelQuit()
    {
        confirmationPanel.SetActive(false);
    }
}
