using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PowerupDetector : MonoBehaviour
{
    [SerializeField] private Vector3 offset, size, direction;
    [SerializeField] private float angle;
    [SerializeField] private int enemyID;
    private RaycastHit2D hit;
    
    private void Update()
    {
        hit = Physics2D.BoxCast(transform.position - offset, size, angle, direction);
        Debug.Log(hit.collider.gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position - offset, size);
    }

    /*private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Powerup")) return; 
        enemy.FireLaser();
    }*/
}
