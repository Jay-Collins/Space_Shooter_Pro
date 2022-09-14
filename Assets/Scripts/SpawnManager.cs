using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    private bool _spawnEnabled = true;
    private bool _spawnPowerupEnabled;
    [SerializeField] private GameObject _enemyContainer, _enemyPrefab;
    [SerializeField] private GameObject[] _powerups;
    [SerializeField] private float _spawnTimer = 5;
    [SerializeField] private float _trippleShotSpawnTimer;

    public void StartSpawning()
    {
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerupRoutine());
    }

    IEnumerator SpawnEnemyRoutine()
    {
        yield return new WaitForSeconds(5);
        while (_spawnEnabled == true)
        {
            var randomX = Random.Range(-9.15f, 9.15f);
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

        while (_spawnPowerupEnabled && _spawnEnabled )
        {
            var randomX = Random.Range(-9.15f, 9.15f);
            Vector3 posToSpawn = new Vector3(randomX, 7.35f, 0);
            _trippleShotSpawnTimer = Random.Range(8f, 12f);

            var randomPowerup = Random.Range(0, 3);
            GameObject newPowerup = Instantiate(_powerups[randomPowerup], posToSpawn, Quaternion.identity);
            yield return new WaitForSeconds(_trippleShotSpawnTimer);
        }
    }

    public void StopSpawning() => _spawnEnabled = false;
}

