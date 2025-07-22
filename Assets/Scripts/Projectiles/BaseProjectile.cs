using Spaceships;
using UnityEngine;
using Utils;

namespace Projectiles
{
    public class BaseProjectile : MonoBehaviour
    {
        private Transform _target = null!;
        private int _speed;

        private bool _inited = false;

        public void Init(Transform target, int projectileSpeed)
        {
            _target = target;
            _speed = projectileSpeed;
            _inited = true;
        }

        private void Update()
        {
            if (!_inited) {
                return;
            }

            transform.Translate(Vector3.forward * _speed * Time.deltaTime);
            Vector3 direction = _target.position - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _speed * Time.deltaTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out SpaceshipEnemy enemy)) {
                return;
            }

            Debug.Log("Hit!");
            enemy.BlowUp();
            this.DestroyObject();
        }
    }
}