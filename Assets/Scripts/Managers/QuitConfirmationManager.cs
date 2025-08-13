using UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class QuitConfirmationManager : MonoBehaviour
{
    [SerializeField, Tooltip("The quit confirmation canvas")] GameObject confirmationCanvas;
    [SerializeField, Tooltip("The pause menu game object")] PauseMenu pauseMenu;
    [SerializeField] private string mainMenuScene = "MainMenu";
    
    //<summary>
    //Show the confirmation panel
    //</summary>
    public void ShowConfirmation()
    {
        confirmationCanvas.SetActive(true);
    }

    //<summary>
    //Quitting out of the game
    //</summary>
    public void ConfirmQuitGame()
    {
        confirmationCanvas.SetActive(false);
        //Resets the state of the pause menu (i.e. inactive)
        pauseMenu.Resume();
        Debug.Log("User is going to main menu!");
        SceneManager.LoadScene(mainMenuScene);
    }

    //<summary>
    //Hides the confirmation panel when user pressed "No"
    //</summary>
    public void CancelQuit()
    {
        confirmationCanvas.SetActive(false);
    }
}
