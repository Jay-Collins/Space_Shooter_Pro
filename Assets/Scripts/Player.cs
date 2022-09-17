using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using static System.Net.WebRequestMethods;

public class Player : MonoBehaviour
{
    private float _speedMultiplier = 1.5f;
    private float _canFire = -1f;
    private bool _trippleShotEnabled, _shieldsEnabled;
    private bool _movementDisabled;
    private bool _shootingDisabled;
    private BoxCollider2D _boxCollider2D;
    private AudioSource _audioSource;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    [Header("Managers")]
    [SerializeField] private SpawnManager _spawnManager;
    [SerializeField] private UImanager _uiManager;

    [Header("General Settings")]
    [SerializeField] private float _speed = 3.0f;
    [SerializeField] private float _fireRate = 0.5f;
    [SerializeField] private int _playerLives = 3;
    [SerializeField] private int _score = 0;

    [Header("Power-up Settings")]
    [SerializeField] private float _trippleShotTimer = 5;
    [SerializeField] private float _speedBoostTimer = 5;

    [Header("Prefab Settings")]
    [SerializeField] private GameObject _shieldVisulizer;
    [SerializeField] private GameObject _leftDamageVisualizer;
    [SerializeField] private GameObject _rightDamageVisualizer;
    [SerializeField] private GameObject _thrusterVisualizer;
    [SerializeField] private GameObject _laserPrefab;
    [SerializeField] private GameObject _trippleShotPrefab;
    [SerializeField] private GameObject _exposionPrefab;

    // Start is called before the first frame update

    private void Awake()
    {
        _boxCollider2D = GetComponent<BoxCollider2D>();
        _audioSource = GetComponent<AudioSource>();

        NullChecks();
    }
    private void NullChecks()
    {
        if (!_spawnManager) Debug.LogError("The spawn manager is NULL.");
        if (!_uiManager) Debug.LogError("The uiManager is NULL.");
    }

    void Start()
    {
        transform.position = new Vector3(0, -1, 0);
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();

        //if (!(Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)) return;
        //ShootLaser();

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire) ShootLaser();
    }

    void CalculateMovement()
    {
        if (_movementDisabled != true)
        {
            var horizontalInput = Input.GetAxis("Horizontal");
            var verticalInput = Input.GetAxis("Vertical");

            // new Vector3(1, 0, 0) * input * speed * real time
            var direction = new Vector3(horizontalInput, verticalInput, 0);

            transform.Translate(direction * _speed * Time.deltaTime);

            transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -3.8f, 0), 0);

            // if player position on the x is greater than 11 set to -11 // else if player position on the x is less than -11 set to 11
            if (transform.position.x > 11)
            {
                transform.position = new Vector3(-11, transform.position.y, 0);
            }
            else if (transform.position.x < -11)
            {
                transform.position = new Vector3(11, transform.position.y, 0);
            }
        }
    }

    void ShootLaser()
    {
        _canFire = Time.time + _fireRate;
        if (_trippleShotEnabled)
        {
            Instantiate(_trippleShotPrefab, transform.position + new Vector3(0, 0, 0), Quaternion.identity);
            _audioSource.Play();
        }
        else if (!_shootingDisabled)
        {
            Instantiate(_laserPrefab, transform.position + new Vector3(0, 1, 0), Quaternion.identity);
            _audioSource.Play();
        }
    }

    public void Damage()
    {
        // if shields is active // do nothing // deactivate shields
        if (_shieldsEnabled)
        {
            _shieldsEnabled = false;
            _shieldVisulizer.SetActive(false);
            return;
        }
        else
        {
            _playerLives -= 1;
            _uiManager.LivesUpdate(_playerLives);

            switch (_playerLives)
            {
                case 0:
                    StartCoroutine(PlayerDeathSequence());
                    break;
                case 1:
                    _rightDamageVisualizer.SetActive(true);
                    break;
                case 2:
                    _leftDamageVisualizer.SetActive(true);
                    break;
            }   
        }
    }

    public void TripleShotActive()
    {
        _trippleShotEnabled = true;
        StartCoroutine(TripleShotPowerDownRoutine());
    }

    IEnumerator TripleShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(_trippleShotTimer);
        _trippleShotEnabled = false;
    }

    public void SpeedBoostActive()
    {
        _speed *= _speedMultiplier;
        StartCoroutine(SpeedBoostPowerDownRoutine());
    }

    IEnumerator SpeedBoostPowerDownRoutine()
    {
        yield return new WaitForSeconds(_speedBoostTimer);
        _speed /= _speedMultiplier;
    }

    public void ShieldsActive()
    {
        _shieldsEnabled = true; 
        _shieldVisulizer.SetActive(true);
    }

    // method to add 10 to the score // communicate with UI to update score
    public void ScoreUpdate(int points)
    {
        _score += points;
        _uiManager.ScoreUpdate(_score);
    }

    IEnumerator PlayerDeathSequence()
    {
        Instantiate(_exposionPrefab, transform.position, Quaternion.identity);
        _movementDisabled = true;
        _shootingDisabled = true;
        _trippleShotEnabled = false;
        _thrusterVisualizer.SetActive(false);
        _leftDamageVisualizer.SetActive(false);
        _rightDamageVisualizer.SetActive(false);
        _uiManager.GameOverDisplay();
        _spawnManager.StopSpawning();
        Destroy(_boxCollider2D);
        yield return new WaitForSeconds(1.2f);
        _spriteRenderer.enabled = false;
        Destroy(gameObject, 1.7f);
    }
}
