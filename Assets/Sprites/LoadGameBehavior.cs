using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadGameBehavior : MonoBehaviour
{
    [SerializeField] private GameObject _slotmachine;

    private void Start()
    {
        Screen.orientation = ScreenOrientation.Portrait;
        StartCoroutine(RestartSlotMachine());
    }

    private IEnumerator RestartSlotMachine()
    {
        yield return new WaitForSeconds(2.0f);
        _slotmachine.SetActive(false);
        _slotmachine.SetActive(true);
        yield return new WaitForSeconds(2.0f);
        _slotmachine.SetActive(false);
        _slotmachine.SetActive(true);
        SceneManager.LoadScene("MainMenu");
    }
}
