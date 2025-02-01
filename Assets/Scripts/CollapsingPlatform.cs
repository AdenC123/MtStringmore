using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollapsingPlatform : MonoBehaviour
{
    #region Serialized Public Fields
    [Header("Collapse Time")] 
    [SerializeField] public float collapsePlatTime;
    [SerializeField] public float restorePlatTime;
    [SerializeField] public bool LeftWallPlatform;
    [SerializeField] public float rotationAngle;
    #endregion

    private bool _isOnPlatform;
    private bool _isPlatformActive = true;
    private float _deltaCollapseTime;
    private float _deltaRestoreTime;
    private float _currentRotation;
    Vector3 _pivotPosition;

    


    void Start()
    {
        _deltaCollapseTime = collapsePlatTime;
        _deltaRestoreTime = restorePlatTime;
        _pivotPosition = transform.parent.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(_isOnPlatform) 
        {
            
            _deltaCollapseTime -= Time.deltaTime;

            if(_deltaCollapseTime <= 0) 
            {
                //might experiment to get the target angle to be a full 90 degrees (so tweek 45)

                if(LeftWallPlatform) 
                {
                    Debug.Log(transform.eulerAngles.z);
                    transform.RotateAround(_pivotPosition, Vector3.back, 180* Time.deltaTime);
                } else 
                {
                    Debug.Log(transform.eulerAngles.z);
                    transform.RotateAround(_pivotPosition, Vector3.forward, 180* Time.deltaTime);
                }
                
                _isPlatformActive = false;
            } 
        } 
        
        if(!_isPlatformActive) 
        {
            _deltaRestoreTime-=Time.deltaTime;
            if(_deltaRestoreTime <= 0) 
            {   
                if(LeftWallPlatform) 
                {
                    Debug.Log(transform.eulerAngles.z);
                    transform.RotateAround(_pivotPosition, Vector3.forward, 180* Time.deltaTime);
                } 
                else
                {
                    Debug.Log(transform.eulerAngles.z);
                    transform.RotateAround(_pivotPosition, Vector3.back, 180* Time.deltaTime);
                }
            } 

            if(transform.eulerAngles.z >=0 && transform.eulerAngles.z<1)//desired range of angles to stop.
            {
                transform.eulerAngles = new Vector3(0f, 0f, 0f); //get the platform exactly horizontal.
                _isPlatformActive = true;
                _deltaRestoreTime = restorePlatTime;
            }

        }
    }
    // private void OnTriggerEnter2D(Collider2D other)
    // {
    //     if (other.gameObject.CompareTag("Player"))
    //     {
    //         _isOnPlatform = true;
    //         Debug.Log("I enter");
    //     }
    // }

    // private void OnTriggerExit2D(Collider2D other)
    // {
    //     if (other.gameObject.CompareTag("Player"))
    //     {
    //         _isOnPlatform = false;
    //         _deltaCollapseTime = collapsePlatTime;
    //         Debug.Log("I exited");
    //     }
    // }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _isOnPlatform = true;
            Debug.Log("I enter");
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _isOnPlatform = false;
            _deltaCollapseTime = collapsePlatTime;
            Debug.Log("I exited");
        }
    }

    

    
}
