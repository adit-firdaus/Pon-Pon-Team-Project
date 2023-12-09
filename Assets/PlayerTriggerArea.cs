using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTriggerArea : MonoBehaviour
{
    public Player player;
    public bool playerInTriggerArea => player != null;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Player entered trigger area");
        if (other.GetComponentInParent<Player>())
        {
            player = other.GetComponentInParent<Player>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Player exited trigger area");
        if (other.GetComponentInParent<Player>())
        {
            player = null;
        }
    }
}
