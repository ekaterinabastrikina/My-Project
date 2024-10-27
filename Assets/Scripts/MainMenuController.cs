using UnityEngine;
using UnityEngine.SceneManagement; // Необходимо для загрузки сцен

public class MainMenuController : MonoBehaviour
{
    public void NewGame()
    {
        // Код для начала новой игры, например, загрузка первой сцены
        SceneManager.LoadScene("Scene1"); // Замените "GameScene" на имя вашей первой сцены
    }

    public void ContinueGame()
    {
        // Код для продолжения игры (например, загрузка сохраненного уровня)
        Debug.Log("Continue Game нажата");
    }

    public void OpenSettings()
    {
        // Открытие настроек
        Debug.Log("Settings нажата");
    }

    public void QuitGame()
    {
        // Выход из игры
        Debug.Log("Quit нажата");
        Application.Quit(); // Работает только в собранной версии игры, не в редакторе Unity
    }
}

