using UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class QuitConfirmationManager : MonoBehaviour
{
    //Access to the QuitConfirmation panel
    [FormerlySerializedAs("confirmationPanel")] [SerializeField] GameObject confirmationCanvas;
    //Access to main menu scene name
    [SerializeField] private string mainMenuScene = "MainMenu";
    //Determines whether the "yes" confirmation should lead to mainMenu

    private void Awake()
    {
        confirmationCanvas.SetActive(false);
    }
    
    //Show the confirmation panel
    public void ShowConfirmation()
    {
        confirmationCanvas.SetActive(true);
    }

    //Quitting out of the game
    public void ConfirmQuitGame()
    {
        PauseMenu pauseMenu = FindObjectOfType<PauseMenu>();
        confirmationCanvas.SetActive(false);
        //Resets the state of the pause menu (i.e. inactive)
        pauseMenu.Resume();
        Debug.Log("User is going to main menu!");
        SceneManager.LoadScene(mainMenuScene);
    }

    //Hides the confirmation panel when user pressed "No"
    public void CancelQuit()
    {
        confirmationCanvas.SetActive(false);
    }
}
