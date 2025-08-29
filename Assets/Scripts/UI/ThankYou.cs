using TMPro;
using UI;
using UnityEngine;

public class ThankYou : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private AchievementPatches ap;

    private void Awake()
    {
        if (ap.isGoldL1 && ap.isGoldL2 && ap.isGoldL3 && ap.isGoldL4)
            text.enabled = true;
        else
            text.enabled = false;
    }
    
}
