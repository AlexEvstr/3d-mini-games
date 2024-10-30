using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class SlotMachine : MonoBehaviour
{
    [SerializeField] private TMP_Text balanceText;
    [SerializeField] private TMP_InputField betInput;
    [SerializeField] private Button betIncreaseButton;
    [SerializeField] private Button betDecreaseButton;
    [SerializeField] private Slot[] slots;
    [SerializeField] private Combinations[] combinations;
    [SerializeField] private Button _spinBtn;
    [SerializeField] private Button _closeBtn;
    private SceneTransition sceneTransition;
    private int balance;
    private int betAmount;
    private float balanceUpdateSpeed = 1000f;
    public float timeInterval = 0.05f;
    private int stoppedSlots = 3;
    private bool isSpin = false;

    private void Start()
    {
        sceneTransition = GetComponent<SceneTransition>();
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        _spinBtn.onClick.AddListener(Spin);
        balance = PlayerPrefs.GetInt("TotalMoney", 1000);
        UpdateBalanceText();

        UpdateBalanceText();
        if (balance > 0) betAmount = 100;

        betInput.text = betAmount.ToString();
        betInput.onEndEdit.AddListener(OnEndEditBetInput); // Срабатывает при завершении ввода

        betIncreaseButton.onClick.AddListener(() => ChangeBetAmount(10));
        betDecreaseButton.onClick.AddListener(() => ChangeBetAmount(-10));

        _closeBtn.onClick.AddListener(CloseBtn);
    }

    private void CloseBtn()
    {
        sceneTransition.LoadSceneWithFade("MainMenu");
    }

    void UpdateBalanceText()
    {
        balanceText.text = balance.ToString();
    }

    void OnEndEditBetInput(string input)
    {
        int result;
        if (balance > 0)
        {
            if (int.TryParse(input, out result))
            {
                // Ограничиваем ставку, если она выходит за пределы
                betAmount = Mathf.Clamp(result, 10, balance);
            }
            else
            {
                betAmount = 10; // Если введено нечисловое значение, ставим минимум 10
            }
            betInput.text = betAmount.ToString(); // Обновляем значение в поле
        }
        else
        {
            betInput.text = 0.ToString();
        }

    }

    void ChangeBetAmount(int change)
    {
        if (balance > 0)
        {
            int.TryParse(betInput.text, out betAmount);
            betAmount = Mathf.Clamp(betAmount + change, 10, balance);
            betInput.text = betAmount.ToString();
        }
    }

    IEnumerator UpdateBalance(int amount)
    {
        
        int targetBalance = Mathf.Max(balance + amount, 0);
        while (balance != targetBalance)
        {
            balance += (int)Mathf.Sign(amount) * Mathf.Min(Mathf.Abs(targetBalance - balance), (int)(balanceUpdateSpeed * Time.deltaTime));
            UpdateBalanceText();
            yield return null;
            DisableSPinBtn();
        }

        EnableSpinBtn();

        if (betAmount > balance)
        {
            betAmount = balance;
            betInput.text = betAmount.ToString(); // Обновляем текстовое значение в InputField
        }

        if (balance <= 0)
        {
            balance = 0;
            betAmount = 0;
            UpdateBalanceText();
            DisableSPinBtn();
            betInput.text = "0";
        }

        PlayerPrefs.SetInt("TotalMoney", balance);
        PlayerPrefs.Save();
    }

    private void Spin()
    {
        if (!isSpin && balance - betAmount >= 0)
        {
            ChangeMoney(-betAmount);
            isSpin = true;
            foreach (Slot i in slots)
            {
                i.StartCoroutine("Spin");
            }
        }
    }

    public void DisableSPinBtn()
    {
        _spinBtn.interactable = false;
    }

    private void EnableSpinBtn()
    {
        _spinBtn.interactable = true;
    }

    public void WaitResults()
    {
        stoppedSlots -= 1;
        if(stoppedSlots <= 0)
        {
            stoppedSlots = 3;
            CheckResults();
        }
    }

    public void CheckResults()
    {
        isSpin = false;
        foreach (Combinations i in combinations)
        {
            if (slots[0].gameObject.GetComponent<Slot>().stoppedSlot.ToString() == i.FirstValue.ToString()
                && slots[1].gameObject.GetComponent<Slot>().stoppedSlot.ToString() == i.SecondValue.ToString()
                && slots[2].gameObject.GetComponent<Slot>().stoppedSlot.ToString() == i.ThirdValue.ToString())
            {
                StartCoroutine(UpdateBalance(10 * betAmount));
            }
        }
        EnableSpinBtn();
    }

    private void ChangeMoney(int count)
    {
        balance += count;
        balanceText.text = balance.ToString();
        PlayerPrefs.SetInt("TotalMoney", balance);
        PlayerPrefs.Save();
    }
}

[System.Serializable]
public class Combinations
{
    public enum SlotValue
    {
        Bar,
        Cherry,
        Diamond,
        Ring,
        Seven
    }

    public SlotValue FirstValue;
    public SlotValue SecondValue;
    public SlotValue ThirdValue;
}
