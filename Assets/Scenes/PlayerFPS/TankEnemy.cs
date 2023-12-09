using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankEnemy : Enemy
{
    public float moveSpeed = 1;
    public float detectionRange = 10f;
    public float stopRange = 5f;
    public GameObject explosionPrefab;

    bool playerInRange = false;
    bool shouldStop = false;

    private void Update()
    {
        if (!player) return;

        playerInRange = Vector3.Distance(transform.position, player.transform.position) < detectionRange;
        shouldStop = Vector3.Distance(transform.position, player.transform.position) < stopRange;

        if (playerInRange && !shouldStop)
        {
            rb.velocity = (player.transform.position - transform.position).normalized * moveSpeed;
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
            rb.MoveRotation(Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 5));
        }
    }

    public override void OnDie()
    {
        base.OnDie();

        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
    }
}

