using System.Collections;
using Managers;
using UnityEngine;

namespace Player
{
    /// <summary>
    /// Renders animation of rope growing from one point to another
    /// </summary>
    [RequireComponent(typeof(LineRenderer))]
    public class RopeRenderer : MonoBehaviour
    {
        [SerializeField] private float ropeGrowDuration = 0.3f;
        [SerializeField] private bool growFromSelfToAttachPoint = true;

        private LineRenderer _ropeRenderer;
        private Vector3 _attachPos;
        private Coroutine _growRoutine;

        private void Awake()
        {
            _ropeRenderer = GetComponent<LineRenderer>();
            _ropeRenderer.positionCount = 2;
            _ropeRenderer.enabled = false;
        }

        private void Update()
        {
            if (!_ropeRenderer.enabled) return;
            _ropeRenderer.SetPosition(growFromSelfToAttachPoint ? 0 : 1, transform.position);
        }

        public void Attach(Vector3 attachPos)
        {
            _attachPos = attachPos;

            if (_growRoutine != null)
                StopCoroutine(_growRoutine);

            _ropeRenderer.enabled = true;
            _growRoutine = StartCoroutine(GrowRope());
        }

        public void Detach()
        {
            if (_growRoutine != null)
                StopCoroutine(_growRoutine);

            _ropeRenderer.enabled = false;
        }

        private IEnumerator GrowRope()
        {
            float t = 0f;

            while (t < 1f)
            {
                t += Time.deltaTime / ropeGrowDuration;
                float lerp = Mathf.Clamp01(t);

                if (growFromSelfToAttachPoint)
                {
                    _ropeRenderer.SetPosition(0, transform.position);
                    _ropeRenderer.SetPosition(1, Vector3.Lerp(transform.position, _attachPos, lerp));
                }
                else
                {
                    _ropeRenderer.SetPosition(0, Vector3.Lerp(_attachPos, transform.position, lerp));
                    _ropeRenderer.SetPosition(1, transform.position);
                }

                yield return null;
            }

            if (growFromSelfToAttachPoint)
            {
                _ropeRenderer.SetPosition(0, transform.position);
                _ropeRenderer.SetPosition(1, _attachPos);
            }
            else
            {
                _ropeRenderer.SetPosition(0, _attachPos);
                _ropeRenderer.SetPosition(1, transform.position);
            }
        }

        private void OnEnable()
        {
            GameManager.Instance.Reset += OnReset;
        }

        private void OnDisable()
        {
            GameManager.Instance.Reset -= OnReset;
        }

        private void OnReset()
        {
            Detach();
        }
    }
}
