using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    [SerializeField] private MenuController _menuController;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("SlotsTrigger"))
        {
            _menuController.ShowPlaySlots();
        }
        else if (other.gameObject.CompareTag("WheelTrigger"))
        {
            _menuController.ShowPlayWheel();
        }

        else if (other.gameObject.CompareTag("DiceTrigger"))
        {
            _menuController.ShowPlayDice();
        }
        else if (other.gameObject.CompareTag("ChestTrigger"))
        {
            _menuController.ShowPlayChest();
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