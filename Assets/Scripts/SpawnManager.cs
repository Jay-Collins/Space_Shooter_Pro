using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnManager : MonoBehaviour
{
    private bool _spawnPowerupEnabled;
    private bool _displayWavesText = true;
    private bool _finalBoss;
    private bool _waveStart = true;
    private int _rareSpawn;
    private int _wave = 1;
    private int _enemiesKilled;
    private int _RNG;
    private int _enemiesToSpawn;

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
        StartCoroutine(SpawnPowerupRoutine());
    }
    
    private void SpawnWavesRoutine()
    {
        switch (_wave)
        {
            case 1:
                _enemiesToSpawn = 5;
                if (_waveStart)
                {
                    StartCoroutine(SpawnWave());
                    _waveStart = false;
                }
                if (_enemiesKilled == _enemiesToSpawn) NextWave();
                break;
            
            case 2:
                _enemiesToSpawn = 8;
                if (_waveStart)
                {
                    StartCoroutine(SpawnWave());
                    _waveStart = false;
                }
                if (_enemiesKilled == _enemiesToSpawn) NextWave();
                break;
            
            case 3:
                _enemiesToSpawn = 12;
                if (_waveStart)
                {
                    StartCoroutine(SpawnWave());
                    _waveStart = false;
                }
                if (_enemiesKilled == _enemiesToSpawn) NextWave();
                break;
            
            case 4: // boss wave
                _enemiesToSpawn = 1;
                if (!_finalBoss)
                {
                    BossSpawn();
                    _enemiesToSpawn--;
                    _finalBoss = true;
                }
                if (_enemiesKilled == 1)
                {
                    StopCoroutine(_uiManager.Waves(_wave));
                    NextWave();
                }
                break;
            
            case 5: // win screen
                _spawnPowerupEnabled = false;
                _uiManager.WinScreenDisplay();
                break;
        }
    }

    private void NextWave()
    {
        _waveStart = true;
        _wave++;
        _displayWavesText = true;
        _enemiesKilled = 0;
    }

    private void ShowWavesText()
    {
        if (!_displayWavesText) return;
        StartCoroutine(_uiManager.Waves(_wave));
        _displayWavesText = false;
    }

    private void EnemySpawn()
    {
        var randomX = Random.Range(-9.15f, 9.15f); 
        var posToSpawn = new Vector3(randomX, 7.35f, 0); 
        _RNG = Random.Range(1, 101);
            
        GameObject newEnemy;
        switch (_RNG)
        { 
            case <= 70: 
                newEnemy = Instantiate(_enemyPrefabs[Random.Range(0, 1)], posToSpawn, Quaternion.identity); 
                newEnemy.transform.parent = _enemyContainer.transform; 
                break;
            case > 70: 
                newEnemy = Instantiate(_rareEnemyPrefabs[Random.Range(0, 2)], posToSpawn, Quaternion.identity);
                newEnemy.transform.parent = _enemyContainer.transform;
                break;
        }
    }

    private IEnumerator SpawnPowerupRoutine()
    {
        yield return new WaitForSeconds(_spawnTimer);
        _spawnPowerupEnabled = true;

        while (_spawnPowerupEnabled)
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

    public void StopSpawning()
    {
        _enemiesToSpawn = -1;
        _spawnPowerupEnabled = false;
    }

    private IEnumerator SpawnWave()
    {
        for (var spawns = 0; spawns < _enemiesToSpawn; spawns++)
        {
            yield return new WaitForSeconds(_spawnTimer);
            EnemySpawn();
        }
    }
}

