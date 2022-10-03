using UnityEngine;

public class PowerUp : MonoBehaviour
{
    private int _speed = 3;
    private bool _powerupMovement= true;
    private Player _player;
    [SerializeField] private GameObject _explosionPrefab;
    [SerializeField] private AudioClip _powerupSFXClip;
    [SerializeField] private int _powerupID; // 0 = Triple shot, 1= speed, 2= shields.

    private void Awake()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        PowerupMovement();
        PlayerMagnetism();
    }

    // ontriggercollision // only collectable by the player, use tags // on collected destroy // communicate with player script 
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();

            AudioSource.PlayClipAtPoint(_powerupSFXClip, transform.position);

            if (player)
            {
                switch (_powerupID)
                {
                    case 0:
                        player.TripleShotActive();
                        break;
                    case 1:
                        player.SpeedBoostActive();
                        break;
                    case 2:
                        player.ShieldsActive();
                        break;
                    case 3:
                        player.RefillAmmo();
                        break;
                    case 4:
                        player.RefillHealth();
                        break;
                    case 5:
                        player.SpreadShotActive();
                        break;
                    case 6:
                        player.Shatter();
                        break;
                    case 7:
                        player.RefillMissiles();
                        break;
                }
            }
            Destroy(gameObject);
        }
    }

    // move down at speed of 3 - able to adjust in inspector // When we leave the screen destroy this object
    private void PowerupMovement()
    {
        if (_powerupMovement) transform.Translate(new Vector3(0, -1f, 0) * (_speed * Time.deltaTime));
        
        if (transform.position.y < -6f) Destroy(gameObject);
    }

    public void DestroyPowerup()
    {
        Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void PlayerMagnetism()
    {
        var _playerMagnetism = Input.GetKey(KeyCode.C);

        if (_playerMagnetism)
        {
            _powerupMovement = false;
            if (!_player) return;
            var playerPos = _player.transform.position;
            var enemyPos = transform.position;
            var moveTowards = (Vector3.MoveTowards(enemyPos, playerPos, _speed * Time.deltaTime));
            transform.position = moveTowards;
        }
        else
        {
            _powerupMovement = true;
        }
    }
}
