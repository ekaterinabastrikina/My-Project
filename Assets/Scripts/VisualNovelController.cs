using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;


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
        // public string expression;
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
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI speakerText;
    public GameObject dialogueTextPanel;
    [SerializeField] private Button[] optionButtons;
    [SerializeField] private Image leftAvatar;
    [SerializeField] private Image rightAvatar;

    [SerializeField] private GameObject speakerTextPanelLeft;
    [SerializeField] private GameObject speakerTextPanelCenter;
    [SerializeField] private GameObject speakerTextPanelRight;





    private VisualNovelData visualNovelData;
    private SceneData currentScene;
    private int currentDialogueIndex = 0;
    private int textCounter = 0; // Счётчик для текстов в массиве texts

    private bool isChoosing = false;

    void Start()
    {
        HideChoices();
        HideCharacterAvatars();
        leftAvatar.gameObject.SetActive(false);
        rightAvatar.gameObject.SetActive(false);
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



    void AdjustSpeakerTextPanelAlignment(Dialogue dialogue)
    {
        // Отключаем все панели перед выбором
        speakerTextPanelCenter.SetActive(false);
        speakerTextPanelLeft.SetActive(false);
        speakerTextPanelRight.SetActive(false);

        if (dialogue.isNarration)
        {
            // Включаем только центральную панель
            speakerTextPanelCenter.SetActive(true);
            speakerText = speakerTextPanelCenter.GetComponentInChildren<TextMeshProUGUI>();
        }
        else if (dialogue.place == 1)
        {
            // Включаем только левую панель
            speakerTextPanelLeft.SetActive(true);
            speakerText = speakerTextPanelLeft.GetComponentInChildren<TextMeshProUGUI>();
        }
        else if (dialogue.place == 2)
        {
            // Включаем только правую панель
            speakerTextPanelRight.SetActive(true);
            speakerText = speakerTextPanelRight.GetComponentInChildren<TextMeshProUGUI>();
        }
    }


    //[SerializeField] private CharacterExpressionController сharacterExpressionController;


    void ShowNextDialogueText()
    {
        // Проверяем, есть ли еще диалоги в текущей сцене
        if (currentDialogueIndex >= currentScene.dialogues.Count)
        {
            Debug.Log("Диалог завершён для этой сцены.");
            return; // Завершить, если нет диалогов
        }

        Dialogue dialogue = currentScene.dialogues[currentDialogueIndex];

        AdjustSpeakerTextPanelAlignment(dialogue); // Активируем нужную панель перед показом текста

        // Устанавливаем имя спикера
        speakerText.text = dialogue.speaker;

        Debug.Log("Отображаем текст: " + dialogueText.text);

        // Проверяем, является ли диалог авторской речью (рассказчиком)
        if (dialogue.isNarration)
        {

            speakerText.text = "...";
            HideCharacterAvatars();

            // Проверка на наличие текстов
            if (textCounter < dialogue.texts.Count)
            {
                dialogueText.text = dialogue.texts[textCounter];
                textCounter++;
            }
            else
            {
                textCounter = 0;
                currentDialogueIndex++;
                ShowNextDialogueText();
                return; // Выход из метода
            }
        }
        else
        {
            // Устанавливаем имя спикера, если оно есть, иначе "Неизвестный"
            speakerText.text = string.IsNullOrEmpty(dialogue.speaker) ? "Неизвестный" : dialogue.speaker;


            DisplayCharacter(dialogue.character, dialogue.place);

            // Проверяем наличие эмоции в JSON и изменяем эмоцию, если она указана
            /*if (!string.IsNullOrEmpty(dialogue.expression))
            {
                сharacterExpressionController.SetExpression(dialogue.expression);
            }*/

            // Проверка на наличие текстов
            if (textCounter < dialogue.texts.Count)
            {
                dialogueText.text = dialogue.texts[textCounter];
                textCounter++;
            }
            else
            {
                textCounter = 0;


                if (dialogue.choices != null && dialogue.choices.Count > 0)
                {
                    ShowChoices(dialogue.choices);
                    dialogueText.gameObject.SetActive(false);
                    isChoosing = true;
                }
                else
                {

                    currentDialogueIndex++;
                    ShowNextDialogueText();
                    return;
                }
            }
        }

        // Проверяем наличие выбора в следующем диалоге
        if (currentDialogueIndex < currentScene.dialogues.Count)
        {
            Dialogue nextDialogue = currentScene.dialogues[currentDialogueIndex];

            if (nextDialogue.choices != null && nextDialogue.choices.Count > 0)
            {
                ShowChoices(nextDialogue.choices);
                dialogueText.gameObject.SetActive(false); // Скрываем текст диалога
                dialogueTextPanel.gameObject.SetActive(false);
            }
            else
            {
                dialogueText.gameObject.SetActive(true); // Показываем текст диалога
                dialogueTextPanel.gameObject.SetActive(true);
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