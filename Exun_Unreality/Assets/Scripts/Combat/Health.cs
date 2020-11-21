using System.Collections;
using Game.Control;
using Game.Networking;
using UnityEngine;

namespace Game.Combat
{
    public class Health : MonoBehaviour
    {
        [SerializeField] private float maxHealth = 200f;
        public float health = 0f;
        [SerializeField] private GameObject deathEffect = null;

        private void Awake()
        {
            health = maxHealth;
        }

        private void KnockDown()
        {
            StartCoroutine(Die(10f));
            
            if (!TryGetComponent(out PlayerController controller)) return;
            controller.KnockDownStatus(true);
        }

        private IEnumerator Die(float time)
        {
            yield return new WaitForSeconds(time);
            if (deathEffect != null) Destroy(Instantiate(deathEffect, transform.position, Quaternion.identity), 5f);
            if (gameObject.CompareTag("Player"))
            {
                GameManager.players.Remove(GetComponent<CharacterManager>().id);
            }
            Destroy(gameObject);
        }

        public bool IsKnocked()
        {
            return health == 0;
        }

        public void SetHealth(float f)
        {
            health = f;
            health = Mathf.Clamp(health, 0, maxHealth);
            if (health == 0)
            {
                if (gameObject.CompareTag("Player"))
                {
                    KnockDown();
                    return;
                }
                
                Destroy(gameObject);
            }
        }

        public void Heal()
        {
            health = maxHealth;
        }
    }
}
