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
        public List<string> texts;  // ������ ������� ������ ������ ������
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
    private int textCounter = 0; // ������� ��� ������� � ������� `texts`

    private bool isChoosing = false;

    void Start()
    {
        HideChoices();
        HideCharacterAvatars();
        leftAvatar.gameObject.SetActive(false); // ������ ������ ����� � ������
        rightAvatar.gameObject.SetActive(false); // ������ ������ ������ � ������
        LoadDataFromFile("dialogues");
        LoadScene(1);
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            if (!isChoosing)
            {
                ShowNextDialogueText();  // ���������� ��������� ����� �� �������
            }
        }
    }

    void LoadDataFromFile(string fileName)
    {
        TextAsset jsonFile = Resources.Load<TextAsset>(fileName);
        if (jsonFile != null)
        {
            visualNovelData = JsonUtility.FromJson<VisualNovelData>(jsonFile.text);
            Debug.Log("JSON �������� �������.");
        }
        else
        {
            Debug.LogError("���� JSON �� ������.");
        }
    }

    void LoadScene(int sceneId)
    {
        currentScene = visualNovelData.scenes.Find(scene => scene.sceneId == sceneId);
        if (currentScene == null)
        {
            Debug.LogError("����� � ID " + sceneId + " �� �������!");
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
            Debug.LogError("��� " + backgroundName + " �� ������ � ����� Resources/Backgrounds.");
        }
    }

    void ShowNextDialogueText()
    {
        // ���������, ���� �� ��� ������� � ������� �����
        if (currentDialogueIndex >= currentScene.dialogues.Count)
        {
            Debug.Log("������ �������� ��� ���� �����.");
            return; // ���������, ���� ��� ��������
        }

        Dialogue dialogue = currentScene.dialogues[currentDialogueIndex];

        // ���������, �������� �� ������ ��������� �����
        if (dialogue.isNarration)
        {
            // ������������� ����� ������� ��� ������� � �������
            speakerText.text = ""; // �������� ��� �������
            HideCharacterAvatars(); // ������ �������

            // �������� �� ������� �������
            if (textCounter < dialogue.texts.Count)
            {
                dialogueText.text = dialogue.texts[textCounter]; // ������������� ����� �� �������
                textCounter++; // ������� � ���������� ������
            }
            else // ���� ������� ������ ���
            {
                textCounter = 0; // ����� ��������
                currentDialogueIndex++; // ������� � ���������� �������
                ShowNextDialogueText(); // �������� ��������� ������
                return; // ����� �� ������
            }
        }
        else // ��������� ������� � ����������
        {
            speakerText.text = string.IsNullOrEmpty(dialogue.speaker) ? "�����������" : dialogue.speaker;
            DisplayCharacter(dialogue.character, dialogue.place); // ������������� ������

            // �������� �� ������� �������
            if (textCounter < dialogue.texts.Count)
            {
                dialogueText.text = dialogue.texts[textCounter]; // ������������� ����� �� �������
                textCounter++; // ������� � ���������� ������
            }
            else // ���� ������� ������ ���
            {
                textCounter = 0; // ����� ��������

                // �������� �� ������� ������, ���� ������� ���
                if (dialogue.choices != null && dialogue.choices.Count > 0)
                {
                    ShowChoices(dialogue.choices); // ���������� ������
                    dialogueText.gameObject.SetActive(false); // �������� ����� �������
                    isChoosing = true; // �������� ����� ������
                }
                else
                {
                    // ���� ��� �� �������, �� �������, ��������� � ���������� �������
                    currentDialogueIndex++;
                    ShowNextDialogueText(); // �������� ��������� ������
                    return; // ����� �� ������
                }
            }
        }

        // ��������� ������� ������
        if (currentDialogueIndex < currentScene.dialogues.Count)
        {
            Dialogue nextDialogue = currentScene.dialogues[currentDialogueIndex];

            if (nextDialogue.choices != null && nextDialogue.choices.Count > 0)
            {
                ShowChoices(nextDialogue.choices);
                dialogueText.gameObject.SetActive(false); // �������� ����� �������
            }
            else
            {
                dialogueText.gameObject.SetActive(true); // ���������� ����� �������
                HideChoices(); // �������� ������ ������
            }
        }
        else
        {
            Debug.Log("������ �������� ��� ���� �����.");
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
                leftAvatar.gameObject.SetActive(false); // ������, ���� �������� �� ������
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
                rightAvatar.gameObject.SetActive(false); // ������, ���� �������� �� ������
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
        leftAvatar.gameObject.SetActive(false); // ������ ����� ������
        rightAvatar.gameObject.SetActive(false); // ������ ������ ������
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