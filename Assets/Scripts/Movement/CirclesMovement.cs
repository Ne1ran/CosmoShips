using UnityEngine;

namespace Movement
{
    public class CirclesMovement : MonoBehaviour
    {
        public float _moveDuration = 4f;
        public float _moveSpeed = 8f;
        public float _turnDuration = 1f;

        public float _timer = 0f;
        public bool _turning = false;
        public Quaternion _startRotation;
        public Quaternion _endRotation;

        private int _turnCount = 0;

        void Start()
        {
            _startRotation = transform.rotation;
            _endRotation = Quaternion.Euler(0, 180, 0) * _startRotation;
        }

        void Update()
        {
            _timer += Time.deltaTime;
            if (_turning) {
                float t = Mathf.Clamp01(_timer / _turnDuration);
                transform.rotation = _turnCount % 2 == 1
                                             ? Quaternion.Lerp(_startRotation, _endRotation, t)
                                             : Quaternion.Lerp(_endRotation, _startRotation, t);

                if (_timer < _turnDuration) {
                    return;
                }

                transform.rotation = _turnCount % 2 == 1 ? _endRotation : _startRotation;
                _turning = false;
                _timer = 0f;
            } else {
                transform.Translate(Vector3.forward * _moveSpeed * Time.deltaTime);
                if (_timer < _moveDuration) {
                    return;
                }

                _turning = true;
                _timer = 0f;
                _turnCount++;
            }
        }
    }
}