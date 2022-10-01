using System;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UIElements;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

public class Enemy : MonoBehaviour
{
    [Header("General Settings")] 
    private float _fireRate = 3.0f;
    private float _canFire = -1f;
    private float _canDodge = 1.5f;
    private float _currentPOS;
    private int _moveChance;
    private bool _movementDisabled;
    private bool _sideMoveEnabledLeft;
    private bool _sideMoveEnabledRight;
    private bool _shieldEnabled;
    private bool _dead;
    private bool _dodge;
    private Player _player;
    private AudioSource _audioSource;
    private SpawnManager _spawnManager;
    [SerializeField] private float distance;
    [SerializeField] private GameObject _enemyLaserPrefab;
    [SerializeField] private GameObject _shieldVisualizer;
    [SerializeField] private float _speed = 5f;
    [SerializeField] private BoxCollider2D _boxCollider2D;
    [SerializeField] private Animator _animator;
    private static readonly int OnEnemyDeath = Animator.StringToHash("OnEnemyDeath");

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
        StartCoroutine(DetectPowerup());
        ShieldChance();
    }

    // Update is called once per frame
    private void Update()
    {
        Movement();
        RamPlayer();
        //FireLaser();

        if (transform.position.y > -6.0f) return;
        EnemyRespawn();
    }

    public void FireLaser()
    {
        if (Time.time > _canFire && !_dead)
        {
            _fireRate = Random.Range(3f, 7f);
            _canFire = Time.time + _fireRate;
            Instantiate(_enemyLaserPrefab, new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity);
            
            /*GameObject enemyLaser = Instantiate(_enemyLaserPrefab, new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity);
            Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();

            for (int i = 0; i < lasers.Length; i++)
            {
                lasers[i].AssignEnemyLaser();
                Debug.Log(lasers[i].IsEnemyLaser);
            }*/
        }
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
                    _shieldEnabled = false;
                    _shieldVisualizer.SetActive(false);
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
            if (_dodge) transform.Translate(new Vector3(-3, -1f, 0) * _speed * Time.deltaTime);
            else transform.Translate(new Vector3(-1, -1f, 0) * _speed * Time.deltaTime);
        }
        else if (_sideMoveEnabledRight)
        {
            if (_dodge) transform.Translate(new Vector3(3, -1f, 0) * _speed * Time.deltaTime);
            else transform.Translate(new Vector3(1, -1f, 0) * _speed * Time.deltaTime);
        }
        else
        {
            transform.Translate(new Vector3(0, -1f, 0) * _speed * Time.deltaTime);
        }
    }

    private IEnumerator MoveChanceRoutine()
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

    private void RamPlayer()
    {
        if (!_player) return;
        var playerPos = _player.transform.position;
        var enemyPos = transform.position;
        var moveTowards = (Vector3.MoveTowards(enemyPos, playerPos, _speed * Time.deltaTime));
        distance = Vector3.Distance(playerPos, enemyPos);

        if (distance < 4)
        {
            _movementDisabled = true;
            transform.position = moveTowards;
        }
        else
        {
            _movementDisabled = false;
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

    // Defines BoxCast and parameters
    private IEnumerator DetectPowerup()
    {
        while (_dead == false)
        {
            hit = Physics2D.BoxCast(transform.position - offset, size, angle, direction);
            //Debug.Log(hit.collider.gameObject);
            if (!hit)
            {
                yield return new WaitForSeconds(0.1f);
                continue;
            }

            if (hit.collider.CompareTag("Powerup"))
            {
                FireLaser();
            }

            if (hit.collider.CompareTag("Laser"))
            {
                StartCoroutine(DodgeLaser());
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

    private IEnumerator DodgeLaser()
    {
        var leftOrRightChance = Random.Range(1, 3);

        if (!(Time.time > _canDodge)) yield break;
        switch (leftOrRightChance)
        {
            case 1:
                _dodge = true;
                _sideMoveEnabledLeft = true;
                yield return new WaitForSeconds(0.2f);
                _sideMoveEnabledLeft = false;
                _dodge = false;
                break;
            
            case 2:
                _dodge = true;
                _sideMoveEnabledRight = true;
                yield return new WaitForSeconds(0.2f);
                _sideMoveEnabledRight = false;
                _dodge = false;
                break;
        }
    }
    
    private void EnemyDeathSequence()
    {
        _dead = true;
        _spawnManager.EnemyKilled();
        transform.gameObject.tag = "DeadEnemy";
        Destroy(_boxCollider2D);
        _speed = 0.5f;
        _audioSource.Play();
        _animator.SetTrigger(OnEnemyDeath);
        Destroy(gameObject, 2.8f);
    }
}