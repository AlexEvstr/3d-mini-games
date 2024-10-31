using UnityEngine;
using UnityEngine.UI;

public class ShopController : MonoBehaviour
{
    [SerializeField] private Button _homeBtn;
    private SceneTransition _sceneTransition;

    private void Start()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        _sceneTransition = GetComponent<SceneTransition>();
        _homeBtn.onClick.AddListener(BackToHome);
    }

    private void BackToHome()
    {
        _sceneTransition.LoadSceneWithFade("MainMenu");
    }
}