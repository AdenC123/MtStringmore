using System;
using Managers;
using UnityEngine;

namespace Interactables
{
    public class SwingArea : MonoBehaviour
    {
        [Min(0)] public float swingRadius;
        
        /// <summary>
        /// Swings are enabled by default, but override with the disable sprite if GameManager flag is set
        /// </summary>
        [SerializeField] private Sprite disabledSprite;

        private void Start()
        {
            if (!GameManager.Instance.areInteractablesEnabled)
            {
                GetComponent<SpriteRenderer>().sprite = disabledSprite;
            }
        }
    }
}
