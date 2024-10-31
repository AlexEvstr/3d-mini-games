using UnityEngine;
using UnityEngine.UI;

public class MenuAudio : MonoBehaviour
{
    [SerializeField] private Button _audioOn;
    [SerializeField] private Button _audioOff;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _clickSound;
    [SerializeField] private AudioClip _cashSound;

    private void Start()
    {
        _audioOn.onClick.AddListener(AudioTurnOff);
        _audioOff.onClick.AddListener(AudioTurnOn);

        float audio = PlayerPrefs.GetFloat("AudioVolume", 1);
        if (audio == 0)
        {
            AudioTurnOff();
        }
        else
        {
            AudioTurnOn();
        }
    }

    private void AudioTurnOff()
    {
        _audioOn.gameObject.SetActive(false);
        _audioOff.gameObject.SetActive(true);
        AudioListener.volume = 0;
        PlayerPrefs.SetFloat("AudioVolume", 0);
    }

    private void AudioTurnOn()
    {
        _audioOff.gameObject.SetActive(false);
        _audioOn.gameObject.SetActive(true);
        AudioListener.volume = 1;
        PlayerPrefs.SetFloat("AudioVolume", 1);
    }

    public void PlayClickSound()
    {
        _audioSource.PlayOneShot(_clickSound);
    }

    public void PlayCashSound()
    {
        _audioSource.PlayOneShot(_cashSound);
    }
}