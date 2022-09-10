using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float _speed = 5f;

    // Update is called once per frame
    void Update()
    {
        // call enemy movement method
        EnemyMovement();

        if (transform.position.y < -5.57f)
        {
            EnemyRespawn();
        }
    }

    // When Enemy collides with other objects. 
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Null checking
            Player player = other.transform.GetComponent<Player>();

            if (player != null)
            {
                player.Damage();
            }

            Object.Destroy(this.gameObject);
        }

        // if other is laser // destroy laser // destroy us
        if (other.CompareTag("Laser"))
        {
            Object.Destroy(other.gameObject);
            // add 10 to score

            Object.Destroy(this.gameObject);
        }
    }

    // define EnemyMovement Method
    void EnemyMovement() => transform.Translate(new Vector3(0, -1f, 0) * _speed * Time.deltaTime);

    // define EnemyRespawn method
    void EnemyRespawn()
    {
        var randomX = Random.Range(-9.15f, 9.15f);
        transform.position = new Vector3(randomX, 7.35f, 0);
    }
}