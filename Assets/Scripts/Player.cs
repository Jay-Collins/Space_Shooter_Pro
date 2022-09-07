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
    private float _fireRate = 0.5f;
    private float _canFire = -1f;
    [SerializeField]
    private int _playerLives = 3;

    private SpawnManager _spawnManager;


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

        // if  player position on the y is greater than 0
        // y position = 0
        // else if position on the y is than -3.8f
        // y pos = -3.8f
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -3.8f, 0), 0);

        // if player position on the x is greater than 11 set to -11
        // else if player position on the x is less than -11 set to 11
        if (transform.position.x > 11)
        {
            transform.position = new Vector3(-11, transform.position.y, 0);
        }
        else if (transform.position.x < -11)
        {
            transform.position = new Vector3(11, transform.position.y, 0);
        }
    }

    void ShootLaser()
    { 
        _canFire = Time.time + _fireRate;
        Instantiate(_laserPrefab, transform.position + new Vector3(0, 0.7f, 0), Quaternion.identity);
    }

    public void Damage()
    {
        _playerLives -= 1;

        // check if dead
        // destroy us
        if (_playerLives < 1 )
        {
            // communicate with spawn manager
            // tell it to stop spawning
            _spawnManager.StopSpawning();
            Destroy(this.gameObject);
        }
    }
}
