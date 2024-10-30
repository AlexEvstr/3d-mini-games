using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    [SerializeField] private MenuController _menuController;
    [SerializeField] private JoystickPlayerController _joystickController;
    [SerializeField] private float groundCheckRadius = 0.2f; // Радиус проверки касания земли
    [SerializeField] private Transform groundCheckPoint; // Точка для проверки касания земли

    private bool isGrounded;

    private void Update()
    {
        CheckGroundStatus();
    }

    private void CheckGroundStatus()
    {
        // Выполняем CheckSphere вокруг groundCheckPoint
        Collider[] colliders = Physics.OverlapSphere(groundCheckPoint.position, groundCheckRadius);

        bool grounded = false;

        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Floor"))
            {
                grounded = true;
                break;
            }
        }

        isGrounded = grounded;
        _joystickController.SetGroundedState(isGrounded);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("SlotsTrigger"))
        {
            _menuController.ShowPlaySlots();
            PlayerPrefs.SetString("SceneNameToPlay", "SlotsGame");
        }
        else if (other.gameObject.CompareTag("WheelTrigger"))
        {
            _menuController.ShowPlayWheel();
            PlayerPrefs.SetString("SceneNameToPlay", "FortuneWheelGame");
        }
        else if (other.gameObject.CompareTag("DiceTrigger"))
        {
            _menuController.ShowPlayDice();
            PlayerPrefs.SetString("SceneNameToPlay", "DiceGame");
        }
        else if (other.gameObject.CompareTag("ChestTrigger"))
        {
            _menuController.ShowPlayChest();
            PlayerPrefs.SetString("SceneNameToPlay", "ChestGame");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("SlotsTrigger"))
        {
            _menuController.HidePlaySlots();
        }
        else if (other.gameObject.CompareTag("WheelTrigger"))
        {
            _menuController.HidePlayWheel();
        }
        else if (other.gameObject.CompareTag("ChestTrigger"))
        {
            _menuController.HidePlayChest();
        }
        else if (other.gameObject.CompareTag("DiceTrigger"))
        {
            _menuController.HidePlayDice();
        }
    }
}
