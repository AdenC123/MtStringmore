using System.Collections;
using UnityEngine;

/// <summary>
/// Choose letter block color, handle shake and break animation
/// </summary>
public class LetterBlock : MonoBehaviour
{
    [SerializeField] private GameObject letter;
    [SerializeField] private GameObject particles;
    [SerializeField] private float blockBreakDelay;
    [SerializeField] [Range(0f, 0.1f)] private float delayBetweenShakes = 0f;
    [SerializeField] [Range(0f, 2f)] private float distance = 0.1f;

    private SpriteRenderer _renderer;
    private Vector3 _startPos;
    private float _timer;

    private void Awake()
    {
        _startPos = transform.position;
        _renderer = GetComponent<SpriteRenderer>();
    }

    public void Crack()
    {
        particles.SetActive(true);
        StartCoroutine(Shake());
        StartCoroutine(Break());
    }
    
    private IEnumerator Shake()
    {
        _timer = 0f;

        while (_timer < blockBreakDelay)
        {
            _timer += Time.deltaTime;

            Vector3 randomPos = _startPos + (Random.insideUnitSphere * distance);

            transform.position = randomPos;

            if (delayBetweenShakes > 0f)
            {
                yield return new WaitForSeconds(delayBetweenShakes);
            }
            else
            {
                yield return null;
            }
        }

        transform.position = _startPos;
    }

    private IEnumerator Break()
    {
        // Hide block and letter after half the block break time
        yield return new WaitForSeconds(blockBreakDelay / 2);
        _renderer.enabled = false;
        letter.SetActive(false);
        // Give time for particles to spawn, then destroy object and children
        yield return new WaitForSeconds(blockBreakDelay / 2);
        Destroy(gameObject);
    }
}
