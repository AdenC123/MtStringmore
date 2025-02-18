using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    #region Serialized Public Fields
    [Header("Collapse Time")] 
    [SerializeField] public float collapsePlatTimer;
    [SerializeField] public float restorePlatTimer;
    [SerializeField] public float destroyTimer;
    [SerializeField] public Rigidbody2D _rb;
    #endregion

    #region Private Properties
    private bool _isOnPlatform;
    private float _deltaCollapseTimer;
    private Vector2 _originalPosition;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        _originalPosition = transform.position;
        _deltaCollapseTimer = collapsePlatTimer;

    }

    // Update is called once per frame
    void Update()
    {
        if(_isOnPlatform) 
        {
            _deltaCollapseTimer-=Time.deltaTime;

            if(_deltaCollapseTimer<=0) {

                StartCoroutine(Falling());
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

    IEnumerator Falling() 
    {
        _isOnPlatform = false;
        _rb.bodyType = RigidbodyType2D.Dynamic;
        yield return new WaitForSeconds(destroyTimer);
        yield return new WaitForSeconds(restorePlatTimer);
        _rb.bodyType = RigidbodyType2D.Static;
        transform.position = _originalPosition;
    }
}
