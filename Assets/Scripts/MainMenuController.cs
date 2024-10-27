using UnityEngine;
using UnityEngine.SceneManagement; // ���������� ��� �������� ����

public class MainMenuController : MonoBehaviour
{
    public void NewGame()
    {
        // ��� ��� ������ ����� ����, ��������, �������� ������ �����
        SceneManager.LoadScene("Scene1"); // �������� "GameScene" �� ��� ����� ������ �����
    }

    public void ContinueGame()
    {
        // ��� ��� ����������� ���� (��������, �������� ������������ ������)
        Debug.Log("Continue Game ������");
    }

    public void OpenSettings()
    {
        // �������� ��������
        Debug.Log("Settings ������");
    }

    public void QuitGame()
    {
        // ����� �� ����
        Debug.Log("Quit ������");
        Application.Quit(); // �������� ������ � ��������� ������ ����, �� � ��������� Unity
    }
}

