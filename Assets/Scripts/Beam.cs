using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Beam : MonoBehaviour
{
    [SerializeField] private AudioClip _playerHitSFX;
    
    void Start() => StartCoroutine(DestroyBeam());

    private IEnumerator DestroyBeam()
    {
        yield return new WaitForSeconds(0.05f);
        Destroy(gameObject);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return; 
        var player = other.GetComponent<Player>();
        
        if (player == null) return;
        AudioSource.PlayClipAtPoint(_playerHitSFX, transform.position);
        player.Damage();
    }
}
