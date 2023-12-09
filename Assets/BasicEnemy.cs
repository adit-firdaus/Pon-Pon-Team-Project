using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BasicEnemy : MonoBehaviour
{
    public int level = 1;
    public GameObject player;
    public float moveSpeed = 1f;
    public float attackRange = 1f;
    public float health = 200;
    public float damage = 50;

    public PlayerTriggerArea attackArea;
    public Vector3 moveDirection;
    public Animator animator;

    public bool canAttackPlayer = false;

    private void Start()
    {
        InitStats();
    }

    public void InitStats()
    {
        health = 200 * Mathf.Pow(level, 2);
        damage = 50 * level;
    }

    private void Update()
    {

        if (player)
        {
            canAttackPlayer = Vector3.Distance(transform.position, player.transform.position) < attackRange;
            animator.SetBool("CanAttack", canAttackPlayer);
        }

        if (!canAttackPlayer) Chase();
    }

    public void Patrol()
    {
        var randomDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;

        moveDirection = Vector3.Lerp(moveDirection, randomDirection, Time.deltaTime).normalized;

        transform.position += moveDirection * moveSpeed * Time.deltaTime;
        transform.rotation = Quaternion.LookRotation(moveDirection);
    }

    public void Chase()
    {
        moveDirection = (player.transform.position - transform.position).normalized;

        transform.position += moveDirection * moveSpeed * Time.deltaTime;
        transform.rotation = Quaternion.LookRotation(moveDirection);


    }

    public void Attack()
    {
        if (attackArea.playerInTriggerArea)
        {
            // attackArea.player.TakeDamage(damage, actor);
        }
    }

    public void Die()
    {
        animator.SetTrigger("Dead");
    }

    public void DestroyEnemy()
    {
        Destroy(gameObject);
    }
}
