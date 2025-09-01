using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class Level4SunriseCutscene : MonoBehaviour
    {
        public void OnEnable()
        {
            GetComponent<Animator>().enabled = true;
        }
    }
}
