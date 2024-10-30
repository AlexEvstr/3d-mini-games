using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [SerializeField] private Button _playButton;
    [SerializeField] private GameObject _slots;
    [SerializeField] private GameObject _slots_anim;
    [SerializeField] private GameObject _wheel;
    [SerializeField] private GameObject _wheel_anim;
    [SerializeField] private GameObject _dice;
    [SerializeField] private GameObject _dice_anim;
    [SerializeField] private GameObject _chest;
    [SerializeField] private GameObject _chest_anim;

    private void Start()
    {
        _playButton.gameObject.SetActive(false);
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
}
