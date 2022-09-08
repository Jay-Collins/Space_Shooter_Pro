using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class Player : MonoBehaviour 
{
    [SerializeField] 
    private float _speed = 3.0f;
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private GameObject _trippleShotPrefab;
    [SerializeField]
    private float _fireRate = 0.5f;
    private float _canFire = -1f;
    [SerializeField]
    private int _playerLives = 3;
    private bool _trippleShotEnabled = false;
    private SpawnManager _spawnManager;
    [SerializeField]
    private float _trippleShotTimer = 5;

// Start is called before the first frame update
void Start()
    {
        // take the current position = new position (0,0,0)
        transform.position = new Vector3(0, -1, 0);
        // define _spawnManager variable. 
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();

        if (_spawnManager == null)
        {
            Debug.LogError("The spawn manager is NULL.");
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
        _playerLives -= 1;

        // check if dead // destroy us
        if (_playerLives < 1 )
        {
            // communicate with spawn manager // tell it to stop spawning
            _spawnManager.StopSpawning();
            Destroy(this.gameObject);
        }
    }

    public void TrippleShotActive()
    {
        // activate tripple shot // start power down coroutine for tripple shot
        _trippleShotEnabled = true;
        StartCoroutine(TrippleShotPowerDownRoutine());
    }

    // IEnumerator TrippleShotPowerDownRoutine // wait 5 seconds // set tripple shot to false
    IEnumerator TrippleShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(_trippleShotTimer);
        _trippleShotEnabled = false;
    }
}
