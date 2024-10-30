using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _shopButton;
    [SerializeField] private GameObject _slots;
    [SerializeField] private GameObject _slots_anim;
    [SerializeField] private GameObject _wheel;
    [SerializeField] private GameObject _wheel_anim;
    [SerializeField] private GameObject _dice;
    [SerializeField] private GameObject _dice_anim;
    [SerializeField] private GameObject _chest;
    [SerializeField] private GameObject _chest_anim;
    [SerializeField] private TMP_Text _balanceText;
    [SerializeField] private Button _plus100Balance;
    private SceneTransition sceneTransition;

    private void Start()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        _playButton.gameObject.SetActive(false);
        _playButton.onClick.AddListener(PlayGame);
        sceneTransition = GetComponent<SceneTransition>();
        _shopButton.onClick.AddListener(OpenShop);

        int balance = PlayerPrefs.GetInt("TotalMoney", 1000);
        _balanceText.text = balance.ToString();

        if (balance <= 0)
        {
            _plus100Balance.gameObject.SetActive(true);
            _plus100Balance.onClick.AddListener(IncreaseBalance);
        }
    }

    public void IncreaseBalance()
    {
        StartCoroutine(IncreaseBalanceCoroutine());
    }

    private IEnumerator IncreaseBalanceCoroutine()
    {
        int currentBalance = 0; // Начальное значение
        _plus100Balance.gameObject.SetActive(true); // Включаем объект, если нужно показать что-то визуально на старте

        // Плавное увеличение баланса
        while (currentBalance < 100)
        {
            currentBalance = Mathf.Min(currentBalance + Mathf.CeilToInt(100 * Time.deltaTime), 100);
            _balanceText.text = currentBalance.ToString();
            yield return null;
        }

        // Сохраняем целевое значение в PlayerPrefs
        PlayerPrefs.SetInt("TotalMoney", 100);

        // Отключаем объект, если нужно
        _plus100Balance.gameObject.SetActive(false);
    }

    private void OpenShop()
    {
        sceneTransition.LoadSceneWithFade("ShopScene");
    }

    public void ShowPlaySlots()
    {
        _playButton.gameObject.SetActive(true);
        _slots.SetActive(false);
        _slots_anim.SetActive(true);
    }

    public void HidePlaySlots()
    {
        _playButton.gameObject.SetActive(false);
        _slots_anim.SetActive(false);
        _slots.SetActive(true);
    }

    public void ShowPlayWheel()
    {
        _playButton.gameObject.SetActive(true);
        _wheel.SetActive(false);
        _wheel_anim.SetActive(true);
    }

    public void HidePlayWheel()
    {
        _playButton.gameObject.SetActive(false);
        _wheel_anim.SetActive(false);
        _wheel.SetActive(true);
    }

    public void ShowPlayDice()
    {
        _playButton.gameObject.SetActive(true);
        _dice.SetActive(false);
        _dice_anim.SetActive(true);
    }

    public void HidePlayDice()
    {
        _playButton.gameObject.SetActive(false);
        _dice_anim.SetActive(false);
        _dice.SetActive(true);
    }

    public void ShowPlayChest()
    {
        _playButton.gameObject.SetActive(true);
        _chest.SetActive(false);
        _chest_anim.SetActive(true);
    }

    public void HidePlayChest()
    {
        _playButton.gameObject.SetActive(false);
        _chest_anim.SetActive(false);
        _chest.SetActive(true);
    }

    private void PlayGame()
    {
        string savedSceneName = PlayerPrefs.GetString("SceneNameToPlay", "");
        sceneTransition.LoadSceneWithFade(savedSceneName);
    }
}