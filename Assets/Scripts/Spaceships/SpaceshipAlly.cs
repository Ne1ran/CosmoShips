using Movement;
using Projectiles;
using UnityEngine;
using Utils;

namespace Spaceships
{
    public class SpaceshipAlly : MonoBehaviour
    {
        private const string ProjectilePath = "BaseProjectile";
        
        public SpaceshipEnemy _enemy = null!;
        public int _rotationSpeed = 5;
        public int _projectileSpeed = 10;

        private Transform _shootingPoint = null!;
        private SinusMovement _sinusMovement = null!;

        private bool _rotating = false;
        private bool _shotDone = false;

        private void Awake()
        {
            _shootingPoint = this.RequireComponentInChild<Transform>("ShootingPoint");
            _sinusMovement = this.RequireComponent<SinusMovement>();
        }

        private void Update()
        {
            if (_shotDone) {
                return;
            }
            
            if (Input.GetKeyDown(KeyCode.E)) {
                _sinusMovement.Enabled = false;
                _rotating = true;
            }
            
            if (Input.GetKeyDown(KeyCode.F)) {
                _shotDone = true;
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
            baseProjectile.Init(_enemy.transform, _projectileSpeed);
        }
    }
}