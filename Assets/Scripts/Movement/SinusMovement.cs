using UnityEngine;

namespace Movement
{
    public class SinusMovement : MonoBehaviour
    {
        public float _infinitySize = 20f;
        public float _speed = 1.5f;
        public float _smooth = 5f;
        private float _t;
        private bool _enabled = true;

        private Vector3 _startPosition;

        private void Awake()
        {
            _startPosition = transform.position;
        }

        private void Update()
        {
            if (!_enabled) {
                return;
            }

            _t += _speed * Time.deltaTime;

            float sinusCalc = 1 + Mathf.Sin(_t) * Mathf.Sin(_t);
            float x = _infinitySize * Mathf.Sin(_t) * Mathf.Cos(_t) / sinusCalc;
            float z = _infinitySize * Mathf.Cos(_t) / sinusCalc;

            Vector3 targetPosition = _startPosition + new Vector3(x, 0, z);
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * _smooth);
            Vector3 velocity =
                    new(_infinitySize * (Mathf.Cos(_t) * Mathf.Cos(_t) - Mathf.Sin(_t) * Mathf.Sin(_t)) / sinusCalc - _infinitySize * Mathf.Sin(_t) * Mathf.Cos(_t) * 2 * Mathf.Sin(_t) * Mathf.Cos(_t) / (sinusCalc * sinusCalc),
                        0,
                        -_infinitySize * Mathf.Sin(_t) / sinusCalc
                        - _infinitySize * Mathf.Cos(_t) * 2 * Mathf.Sin(_t) * Mathf.Cos(_t) / (sinusCalc * sinusCalc));
            if (velocity.sqrMagnitude > 0.001f) {
                transform.rotation = Quaternion.LookRotation(velocity);
            }
        }

        public bool Enabled
        {
            get => _enabled;
            set => _enabled = value;
        }
    }
}