using Game.Combat;
using UnityEngine;

namespace Game.Control
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float speed = 3f;
        private bool isKnocked = false;
        
        [SerializeField] private bool gay = false;

        private Vector3 minClamp = Vector3.zero;
        private Vector3 maxClamp = Vector3.zero;

        private Animator animator = null;
        private Transform walls = null;
        private Fighter fighter = null;

        private Transform sprite = null;
        
        private Vector3 facingLeft = Vector3.zero;
        private Vector3 facingRight = Vector3.zero;

        private Camera cam = null;


        void Awake()
        {
            fighter = GetComponent<Fighter>();
            animator = GetComponent<Animator>();
            cam = Camera.main;

            sprite = transform.GetChild(0);
            
            facingRight = sprite.localScale;
            
            facingLeft = sprite.localScale;
            facingLeft.x *= -1;

            GameObject wallsGO = GameObject.Find("Walls");
            if (wallsGO == null) return;
            walls = wallsGO.transform;
            if (walls.childCount <= 1) return;
            minClamp = walls.GetChild(0).position;
            maxClamp = walls.GetChild(1).position;
        }

        
        void Update()
        {
            if (isKnocked) return;

            CheckAndMove();
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 direction =  cam.ScreenToWorldPoint(Input.mousePosition) - transform.position;
                Debug.DrawRay(transform.position, direction, Color.green, 3f);

                RaycastHit2D[] rayHit = Physics2D.RaycastAll(transform.position, direction);
                print($"Raycast hit {rayHit.Length} colliders!");
                foreach (var hit in rayHit)
                {
                    GameObject target = hit.collider.gameObject;
                    if (!target.CompareTag("Enemy")) continue;
                    if (fighter.CanAttack(target))
                    {
                        fighter.Attack(target);
                        print("attacked target!");
                    }
                }
            }
        }

        private void CheckAndMove()
        {
            Vector3 newPos = transform.position;

            // Check for input and add/subtract from the vector
            if (gay)
            {
                if (Input.GetKey(KeyCode.RightArrow))
                {
                    newPos.x += speed * Time.deltaTime;
                }
                if (Input.GetKey(KeyCode.LeftArrow))
                {
                    newPos.x -= speed * Time.deltaTime;
                }
            
                if (Input.GetKey(KeyCode.UpArrow))
                {
                    newPos.y += speed * Time.deltaTime;
                }
                if (Input.GetKey(KeyCode.DownArrow))
                {
                    newPos.y -= speed * Time.deltaTime;
                }
            }
            else
            {
                
                if (Input.GetKey(KeyCode.D))
                {
                    newPos.x += speed * Time.deltaTime;
                }
                if (Input.GetKey(KeyCode.A))
                {
                    newPos.x -= speed * Time.deltaTime;
                }
            
                if (Input.GetKey(KeyCode.W))
                {
                    newPos.y += speed * Time.deltaTime;
                }
                if (Input.GetKey(KeyCode.S))
                {
                    newPos.y -= speed * Time.deltaTime;
                }
            }

            // newPos.x = Mathf.Clamp(newPos.x, minClamp.x, maxClamp.x);
            // newPos.y = Mathf.Clamp(newPos.y, minClamp.y, maxClamp.y);
            
            // Check if the vector has changed or not
            if (newPos == transform.position)
            {
                animator.SetBool("isMoving", false);
                return;
            }

            sprite.localScale = newPos.x - transform.position.x < 0f ? facingLeft : facingRight;

            // If changed, apply the change to the player
            // Also set the bool for the animation
            transform.position = newPos;
            animator.SetBool("isMoving", true);
        }

        public void KnockDown()
        {
            isKnocked = true;
            transform.GetChild(0).gameObject.SetActive(false);
        }
    }
}
