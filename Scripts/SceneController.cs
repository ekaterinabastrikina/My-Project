using UnityEngine;
using UnityEngine.UI;

public class SceneController : MonoBehaviour
{
    [SerializeField] private Image backgroundImage;

    void Start()
    {
        // Инициализируем backgroundImage, если он не был установлен через инспектор
        if (backgroundImage == null)
        {
            backgroundImage = Object.FindFirstObjectByType<Image>();  // Заменено на FindFirstObjectByType
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
            Debug.LogError("Фон " + backgroundName + " не найден в папке Resources/Backgrounds.");
        }
    }
}
