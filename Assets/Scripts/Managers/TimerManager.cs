using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Managers
{
    public class TimerManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private GameObject resultsWindow;
        
        // private float _elapsedGameTime;
        private float _elapsedLevelTime;
        
        void Start()
        {
            _elapsedLevelTime = 0;
        }
        
        void Update()
        {
            while (resultsWindow.activeSelf) return; 
            
            _elapsedLevelTime += Time.deltaTime;
            int minutes = (int) (_elapsedLevelTime / 60);
            int seconds = (int) (_elapsedLevelTime % 60);
            int milliseconds = (int) ((_elapsedLevelTime * 100) % 100);
            timerText.text = $"{minutes:00}:{seconds:00}:{milliseconds:00}";
        }
    }
}
