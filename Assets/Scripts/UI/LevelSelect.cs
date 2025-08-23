using System;
using System.Collections.Generic;
using System.Linq;
using Managers;
using Save;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// Level select menu logic.
    /// </summary>
    public class LevelSelectMenu : MonoBehaviour
    {
        [SerializeField] private LevelSelectButton buttonPrefab;
        [SerializeField] private Transform buttonContainer;
        [SerializeField] private Button playButton;

        [SerializeField] private List<Sprite> levelCandyImages;

        [SerializeField] private TextMeshProUGUI levelText, timeText, deathText, candyText;
        [SerializeField] private Image candyImage;

        [SerializeField] private AchievementPatches ap;

        private const string EmptySaveTime = "--:--:--";
        private string _selectedScene;
        private LevelSelectButton _selectedButton;
        private int _debugButtonPressed;

        private void Start()
        {
            // Ensures GameManager load save data before loading level select menu
            GameManager.Instance.UpdateFromSaveData();
            HashSet<string> unlockedScenes = new(GameManager.Instance.LevelsAccessed);
            playButton.interactable = false;
            unlockedScenes.Add(SceneListManager.Instance.Level1IntroSceneName);
            string[] levelLoadingScenes = SceneListManager.Instance.LevelLoadScenes.ToArray();
            LevelSelectButton farthestUnlockedButton = null;
            for (int i = 0; i < levelLoadingScenes.Length; i++)
            {
                LevelSelectButton btnObj = Instantiate(buttonPrefab, buttonContainer);
                bool isUnlocked = unlockedScenes.Contains(levelLoadingScenes[i]);
                if (isUnlocked) farthestUnlockedButton = btnObj;
                btnObj.Initialize(i + 1, isUnlocked);
                ap.CheckSetGold(btnObj, i+1);
            }

            playButton.onClick.AddListener(OnPlayClicked);
            Debug.Assert(farthestUnlockedButton, "Yo what the hell bruh where all my b u t t o n s");
            OnLevelSelected(farthestUnlockedButton.LevelNumber, farthestUnlockedButton);
        }

        /// <summary>
        /// Called on a level select button click: selects the level.
        /// </summary>
        /// <param name="levelNumber">Level number (1 indexed)</param>
        /// <param name="button">Button clicked</param>
        public void OnLevelSelected(int levelNumber, LevelSelectButton button)
        {
            string sceneName = SceneListManager.Instance.LevelLoadScenes[levelNumber - 1];
            _selectedScene = sceneName;
            playButton.interactable = true;

            //change the text of the level stats
            levelText.text = "Level " + levelNumber;
            LevelData selectedLevel = GameManager.Instance.AllLevelData[levelNumber - 1];

            // Candy
            int mostCandiesCollected = selectedLevel.mostCandiesCollected;
            int totalCandiesInLevel = selectedLevel.totalCandiesInLevel;
            if (selectedLevel.totalCandiesInLevel < 0)
                candyText.text = "N/A";
            else
                candyText.text = mostCandiesCollected + "/" + totalCandiesInLevel;

            // Deaths
            Debug.Log(selectedLevel.leastDeaths);
            int deaths = selectedLevel.leastDeaths;
            deathText.text = deaths == -1 ? "N/A" : selectedLevel.leastDeaths.ToString();

            // Time
            float bestTime = selectedLevel.bestTime;
            timeText.text = float.IsNaN(bestTime)
                ? EmptySaveTime
                : TimeSpan.FromSeconds(selectedLevel.bestTime).ToString(@"mm\:ss\:ff");

            // CandyImage
            Sprite candy = levelCandyImages[levelNumber - 1];
            candyImage.sprite = candy;
            candyImage.enabled = true;
            
            // Show Achievement Patches if applicable
            ap.DisplayAchievementPatches(levelNumber, bestTime, mostCandiesCollected, totalCandiesInLevel, deaths, 
                candy);

            if (_selectedButton) _selectedButton.MarkUnselected();
            _selectedButton = button;
            button.MarkSelected();
        }

        /// <summary>
        /// Debug button pressed.
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public void DebugButtonPressed()
        {
            _debugButtonPressed++;
            if (_debugButtonPressed > 10)
            {
                int childCount = buttonContainer.childCount;
                for (int i = 0; i < childCount; i++)
                {
                    LevelSelectButton btn = buttonContainer.GetChild(i).GetComponent<LevelSelectButton>();
                    btn.Initialize(i + 1, true);
                }

                transform.Find("SecretDebugOption").GetChild(0).gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// Called on play button clicked.
        /// </summary>
        private void OnPlayClicked()
        {
            if (string.IsNullOrWhiteSpace(_selectedScene)) return;
            Debug.Log("Loading selected level: " + _selectedScene);
            SceneManager.LoadScene(_selectedScene);
        }
    }
}
