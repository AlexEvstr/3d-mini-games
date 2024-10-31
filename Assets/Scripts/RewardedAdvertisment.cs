using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;
using System.Collections;
using TMPro;

public class RewardedAdvertisment : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [SerializeField] private Button _showAdButton;
    [SerializeField] private TMP_Text _balanceText;
    private MenuAudio _menuAudio;
    private int targetBalance;
    private int currentBalance;
    private float increaseSpeed = 500f;

    #region Android ID

    private readonly string _androidAdsID = "Rewarded_Android";

    #endregion


    #region iOS ID

    private readonly string _iOSAdsID = "Rewarded_iOS";

    #endregion

    private string _adId;

    private void OnEnable()
    {
        _showAdButton.onClick.AddListener(ShowAd);
        _menuAudio = GetComponent<MenuAudio>();
    }

    private void Start()
    {
        currentBalance = PlayerPrefs.GetInt("TotalMoney", 1000);

        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            _adId = _iOSAdsID;
        }
        else if (Application.platform == RuntimePlatform.Android)
        {
            _adId = _androidAdsID;
        }
        else
        {
            _adId = _androidAdsID;
        }

        _showAdButton.gameObject.SetActive(false);
        LoadAd();
    }

    public void LoadAd()
    {
        Advertisement.Load(_adId, this);
    }

    public void ShowAd()
    {
        _showAdButton.gameObject.SetActive(false);
        Advertisement.Show(_adId, this);
    }

    public void OnUnityAdsAdLoaded(string placementId)
    {
        if (placementId.Equals(_adId))
        {
            _showAdButton.gameObject.SetActive(true);
        }
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        
    }

    public void OnUnityAdsShowStart(string placementId) { }

    public void OnUnityAdsShowClick(string placementId) { }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        if (showCompletionState == UnityAdsShowCompletionState.COMPLETED)
        {
            
            IncreaseBalanceBy1000();
            _menuAudio.PlayCashSound();
            _showAdButton.gameObject.SetActive(false);
            LoadAd();
        }
    }

    public void IncreaseBalanceBy1000()
    {
        targetBalance = currentBalance + 1000;
        StartCoroutine(IncreaseBalanceCoroutine());
    }

    private IEnumerator IncreaseBalanceCoroutine()
    {
        while (currentBalance < targetBalance)
        {
            currentBalance = Mathf.Min(currentBalance + Mathf.CeilToInt(increaseSpeed * Time.deltaTime), targetBalance);
            _balanceText.text = currentBalance.ToString();
            yield return null;
        }
        PlayerPrefs.SetInt("TotalMoney", currentBalance);
    }
}