using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class DiceThrow : MonoBehaviour
{
    [SerializeField] private Transform playerDice;
    [SerializeField] private Transform computerDice;
    private Rigidbody playerRb;
    private Rigidbody computerRb;

    [SerializeField] private GameObject shakeToThrowUI;
    [SerializeField] private TMP_Text balanceText;
    [SerializeField] private TMP_InputField betInput;
    [SerializeField] private Button betIncreaseButton;
    [SerializeField] private Button betDecreaseButton;
    [SerializeField] private Button closeButton;
    private SceneTransition sceneTransition;

    private bool isPlayerThrown = false;
    private bool isComputerThrown = false;
    private int playerResult = 0;
    private int computerResult = 0;

    private Vector3 playerStartPos;
    private Vector3 computerStartPos;
    private Quaternion playerStartRot;
    private Quaternion computerStartRot;

    private float shakeThreshold = 2.0f;

    private int balance;
    private int betAmount;
    private float balanceUpdateSpeed = 1000f;

    void Start()
    {
        sceneTransition = GetComponent<SceneTransition>();
        Screen.orientation = ScreenOrientation.LandscapeLeft;

        playerRb = playerDice.GetComponent<Rigidbody>();
        computerRb = computerDice.GetComponent<Rigidbody>();

        playerStartPos = playerDice.position;
        computerStartPos = computerDice.position;
        playerStartRot = playerDice.rotation;
        computerStartRot = computerDice.rotation;

        Input.gyro.enabled = true;

        balance = PlayerPrefs.GetInt("TotalMoney", 1000);
        UpdateBalanceText();
        if (balance > 0) betAmount = 100;

        betInput.text = betAmount.ToString();
        betInput.onEndEdit.AddListener(OnEndEditBetInput); // Срабатывает при завершении ввода

        betIncreaseButton.onClick.AddListener(() => ChangeBetAmount(10));
        betDecreaseButton.onClick.AddListener(() => ChangeBetAmount(-10));

        closeButton.onClick.AddListener(CloseBtn);
    }

    private void CloseBtn()
    {
        sceneTransition.LoadSceneWithFade("MainMenu");
    }

    void Update()
    {
        // Проверка на тряску устройства для подбрасывания кубика игрока
        if (!isPlayerThrown && Mathf.Abs(Input.gyro.userAcceleration.magnitude) > shakeThreshold && betAmount > 0)
        {
            ThrowDice(playerRb);
            isPlayerThrown = true;
            shakeToThrowUI.SetActive(false);
        }

        if (isPlayerThrown && playerRb.IsSleeping() && playerResult == 0)
        {
            playerResult = DetermineTopFace(playerDice);
            Debug.Log("Игрок выбросил: " + playerResult);

            ThrowDice(computerRb);
            isComputerThrown = true;
        }

        if (isComputerThrown && computerRb.IsSleeping() && computerResult == 0)
        {
            computerResult = DetermineTopFace(computerDice);
            Debug.Log("Компьютер выбросил: " + computerResult);

            DetermineWinner();
            StartCoroutine(ResetDiceAfterDelay(2f));
        }
    }

    void ThrowDice(Rigidbody rb)
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        Vector3 randomDirection = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(0.5f, 1.5f),
            Random.Range(-1f, 1f)
        ).normalized;

        rb.AddForce(randomDirection * Random.Range(4f, 8f), ForceMode.Impulse);
        rb.AddTorque(Random.insideUnitSphere * Random.Range(8f, 15f), ForceMode.Impulse);
    }

    int DetermineTopFace(Transform dice)
    {
        Vector3 upDirection = dice.up;

        if (Vector3.Dot(upDirection, Vector3.up) > 0.9f)
            return 2;
        else if (Vector3.Dot(-dice.forward, Vector3.up) > 0.9f)
            return 6;
        else if (Vector3.Dot(dice.right, Vector3.up) > 0.9f)
            return 4;
        else if (Vector3.Dot(-dice.right, Vector3.up) > 0.9f)
            return 3;
        else if (Vector3.Dot(dice.forward, Vector3.up) > 0.9f)
            return 1;
        else if (Vector3.Dot(-dice.up, Vector3.up) > 0.9f)
            return 5;

        return 0;
    }

    void DetermineWinner()
    {
        if (playerResult > computerResult)
        {
            Debug.Log("Игрок выиграл!");
            StartCoroutine(UpdateBalance(betAmount));
        }
        else if (playerResult < computerResult)
        {
            Debug.Log("Компьютер выиграл!");
            StartCoroutine(UpdateBalance(-betAmount));
            
        }
        else
        {
            Debug.Log("Ничья!");
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

    IEnumerator ResetDiceAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        playerDice.position = playerStartPos;
        playerDice.rotation = playerStartRot;
        computerDice.position = computerStartPos;
        computerDice.rotation = computerStartRot;

        isPlayerThrown = false;
        isComputerThrown = false;
        playerResult = 0;
        computerResult = 0;

        shakeToThrowUI.SetActive(true);
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

    void UpdateBalanceText()
    {
        balanceText.text = balance.ToString();
    }
}
