using System.Collections;
using Game.Control;
using UnityEngine;

namespace Game.Combat
{
    public class Health : MonoBehaviour
    {
        [SerializeField] private float maxHealth = 100f;
        private float health = 0f;
        [SerializeField] private GameObject deathEffect = null;

        private void Start()
        {
            health = maxHealth;
        }

        public void TakeDamage(float damage)
        {
            if (health == 0) return;
            
            health -= damage;
            health = Mathf.Clamp(health, 0, Mathf.Infinity);

            if (health == 0)
            {
                if (gameObject.CompareTag("Player"))
                {
                    KnockDown();
                    return;
                }

                StartCoroutine(Die(0f));
            }
        }
        
        private void KnockDown()
        {
            StartCoroutine(Die(10f));
            if (!TryGetComponent(out PlayerController controller)) return;
            controller.KnockDown();
        }

        private IEnumerator Die(float time)
        {
            yield return new WaitForSeconds(time);
            if (deathEffect != null) Destroy(Instantiate(deathEffect, transform.position, Quaternion.identity), 5f);
            Destroy(gameObject);
        }

        public bool IsKnocked()
        {
            return health == 0;
        }
    }
}
