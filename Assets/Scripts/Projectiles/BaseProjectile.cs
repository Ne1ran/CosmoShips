using Spaceships;
using UnityEngine;
using Utils;
using Random = System.Random;
using UnityRandom = UnityEngine.Random;


namespace Projectiles
{
    public class BaseProjectile : MonoBehaviour
    {
        private Transform _target = null!;
        private Spaceship _creator = null!;
        private int _speed;
        private int _damage;

        private bool _inited = false;
        private bool _isMissing;
        
        public void Init(Transform target, Spaceship creator, int projectileSpeed, int damage)
        {
            _creator = creator;
            
            _target = target;
            _speed = projectileSpeed;
            _damage = damage;
            _inited = true;
            Random random = new Random();
            _isMissing = random.Next(0, 2) == 0;
        }

        private void Update()
        {
            if (!_inited) {
                return;
            }

            if (_isMissing) {
                transform.Translate(Vector3.forward * _speed * Time.deltaTime);
            } else {
                transform.Translate(Vector3.forward * _speed * Time.deltaTime);
                Vector3 direction = _target.position - transform.position;
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _speed * Time.deltaTime);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            Spaceship spaceship = other.GetComponent<Spaceship>();
            if (spaceship == null) {
                return;
            }
            
            SpaceshipAlly spaceshipAlly = other.GetComponent<SpaceshipAlly>();
            if (spaceshipAlly != null && spaceshipAlly != _creator) {
                Debug.Log("Hit ally!");
                spaceshipAlly.DealDamage(_damage);
                this.DestroyObject();
            }
            
            SpaceshipEnemy enemy = other.GetComponent<SpaceshipEnemy>();
            if (enemy != null && enemy != _creator) {
                Debug.Log("Hit enemy!");
                enemy.DealDamage(_damage);
                this.DestroyObject();
            }
        }
    }
}