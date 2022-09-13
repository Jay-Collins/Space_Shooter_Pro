using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor.Search;
using UnityEngine;

public class Player : MonoBehaviour 
{
    private float _speedMultiplier = 1.5f;
    private float _canFire = -1f;
    private bool _trippleShotEnabled, _shieldsEnabled;
    private SpawnManager _spawnManager;
    private UImanager _uiManager;

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
    [SerializeField] private GameObject _laserPrefab;
    [SerializeField] private GameObject _trippleShotPrefab;

    // Start is called before the first frame update
    void Start()
    {
        // take the current position = new position (0,0,0)
        transform.position = new Vector3(0, -1, 0);
 
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();

        _uiManager = GameObject.Find("Canvas").GetComponent<UImanager>();

        if (_spawnManager == null)
        {
            Debug.LogError("The spawn manager is NULL.");
        }

        if (_uiManager == null)
        {
            Debug.LogError("The uiManager is NULL.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();
        
        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
        {
            ShootLaser();
        }
    }

    // player movement method
    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // new Vector3(1, 0, 0) * input * speed * real time
        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);

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

    // ShootLaser method
    void ShootLaser()
    {
        _canFire = Time.time + _fireRate;
        if (_trippleShotEnabled == true)
        {
            Instantiate(_trippleShotPrefab, transform.position + new Vector3(0, 0, 0), Quaternion.identity);
        }
        else
        {
            Instantiate(_laserPrefab, transform.position + new Vector3(0, 1, 0), Quaternion.identity);
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

            if (_playerLives == 2)
            {
                _leftDamageVisualizer.SetActive(true);
            }
            else if (_playerLives == 1)
            {
                _rightDamageVisualizer.SetActive(true);
            }

            if (_playerLives < 1)
            {
                // communicate with spawn manager // tell it to stop spawning // Visualize game over text
                _uiManager.GameOverDisplay();
                _spawnManager.StopSpawning();
                Destroy(this.gameObject);
            }
        }
    }

    public void TripleShotActive()
    {
        // activate tripple shot // start power down coroutine for tripple shot
        _trippleShotEnabled = true;
        StartCoroutine(TripleShotPowerDownRoutine());
    }

    // IEnumerator TrippleShotPowerDownRoutine // wait 5 seconds // set tripple shot to false
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

    public void SheildsActive()
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
}
