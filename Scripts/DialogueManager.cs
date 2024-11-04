using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
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
    private int textCounter = 0;
    private bool isChoosing = false;

    private DataLoader dataLoader;
    private SceneController sceneController;

    void Start()
    {
        dataLoader = FindFirstObjectByType<DataLoader>();
        sceneController = FindFirstObjectByType<SceneController>();
        visualNovelData = dataLoader.LoadData("dialogues");
        LoadScene(1);
        HideChoices();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            if (!isChoosing)
            {
                ShowNextDialogueText();
            }
        }
    }

    public void LoadScene(int sceneId)
    {
        currentScene = visualNovelData.scenes.Find(scene => scene.sceneId == sceneId);
        if (currentScene == null)
        {
            Debug.LogError("Сцена с ID " + sceneId + " не найдена!");
            return;
        }

        sceneController.SetBackground(currentScene.background);
        currentDialogueIndex = 0;
        textCounter = 0;
        isChoosing = false;

        ShowNextDialogueText();
    }

    void ShowNextDialogueText()
    {
        if (currentDialogueIndex >= currentScene.dialogues.Count)
        {
            Debug.Log("Диалог завершён для этой сцены.");
            return;
        }

        Dialogue dialogue = currentScene.dialogues[currentDialogueIndex];
        AdjustSpeakerTextPanelAlignment(dialogue);

        speakerText.text = dialogue.speaker;
        Debug.Log("Отображаем текст: " + dialogueText.text);


        if (dialogue.isNarration)
        {
            speakerText.text = "...";
            HideCharacterAvatars();

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
                return;
            }
        }
        else
        {
            speakerText.text = string.IsNullOrEmpty(dialogue.speaker) ? "Неизвестный" : dialogue.speaker;
            DisplayCharacter(dialogue.character, dialogue.place);

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

    void AdjustSpeakerTextPanelAlignment(Dialogue dialogue)
    {
        speakerTextPanelCenter.SetActive(false);
        speakerTextPanelLeft.SetActive(false);
        speakerTextPanelRight.SetActive(false);

        if (dialogue.isNarration)
        {
            speakerTextPanelCenter.SetActive(true);
            speakerText = speakerTextPanelCenter.GetComponentInChildren<TextMeshProUGUI>();
        }
        else if (dialogue.place == 1)
        {
            speakerTextPanelLeft.SetActive(true);
            speakerText = speakerTextPanelLeft.GetComponentInChildren<TextMeshProUGUI>();
        }
        else if (dialogue.place == 2)
        {
            speakerTextPanelRight.SetActive(true);
            speakerText = speakerTextPanelRight.GetComponentInChildren<TextMeshProUGUI>();
        }
    }

    void DisplayCharacter(string character, int place)
    {
        if (place == 1)
        {
            if (!string.IsNullOrEmpty(character))
            {
                Sprite mainLayerSprite = Resources.Load<Sprite>("Characters/" + character);
                if (mainLayerSprite != null)
                {
                    leftAvatar.sprite = mainLayerSprite;
                }
                leftAvatar.gameObject.SetActive(true);
            }
            else
            {
                leftAvatar.gameObject.SetActive(false);
            }
        }
        else if (place == 2)
        {
            if (!string.IsNullOrEmpty(character))
            {
                rightAvatar.sprite = Resources.Load<Sprite>("Characters/" + character);
                rightAvatar.gameObject.SetActive(true);
            }
            else
            {
                rightAvatar.gameObject.SetActive(false);
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
        leftAvatar.gameObject.SetActive(false);
        rightAvatar.gameObject.SetActive(false);
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
