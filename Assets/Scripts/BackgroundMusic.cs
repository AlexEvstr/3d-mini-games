using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    private static BackgroundMusic instance;

    private void Awake()
    {
        // Проверяем, есть ли уже экземпляр
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);  // Сохраняем объект на протяжении всех сцен
        }
        else
        {
            Destroy(gameObject);  // Уничтожаем дубликаты
        }
    }
}
