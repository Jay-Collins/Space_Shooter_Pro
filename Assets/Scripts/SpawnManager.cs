using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnManager : MonoBehaviour
{
    private bool _spawnEnabled = true;
    private bool _spawnEnemiesEnabled = true;
    private bool _spawnPowerupEnabled;
    private bool _displayWavesText = true;
    private bool _finalBoss;
    private int _rareSpawn;
    private int _wave = 1;
    private int _enemiesSpawned;
    private int _enemiesKilled;
    private int _RNG;

    private UImanager _uiManager;
    [SerializeField] private GameObject _enemyContainer;
    [SerializeField] private GameObject _bossPrefab;
    [SerializeField] private GameObject[] _enemyPrefabs;
    [SerializeField] private GameObject[] _rareEnemyPrefabs;
    [SerializeField] private GameObject[] _commonPowerups;
    [SerializeField] private GameObject[] _mediumPowerups;
    [SerializeField] private GameObject[] _rarePowerups;
    [SerializeField] private GameObject _ammoPickup;
    [SerializeField] private float _spawnTimer = 1;
    [SerializeField] private float _powerupSpawnTimer;

    public void Awake()
    {
        _uiManager = GameObject.Find("Canvas").GetComponent<UImanager>();
    }

    private void Update()
    {
        SpawnWavesRoutine();
        ShowWavesText();
    }

    public void StartSpawning()
    {
        StartCoroutine(EnemySpawn());
        StartCoroutine(SpawnPowerupRoutine());
    }
    
    private void SpawnWavesRoutine()
    {
        switch (_wave)
        {
            case 1:
                if (_enemiesSpawned == 2) _spawnEnemiesEnabled = false;
                if (_enemiesKilled == 2) NextWave();
                break;
            
            case 2:
                if (_enemiesSpawned == 2) _spawnEnemiesEnabled = false;
                if (_enemiesKilled == 2) NextWave();
                break;
            
            case 3:
                if (_enemiesSpawned == 2) _spawnEnemiesEnabled = false;
                if (_enemiesKilled == 2) NextWave();
                break;
            
            case 4: // boss wave
                _spawnEnemiesEnabled = false;
                if (!_finalBoss)
                {
                    BossSpawn();
                    _finalBoss = true;
                }
                if (_enemiesSpawned == 1) _spawnEnemiesEnabled = false;
                if (_enemiesKilled == 1)
                {
                    StopCoroutine(_uiManager.Waves(_wave));
                    NextWave();
                }
                break;
            
            case 5: // win screen
                _uiManager.WinScreenDisplay();
                break;
        }
    }

    private void NextWave()
    {
        _wave++;
        _displayWavesText = true;
        _enemiesSpawned = 0;
        _enemiesKilled = 0;
        _spawnEnemiesEnabled = true;
    }

    private void ShowWavesText()
    {
        if (!_displayWavesText) return;
        StartCoroutine(_uiManager.Waves(_wave));
        _displayWavesText = false;
    }

    private IEnumerator EnemySpawn()
    {
        yield return new WaitForSeconds(_spawnTimer);
        while (_spawnEnabled && _spawnEnemiesEnabled)
        {
            var randomX = Random.Range(-9.15f, 9.15f);
            var posToSpawn = new Vector3(randomX, 7.35f, 0);
            _RNG = Random.Range(1, 101);
            
            GameObject newEnemy;
            switch (_RNG)
            {
                case <= 70:
                    _enemiesSpawned++;
                    newEnemy = Instantiate(_enemyPrefabs[Random.Range(0, 1)], posToSpawn, Quaternion.identity);
                    newEnemy.transform.parent = _enemyContainer.transform;
                    break;
                case > 70:
                    _enemiesSpawned++;
                    newEnemy = Instantiate(_rareEnemyPrefabs[Random.Range(0, 2)], posToSpawn, Quaternion.identity);
                    newEnemy.transform.parent = _enemyContainer.transform;
                    break;
            }
            yield return new WaitForSeconds(_spawnTimer);
        }
    }

    private IEnumerator SpawnPowerupRoutine()
    {
        yield return new WaitForSeconds(_spawnTimer);
        _spawnPowerupEnabled = true;

        while (_spawnPowerupEnabled && _spawnEnabled )
        {
            var randomX = Random.Range(-9.15f, 9.15f);
            Vector3 posToSpawn = new Vector3(randomX, 7.35f, 0);
            _powerupSpawnTimer = Random.Range(4f, 6f);

            _RNG = Random.Range(1, 101);
            switch (_RNG)
            {
                case <= 5:
                    Instantiate(_rarePowerups[Random.Range(0, 2)], posToSpawn, Quaternion.identity);
                    break;
                case >= 6 and <=20:
                    Instantiate(_mediumPowerups[Random.Range(0, 2)], posToSpawn, Quaternion.identity);
                    break;
                case >=21 and < 70:
                    Instantiate(_commonPowerups[Random.Range(0, 3)], posToSpawn, Quaternion.identity);
                    break;
                case >= 70:
                    Instantiate(_ammoPickup, posToSpawn, Quaternion.identity);
                    break;
            }
            yield return new WaitForSeconds(_powerupSpawnTimer);
        }
    }

    private void BossSpawn() => Instantiate(_bossPrefab, new Vector3(0, 11, 0), Quaternion.identity);
    
    public void EnemyKilled() => _enemiesKilled++;

    public void StopSpawning() => _spawnEnabled = false;
}