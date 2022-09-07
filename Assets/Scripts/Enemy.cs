using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _speed = 5f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // call enemy movement method
        EnemyMovement();

        //if bottom of screen respawn at top with a new random position method
        if (transform.position.y < -5.57f)
        {
            EnemyRespawn();
        }
    }

    // When Enemy collides with other objects. 
    private void OnTriggerEnter(Collider other)
    {
        // if other is Player
        // destroy player
        // destroy us
        if (other.CompareTag("Player"))
        {
            // damage player
            // Null checking
            Player player = other.transform.GetComponent<Player>();

            if (player != null)
            {
                player.Damage();
            }

            Object.Destroy(this.gameObject);
        }

        // if other is laser
        // destroy laser
        // destroy us
        if (other.CompareTag("Laser"))
        {
            Object.Destroy(other.gameObject);
            Object.Destroy(this.gameObject);
        }
    }

    // define EnemyMovement Method
    void EnemyMovement()
    {
        transform.Translate(new Vector3(0, -1f, 0) * _speed * Time.deltaTime);
    }

    // define EnemyRespawn method
    void EnemyRespawn()
    {
        float randomX = Random.Range(-9.15f, 9.15f);
        transform.position = new Vector3(randomX, 7.35f, 0);
    }
}