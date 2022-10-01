using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy2 : MonoBehaviour
{
    private float _canFire = -1f;
    private float _fireRate;
    private bool _movementDisabled;
    private bool _dead;
    private bool _shieldEnabled;
    private SpawnManager _spawnManager;
    private Vector3 _movement;
    private Player _player;
    [SerializeField] float _speed = 2f;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private GameObject _explosionPrefab;
    [SerializeField] private BoxCollider2D _boxCollider2D;
    [SerializeField] private GameObject _beamPrefab;
    [SerializeField] private GameObject _shieldVisualizer;

    private void Awake()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        _audioSource = GetComponent<AudioSource>();
        _movement = new Vector3(0f, -1f, 0f);
    }

    private void Start()
    {
        StartCoroutine(ZigZag());
        ShieldChance();
    }

    private void Update()
    {
        Movement();
        
        if (!_dead) FireBeam();
        
        if (transform.position.y > -6.0f) return;
        EnemyRespawn();
    }

    private void Movement()
    {
        if (_movementDisabled) return;
        transform.Translate(_movement * (_speed * Time.deltaTime));
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            switch (_shieldEnabled)
            {
                case true:
                    _player.Damage();
                    _shieldEnabled = false;
                    _shieldVisualizer.SetActive(false);
                    break;
                case false:
                    _player.Damage();
                    EnemyDeathSequence();
                    break;
            }
        }
        
        if (other.CompareTag("Laser"))
        {
            switch (_shieldEnabled)
            {
                case true:
                    Destroy(other.gameObject);
                    _shieldEnabled = false;
                    _shieldVisualizer.SetActive(false);
                    break;
                case false:
                    Destroy(other.gameObject);
                    // Null check // score update
                    if (_player) _player.ScoreUpdate(10);
                    EnemyDeathSequence();
                    break;
            }
        }
    }
    private IEnumerator ZigZag()
    {
        var wait = new WaitForSeconds(1);
        while (true)
        {
            _movement.x = 2f;
            yield return wait;
            _movement.x = -2f;
            yield return wait;
        }
    }
    
    private void FireBeam()
    {
        if (Time.time > _canFire)
        {
            _fireRate = Random.Range(1f, 4.1f);
            _canFire = Time.time + _fireRate;
            Instantiate(_beamPrefab, new Vector3(transform.position.x, transform.position.y - 6, 0), Quaternion.identity);
        }
    }

    private void ShieldChance()
    {
        var shieldRNG = Random.Range(0, 101);
        if (shieldRNG < 85) return;
        _shieldVisualizer.SetActive(true);
        _shieldEnabled = true;
    }

    private void EnemyRespawn()
    {
        var randomX = Random.Range(-9.15f, 9.15f);
        transform.position = new Vector3(randomX, 7.35f, 0f);
    }
    
    private void EnemyDeathSequence()
    {
        _spawnManager.EnemyKilled();
        _dead = true;
        transform.gameObject.tag = "DeadEnemy";
        Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
        Destroy(_boxCollider2D);
        _speed = 0.5f;
        Destroy(gameObject, 1f);
    }
}
