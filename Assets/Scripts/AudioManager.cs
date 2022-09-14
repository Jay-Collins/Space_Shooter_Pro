using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private AudioSource _audioSource;
    [SerializeField] private AudioClip _laserSFX;
    [SerializeField] private AudioClip _explosionSFX;
    [SerializeField] private AudioClip _powerupSFX;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void PlayLaserSound()
    {
        _audioSource.clip = _laserSFX;
        _audioSource.Play();
    }

    public void PlayExplosionSound()
    {
        _audioSource.clip = _explosionSFX;
        _audioSource.Play();
    }

    public void PlayerPoweripSound()
    {
        _audioSource.clip = _powerupSFX;
        _audioSource.Play();
    }
}