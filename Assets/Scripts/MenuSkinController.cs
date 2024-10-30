using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSkinController : MonoBehaviour
{
    [SerializeField] private GameObject[] _glasses;
    private void Start()
    {
        int selectedGlassesIndex = PlayerPrefs.GetInt("SelectedGlasses", -1);
        if (selectedGlassesIndex != -1)
        {
            _glasses[selectedGlassesIndex].SetActive(true);
        }
    }
}
