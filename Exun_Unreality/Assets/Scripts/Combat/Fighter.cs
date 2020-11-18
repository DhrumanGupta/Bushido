using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Combat
{
    [ExecuteInEditMode]
    public class Fighter : MonoBehaviour
    {
        [SerializeField] float timeBetweenAttacks = 1f;
        [SerializeField] private float attackRange = .5f;
        [SerializeField] private float damage = 5f;

        float timeSinceLastAttack = Mathf.Infinity;

        private void Update()
        {
            timeSinceLastAttack += Time.deltaTime;
        }

        public void Attack(GameObject target)
        {
            if (timeSinceLastAttack < timeBetweenAttacks) return;
            target.GetComponent<Health>().TakeDamage(damage);
            timeSinceLastAttack = 0f;
        }

        public bool CanAttack(GameObject combatTarget)
        {
            if (combatTarget == null) { return false; }
            Health targetToTest = combatTarget.GetComponent<Health>();
            bool isInRange = attackRange * attackRange > (transform.position - combatTarget.transform.position).sqrMagnitude;
            return targetToTest != null && !targetToTest.IsKnocked() && isInRange;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.black;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }
}
