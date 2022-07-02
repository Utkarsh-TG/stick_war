using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleArmour : MonoBehaviour
{
    private PlayerController playerController;
    public float despawnTime = 5;
    
    // Start is called before the first frame update
    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        StartCoroutine("DeSpawn");
    }

    IEnumerator DeSpawn()
    {
        yield return new WaitForSecondsRealtime(despawnTime);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerController.ActivateDoubleArmour();
            Destroy(gameObject);
        }
    }
}
