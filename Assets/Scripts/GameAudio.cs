using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameAudio : MonoBehaviour
{
    [SerializeField] private AudioClip _clickSound;
    [SerializeField] private AudioClip _spinSound;
    [SerializeField] private AudioClip _winSound;
    [SerializeField] private AudioClip _changeBetSound;
    [SerializeField] private AudioClip _cashSound;
    [SerializeField] private AudioClip _diceGameSound;
    [SerializeField] private AudioClip _loseGameSound;
    [SerializeField] private AudioClip _failLockSound;
    [SerializeField] private AudioClip _openLockSound;
    private AudioSource _audioSource;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void PlayClickSound()
    {
        _audioSource.PlayOneShot(_clickSound);
    }

    public void PlaySpinSound()
    {
        _audioSource.PlayOneShot(_spinSound);
    }

    public void PlayWinSound()
    {
        _audioSource.PlayOneShot(_winSound);
    }

    public void PlayChangeBetSound()
    {
        _audioSource.PlayOneShot(_changeBetSound);
    }

    public void PlayCashSound()
    {
        _audioSource.PlayOneShot(_cashSound);
    }

    public void PlayDiceSound()
    {
        _audioSource.PlayOneShot(_diceGameSound);
    }

    public void PlayLoseSound()
    {
        _audioSource.PlayOneShot(_loseGameSound);
    }

    public void PlayFailLockSound()
    {
        _audioSource.PlayOneShot(_failLockSound);
    }

    public void PlayOpenLockSound()
    {
        _audioSource.PlayOneShot(_openLockSound);
    }
}
