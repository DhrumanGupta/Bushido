using System.Collections;
using System.Collections.Generic;
using Game.Combat;
using UnityEngine;

namespace Game.Control
{
    public class AIController : MonoBehaviour
    {
        [SerializeField] private GameObject deathEffect = null;
        [SerializeField] private float speed = 4f;

        private Animator animator = null;
        private Fighter fighter = null;
            
        void Awake()
        {
            animator = GetComponent<Animator>();
            fighter = GetComponent<Fighter>();
        }

        private int relativeIteration = 0;
        Transform closestEnemy;
        void FixedUpdate()
        {
            relativeIteration++;
            // Get the closest enemy every 6 frames
            if (relativeIteration == 1)
            {
                float greatestDist = Mathf.Infinity;

                foreach (Transform currentPlayer in CameraController.Instance.targets)
                {
                    if (currentPlayer == null) continue;
                    if (currentPlayer.GetComponent<Health>().IsKnocked()) continue;
                    
                    float distance = (transform.position - currentPlayer.transform.position).sqrMagnitude;
                    // print($"{distance} units far away from {currentPlayer.name}");

                    if (greatestDist * greatestDist > distance)
                    {
                        greatestDist = distance;
                        closestEnemy = currentPlayer;
                    }
                }
            }
            else if (relativeIteration == 3) relativeIteration = 0;

            if (closestEnemy == null) return;
            // print(closestEnemy.name);
            
            if (fighter.CanAttack(closestEnemy.gameObject))
            {
                fighter.Attack(closestEnemy.gameObject);
                animator.SetBool("isMoving", false);
                return;
            }

            // Move to the closest enemy
            Vector3 newPos = Vector3.MoveTowards(transform.position, closestEnemy.transform.position,
                speed * Time.deltaTime);
            transform.position = newPos;
            animator.SetBool("isMoving", true);
            
        }
    }
}
