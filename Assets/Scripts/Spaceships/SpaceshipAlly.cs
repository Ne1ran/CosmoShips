using Movement;
using Projectiles;
using TMPro;
using UnityEngine;
using Utils;

namespace Spaceships
{
    public class SpaceshipAlly : Spaceship
    {
        private const string ProjectilePath = "BaseProjectile";
        private const string ExplosionPath = "Explosion";

        public TMP_Text text;
        public SpaceshipEnemy _enemy = null!;
        public int _rotationSpeed = 5;
        public int _projectileSpeed = 10;
        public int _damage = 30;

        public int _health = 100;
        public bool _destroyed = false;

        private Transform _shootingPoint = null!;
        private SinusMovement _sinusMovement = null!;

        private bool _rotating = false;

        private void Awake()
        {
            _shootingPoint = this.RequireComponentInChild<Transform>("ShootingPoint");
            _sinusMovement = this.RequireComponent<SinusMovement>();
            text.text = _health.ToString();
        }

        private void Update()
        {
            if (_enemy == null || _enemy.Destroyed) {
                return;
            }

            if (Input.GetKeyDown(KeyCode.E)) {
                _sinusMovement.Enabled = false;
                _rotating = true;
            }

            if (Input.GetKeyDown(KeyCode.F)) {
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

        private void ShotProjectile()
        {
            GameObject projectileObject = Resources.Load<GameObject>(ProjectilePath);
            GameObject obj = Instantiate(projectileObject);
            BaseProjectile baseProjectile = obj.RequireComponent<BaseProjectile>();
            baseProjectile.transform.position = _shootingPoint.position;
            baseProjectile.Init(_enemy.transform, this, _projectileSpeed, _damage);
        }

        private void BlowUp()
        {
            GameObject explosion = Resources.Load<GameObject>(ExplosionPath);
            GameObject explObj = Instantiate(explosion);
            explObj.transform.position = transform.position;
            this.DestroyObject();
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

        public bool Destroyed => _destroyed;
    }
}