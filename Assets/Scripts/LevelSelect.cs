using System;
using System.Collections;
using System.Collections.Generic;
using Save;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour
{
    [SerializeField] private string mainMenu;
    [SerializeField] private string level1;
    [SerializeField] private string level2;
    [SerializeField] private string level3;
    [SerializeField] private string level4;
    
    [SerializeField] private Button level1button;
    [SerializeField] private Button level2button;
    [SerializeField] private Button level3button;
    [SerializeField] private Button level4button;
    
    private Color originalColor1 = Color.white; //color for button 1
    private Color originalColor2 = Color.white; //color for button 2
    private Color originalColor3 = Color.white; //color for button 3
    private Color originalColor4 = Color.white; //color for button 4
    private Color disabledColor = Color.gray;
    
    
    void Start()
    {
        originalColor1 = level1button.GetComponent<Image>().color;
        originalColor1 = level2button.GetComponent<Image>().color;
        originalColor1 = level3button.GetComponent<Image>().color;
        originalColor1 = level4button.GetComponent<Image>().color;


        // Disable the button
        level1button.interactable = false;
        level2button.interactable = false;
        level3button.interactable = false;
        level4button.interactable = false;

        // Change color to show it's disabled
        level1button.GetComponent<Image>().color = disabledColor;
        level2button.GetComponent<Image>().color = disabledColor;
        level3button.GetComponent<Image>().color = disabledColor;
        level4button.GetComponent<Image>().color = disabledColor;
    }
    
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

    void OnSceneLoaded()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("SaveData");
        SaveData data = JsonUtility.FromJson<SaveData>(jsonFile.text);
        
        foreach (string level in data.levelsAccessed)
        {
            enableLevelButtons(level);
        }
    }

    //enables buttons for levels that have not yet been accessed
    void enableLevelButtons(string levelName)
    {
        if (levelName == level1)
        {
            level1button.GetComponent<Image>().color = originalColor1;
        }
        
        if (levelName == level2)
        {
            level2button.GetComponent<Image>().color = originalColor2;
        }
        
        if (levelName == level3)
        {
            level3button.GetComponent<Image>().color = originalColor3;
        }
        
        if (levelName == level4)
        {
            level4button.GetComponent<Image>().color = originalColor4;
        }
    }

    private void OnEnable()
    {
        OnSceneLoaded();
    }

    private void OnDisable()
    {
        OnSceneLoaded();
    }
}
