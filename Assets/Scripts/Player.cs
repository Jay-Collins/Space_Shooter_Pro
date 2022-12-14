using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    private float _speedPowerupAdded = 5.0f;
    private float _canFire = -1f;
    private float _timeChangePerSecond;
    private int _missiles = 3;
    private int _shieldStength = 3;
    private int _rng;
    private bool _trippleShotEnabled, _shieldsEnabled, _spreadShotEnabled;
    private bool _movementDisabled;
    private bool _shootingDisabled;
    private bool _boostDisabled;
    private bool _boosting;
    private bool _missilesDisabled;
    private bool _spinEnabled;
    private bool _launchToSideLeft;
    private bool _launchToSideRight;
    private Quaternion _rotation;
    private BoxCollider2D _boxCollider2D;
    private AudioSource _audioSource;
    private SpriteRenderer _shieldSpriteRenderer;
    
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private GameObject _camera;
    [SerializeField] private SpriteRenderer _shieldColor;

    [Header("Managers")]
    [SerializeField] private SpawnManager _spawnManager;
    [SerializeField] private UImanager _uiManager;

    [Header("General Settings")]
    [SerializeField] private float _speed = 3.0f;
    [SerializeField] private float _fireRate = 0.5f;
    [SerializeField] private float _boostTimer = 1f;
    [SerializeField] private int _playerLives = 3;
    [SerializeField] private int _score = 0;
    [SerializeField] private float _boostSpeedAdded = 5.0f;
    [SerializeField] private int _ammoCount = 30;

    [Header("Power-up Settings")]
    [SerializeField] private float _trippleShotTimer = 5;
    [SerializeField] private float _speedBoostTimer = 5;
    [SerializeField] private float _spreadShotTimer = 5;

    [Header("Prefab Settings")]
    [SerializeField] private GameObject _shieldVisulizer;
    [SerializeField] private GameObject _leftDamageVisualizer;
    [SerializeField] private GameObject _rightDamageVisualizer;
    [SerializeField] private GameObject _thrusterVisualizer;
    [SerializeField] private GameObject _laserPrefab;
    [SerializeField] private GameObject _trippleShotPrefab;
    [SerializeField] private GameObject _spreadShotPrefab;
    [SerializeField] private GameObject _explosionPrefab;
    [SerializeField] private GameObject _missilePrefab;
    
    [Header("SFX Settings")]
    [SerializeField] private AudioClip _outOfAmmoSFX;
    

    // Start is called before the first frame update

    private void Awake()
    {
        _shieldColor = _shieldVisulizer.GetComponent<SpriteRenderer>();
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
        AmmoUpdate();
        ActivateBoost();
        FireMissile();
        Spin();
        LaunchToSideMovement();
        ShieldStatus();

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire && _ammoCount >= 1)
        {
            ShootLaser();
        }
        else if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire && _ammoCount == 0)
        {
            AudioSource.PlayClipAtPoint(_outOfAmmoSFX, transform.position); 
        }
    }

    void CalculateMovement()
    {
        if (_movementDisabled != true)
        {
            var horizontalInput = Input.GetAxis("Horizontal");
            var verticalInput = Input.GetAxis("Vertical");

            // new Vector3(1, 0, 0) * input * speed * real time
            var direction = new Vector3(horizontalInput, verticalInput, 0);

            transform.Translate(direction * (_speed * Time.deltaTime));

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
        _ammoCount--;
        _canFire = Time.time + _fireRate;
        if (_trippleShotEnabled)
        {
            Instantiate(_trippleShotPrefab, transform.position + new Vector3(0, 0, 0), Quaternion.identity);
            _audioSource.Play();
        }
        else if (_spreadShotEnabled)
        {
            Instantiate(_spreadShotPrefab, transform.position + new Vector3(0, 0, 0), Quaternion.identity);
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
            _shieldStength--;
        }
        else
        {
            _playerLives -= 1;
            _uiManager.LivesUpdate(_playerLives);
            StartCoroutine(CameraShake());

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

    IEnumerator CameraShake()
    {
        var shakeTime = new WaitForSeconds(0.05f);
        Vector3 _defaultPosition = new Vector3(0, 1, -10);
        
        yield return shakeTime;
        _camera.transform.position = new Vector3(0.3f, 1, -10);
        yield return shakeTime;
        _camera.transform.position = new Vector3(-0.3f, 1, -10);
        yield return shakeTime;
        _camera.transform.position = new Vector3(0, 0.7f, -10);
        yield return shakeTime;
        _camera.transform.position = new Vector3(0, 1.3f, -10);
        yield return shakeTime;
        _camera.transform.position = _defaultPosition;
    }

    public void ActivateBoost()
    {
        if (_boostDisabled) return;
        _boosting = Input.GetKey(KeyCode.LeftShift);

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            _speed += _boostSpeedAdded;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            _speed -= _boostSpeedAdded;
        }

        switch (_boosting)
        {
            case true:
                _timeChangePerSecond = 0.5f;
                _boostTimer = Mathf.Clamp((_boostTimer -= _timeChangePerSecond * Time.deltaTime), 0, 1);
                _uiManager.BoostUpdate(_boostTimer);
                break;
            case false:
                _timeChangePerSecond = 0.1f;
                _boostTimer = Mathf.Clamp((_boostTimer += _timeChangePerSecond * Time.deltaTime), 0, 1);
                _uiManager.BoostUpdate(_boostTimer);
                break;
        }
        
        if (!(_boostTimer <= 0f)) return;
        _speed = 12;
        StartCoroutine(CannotBoostRoutine());
    }

    private IEnumerator CannotBoostRoutine()
    {
        _boostDisabled = true;
        yield return new WaitForSeconds(5);
        _boostDisabled = false;
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

    public void SpreadShotActive()
    {
        _trippleShotEnabled = false;
        _spreadShotEnabled = true;
        StartCoroutine(SpreadShotPowerDownRoutine());
    }

    IEnumerator SpreadShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(_spreadShotTimer);
        _spreadShotEnabled = false;
    }

    public void SpeedBoostActive()
    {
        _speed += _speedPowerupAdded;
        StartCoroutine(SpeedBoostPowerDownRoutine());
    }

    IEnumerator SpeedBoostPowerDownRoutine()
    {
        yield return new WaitForSeconds(_speedBoostTimer);
        _speed -= _speedPowerupAdded;
    }

    public void ShieldsActive()
    {
        _shieldStength = 3;
        if (_shieldsEnabled) return;
        _shieldsEnabled = true; 
        _shieldVisulizer.SetActive(true);
    }

    public void Shatter()
    {
        Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
        _shieldStength = 0;
        _shieldsEnabled = false;
        _shieldVisulizer.SetActive(false);
        Damage();
    }

    // refill players ammo
    public void RefillAmmo() =>_ammoCount = 30;
    
    public void RefillHealth()
    {
        if (_playerLives < 3)
        {
            _playerLives += 1;
            _uiManager.LivesUpdate(_playerLives);
        
            switch (_playerLives)
            {
                case 2:
                    _rightDamageVisualizer.SetActive(false);
                    break;
                case 3:
                    _leftDamageVisualizer.SetActive(false);
                    break;
            }
        }
    }

    // method to add 10 to the score // communicate with UI to update score
    public void ScoreUpdate(int points)
    {
        _score += points;
        _uiManager.ScoreUpdate(_score);
    }

    private void AmmoUpdate() => _uiManager.AmmoUpdate(_ammoCount);

    private IEnumerator PlayerDeathSequence()
    {
        var deadExplosion = Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
        _uiManager.PlayerDead();
        deadExplosion.transform.parent = gameObject.transform;
        _missilesDisabled = true;
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

    private void FireMissile()
    {
        if (_missilesDisabled == false && _missiles > 0 && Input.GetKeyDown(KeyCode.E))
        {
            Instantiate(_missilePrefab, transform.position + new Vector3(0, 1.65f, 0), Quaternion.identity);
            _missiles--;
            _uiManager.MissilesUpdate(_missiles);
        }
    }

    public void RefillMissiles()
    {
        _missiles = 3;
        _uiManager.MissilesUpdate(_missiles);
    }

    public IEnumerator PlayerSpin()
    {
        _rotation = transform.rotation;
        _spinEnabled = true;
        yield return new WaitForSeconds(0.445f);
        _spinEnabled = false;
        transform.rotation = _rotation;
    }

    private void Spin()
    {
        if (!_spinEnabled) return;
        var cameraTransform = Camera.main.transform;
        transform.Rotate(new Vector3(0, 0, 800 * Time.deltaTime), Space.Self);
    }

    public IEnumerator LaunchToSide()
    {
        _rng = Random.Range(1, 3);
        switch (_rng)
        {
            case 1:
                _movementDisabled = true;
                _launchToSideLeft = true;
                yield return new WaitForSeconds(0.1f);
                _launchToSideLeft = false;
                _movementDisabled = false;
                break;
            case 2:
                _movementDisabled = true;
                _launchToSideRight = true;
                yield return new WaitForSeconds(0.1f);
                _launchToSideRight = false;
                _movementDisabled = false;
                break;
        }
    }

    private void LaunchToSideMovement()
    {
        if (_launchToSideLeft)
        {
            transform.Translate(new Vector3(-6,0,0) * (_speed * Time.deltaTime));
        }
        else if (_launchToSideRight)
        {
            transform.Translate(new Vector3(6,0,0) * (_speed * Time.deltaTime));
        }
    }

    private void ShieldStatus()
    {
        //null check
        if (!_spriteRenderer) return;
        switch (_shieldStength)
        {
            case 3:
                _shieldColor.color = Color.white;
                break;
            case 2:
                _shieldColor.color = new Color(0.25f, 0.69f, 0.25f);
                break;
            case 1:
                _shieldColor.color = Color.red;
                break;
            case 0:
                _shieldsEnabled = false;
                _shieldVisulizer.SetActive(false);
                break;
        }
    }
}
