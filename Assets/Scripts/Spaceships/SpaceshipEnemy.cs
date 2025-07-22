using UnityEngine;
using Utils;

namespace Spaceships
{
    public class SpaceshipEnemy : MonoBehaviour
    {
        private const string ExplosionPath = "Explosion";

        public void BlowUp()
        {
            GameObject explosion = Resources.Load<GameObject>(ExplosionPath);
            GameObject explObj = Instantiate(explosion);
            explObj.transform.position = transform.position;
            this.DestroyObject();
        }
    }
}