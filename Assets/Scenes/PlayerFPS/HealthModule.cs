using UnityEngine;
using UnityEngine.Events;

public class HealthModule : MonoBehaviour
{
    public float health = 100;
    public UnityEvent onDie;

    public void TakeDamage(float damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        onDie.Invoke();
    }
}
