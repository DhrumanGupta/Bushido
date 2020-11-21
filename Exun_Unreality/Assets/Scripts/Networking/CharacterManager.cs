using Game.Combat;
using Game.Control;
using UnityEngine;

namespace Game.Networking
{
    public class CharacterManager : MonoBehaviour
    {
        public int id;
        public string username;

        private Animator animator = null;
        private Health health = null;
        private PlayerController playerController = null;
        
        private Transform sprite;
        private Vector3 facingLeft = Vector3.zero;
        private Vector3 facingRight = Vector3.zero;

        public bool isKnocked = false;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            health = GetComponent<Health>();
            playerController = GetComponent<PlayerController>();
            
            sprite = transform.GetChild(0);
            
            facingRight = sprite.localScale;
            
            facingLeft = sprite.localScale;
            facingLeft.x *= -1;
        }

        public void MoveTo(Vector3 newPos)
        {
            if (transform.position == newPos)
            {
                animator.SetBool("isMoving", false);
                return;
            }
            
            sprite.localScale = newPos.x - transform.position.x < 0f ? facingLeft : facingRight;
            
            transform.position = newPos;
            animator.SetBool("isMoving", true);
        }

        public void KnockDownStatus(bool status)
        {
            isKnocked = status;
            animator.SetBool("isKnocked", isKnocked);
        }

        public void Heal()
        {
            if (id == Client.Instance.myId)
            {
                playerController.Heal();
            }
            else
            {
                KnockDownStatus(false);
            }
            health.Heal();
        }

        public void SetHealth(float newHealth)
        {
            health.SetHealth(newHealth);
        }

        public void Attack()
        {
            animator.SetTrigger("attack");
        }
    }
}
