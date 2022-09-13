using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Player _player;
    private Animator _animator;
    [SerializeField] private BoxCollider2D _boxCollider2D;  
    [SerializeField] private float _speed = 5f;

    private void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null)    
        {
            Debug.LogError("The Player is Null");
        }

        _animator = GetComponent<Animator>();
        if (_animator == null)
        {
            Debug.LogError("The Animator is Null");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // call enemy movement method
        EnemyMovement();

        if (transform.position.y < -6.0f)
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
            EnemyDeathSequence();
        }

        // if other is laser // destroy laser // destroy us
        if (other.CompareTag("Laser"))
        {
            Destroy(other.gameObject);
            if (_player != null)
            {
                _player.ScoreUpdate(10);
            }
            EnemyDeathSequence();
        }
    }

    void EnemyMovement() => transform.Translate(new Vector3(0, -1f, 0) * _speed * Time.deltaTime);

    void EnemyRespawn()
    {
        var randomX = Random.Range(-9.15f, 9.15f);
        transform.position = new Vector3(randomX, 7.35f, 0);
    }

    private void EnemyDeathSequence()
    {
        Destroy(this._boxCollider2D);
        _speed = 0.5f;
        _animator.SetTrigger("OnEnemyDeath");
        Destroy(this.gameObject, 2.8f);

    }
}