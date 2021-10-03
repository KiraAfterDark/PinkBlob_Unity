using UnityEngine;

namespace PinkBlob.Gameplay
{
    public class Health : MonoBehaviour
    {
        [SerializeField]
        private int maxHealth = 5;

        private int health = 0;

        private void Awake()
        {
            health = maxHealth;
        }

        public void DealDamage(int damage)
        {
            health -= damage;

            if (health <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
