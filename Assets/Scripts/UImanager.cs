using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UImanager : MonoBehaviour
{
    private int _score = 0;
    private GameManager _gameManager;
    [SerializeField] private Image _livesImage;
    [SerializeField] private TMP_Text _gameOverText;
    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private TMP_Text _restartText;
    [SerializeField] private Sprite[] _liveSprites;

    // Start is called before the first frame update
    void Start()
    {

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

    public void LivesUpdate(int currentLives) => _livesImage.sprite = _liveSprites[currentLives];

    public void GameOverDisplay()
    {
        _gameManager.GameOver();
        _gameOverText.gameObject.SetActive(true);
        _restartText.gameObject.SetActive(true);
        StartCoroutine(GameOverFlicker());
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
}