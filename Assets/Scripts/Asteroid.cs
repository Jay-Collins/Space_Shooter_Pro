using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    
    private Player _player;
    private SpawnManager _spawnManager;
    private CircleCollider2D _circleCollider;
    [SerializeField] private GameObject _exposionPrefab;
    [SerializeField] float _rotateSpeed = 10f;

    private void Awake()
    {
        _circleCollider = GetComponent<CircleCollider2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        _player = GameObject.Find("Player").GetComponent<Player>();
        if (!_player)
        {
            Debug.LogError("Player is NULL");
        }
    }

    // Update is called once per frame
    void Update()
    {
        AsteroidMovement();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Laser"))
        {
            _circleCollider.enabled = false;
            Destroy(other.gameObject);
         
            _spawnManager.StartSpawning();
            AsteroidDeathSequence();
        }
        else if (other.CompareTag("Player"))
        {
            _player.Damage();
            AsteroidDeathSequence();
        }
    } 

    void AsteroidMovement()
    {
        transform.Rotate(new Vector3(0, 0, 1) * _rotateSpeed * Time.deltaTime);

        if (transform.position.y > -6.0f) return;
        Destroy(gameObject);
    }

    void AsteroidDeathSequence()
    {
        Instantiate(_exposionPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject, 1.2f);
    }
}
