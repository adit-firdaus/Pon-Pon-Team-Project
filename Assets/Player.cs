using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HealthModule), typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    public static Player Instance;

    public HealthModule healthModule;

    [NonSerialized] public Rigidbody rb;

    private void Awake()
    {
        Instance = this;

        healthModule = GetComponent<HealthModule>();
        rb = GetComponent<Rigidbody>();
    }
}
