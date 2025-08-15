﻿using Interactables;
using Knitby;
using Managers;
using Player;
using StringmoreCamera;
using UnityEngine;
using UnityEngine.Events;
using Yarn.Unity;

namespace Level3
{
    /// <summary>
    /// Custom level 3 logic.
    /// </summary>
    public class Level3Logic : MonoBehaviour
    {
        [SerializeField] private UnityEvent onSecondHalfReached;
        [SerializeField] private GameObject[] gameObjects;
        [SerializeField] private GameObject[] cutsceneObjects;
        [SerializeField] private AudioClip secondHalfBGM;
        [SerializeField] private Checkpoint secondHalfCheckpoint;
        [SerializeField] private float SecondHalfCameraYOffset;

        private Checkpoint[] _checkpoints;
        private AttachableMovingObject[] _zippers;
        private PlayerController _player;
        private KnitbyController _knitby;
        private FollowCamera _camera;

        private void Awake()
        {
            _checkpoints = FindObjectsByType<Checkpoint>(FindObjectsSortMode.None);
            _zippers = FindObjectsByType<AttachableMovingObject>(FindObjectsSortMode.None);
            
            _player = FindAnyObjectByType<PlayerController>();
            _knitby = FindAnyObjectByType<KnitbyController>();
            _camera = FindAnyObjectByType<FollowCamera>();
        }

        private void Start()
        {
            // there's no guarantee we grab the right instance in Awake so we use Start
            GameManager.SetInteractablesEnabled(false);
            
            foreach (AttachableMovingObject zipper in _zippers)
            {
                zipper.SetTabVisible(false);
            }
            
            _knitby.gameObject.SetActive(false);
        }
        
        [YarnCommand("cutscene_state")]
        public void SetCutsceneState(bool value)
        {
            foreach (GameObject obj in gameObjects)
            {
                obj.SetActive(!value);
            }
            foreach (GameObject obj in cutsceneObjects)
            {
                obj.SetActive(value);
            }
        }

        /// <summary>
        /// Called to start second half - enables interactables, clears checkpoints and flips them.
        /// </summary>
        [YarnCommand("start_second_half")]
        public void ReachSecondHalf()
        {
            GameManager.Instance.AreInteractablesEnabled = true;
            // seems checkpoint calls this before adding itself
            // makes our life easier
            GameManager.Instance.ClearCheckpointData();
            GameManager.Instance.GetComponent<BGMManager>().PlayBGM(secondHalfBGM);
            
            foreach (Checkpoint checkpoint in _checkpoints)
            {
                if (checkpoint != secondHalfCheckpoint) 
                    checkpoint.FlipAndResetCheckpoint();
            }
            secondHalfCheckpoint.respawnFacingLeft = true;
            GameManager.Instance.RespawnFacingLeft = true;
            
            onSecondHalfReached.Invoke();
            
            foreach (AttachableMovingObject zipper in _zippers)
            {
                zipper.SetTabVisible(true);
            }
            
            // make player face left
            _player.transform.position = secondHalfCheckpoint.transform.position;
            _player.AddPlayerVelocityEffector(new SimpleVelocityEffector(_ => new Vector2(-1.0f, 1.0f)), true);
            
            _knitby.gameObject.SetActive(true);
            _knitby.transform.position = secondHalfCheckpoint.transform.position;
            _camera.SetYOffset(SecondHalfCameraYOffset);
        }
    }
}
