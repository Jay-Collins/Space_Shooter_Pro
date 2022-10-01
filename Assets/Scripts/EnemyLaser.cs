using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLaser : MonoBehaviour
{
// speed variable of 8
    private bool _isEnemyHornetLaser;
    [SerializeField] private float _speed = 8.0f;
    [SerializeField] private AudioClip _playerHitSFX;

    // Update is called once per frame
    void Update()
    {
        if (_isEnemyHornetLaser)
        {
            MoveUp();
        }
        else
        {
            MoveDown();
        }
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
    
    public void AssignEnemyHornetLaser() => _isEnemyHornetLaser = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy")) return;

        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            
            if (player == null) return;
            AudioSource.PlayClipAtPoint(_playerHitSFX, transform.position);
            player.Damage();
            if (transform.parent) Destroy(transform.parent.gameObject);
            Destroy(gameObject);
        }

        if (other.CompareTag("Powerup"))
        {
            var powerUp = other.GetComponent<PowerUp>();
            powerUp.DestroyPowerup();
            Destroy(gameObject);
        }
    }
}
