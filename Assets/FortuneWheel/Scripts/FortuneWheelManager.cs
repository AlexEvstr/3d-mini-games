using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Linq;
using UnityEngine.Events;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class FortuneWheelManager : MonoBehaviour
{
	[Header("Game Objects for some elements")]
	public Button PaidTurnButton;
	public Button FreeTurnButton;
	public Button closeBtn;
	public GameObject Circle;
	public Text DeltaCoinsText;
	public Text CurrentCoinsText;
	public GameObject NextTurnTimerWrapper;
	public Text NextFreeTurnTimerText;
	public GameObject WheelSound;
	public GameAudio _gameAudio;

	[Header("How much currency one paid turn costs")]
	public int TurnCost = 300;

	private bool _isStarted;

	[Header("Params for each sector")]
	public FortuneWheelSector[] Sectors;
	private SceneTransition sceneTransition;

	private float _finalAngle;
	private float _startAngle;
	private float _currentLerpRotationTime;
	private int _currentCoinsAmount;
	private int _previousCoinsAmount;

	[Header("Time Between Two Free Turns")]
	public int TimerMaxHours;
	[RangeAttribute(0, 59)]
	public int TimerMaxMinutes;
	[RangeAttribute(0, 59)]
	public int TimerMaxSeconds = 10;

	private int _timerRemainingHours;
	private int _timerRemainingMinutes;
	private int _timerRemainingSeconds;

	private DateTime _nextFreeTurnTime;

	private const string LAST_FREE_TURN_TIME_NAME = "LastFreeTurnTimeTicks";

	[Header("Can players turn the wheel for currency?")]
	public bool IsPaidTurnEnabled = true;

	[Header("Can players turn the wheel for FREE from time to time?")]
	public bool IsFreeTurnEnabled = true;

	private bool _isFreeTurnAvailable;

	private FortuneWheelSector _finalSector;

	private void Awake ()
	{
		sceneTransition = GetComponent<SceneTransition>();
		Screen.orientation = ScreenOrientation.Portrait;
		_previousCoinsAmount = _currentCoinsAmount;
	
		_currentCoinsAmount = PlayerPrefs.GetInt("TotalMoney", 1000);
		CurrentCoinsText.text = _currentCoinsAmount.ToString ();

		foreach (var sector in Sectors)
		{
			if (sector.ValueTextObject != null)
				sector.ValueTextObject.GetComponent<Text>().text = sector.RewardValue.ToString();
		}

		if (IsFreeTurnEnabled) {
			SetNextFreeTime ();

			if (!PlayerPrefs.HasKey(LAST_FREE_TURN_TIME_NAME)) {
				PlayerPrefs.SetString (LAST_FREE_TURN_TIME_NAME, DateTime.Now.Ticks.ToString());
			}
		} else {
			NextTurnTimerWrapper.gameObject.SetActive (false);
		}

		closeBtn.onClick.AddListener(CloseWheelBtn);
	}

	private void CloseWheelBtn()
    {
		sceneTransition.LoadSceneWithFade("MainMenu");

	}

	private void TurnWheelForFree() { TurnWheel (true);	}
	private void TurnWheelForCoins() { TurnWheel (false); }

	private void TurnWheel (bool isFree)
	{
		_currentLerpRotationTime = 0f;

		int[] sectorsAngles = new int[Sectors.Length];

		for (int i = 1; i <= Sectors.Length; i++)
		{
			sectorsAngles[i - 1] =  360 / Sectors.Length * i;
		}

		double rndNumber = UnityEngine.Random.Range (1, Sectors.Sum(sector => sector.Probability));

		int cumulativeProbability = 0;
		int randomFinalAngle = sectorsAngles [0];
		_finalSector = Sectors[0];

		for (int i = 0; i < Sectors.Length; i++) {
			cumulativeProbability += Sectors[i].Probability;

			if (rndNumber <= cumulativeProbability) {
				randomFinalAngle = sectorsAngles [i];
				_finalSector = Sectors[i];
				break;
			}
		}

		int fullTurnovers = 5;

		_finalAngle = fullTurnovers * 360 + randomFinalAngle;

		_isStarted = true;
		WheelSound.SetActive(true);

		_previousCoinsAmount = _currentCoinsAmount;

		if (!isFree) {
			_currentCoinsAmount -= TurnCost;

			DeltaCoinsText.text = String.Format ("-{0}", TurnCost);
			DeltaCoinsText.gameObject.SetActive (true);

			StartCoroutine (HideCoinsDelta ());
			StartCoroutine (UpdateCoinsAmount ());
		} else {
			
			PlayerPrefs.SetString (LAST_FREE_TURN_TIME_NAME, DateTime.Now.Ticks.ToString());

			SetNextFreeTime ();
		}
	}

	public void TurnWheelButtonClick ()
	{
		if (_isFreeTurnAvailable) {
			TurnWheelForFree ();
		} else {
			if (IsPaidTurnEnabled) {
				if (_currentCoinsAmount >= TurnCost) {
					TurnWheelForCoins ();
				}
			}
		}
	}

	public void SetNextFreeTime() {
		_timerRemainingHours = TimerMaxHours;
		_timerRemainingMinutes = TimerMaxMinutes;
		_timerRemainingSeconds = TimerMaxSeconds;

		_nextFreeTurnTime = new DateTime(Convert.ToInt64(PlayerPrefs.GetString (LAST_FREE_TURN_TIME_NAME, DateTime.Now.Ticks.ToString())))
			.AddHours(TimerMaxHours)
			.AddMinutes(TimerMaxMinutes)
			.AddSeconds(TimerMaxSeconds);

		_isFreeTurnAvailable = false;
	}

	private void ShowTurnButtons ()
	{
		if (_isFreeTurnAvailable)
		{			
			ShowFreeTurnButton ();
			EnableFreeTurnButton ();

		} else {

			if (!IsPaidTurnEnabled)
			{
				ShowFreeTurnButton ();
				DisableFreeTurnButton ();

			} else {
				ShowPaidTurnButton ();

				if (_isStarted || _currentCoinsAmount < TurnCost)
					DisablePaidTurnButton ();
				else
					EnablePaidTurnButton ();
			}
		}
	}

	private void Update ()
	{
		ShowTurnButtons ();

		if (IsFreeTurnEnabled)
			UpdateFreeTurnTimer ();

		if (!_isStarted)
			return;

		float maxLerpRotationTime = 4f;

		_currentLerpRotationTime += Time.deltaTime;

		if (_currentLerpRotationTime > maxLerpRotationTime || Circle.transform.eulerAngles.z == _finalAngle) {
			_currentLerpRotationTime = maxLerpRotationTime;
			_isStarted = false;
			WheelSound.SetActive(false);
			_startAngle = _finalAngle % 360;

			_finalSector.RewardCallback.Invoke();
			StartCoroutine (HideCoinsDelta ());
		} else {

			float t = _currentLerpRotationTime / maxLerpRotationTime;

			t = t * t * t * (t * (6f * t - 15f) + 10f);

			float angle = Mathf.Lerp (_startAngle, _finalAngle, t);
			Circle.transform.eulerAngles = new Vector3 (0, 0, angle);	
		}
	}

	/// <summary>
	/// Sample callback for giving reward (in editor each sector have Reward Callback field pointed to this method)
	/// </summary>
	/// <param name="awardCoins">Coins for user</param>
	public void RewardCoins (int awardCoins)
	{
		_currentCoinsAmount += awardCoins;

		if (awardCoins > 0) DeltaCoinsText.text = String.Format("+{0}", awardCoins);
		else DeltaCoinsText.text = String.Format("-{0}", awardCoins);

		DeltaCoinsText.gameObject.SetActive (true);
		StartCoroutine (UpdateCoinsAmount ());
		_gameAudio.PlayCashSound();
	}


	private IEnumerator HideCoinsDelta ()
	{
		yield return new WaitForSeconds (1f);
		DeltaCoinsText.gameObject.SetActive (false);
	}

	private IEnumerator UpdateCoinsAmount ()
	{
		const float seconds = 0.5f;
		float elapsedTime = 0;

		while (elapsedTime < seconds) {
			CurrentCoinsText.text = Mathf.Floor(Mathf.Lerp (_previousCoinsAmount, _currentCoinsAmount, (elapsedTime / seconds))).ToString ();
			elapsedTime += Time.deltaTime;

			yield return new WaitForEndOfFrame ();
		}
		_previousCoinsAmount = _currentCoinsAmount;

		CurrentCoinsText.text = _currentCoinsAmount.ToString ();

		PlayerPrefs.SetInt("TotalMoney", _currentCoinsAmount);
	}

	private void UpdateFreeTurnTimer () 
	{
		if (_isFreeTurnAvailable)
			return;

		_timerRemainingHours = (int)(_nextFreeTurnTime - DateTime.Now).Hours;
		_timerRemainingMinutes = (int)(_nextFreeTurnTime - DateTime.Now).Minutes;
		_timerRemainingSeconds = (int)(_nextFreeTurnTime - DateTime.Now).Seconds;

		if (_timerRemainingHours <= 0 && _timerRemainingMinutes <= 0 && _timerRemainingSeconds <= 0) {
			NextFreeTurnTimerText.text = "Ready!";
			_isFreeTurnAvailable = true;
		} else {
			NextFreeTurnTimerText.text = String.Format ("{0:00}:{1:00}:{2:00}", _timerRemainingHours, _timerRemainingMinutes, _timerRemainingSeconds);
			_isFreeTurnAvailable = false;
		}
	}

	private void EnableButton (Button button)
	{
		button.interactable = true;
		button.GetComponent<Image> ().color = new Color(255, 255, 255, 1f);
	}

	private void DisableButton (Button button)
	{
		button.interactable = false;
		button.GetComponent<Image> ().color = new Color(255, 255, 255, 0.5f);
	}

	private void EnableFreeTurnButton () { EnableButton (FreeTurnButton); }
	private void DisableFreeTurnButton () {	DisableButton (FreeTurnButton);	}
	private void EnablePaidTurnButton () { EnableButton (PaidTurnButton); }
	private void DisablePaidTurnButton () { DisableButton (PaidTurnButton); }

	private void ShowFreeTurnButton ()
	{
		FreeTurnButton.gameObject.SetActive(true); 
		PaidTurnButton.gameObject.SetActive(false);
	}

	private void ShowPaidTurnButton ()
	{
		PaidTurnButton.gameObject.SetActive(true); 
		FreeTurnButton.gameObject.SetActive(false);
	}

	public void ResetTimer() 
	{
		PlayerPrefs.DeleteKey (LAST_FREE_TURN_TIME_NAME);
	}
}

[Serializable]
public class FortuneWheelSector : System.Object
{
	[Tooltip("Text object where value will be placed (not required)")]
	public GameObject ValueTextObject;

	[Tooltip("Value of reward")]
	public int RewardValue = 100;

	[Tooltip("Chance that this sector will be randomly selected")]
	[RangeAttribute(0, 100)]
	public int Probability = 100;

	[Tooltip("Method that will be invoked if this sector will be randomly selected")]
	public UnityEvent RewardCallback;
}

#if UNITY_EDITOR
[CustomEditor(typeof(FortuneWheelManager))]
public class FortuneWheelManagerEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		FortuneWheelManager myScript = (FortuneWheelManager)target;
		if(GUILayout.Button("Reset Timer"))
		{
			myScript.ResetTimer();
		}
	}
}
#endif