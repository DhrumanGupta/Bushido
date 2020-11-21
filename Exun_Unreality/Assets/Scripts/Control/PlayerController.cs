using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Game.Combat;
using Game.Networking;
using UnityEngine;
using UnityEngine.Analytics;

namespace Game.Control
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float speed = 3f;
        private bool isKnocked = false;

        private Vector3 minClamp = Vector3.zero;
        private Vector3 maxClamp = Vector3.zero;
        
        private Animator animator = null;
        private Fighter fighter = null;
        private CharacterManager characterController = null;
        
        private Transform walls = null;
        private Vector3 facingLeft = Vector3.zero;
        private Vector3 facingRight = Vector3.zero;
        
        private Camera cam = null;
        private Transform sprite = null;
        
        [SerializeField] private float healCooldown = 25f;
        private float timeSinceLastHealed = Mathf.Infinity;

        void Awake()
        {
            fighter = GetComponent<Fighter>();
            animator = GetComponent<Animator>();
            characterController = GetComponent<CharacterManager>();
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

        private void Update()
        {
            if (isKnocked) return;
            
            timeSinceLastHealed += Time.deltaTime;
            CheckAndMove();
            
            if (Input.GetMouseButtonDown(0))
            {
                animator.SetTrigger("attack");
                Vector3 direction =  cam.ScreenToWorldPoint(Input.mousePosition) - transform.position;
                Debug.DrawRay(transform.position, direction, Color.green, 3f);
        
                RaycastHit2D[] rayHit = Physics2D.RaycastAll(transform.position, direction, fighter.GetAttackRange());

                List<int> enemiesHit = new List<int>();
                    
                foreach (var hit in rayHit)
                {
                    GameObject target = hit.collider.gameObject;
                    if (!target.CompareTag("Enemy")) continue;
                    if (fighter.CanAttack(target))
                    {
                        enemiesHit.Add(target.GetComponent<CharacterManager>().id);
                    }
                }
                
                if (enemiesHit.Count == 0) return;
                int[] hitEnemies = enemiesHit.ToArray();
                ClientSend.PlayerAttack(hitEnemies, fighter.damage);
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                foreach (var player in GameManager.players.Values)
                {
                    if (player.id == characterController.id) continue;
                    if (!player.isKnocked) continue;
                    
                    float range = fighter.GetAttackRange();

                    if ((player.transform.position - transform.position).sqrMagnitude < range * range && timeSinceLastHealed > healCooldown)
                    {
                        ClientSend.PlayerHeal(player.id);
                        healCooldown = 0;
                        print($"Healing player {player.username}");
                    }
                }
            }
        }

        private Vector3 lastSendPos;
        private bool debounce = false;
        void FixedUpdate()
        {
            if (transform.position == lastSendPos)
            {
                if (debounce) return;
                debounce = true;
                ClientSend.PlayerMovement(transform.position);
                return;
            }
            lastSendPos = transform.position;
            debounce = false;
            ClientSend.PlayerMovement(transform.position);
        }
        
        private void CheckAndMove()
        {
            // Check for input and add/subtract from the vector
            #if UNITY_STANDALONE || UNITY_EDITOR
            
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            
            Vector3 newPos = new Vector3(horizontal, vertical);
            if (newPos == Vector3.zero)
            {
                animator.SetBool("isMoving", false);
                return;
            }
            newPos.Normalize();
            newPos *= Time.deltaTime * speed;

            #elif UNITY_ANDROID || UNITY_IOS
            #endif

            newPos.x = Mathf.Clamp(newPos.x, minClamp.x - transform.position.x, maxClamp.x  - transform.position.x);
            newPos.y = Mathf.Clamp(newPos.y, minClamp.y - transform.position.y, maxClamp.y  - transform.position.y);

            sprite.localScale = newPos.x < 0f ? facingLeft : facingRight;
        
            // If changed, apply the change to the player
            // Also set the bool for the animation
            transform.position += newPos;
            animator.SetBool("isMoving", true);
        }
        
        public void KnockDownStatus(bool status)
        {
            isKnocked = status;
            animator.SetBool("isKnocked", isKnocked);
            ClientSend.PlayerKnocked(isKnocked);
        }

        public void Heal()
        {
            isKnocked = false;
            animator.SetBool("isKnocked", isKnocked);
        }
    }
    
}
