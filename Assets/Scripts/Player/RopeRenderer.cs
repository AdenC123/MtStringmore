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
        [SerializeField] private bool useWorldSpace = true;

        private LineRenderer _lineRenderer;
        private Vector3 _attachPos;
        private Coroutine _growRoutine;

        private void Awake()
        {
            _lineRenderer = GetComponent<LineRenderer>();
            _lineRenderer.enabled = false;
            _lineRenderer.positionCount = 2;
            _lineRenderer.SetPosition(growFromSelfToAttachPoint ? 0 : 1, transform.position);
        }

        private void Update()
        {
            if (!_lineRenderer.enabled) return;
            _lineRenderer.SetPosition(
                growFromSelfToAttachPoint ? 0 : 1,
                useWorldSpace ? transform.position : transform.localPosition
            );
        }

        public void Attach(Vector3 attachPos)
        {
            _attachPos = attachPos;

            if (_growRoutine != null)
                StopCoroutine(_growRoutine);

            _lineRenderer.enabled = true;
            _growRoutine = StartCoroutine(GrowRope());
        }

        public void Detach()
        {
            if (_growRoutine != null)
                StopCoroutine(_growRoutine);

            _lineRenderer.enabled = false;
        }

        private IEnumerator GrowRope()
        {
            Vector3 selfPos = useWorldSpace ? transform.position : transform.localPosition;
            float t = 0f;

            while (t < 1f)
            {
                t += Time.deltaTime / ropeGrowDuration;
                float lerp = Mathf.Clamp01(t);
                selfPos = useWorldSpace ? transform.position : transform.localPosition;

                if (growFromSelfToAttachPoint)
                {
                    _lineRenderer.SetPosition(0, selfPos);
                    _lineRenderer.SetPosition(1, Vector3.Lerp(selfPos, _attachPos, lerp));
                }
                else
                {
                    _lineRenderer.SetPosition(0, Vector3.Lerp(_attachPos, selfPos, lerp));
                    _lineRenderer.SetPosition(1, selfPos);
                }

                yield return null;
            }

            if (growFromSelfToAttachPoint)
            {
                _lineRenderer.SetPosition(0, selfPos);
                _lineRenderer.SetPosition(1, _attachPos);
            }
            else
            {
                _lineRenderer.SetPosition(0, _attachPos);
                _lineRenderer.SetPosition(1, selfPos);
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
