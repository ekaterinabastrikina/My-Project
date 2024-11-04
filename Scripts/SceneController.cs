using UnityEngine;
using UnityEngine.UI;

public class SceneController : MonoBehaviour
{
    [SerializeField] private Image backgroundImage;

    void Start()
    {
        // �������������� backgroundImage, ���� �� �� ��� ���������� ����� ���������
        if (backgroundImage == null)
        {
            backgroundImage = Object.FindFirstObjectByType<Image>();  // �������� �� FindFirstObjectByType
        }
    }

    public void SetBackground(string backgroundName)
    {
        Sprite bgSprite = Resources.Load<Sprite>("Backgrounds/" + backgroundName);
        if (bgSprite != null)
        {
            backgroundImage.sprite = bgSprite;
        }
        else
        {
            Debug.LogError("��� " + backgroundName + " �� ������ � ����� Resources/Backgrounds.");
        }
    }
}
