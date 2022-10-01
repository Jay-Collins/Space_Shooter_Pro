using System;
using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy3 : MonoBehaviour
{
    [Header("General Settings")] 
    private float _fireRate = 3f;
    private float _canFire = -1;
    private float _backwardsFireRate = 1f;
    private float _backwardsCanFire = -1;
    private bool _dead;
    private Player _player;
    private AudioSource _audioSource;
    private SpawnManager _spawnManager;
    [SerializeField] private GameObject _explosionPrefab;
    [SerializeField] private BoxCollider2D _boxCollider2D;
    [SerializeField] private GameObject _enemyLaserPrefab;
    [SerializeField] private GameObject _enemySingleLaserPrefab;
    [SerializeField] private float _speed = 3f;
    
    [Header("Powerup Detector Settings")] 
    [SerializeField] private Vector3 offset;
    [SerializeField] private Vector3 size;
    [SerializeField] private Vector3 direction;
    [SerializeField] private float angle;
    private RaycastHit2D hit;

    private void Awake()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        NullChecks();
    }

    private void NullChecks()
    {
        if (!_player) Debug.LogError("The Player is NULL.");
        if (!_spawnManager) Debug.LogError("The Spawn Manager is NULL.");
    }

    private void Start()
    {
        StartCoroutine(DetectPowerup());
    }

    void Update()
    {
        Movement();
        if (transform.position.y > -5.5f) return;
        EnemyRespawn();
    }

    private void Movement()
    {
        if (_player == null) return;
        if (transform.position.y < _player.transform.position.y - 2f)
        {
            FireBackwards();
            var targetPos = new Vector3(_player.transform.position.x, transform.position.y, transform.position.z);
            _speed = 1f;
            transform.position = Vector3.MoveTowards(transform.position, targetPos, 5 * Time.deltaTime);
            transform.Translate(new Vector3(0, -1, 0) * (_speed * Time.deltaTime));
        }
        else
        {
            FireForwards();
            _speed = 3;
            transform.Translate(new Vector3(0,-1,0) * (_speed * Time.deltaTime));
        }
    }

    private void FireForwards()
    {
        if (_dead) return;
        if (!(Time.time > _canFire)) return;
        _fireRate = Random.Range(3f, 7f);
        _canFire = Time.time + _fireRate;
        Instantiate(_enemyLaserPrefab,new Vector3(transform.position.x, transform.position.y - 1.35f, 0), Quaternion.identity);
    }

    private void FireBackwards()
    {
        if (_dead) return;
        if (!(Time.time > _backwardsCanFire)) return;
        _backwardsFireRate = Random.Range(0.5f, 1.5f);
        _backwardsCanFire = Time.time + _backwardsFireRate;
        
        if (transform.position.y == _player.transform.position.y) return;
        GameObject enemyLaser = Instantiate(_enemySingleLaserPrefab,new Vector3(transform.position.x, transform.position.y + 1.15f, 0), Quaternion.identity);
        var EnemyLaser = enemyLaser.GetComponentInChildren<EnemyLaser>();
        EnemyLaser.AssignEnemyHornetLaser();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Laser"))
        {
            Destroy(other.gameObject);
            if (_player) _player.ScoreUpdate(10);
            EnemyDeathSequence();
        }
        else if (other.CompareTag("Player"))
        {
            _player.Damage();
            EnemyDeathSequence();
        }
    }

    private void EnemyRespawn()
    {
        var randomX = Random.Range(-9.15f, 9.15f);
        transform.position = new Vector3(randomX, 7.35f, 0);
    }

    // Defines BoxCast and parameters
    private IEnumerator DetectPowerup()
    {
        while (true)
        {
            hit = Physics2D.BoxCast(transform.position - offset, size, angle, direction);
            // Debug.Log(hit.collider.gameObject);
            if (!hit)
            {
                yield return new WaitForSeconds(0.5f);
                continue;
            }

            if (hit.collider.CompareTag("Powerup"))
            {
                FireForwards();
            }
            yield return new WaitForSeconds(0.5f);
        }
    }
    
    // Draws BoxCast parameters in editor to make it visible
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position - offset, size); // actual size of BoxCast is infinity
    }
    
    private void EnemyDeathSequence()
    {
        _dead = true;
        _spawnManager.EnemyKilled();
        transform.gameObject.tag = "DeadEnemy";
        Destroy(_boxCollider2D);
        _speed = 0.5f;
        Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject, 0.2f);
    }
}
