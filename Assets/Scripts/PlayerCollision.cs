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