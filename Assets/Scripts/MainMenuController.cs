using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void NewGame()
    {
        // Загрузите начальную сцену вашей визуальной новеллы
        SceneManager.LoadScene("Scene1");
    }

    public void ContinueGame()
    {
        // Логика для продолжения игры (например, загрузка сохранённого состояния)
        Debug.Log("Продолжение игры");
    }

    public void OpenSettings()
    {
        // Открыть меню настроек
        Debug.Log("Открытие настроек");
    }

    public void QuitGame()
    {
        // Выход из игры
        Application.Quit();
        Debug.Log("Выход из игры");
    }
}
