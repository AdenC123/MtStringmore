// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class TestCollapsing : MonoBehaviour
// {
//     // Start is called before the first frame update
//     private Rigidbody2D _rb;
//     private bool _isOnPlatform;
//     void Start()
//     {
//         _rb = GetComponent<Rigidbody2D>();
//     }

//     // Update is called once per frame
//     void Update()
//     {
//         if(_isOnPlatform) {
//             _rb.AddForceAtPosition(collapseForce * Vector2.down, oppositeEndPosition, ForceMode2D.Impulse);
//         }
        
//     }

//     private void OnCollisionEnter2D(Collision2D other)
//     {
//         if (other.gameObject.CompareTag("Player"))
//         {
//             _isOnPlatform = true;
//         }
//     }

//     private void OnCollisionExit2D(Collision2D other)
//     {
//         if (other.gameObject.CompareTag("Player"))
//         {
//             _isOnPlatform = false;
            
//         }
//     }

// }
