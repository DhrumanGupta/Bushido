using UnityEngine;

namespace Game.Networking
{
    public class CharacterManager : MonoBehaviour
    {
        public int id;
        public string username;

        private Animator animator;
        
        private Transform sprite;
        private Vector3 facingLeft = Vector3.zero;
        private Vector3 facingRight = Vector3.zero;

        private bool isKnocked = false;

        private void Awake()
        {
            animator = GetComponent<Animator>();
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
    }
}
