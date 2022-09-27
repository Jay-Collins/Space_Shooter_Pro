using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    // speed variable of 8
    private bool _isEnemyLaser;
    [SerializeField] private float _speed = 8.0f;
    [SerializeField] private AudioClip _playerHitSFX;

    // Update is called once per frame
    void Update()
    {
        // check if enemy laser is false or else (true)
        if (!_isEnemyLaser)
            MoveUp();
        else
            MoveDown();
    }

    void MoveUp()
    {
        // translate laser up
        transform.Translate(new Vector3(0, 1, 0) * _speed * Time.deltaTime);

        // if laser position is greater than 7 on y axis // destroy the object
        if (transform.position.y < 9f) return;
        // null check and destroy parent.
        if (transform.parent) Destroy(transform.parent.gameObject);
        Destroy(gameObject);
    }

    void MoveDown()
    {
        // translate laser down
        transform.Translate(new Vector3(0, -1, 0) * _speed * Time.deltaTime);

        // if laser position is greater than 7 on y axis // destroy the object
        if (transform.position.y > -9f) return;
        // null check and destroy parent. 
        if (transform.parent) Destroy(transform.parent.gameObject);
        Destroy(gameObject);
    }

    public void AssignEnemyLaser() => _isEnemyLaser = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        //Check if other is "Player" AND _isEnemyLaser is true. 
        if (other.CompareTag("Enemy") && _isEnemyLaser) return;

        if (!other.CompareTag("Player") || !_isEnemyLaser) return;
        Player player = other.GetComponent<Player>();
        
        // Null check and run player Damage method.
        if (player == null) return;
        AudioSource.PlayClipAtPoint(_playerHitSFX, transform.position);
        player.Damage();

    }
}
