using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [SerializeField]private float _speed = 3;
    [SerializeField] private int _powerupID; // 0 = Triple shot, 1= speed, 2= shields.

    // Update is called once per frame
    void Update() => PowerupMovement();

    // ontriggercollision // only collectable by the player, use tags // on collected destroy // communicate with player script 
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();

            if (!player)
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
                        player.SheildsActive();
                        break;
                }
            }
            Destroy(gameObject);
        }
    }

    // move down at speed of 3 - able to adjust in inspector // When we leave the screen destroy this object
    void PowerupMovement()
    {
        transform.Translate(new Vector3(0, -1f, 0) * _speed * Time.deltaTime);

        if (transform.position.y < -6f)
        {
            Destroy(gameObject);
        }
    }
}
