using Movement;
using Projectiles;
using TMPro;
using UnityEngine;
using Utils;

namespace Spaceships
{
    public class SpaceshipEnemy : Spaceship
    {
        private const string ExplosionPath = "Explosion";
        private const string ProjectilePath = "BaseProjectile";

        public TMP_Text text;
        public SpaceshipAlly _enemy = null!;

        public int _health = 100;
        public int _rotationSpeed = 5;
        public int _projectileSpeed = 10;
        public int _damage = 30;
        public bool _destroyed = false;
        private bool _rotating = true;

        private Transform _shootingPoint = null!;
        private CirclesMovement _movement = null!;

        private void Awake()
        {
            text.text = _health.ToString();
            _movement = this.RequireComponent<CirclesMovement>();
            _shootingPoint = this.RequireComponentInChild<Transform>("ShootingPoint");
        }

        private void Update()
        {
            if (_enemy == null || _enemy.Destroyed) {
                return;
            }

            if (Input.GetKeyDown(KeyCode.E)) {
                _rotating = true;
            }

            if (Input.GetKeyDown(KeyCode.F)) {
                _movement.Enabled = false;
                ShotProjectile();
            }

            if (!_rotating) {
                return;
            }

            Vector3 direction = _enemy.transform.position - transform.position;
            if (direction == Vector3.zero) {
                return;
            }

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
        }

        public void DealDamage(int damage)
        {
            _health -= damage;
            if (_health <= 0) {
                BlowUp();
            } else {
                text.text = _health.ToString();
            }
        }

        private void BlowUp()
        {
            GameObject explosion = Resources.Load<GameObject>(ExplosionPath);
            GameObject explObj = Instantiate(explosion);
            explObj.transform.position = transform.position;
            this.DestroyObject();
        }

        private void ShotProjectile()
        {
            GameObject projectileObject = Resources.Load<GameObject>(ProjectilePath);
            GameObject obj = Instantiate(projectileObject);
            BaseProjectile baseProjectile = obj.RequireComponent<BaseProjectile>();
            baseProjectile.transform.position = _shootingPoint.position;
            baseProjectile.Init(_enemy.transform, this, _projectileSpeed, _damage);
        }

        public bool Destroyed => _destroyed;
    }
}