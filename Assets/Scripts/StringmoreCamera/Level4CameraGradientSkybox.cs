using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace StringmoreCamera
{
    public class Level4CameraGradientSkybox : MonoBehaviour
    {
        private Material dynamicSkyBox;
        [SerializeField] private Color startColor;
        [SerializeField] private Color endColor;

        [SerializeField] private float transitionStartHeight;
        [SerializeField] private float transitionEndHeight;

        private void Awake()
        {
            Skybox currSkybox;
            dynamicSkyBox = new Material(Shader.Find("Skybox/Procedural"));
            if (gameObject.GetComponent<Skybox>())
            {
                currSkybox = gameObject.GetComponent<Skybox>();
            }
            else
            {
                currSkybox = gameObject.AddComponent<Skybox>();
            }
            currSkybox.material = dynamicSkyBox;
        }

        private void Update()
        {
            updateSkyboxOnPosition();
        }

        private void updateSkyboxOnPosition()
        {
            if (dynamicSkyBox)
            {
                /// Transitioning from endColor to startColor cos y decreases the higher you go
                float transitionGradient = Mathf.InverseLerp(transitionStartHeight, transitionEndHeight, transform.position.y);
                Color currentColor = Color.Lerp(endColor, startColor, transitionGradient);

                dynamicSkyBox.SetColor("_SkyTint", currentColor);
            }

        }
    }
    
}
