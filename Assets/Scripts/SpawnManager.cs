using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemyContainer;
    [SerializeField]
    private GameObject _enemyPrefab;
    [SerializeField]
    private GameObject _trippleShotPrefab;
    
    [SerializeField]
    private float _spawnTimer = 5;
    
    [SerializeField]
    private float _trippleShotSpawnTimer;


    private bool _spawnEnabled = true;
    private bool _spawnPowerupEnabled = false;


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerupRoutine());
    }

    // Update is called once per frame
    void Update()
    {

    }

    // spawn game object every 5 seconds
    // create a coroutine of type IEnumerator -- Yield Events
    // while loop

    IEnumerator SpawnEnemyRoutine()
    {
        //while loop (infinite loop)
        //Instantiate enemy prefab
        //yield wait for 5 seconds
        while (_spawnEnabled == true)
        {
            float randomX = Random.Range(-9.15f, 9.15f);
            Vector3 posToSpawn = new Vector3(randomX, 7.35f, 0);

            GameObject newEnemy = Instantiate(_enemyPrefab, posToSpawn, Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;
            yield return new WaitForSeconds(_spawnTimer);
        }
    }

    IEnumerator SpawnPowerupRoutine()
    {
        //every 3-7 seconds, spawn in a powerup
        yield return new WaitForSeconds(_spawnTimer);
        _spawnPowerupEnabled = true;

        while (_spawnPowerupEnabled == true && _spawnEnabled == true)
        {
            float randomX = Random.Range(-9.15f, 9.15f);
            Vector3 posToSpawn = new Vector3(randomX, 7.35f, 0);
            _trippleShotSpawnTimer = Random.Range(3f, 8f);

            GameObject newPowerup = Instantiate(_trippleShotPrefab, posToSpawn, Quaternion.identity);
            yield return new WaitForSeconds(_trippleShotSpawnTimer);
        }
        
    }

    public void StopSpawning()
    {
        _spawnEnabled = false;
    }

    public void SpawnLocation()
    {
        float randomX = Random.Range(-9.15f, 9.15f);
        Vector3 posToSpawn = new Vector3(randomX, 7.35f, 0);
    }
}

