using System.Collections;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using Unity.VisualScripting;

public class ChesGameController : MonoBehaviour
{
    [SerializeField] private GameObject _lockAnimated;
    [SerializeField] private GameObject _lock;
    [SerializeField] private GameObject _lockFalse;
    [SerializeField] private GameObject _chestAnimated;
    [SerializeField] private GameObject _chest;

    [SerializeField] private GameObject _bet;
    [SerializeField] private GameObject _choose;

    [SerializeField] private TMP_Text balanceText;
    [SerializeField] private TMP_InputField betInput;
    [SerializeField] private Button betIncreaseButton;
    [SerializeField] private Button betDecreaseButton;
    [SerializeField] private Button okButton;
    [SerializeField] private Button[] chooseButton;
    [SerializeField] private GameObject _cube;
    private Rigidbody lockRigidbody;

    private int balance;
    private int betAmount;
    private float balanceUpdateSpeed = 1000f;
    private int randomLock;

    private void Start()
    {
        Screen.orientation = ScreenOrientation.Portrait;
        balance = PlayerPrefs.GetInt("TotalMoney", 1000);
        UpdateBalanceText();
        if (balance > 0) betAmount = 100;

        betInput.text = betAmount.ToString();
        betInput.onEndEdit.AddListener(OnEndEditBetInput); // Срабатывает при завершении ввода

        betIncreaseButton.onClick.AddListener(() => ChangeBetAmount(10));
        betDecreaseButton.onClick.AddListener(() => ChangeBetAmount(-10));
        okButton.onClick.AddListener(ShowChooseUI);

        foreach (Button button in chooseButton)
        {
            button.onClick.AddListener(() => ChooseLock(button.name));
        }

        ShowBet();

        lockRigidbody = _lockAnimated.GetComponent<Rigidbody>();
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

    private IEnumerator UnlockChest()
    {
        yield return new WaitForSeconds(1.0f);
        _lock.SetActive(false);
        _lockAnimated.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        _lockAnimated.GetComponent<Rigidbody>().useGravity = true;
        yield return new WaitForSeconds(0.5f);
        _chest.SetActive(false);
        _chestAnimated.SetActive(true);
        StartCoroutine(UpdateBalance(betAmount));
        yield return new WaitForSeconds(2.0f);
        _lock.SetActive(true);
        //Destroy(_chestAnimated);
        _lockAnimated.SetActive(false);

        lockRigidbody.useGravity = false;
        lockRigidbody.velocity = Vector3.zero;
        lockRigidbody.angularVelocity = Vector3.zero;
        _lockAnimated.transform.position = new Vector3(0.06f, 0.5f, -2.246284f);

        //_chestAnimated = Instantiate(_chestAnimated, _cube.transform);
        _chestAnimated.SetActive(false);
        _chest.SetActive(true);
        ShowBet();
    }

    private IEnumerator ShowFalseLock()
    {
        _lock.SetActive(false);
        _lockFalse.SetActive(true);
        StartCoroutine(UpdateBalance(-betAmount));
        yield return new WaitForSeconds(2.0f);
        _lockFalse.SetActive(false);
        _lock.SetActive(true);
        ShowBet();
    }

    private void ShowBet()
    {
        _bet.SetActive(true);
        randomLock = Random.Range(0, 3);
    }

    private void ShowChooseUI()
    {
        _bet.SetActive(false);
        _choose.SetActive(true);
    }

    private void ChooseLock(string buttonName)
    {
        _choose.SetActive(false);
        CheckLock(buttonName);
    }

    public void CheckLock(string name)
    {
        if (randomLock.ToString() == name)
        {
            StartCoroutine(UnlockChest());
        }
        else
        {
            StartCoroutine(ShowFalseLock());
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
        }

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
            betInput.text = "0";
        }

        PlayerPrefs.SetInt("TotalMoney", balance);
        PlayerPrefs.Save();
    }
}
