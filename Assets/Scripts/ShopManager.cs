using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ShopManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TMP_Text balanceText;
    [SerializeField] private TMP_Text notEnoughMoneyText;
    [SerializeField] private TMP_Text[] glassesNames;
    [SerializeField] private TMP_Text[] glassesPrices;

    [Header("Shop Items")]
    [SerializeField] private int[] glassesCosts;
    [SerializeField] private GameObject[] glassesObjects;

    private int balance;
    private int displayedBalance;
    private int selectedGlassesIndex = -1;
    private float balanceUpdateSpeed = 500f; // Скорость уменьшения баланса
    private GameAudio _gameAudio;

    private void Start()
    {
        balance = PlayerPrefs.GetInt("TotalMoney", 1000);
        displayedBalance = balance;
        UpdateBalanceText();

        notEnoughMoneyText.gameObject.SetActive(false);
        LoadShopState();

        _gameAudio = GetComponent<GameAudio>();
    }

    private void UpdateBalanceText()
    {
        balanceText.text = "Balance: " + displayedBalance;
    }

    private void LoadShopState()
    {
        selectedGlassesIndex = PlayerPrefs.GetInt("SelectedGlasses", -1);

        for (int i = 0; i < glassesNames.Length; i++)
        {
            if (PlayerPrefs.GetInt("GlassesBought_" + i, 0) == 1)
            {
                glassesPrices[i].text = (i == selectedGlassesIndex) ? "Selected" : "Select";
                glassesObjects[i].SetActive(i == selectedGlassesIndex);
            }
            else
            {
                glassesPrices[i].text = glassesCosts[i].ToString();
                glassesObjects[i].SetActive(false);
            }
        }
    }

    public void BuyOrSelectGlasses(int index)
    {
        if (PlayerPrefs.GetInt("GlassesBought_" + index, 0) == 1)
        {
            SetSelectedGlasses(index);
            _gameAudio.PlaySpinSound();
        }
        else
        {
            if (balance >= glassesCosts[index])
            {
                int cost = glassesCosts[index];
                balance -= cost;
                PlayerPrefs.SetInt("TotalMoney", balance);
                _gameAudio.PlayCashSound();
                // Запускаем плавное уменьшение отображаемого баланса
                StartCoroutine(DecreaseBalanceSmoothly(cost));

                PlayerPrefs.SetInt("GlassesBought_" + index, 1);
                SetSelectedGlasses(index);
            }
            else
            {
                StartCoroutine(ShowNotEnoughMoneyMessage());
            }
        }
    }

    private IEnumerator DecreaseBalanceSmoothly(int amount)
    {
        while (displayedBalance > balance)
        {
            displayedBalance -= Mathf.CeilToInt(balanceUpdateSpeed * Time.deltaTime);
            displayedBalance = Mathf.Max(displayedBalance, balance); // Останавливаемся на целевом балансе
            UpdateBalanceText();
            yield return null;
        }
    }

    private void SetSelectedGlasses(int index)
    {
        for (int i = 0; i < glassesPrices.Length; i++)
        {
            if (i == index)
            {
                glassesPrices[i].text = "Selected";
                glassesObjects[i].SetActive(true);
            }
            else
            {
                if (PlayerPrefs.GetInt("GlassesBought_" + i, 0) == 1)
                {
                    glassesPrices[i].text = "Select";
                }
                else
                {
                    glassesPrices[i].text = glassesCosts[i].ToString();
                }
                glassesObjects[i].SetActive(false);
            }
        }

        selectedGlassesIndex = index;
        PlayerPrefs.SetInt("SelectedGlasses", index);
        PlayerPrefs.Save();
    }

    private IEnumerator ShowNotEnoughMoneyMessage()
    {
        notEnoughMoneyText.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        notEnoughMoneyText.gameObject.SetActive(false);
    }
}
