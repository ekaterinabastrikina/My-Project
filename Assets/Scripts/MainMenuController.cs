using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void NewGame()
    {
        // ��������� ��������� ����� ����� ���������� �������
        SceneManager.LoadScene("Scene1");
    }

    public void ContinueGame()
    {
        // ������ ��� ����������� ���� (��������, �������� ����������� ���������)
        Debug.Log("����������� ����");
    }

    public void OpenSettings()
    {
        // ������� ���� ��������
        Debug.Log("�������� ��������");
    }

    public void QuitGame()
    {
        // ����� �� ����
        Application.Quit();
        Debug.Log("����� �� ����");
    }
}
