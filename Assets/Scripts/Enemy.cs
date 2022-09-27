using System;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    private float _fireRate = 3.0f;
    private float _canFire = -1f;
    private float _currentPOS;
    private int _moveChance;
    private bool _movementDisabled;
    private bool _sideMoveEnabledLeft;
    private bool _sideMoveEnabledRight;
    private bool _shieldEnabled;

    private Player _player;
    private AudioSource _audioSource;
    private SpawnManager _spawnManager;
    [SerializeField] private GameObject _enemyLaserPrefab;
    [SerializeField] private GameObject _shieldVisualizer;
    [SerializeField] private float _speed = 5f;
    [SerializeField] private BoxCollider2D _boxCollider2D;  
    [SerializeField] private Animator _animator;
    private static readonly int OnEnemyDeath = Animator.StringToHash("OnEnemyDeath");

    private void Awake()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        _audioSource = GetComponent<AudioSource>();
        NullChecks();
    }

    private void NullChecks()
    {
        if (!_player) Debug.LogError("The Player is Null");
        if (!_animator) Debug.LogError("The Animator is Null");
        if (!_spawnManager) Debug.LogError("The Spawn Manager is Null");
    }

    private void Start()
    {
        StartCoroutine(MoveChanceRoutine());
        ShieldChance();
    }

    // Update is called once per frame
    void Update()
    {
        Movement();

        if (Time.time > _canFire)
        {
            _fireRate = Random.Range(3f, 7f);
            _canFire = Time.time + _fireRate;
            GameObject enemyLaser = Instantiate(_enemyLaserPrefab, new Vector3(transform.position.x,transform.position.y - 1.35f,0), Quaternion.identity);
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
            switch (_shieldEnabled)
            {
                case true:
                    _player.Damage();
                    break;
                case false:
                    _player.Damage();
                    EnemyDeathSequence();
                    break;
            }
        }

        // if other is laser // destroy laser // destroy us
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

    private void Movement()
    {
        if (_movementDisabled) return;
        if (_sideMoveEnabledLeft)
        {
            transform.Translate(new Vector3(-1, -1f, 0) * _speed * Time.deltaTime);
        }
        else if (_sideMoveEnabledRight)
        {
            transform.Translate(new Vector3(1, -1f, 0) * _speed * Time.deltaTime);
        }
        else
        {
            transform.Translate(new Vector3(0, -1f, 0) * _speed * Time.deltaTime);
        }
    }

    IEnumerator MoveChanceRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            var sideMoveChance = Random.Range(1, 4);
            var leftOrRightChance = Random.Range(1, 3);
            if (sideMoveChance == 2 && leftOrRightChance == 1)
            {
                _sideMoveEnabledLeft = true;
                yield return new WaitForSeconds(0.5f);
                _sideMoveEnabledLeft = false;
            }
            else if (sideMoveChance == 2 && leftOrRightChance == 2)
            {
                _sideMoveEnabledRight = true;
                yield return new WaitForSeconds(0.5f);
                _sideMoveEnabledRight = false;
            }
            yield return new WaitForSeconds(3);
        }
    } 
    
    private void ShieldChance()
    {
        var shieldRNG = Random.Range(0, 101);
        if (shieldRNG < 85) return;
        _shieldVisualizer.SetActive(true);
        _shieldEnabled = true;
    }
    
    void EnemyRespawn()
    {
        var randomX = Random.Range(-9.15f, 9.15f);
        transform.position = new Vector3(randomX, 7.35f, 0);
    }

    private void EnemyDeathSequence()
    {
        _spawnManager.EnemyKilled();
        Destroy(_boxCollider2D);
        _speed = 0.5f;
        _audioSource.Play();
        _animator.SetTrigger(OnEnemyDeath);
        Destroy(gameObject, 2.8f);
    }
}