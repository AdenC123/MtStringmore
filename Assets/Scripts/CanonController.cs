using UnityEngine;

/// <summary>
/// Controls the canon that shoots the boulders
/// </summary>
public class CanonController : MonoBehaviour
{
    private Rigidbody2D rb;
    public GameObject Boulder;
    private float MaxTimeBtwnShots;
    private float TimeBtwnShots;
    
    [SerializeField] public float Angle = 45f; // angle in degrees
    [SerializeField] public float Speed = 10f; // initial speed of the boulder
    [SerializeField] public float minTimeBetweenShots = 1f;
    [SerializeField] public float maxTimeBetweenShots = 3f;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        SetRandomShotInterval();
    }

    void Update()
    {
        if (TimeBtwnShots <= 0)
        {
            GameObject newBoulder = Instantiate(Boulder, transform.position, Quaternion.identity);
            Rigidbody2D boulderRb = newBoulder.GetComponent<Rigidbody2D>();

            float angleInRadians = Angle * Mathf.Deg2Rad;
            Vector2 velocity = new Vector2(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians)) * Speed;
            boulderRb.velocity = velocity;
            SetRandomShotInterval();
        }
        else
        {
            TimeBtwnShots -= Time.deltaTime;
        }
    }
    
    private void SetRandomShotInterval()
    {
        TimeBtwnShots = Random.Range(minTimeBetweenShots, maxTimeBetweenShots);
    }
    
}