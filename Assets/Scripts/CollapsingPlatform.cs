using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollapsingPlatform : MonoBehaviour
{
    #region Serialized Public Fields
    [Header("Collapse Time")] 
    [SerializeField] public float collapsePlatTimer;
    [SerializeField] public float restorePlatTimer;
    [SerializeField] public bool isLeftWall;
    [SerializeField] public float motorForce;
    [SerializeField] public float motorSpeed;
    #endregion

    private HingeJoint2D _hinge;
    private JointMotor2D _motor;
    private float _deltaCollapseTimer;
    private float _deltaRestoreTimer;
    private bool _isOnPlatform;

    // Start is called before the first frame update
    void Start()
    {
        _deltaCollapseTimer = collapsePlatTimer;
        _deltaRestoreTimer = restorePlatTimer;
        _hinge = GetComponent<HingeJoint2D>();
        _motor = _hinge.motor;
    }

    // Update is called once per frame
    void Update()
    {
        if(_isOnPlatform) 
        {
            _deltaCollapseTimer-=Time.deltaTime;

            if(_deltaCollapseTimer<=0) {
                //instantly disable then re-enable current platform's motor to set variables at the same time
                _hinge.useMotor = false;
                _motor.motorSpeed = motorSpeed;
                _motor.maxMotorTorque = motorForce;
                _hinge.motor = _motor;
                _hinge.useMotor = true;

                //call coroutine that pauses execution after retorePlatTime expires
                StartCoroutine(RestorePlatform());
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _isOnPlatform = true;
        }
    }

    //coroutine to restore platform after restore time elapsed
    IEnumerator RestorePlatform()
    {
        _isOnPlatform = false;

        yield return new WaitForSeconds(restorePlatTimer);

        _hinge.useMotor = false;
        if(isLeftWall) {
            _motor.motorSpeed = -100f;
        } else 
        {
            _motor.motorSpeed = 100f;
        }
        _motor.maxMotorTorque = 30f;
        _hinge.motor = _motor;
        _hinge.useMotor = true;
    }
}
