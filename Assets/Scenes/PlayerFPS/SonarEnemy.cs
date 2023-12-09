using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SonarEnemy : Enemy
{
    public float moveSpeed = 1;
    public float detectionRange = 10f;
    public float attackRange = 5f;
    public float attackDamage = 10f;
    public Weapon weapon;
    public GameObject explosionPrefab;

    bool playerInRange = false;
    bool canAttackPlayer = false;

    private void Update()
    {
        if (!player) return;

        playerInRange = Vector3.Distance(transform.position, player.transform.position) < detectionRange;
        canAttackPlayer = Vector3.Distance(transform.position, player.transform.position) < attackRange;

        if (playerInRange && !canAttackPlayer)
        {
            rb.velocity = transform.forward * moveSpeed;
        }
        else
        {
            rb.velocity = Vector3.zero;
        }

        if (playerInRange)
        {
            Vector3 targetDirection = player.transform.position - transform.position;
            targetDirection.y = 0;
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            rb.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 5);
        }

        weapon.SetFiring(canAttackPlayer);

        weapon.transform.LookAt(player.transform.position);
    }

    public override void OnDie()
    {
        base.OnDie();

        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
    }
}
