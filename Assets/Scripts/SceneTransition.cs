using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] private Image fadeImage; // Ссылка на черный Image для затемнения
    [SerializeField] private float fadeDuration = 1f; // Длительность затемнения

    private void Start()
    {
        // Убедимся, что изображение изначально черное
        fadeImage.color = new Color(0, 0, 0, 1);

        // Запускаем появление из черного в прозрачный при старте сцены
        StartCoroutine(FadeOut());
    }

    public void LoadSceneWithFade(string sceneName)
    {
        StartCoroutine(FadeAndLoadScene(sceneName));
    }

    private IEnumerator FadeAndLoadScene(string sceneName)
    {
        // Затемнение перед загрузкой
        yield return StartCoroutine(FadeIn());

        // Загрузка сцены
        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator FadeIn()
    {
        float timer = 0f;
        Color color = fadeImage.color;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            color.a = Mathf.Lerp(0, 1, timer / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }

        // Убедимся, что изображение стало полностью черным
        color.a = 1;
        fadeImage.color = color;
    }

    private IEnumerator FadeOut()
    {
        float timer = 0f;
        Color color = fadeImage.color;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            color.a = Mathf.Lerp(1, 0, timer / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }

        // Убедимся, что изображение стало полностью прозрачным
        color.a = 0;
        fadeImage.color = color;
    }
}
