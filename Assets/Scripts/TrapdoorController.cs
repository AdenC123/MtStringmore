using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HingeJoint2D))]
public class TrapdoorController : MonoBehaviour
{
    #region Serialized Public Fields
    [Header("Collapse Time")] 
    [SerializeField] float collapsePlatTimer;
    [SerializeField] float restorePlatTimer;
    [SerializeField] bool isLeftWall;
    //this value should be smaller than the player/rb's "weight"
    [SerializeField] float motorForce;
    [SerializeField] float motorSpeed;
    #endregion

    #region Private Properties
    private HingeJoint2D _hinge;
    private JointMotor2D _motor;
    private IEnumerator _activeRoutine;
    #endregion

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            StartCoroutine(_activeRoutine = Folding());    
        }
    }

    IEnumerator Folding()
    {
        _hinge = GetComponent<HingeJoint2D>();
        _motor = _hinge.motor;
        //collapse the platform after time has passed
        yield return new WaitForSeconds(collapsePlatTimer);
        _motor.motorSpeed = motorSpeed;
        _motor.maxMotorTorque = motorForce;
        _hinge.motor = _motor;

        //restore the platform after time has passed
        yield return new WaitForSeconds(restorePlatTimer);
        
        if(isLeftWall) 
        {
            _motor.motorSpeed = -100f;
        } else 
        {
            _motor.motorSpeed = 100f;
        }
        _motor.maxMotorTorque = 30f;
        _hinge.motor = _motor;
        _activeRoutine = null;
    }
}
