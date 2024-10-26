using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;
using static VisualNovelController;

public class VisualNovelController : MonoBehaviour
{
    [System.Serializable]
    public class Choice
    {
        public string text;
        public int nextSceneId;
    }

    [System.Serializable]
    public class Dialogue
    {
        public string speaker;
        public string character;
        public int place;
        public bool isNarration;
        public List<string> texts;  // Список текстов вместо одного текста
        public List<Choice> choices;
    }

    [System.Serializable]
    public class SceneData
    {
        public int sceneId;
        public string background;
        public List<Dialogue> dialogues;
    }

    [System.Serializable]
    public class VisualNovelData
    {
        public List<SceneData> scenes;
    }

    [SerializeField] private Image backgroundImage;
    [SerializeField] public TextMeshProUGUI dialogueText;
    [SerializeField] public TextMeshProUGUI speakerText;
    [SerializeField] private Button[] optionButtons;
    [SerializeField] private Image leftAvatar;
    [SerializeField] private Image rightAvatar;

    private VisualNovelData visualNovelData;
    private SceneData currentScene;
    private int currentDialogueIndex = 0;
    private int textCounter = 0; // Счётчик для текстов в массиве `texts`

    private bool isChoosing = false;

    void Start()
    {
        HideChoices();
        HideCharacterAvatars();
        leftAvatar.gameObject.SetActive(false); // Скрыть аватар слева в начале
        rightAvatar.gameObject.SetActive(false); // Скрыть аватар справа в начале
        LoadDataFromFile("dialogues");
        LoadScene(1);
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            if (!isChoosing)
            {
                ShowNextDialogueText();  // Показываем следующий текст из массива
            }
        }
    }

    void LoadDataFromFile(string fileName)
    {
        TextAsset jsonFile = Resources.Load<TextAsset>(fileName);
        if (jsonFile != null)
        {
            visualNovelData = JsonUtility.FromJson<VisualNovelData>(jsonFile.text);
            Debug.Log("JSON загружен успешно.");
        }
        else
        {
            Debug.LogError("Файл JSON не найден.");
        }
    }

    void LoadScene(int sceneId)
    {
        currentScene = visualNovelData.scenes.Find(scene => scene.sceneId == sceneId);
        if (currentScene == null)
        {
            Debug.LogError("Сцена с ID " + sceneId + " не найдена!");
            return;
        }

        SetBackground(currentScene.background);
        currentDialogueIndex = 0;
        textCounter = 0;
        isChoosing = false;

        ShowNextDialogueText();
    }

    void SetBackground(string backgroundName)
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

    void ShowNextDialogueText()
    {
        // Проверяем, есть ли еще диалоги в текущей сцене
        if (currentDialogueIndex >= currentScene.dialogues.Count)
        {
            Debug.Log("Диалог завершён для этой сцены.");
            return; // Завершить, если нет диалогов
        }

        Dialogue dialogue = currentScene.dialogues[currentDialogueIndex];

        // Проверяем, является ли диалог авторской речью
        if (dialogue.isNarration)
        {
            // Устанавливаем текст диалога без спикера и аватара
            speakerText.text = ""; // Очистить имя спикера
            HideCharacterAvatars(); // Скрыть аватары

            // Проверка на наличие текстов
            if (textCounter < dialogue.texts.Count)
            {
                dialogueText.text = dialogue.texts[textCounter]; // Устанавливаем текст из массива
                textCounter++; // Переход к следующему тексту
            }
            else // Если текстов больше нет
            {
                textCounter = 0; // Сброс счетчика
                currentDialogueIndex++; // Переход к следующему диалогу
                ShowNextDialogueText(); // Показать следующий диалог
                return; // Выход из метода
            }
        }
        else // Обработка диалога с персонажем
        {
            speakerText.text = string.IsNullOrEmpty(dialogue.speaker) ? "Неизвестный" : dialogue.speaker;
            DisplayCharacter(dialogue.character, dialogue.place); // Устанавливаем аватар

            // Проверка на наличие текстов
            if (textCounter < dialogue.texts.Count)
            {
                dialogueText.text = dialogue.texts[textCounter]; // Устанавливаем текст из массива
                textCounter++; // Переход к следующему тексту
            }
            else // Если текстов больше нет
            {
                textCounter = 0; // Сброс счетчика

                // Проверка на наличие выбора, если текстов нет
                if (dialogue.choices != null && dialogue.choices.Count > 0)
                {
                    ShowChoices(dialogue.choices); // Отображаем выборы
                    dialogueText.gameObject.SetActive(false); // Скрываем текст диалога
                    isChoosing = true; // Включаем режим выбора
                }
                else
                {
                    // Если нет ни текстов, ни выборов, переходим к следующему диалогу
                    currentDialogueIndex++;
                    ShowNextDialogueText(); // Показать следующий диалог
                    return; // Выход из метода
                }
            }
        }

        // Проверяем наличие выбора
        if (currentDialogueIndex < currentScene.dialogues.Count)
        {
            Dialogue nextDialogue = currentScene.dialogues[currentDialogueIndex];

            if (nextDialogue.choices != null && nextDialogue.choices.Count > 0)
            {
                ShowChoices(nextDialogue.choices);
                dialogueText.gameObject.SetActive(false); // Скрываем текст диалога
            }
            else
            {
                dialogueText.gameObject.SetActive(true); // Показываем текст диалога
                HideChoices(); // Скрываем кнопки выбора
            }
        }
        else
        {
            Debug.Log("Диалог завершён для этой сцены.");
        }
    }



    void DisplayCharacter(string characterName, int place)
    {
        if (place == 1)
        {
            if (!string.IsNullOrEmpty(characterName))
            {
                leftAvatar.sprite = Resources.Load<Sprite>("Characters/" + characterName);
                leftAvatar.gameObject.SetActive(true);
            }
            else
            {
                leftAvatar.gameObject.SetActive(false); // Скрыть, если персонаж не указан
            }
        }
        else if (place == 2)
        {
            if (!string.IsNullOrEmpty(characterName))
            {
                rightAvatar.sprite = Resources.Load<Sprite>("Characters/" + characterName);
                rightAvatar.gameObject.SetActive(true);
            }
            else
            {
                rightAvatar.gameObject.SetActive(false); // Скрыть, если персонаж не указан
            }
        }
    }


    void ShowChoices(List<Choice> choices)
    {
        dialogueText.gameObject.SetActive(false);
        isChoosing = true;

        for (int i = 0; i < optionButtons.Length; i++)
        {
            if (i < choices.Count)
            {
                optionButtons[i].gameObject.SetActive(true);
                optionButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = choices[i].text;
                int nextSceneId = choices[i].nextSceneId;
                optionButtons[i].onClick.RemoveAllListeners();
                optionButtons[i].onClick.AddListener(() => OnChoiceSelected(nextSceneId));
            }
            else
            {
                optionButtons[i].gameObject.SetActive(false);
            }
        }
    }

    void HideCharacterAvatars()
    {
        leftAvatar.gameObject.SetActive(false); // Скрыть левый аватар
        rightAvatar.gameObject.SetActive(false); // Скрыть правый аватар
    }

    void HideChoices()
    {
        dialogueText.gameObject.SetActive(true);
        foreach (Button button in optionButtons)
        {
            button.gameObject.SetActive(false);
        }
    }

    void OnChoiceSelected(int nextSceneId)
    {
        LoadScene(nextSceneId);
    }
}