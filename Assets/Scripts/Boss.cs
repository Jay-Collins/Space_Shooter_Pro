using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class Boss : MonoBehaviour
{
    private int _speed = 2;
    private int _health = 200;
    private int _rng;
    private bool _arrival;
    private bool _rammingShake;
    private bool _ram;
    private bool _moveBack;
    private bool _dead;
    private bool _shootingEnabled;
    private bool _strafeLeft;
    private bool _strafeRight;
    private bool _strafe;
    private GameObject _camera;
    private SpriteRenderer _spriteRenderer;
    private Player _player;
    private SpawnManager _spawnManager;
    [SerializeField] private Collider2D collider2D;
    [SerializeField] private GameObject beamPrefab;
    [SerializeField] private GameObject bossLaserPrefab;
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private Sprite[] bossSprite;
    [SerializeField] private GameObject firstDamageVisualizer;
    [SerializeField] private GameObject secondDamageVisualizer;
    [SerializeField] private GameObject thirdDamageVisualizer;

    // Start is called before the first frame update
    private void Start()
    {
        _camera = GameObject.Find("Main Camera");
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _player = GameObject.Find("Player").GetComponent<Player>();
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        StartCoroutine(CameraShake());
        StartCoroutine(StrafeCountdown());
        StartCoroutine(FireBeams());
        StartCoroutine(FireLasers());
    }

    // Update is called once per frame
    private void Update()
    {
        Arrival();
        Ram();
        MoveBack();
        Strafe();
    }

    private IEnumerator CameraShake()
    {
        var shakeTime = new WaitForSeconds(0.05f);
        var defaultPosition = new Vector3(0, 1, -10);

        while (_arrival == false)
        {
            yield return shakeTime;
            _camera.transform.position = new Vector3(0.3f, 1, -10);
            yield return shakeTime;
            _camera.transform.position = new Vector3(-0.3f, 1, -10);
            yield return shakeTime;
            _camera.transform.position = new Vector3(0, 0.7f, -10);
            yield return shakeTime;
            _camera.transform.position = new Vector3(0, 1.3f, -10);
            yield return shakeTime;
            _camera.transform.position = defaultPosition;
        }
    }

    private void Arrival()
    {
        if (_arrival) return;
        transform.Translate(new Vector3(0, -2, 0) * (_speed * Time.deltaTime));

        if (transform.position.y <= 5.05f)
        {
            _arrival = true;
            _shootingEnabled = true;
            StartCoroutine(BattleManager());
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Laser"))
        {
            _health--;
            StartCoroutine(DamageFlash());
            Destroy(other.gameObject);
            
            switch (_health)
            {
                case 75:
                    firstDamageVisualizer.SetActive(true);
                    break;
                case 50:
                    secondDamageVisualizer.SetActive(true);
                    break;
                case 25:
                    thirdDamageVisualizer.SetActive(true);
                    break;
                case <= 0:
                    if (_dead) return;
                    StartCoroutine(BossDeathSequence());
                        break;
            }   
        }

        if (other.CompareTag("Player") && _ram)
        {
            _player.Shatter();
            StartCoroutine(_player.PlayerSpin());
            StartCoroutine(_player.LaunchToSide());
        }
        else if (other.CompareTag("Player"))
        {
            _player.Damage();
        }
    }

    private IEnumerator DamageFlash()
    {
        _spriteRenderer.sprite = bossSprite[1];
        yield return new WaitForSeconds(0.05f);
        _spriteRenderer.sprite = bossSprite[0];
    }

    private IEnumerator RamShake()
    {
        var ramShake = new WaitForSeconds(.05f);
        var defaultPosition = transform.position;
        _shootingEnabled = false;

        for (var shakes = 0; shakes < 5; shakes++)
        {
            yield return ramShake;
            transform.position = new Vector3(transform.position.x + .3f, transform.position.y, 0);
            yield return ramShake;
            transform.position = new Vector3(transform.position.x - 0.3f, transform.position.y, 0);
            yield return ramShake;
            transform.position = new Vector3(transform.position.x, transform.position.y + 0.3f, 0);
            yield return ramShake;
            transform.position = new Vector3(transform.position.x, transform.position.y - 0.3f, 0);
            yield return ramShake;
            transform.position = defaultPosition;
        }
        _ram = true;
    }

    private void Ram()
    {
        if (!_ram) return;
        transform.Translate(new Vector3(0, -20, 0) * (_speed * Time.deltaTime));
        if (transform.position.y <= -1.5f)
        {
            _ram = false;
            _moveBack = true;
        }
    }

    private void MoveBack()
    {
        if (!_moveBack) return;
        transform.Translate(new Vector3(0, 2f, 0) * (_speed * Time.deltaTime));
        if (transform.position.y >= 5.05f)
        {
            _moveBack = false;
            _shootingEnabled = true;
        }
    }

    private IEnumerator BattleManager()
    {
        StartCoroutine(FireBeams());

        yield return null;
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(5, 10));
            StartCoroutine(RamShake());
        }
    }

    private IEnumerator FireBeams()
    {
        var beamPause = new WaitForSeconds(.5f);
        while (true)
        {
            yield return new WaitForSeconds(2f);
            if (!_shootingEnabled) continue;
            // left beam
            Instantiate(beamPrefab, new Vector3(transform.position.x + 0.9f, transform.position.y - 10.25f, 0), Quaternion.identity);
            yield return beamPause;
            if (!_shootingEnabled) continue;
            // right beam
            Instantiate(beamPrefab, new Vector3(transform.position.x - 0.9f, transform.position.y - 10.25f, 0), Quaternion.identity);
        }
    }

    private void Strafe()
    {
        if (!_strafe) return;
        if (_strafeLeft)
        {
            var moveTowards = (Vector3.MoveTowards(transform.position, new Vector3(-8, transform.position.y, 0), _speed * Time.deltaTime));
            transform.position = moveTowards;
            if (transform.position.x != -8) return;
            _strafeLeft = false;
            _strafeRight = true;
        }
        else if (_strafeRight)
        {
            var moveTowards = (Vector3.MoveTowards(transform.position, new Vector3(8, transform.position.y, 0), _speed * Time.deltaTime));
            transform.position = moveTowards;
            if (transform.position.x != 8) return;
            _strafeRight = false;
            _strafeLeft = true;
        }
    }

    private IEnumerator StrafeCountdown()
    {
        yield return new WaitForSeconds(1);
        _strafeLeft = true;
        _strafe = true;
    }

    private IEnumerator FireLasers()
    {
        while (true)
        {
            yield return new WaitForSeconds(2f);
            if (!_shootingEnabled) continue;
            Instantiate(bossLaserPrefab, new Vector3(transform.position.x, transform.position.y - 5.05f, 0), Quaternion.identity);
        }
    }

    private IEnumerator BossDeathSequence()
    {
        yield return null;
        _shootingEnabled = false;
        _strafe = false;
        _strafeLeft = false;
        _strafeRight = false;
        _ram = false;
        StopCoroutine(BattleManager());
        StopCoroutine(StrafeCountdown());
        collider2D.enabled = false;
        var finalExplosion = Instantiate(explosionPrefab, new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity);
        finalExplosion.transform.localScale = new Vector3(3f, 3f, 3f);
        yield return new WaitForSeconds(0.5f);
        _spawnManager.EnemyKilled();
        Destroy(gameObject);
    }
}