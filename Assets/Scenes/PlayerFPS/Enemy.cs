using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HealthModule), typeof(Rigidbody))]
public class Enemy : MonoBehaviour
{
    public static Player player => Player.Instance;
    protected HealthModule healthModule;
    protected Rigidbody rb;

    private void Awake()
    {
        healthModule = GetComponent<HealthModule>();
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        healthModule.onDie.AddListener(OnDie);
    }

    public virtual void OnDie()
    {

    }

    public void DestroyEnemy()
    {
        Destroy(gameObject);
    }
}
