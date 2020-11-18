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

        private GameObject[] players = null;

        private Animator animator = null;
        private Fighter fighter = null;
            
        void Awake()
        {
            animator = GetComponent<Animator>();
            fighter = GetComponent<Fighter>();
            players = GameObject.FindGameObjectsWithTag("Player");
        }

        private int relativeIteration = 0;
        GameObject closestEnemy;
        void Update()
        {
            relativeIteration++;
            // Get the closest enemy every 6 frames
            if (relativeIteration == 1)
            {
                float greatestDist = Mathf.Infinity;

                foreach (GameObject currentPlayer in players)
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
            else if (relativeIteration == 6) relativeIteration = 0;

            if (closestEnemy == null) return;
            // print(closestEnemy.name);
            
            if (fighter.CanAttack(closestEnemy))
            {
                fighter.Attack(closestEnemy);
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
