using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UImanager : MonoBehaviour
{
    // handle to Text
    [SerializeField] private TMP_Text _scoreText;

    private int _points = 0;

    // Start is called before the first frame update
    void Start()
    {
        // asign text component to the handle
        _scoreText.text = "Score: " + _points;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ScoreUpdate() => _points += 10;
}
