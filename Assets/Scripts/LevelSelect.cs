using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelect : MonoBehaviour
{
    [SerializeField] private string mainMenu;
    [SerializeField] private string level1;
    [SerializeField] private string level2;
    [SerializeField] private string level3;
    [SerializeField] private string level4;

    public void backToMain()
    {
        SceneManager.LoadScene(mainMenu);
    }

    public void loadlevel1()
    {
        SceneManager.LoadScene(level1);
    }
    
    public void loadlevel2()
    {
        SceneManager.LoadScene(level2);
    }

    public void loadlevel3()
    {
        SceneManager.LoadScene(level3);
    }

    public void loadlevel4()
    {
        SceneManager.LoadScene(level4);
    }
}
