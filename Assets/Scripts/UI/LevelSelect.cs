using System.Collections.Generic;
using System.IO;
using Save;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Managers;

public class LevelSelect : MonoBehaviour
{
    [SerializeField] private string level1;
    [SerializeField] private string level2;
    [SerializeField] private string level3;
    [SerializeField] private string level4;
    
    [SerializeField] private Button level1button;
    [SerializeField] private Button level2button;
    [SerializeField] private Button level3button;
    [SerializeField] private Button level4button;

    private Color originalColor1 = Color.white;
    private Color originalColor2 = Color.white;
    private Color originalColor3 = Color.white;
    private Color originalColor4 = Color.white;
    private Color disabledColor = Color.gray;

    void Start()
    {
        originalColor1 = level1button.GetComponent<Image>().color;
        originalColor2 = level2button.GetComponent<Image>().color;
        originalColor3 = level3button.GetComponent<Image>().color;
        originalColor4 = level4button.GetComponent<Image>().color;

        DisableAllButtons();
        UpdateLevelButtons();

        if (GameManager.Instance != null)
            GameManager.Instance.GameDataChanged += UpdateLevelButtons;
    }

    void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.GameDataChanged -= UpdateLevelButtons;
    }

    void DisableAllButtons()
    {
        level1button.interactable = false;
        level2button.interactable = false;
        level3button.interactable = false;
        level4button.interactable = false;

        level1button.GetComponent<Image>().color = disabledColor;
        level2button.GetComponent<Image>().color = disabledColor;
        level3button.GetComponent<Image>().color = disabledColor;
        level4button.GetComponent<Image>().color = disabledColor;
    }

    void UpdateLevelButtons()
    {
        List<string> levels = GameManager.Instance.levelsAccessed;
        

        foreach (string level in levels)
        {
            enableLevelButtons(level);
        }
    }

    public List<string> LoadLevelsAccessed(string fileName)
    {
        string folderLocation = Path.Combine(Application.persistentDataPath, "saves");
        string filePath = Path.Combine(folderLocation, fileName);

        if (!File.Exists(filePath))
        {
            Debug.LogWarning($"Save file does not exist: {filePath}");
            return new List<string>();
        }

        try
        {
            string json = File.ReadAllText(filePath);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            return data.levelsAccessed ?? new List<string>();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to load save file {filePath}: {e.Message}");
            return new List<string>();
        }
    }

    void enableLevelButtons(string levelName)
    {
        if (levelName == level1)
        {
            level1button.GetComponent<Image>().color = originalColor1;
            level1button.interactable = true;
        }
        if (levelName == level2)
        {
            level2button.GetComponent<Image>().color = originalColor2;
            level2button.interactable = true;
        }
        if (levelName == level3)
        {
            level3button.GetComponent<Image>().color = originalColor3;
            level3button.interactable = true;
        }
        if (levelName == level4)
        {
            level4button.GetComponent<Image>().color = originalColor4;
            level4button.interactable = true;
        }
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
