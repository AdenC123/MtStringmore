using UnityEngine;

/// <summary>
/// Controls the canon that shoots the boulders
/// </summary>
public class CanonController : MonoBehaviour
{
    public GameObject boulder;
    private float _maxTimeBetweenShots;
    private float _timeBetweenShots;
    
    [SerializeField] private float angle = 45f; // angle in DEGREES
    [SerializeField] private float speed = 10f;
    [SerializeField] private float minTimeBetweenShots = 1f;
    [SerializeField] private float maxTimeBetweenShots = 3f;
    void Start()
    {
        SetRandomShotInterval();
    }

    void Update()
    {
        if (_timeBetweenShots <= 0)
        {
            GameObject newBoulder = Instantiate(boulder, transform.position, Quaternion.identity);
            Rigidbody2D boulderRb = newBoulder.GetComponent<Rigidbody2D>();

            // create the angle that the boulder shoots out
            float angleInRadians = angle * Mathf.Deg2Rad;
            Vector2 velocity = new Vector2(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians)) * speed;
            boulderRb.velocity = velocity;
            SetRandomShotInterval();
        }
        else
        {
            _timeBetweenShots -= Time.deltaTime;
        }
    }
    
    private void SetRandomShotInterval()
    {
        _timeBetweenShots = Random.Range(minTimeBetweenShots, maxTimeBetweenShots);
    }
    
}