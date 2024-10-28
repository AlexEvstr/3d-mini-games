using UnityEngine;
using System.Collections;

public class DiceThrow : MonoBehaviour
{
    public Transform playerDice; // Объект кубика игрока
    public Transform computerDice; // Объект кубика компьютера
    private Rigidbody playerRb;
    private Rigidbody computerRb;
    private bool isPlayerThrown = false;
    private bool isComputerThrown = false;
    private int playerResult = 0;
    private int computerResult = 0;

    private Vector3 playerStartPos;
    private Vector3 computerStartPos;
    private Quaternion playerStartRot;
    private Quaternion computerStartRot;

    // Порог силы тряски для подбрасывания кубика
    private float shakeThreshold = 2.0f;

    void Start()
    {
        playerRb = playerDice.GetComponent<Rigidbody>();
        computerRb = computerDice.GetComponent<Rigidbody>();

        // Сохраняем начальные позиции и повороты кубиков
        playerStartPos = playerDice.position;
        computerStartPos = computerDice.position;
        playerStartRot = playerDice.rotation;
        computerStartRot = computerDice.rotation;

        // Устанавливаем нужную частоту обновления для тряски
        Input.gyro.enabled = true;
    }

    void Update()
    {
        // Проверка на тряску устройства для подбрасывания кубика игрока
        if (!isPlayerThrown && Mathf.Abs(Input.gyro.userAcceleration.magnitude) > shakeThreshold)
        {
            ThrowDice(playerRb);
            isPlayerThrown = true;
        }

        // Определяем результат броска игрока
        if (isPlayerThrown && playerRb.IsSleeping() && playerResult == 0)
        {
            playerResult = DetermineTopFace(playerDice);
            Debug.Log("Игрок выбросил: " + playerResult);

            // После определения броска игрока, бросаем кубик компьютера
            ThrowDice(computerRb);
            isComputerThrown = true;
        }

        // Определяем результат броска компьютера
        if (isComputerThrown && computerRb.IsSleeping() && computerResult == 0)
        {
            computerResult = DetermineTopFace(computerDice);
            Debug.Log("Компьютер выбросил: " + computerResult);

            // Определяем победителя
            DetermineWinner();

            // Запускаем корутину для возврата кубиков через 2 секунды
            StartCoroutine(ResetDiceAfterDelay(2f));
        }
    }

    void ThrowDice(Rigidbody rb)
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Добавляем случайность в силу и вращение
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

        return 0; // На случай, если не удастся определить сторону
    }

    void DetermineWinner()
    {
        if (playerResult > computerResult)
        {
            Debug.Log("Игрок выиграл!");
        }
        else if (playerResult < computerResult)
        {
            Debug.Log("Компьютер выиграл!");
        }
        else
        {
            Debug.Log("Ничья!");
        }
    }

    IEnumerator ResetDiceAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Возвращаем кубики на их начальные позиции и повороты
        playerDice.position = playerStartPos;
        playerDice.rotation = playerStartRot;
        computerDice.position = computerStartPos;
        computerDice.rotation = computerStartRot;

        // Сбрасываем переменные для нового броска
        isPlayerThrown = false;
        isComputerThrown = false;
        playerResult = 0;
        computerResult = 0;
    }
}
