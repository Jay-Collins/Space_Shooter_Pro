using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UImanager : MonoBehaviour
{
    private int _ammoAmount = 15;
    private int _score = 0;
    private bool _playerDead;
    private GameManager _gameManager;

    [SerializeField] private Image _boostSlider;
    [SerializeField] private Image _livesImage;
    [SerializeField] private Image _missileImage;
    [SerializeField] private TMP_Text _gameOverText;
    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private TMP_Text _restartText;
    [SerializeField] private TMP_Text _ammoText;
    [SerializeField] private TMP_Text _wavesText;
    [SerializeField] private TMP_Text _mainMenuText;
    [SerializeField] private TMP_Text _youWinText;
    [SerializeField] private Sprite[] _liveSprites;
    [SerializeField] private Sprite[] _missileSprites;

    // Start is called before the first frame update
    private void Start()
    {
        _ammoText.text = "Ammo: " + _ammoAmount;
        _scoreText.text = "Score: " + _score;
        _gameOverText.gameObject.SetActive(false);
        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();

        if (_gameManager == null)
        {
            Debug.LogError("GameManager is NULL");
        }
    }

    public void ScoreUpdate(int playerScore)
    {
        _scoreText.text = "Score: " + playerScore.ToString();
    }

    public void AmmoUpdate(int ammo)
    {
        _ammoText.text = "Ammo: " + ammo + "/30";
    }
    
    public void BoostUpdate(float boost)
    {
        _boostSlider.fillAmount = boost;
    }
    public void LivesUpdate(int currentLives) => _livesImage.sprite = _liveSprites[currentLives];

    public void MissilesUpdate(int missiles) => _missileImage.sprite = _missileSprites[missiles];

    public void GameOverDisplay()
    {
        _gameManager.GameOver();
        _gameOverText.gameObject.SetActive(true);
        _restartText.gameObject.SetActive(true);
        StartCoroutine(GameOverFlicker());
    }

    public void WinScreenDisplay()
    {
        if (_playerDead) return;
        _gameManager.GameWin();
        _mainMenuText.gameObject.SetActive(true);
        _youWinText.gameObject.SetActive(true);
    }
    
    IEnumerator GameOverFlicker()
    {
        WaitForSeconds _waitForSeconds = new(0.2f);
        while (true)
        {
            yield return _waitForSeconds;
            _gameOverText.gameObject.SetActive(false);
            yield return _waitForSeconds;
            _gameOverText.gameObject.SetActive(true);
        }
    }

    public IEnumerator Waves(int wave)
    {
        _wavesText.gameObject.SetActive(true);
        _wavesText.text = "Wave: " + wave;
        yield return new WaitForSeconds(2.5f);
        _wavesText.gameObject.SetActive(false);
    }

    public void PlayerDead() => _playerDead = true;
}