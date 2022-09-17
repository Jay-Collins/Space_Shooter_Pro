using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private float _fireRate = 3.0f;
    private float _canFire = -1f;

    private Player _player;
    private AudioSource _audioSource;
    [SerializeField] private GameObject _enemyLaserPrefab;
    [SerializeField] private float _speed = 5f;
    [SerializeField] private BoxCollider2D _boxCollider2D;  
    [SerializeField] private Animator _animator;

    private void Awake()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        _audioSource = GetComponent<AudioSource>();
        NullChecks();
    }

    private void NullChecks()
    {
        if (!_player) Debug.LogError("The Player is Null");
        if (!_animator) Debug.LogError("The Animator is Null");
    }

    // Update is called once per frame
    void Update()
    {
        EnemyMovement();

        if (Time.time > _canFire)
        {
            _fireRate = Random.Range(3f, 7f);
            _canFire = Time.time + _fireRate;
            /*Instantiate(_enemyLaserPrefab, transform.position, Quaternion.identity);
            Debug.Break();*/
            GameObject enemyLaser = Instantiate(_enemyLaserPrefab, transform.position, Quaternion.identity);
            Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();
            
            for (int i = 0; i < lasers.Length; i++ )
            {
                lasers[i].AssignEnemyLaser();
            }
        }

        if (transform.position.y > -6.0f) return;
        EnemyRespawn();
    }

    // When Enemy collides with other objects. 
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _player.Damage();
            EnemyDeathSequence();
        }

        // if other is laser // destroy laser // destroy us
        if (other.CompareTag("Laser"))
        {
            Destroy(other.gameObject);
            // Null check // score update
            if (_player) _player.ScoreUpdate(10);
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
        Destroy(_boxCollider2D);
        _speed = 0.5f;
        _audioSource.Play();
        _animator.SetTrigger("OnEnemyDeath");
        Destroy(gameObject, 2.8f);
    }
}