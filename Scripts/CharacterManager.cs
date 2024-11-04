using UnityEngine;
using UnityEngine.UI;

public class CharacterManager : MonoBehaviour
{
    [SerializeField] private Image leftAvatar;
    [SerializeField] private Image rightAvatar;
    [SerializeField] private GameObject speakerPanelLeft;
    [SerializeField] private GameObject speakerPanelCenter;
    [SerializeField] private GameObject speakerPanelRight;

    public void SetCharacter(string characterName, int place)
    {
        if (place == 1)
        {
            UpdateAvatar(leftAvatar, characterName);
            ShowPanel(speakerPanelLeft);
        }
        else if (place == 2)
        {
            UpdateAvatar(rightAvatar, characterName);
            ShowPanel(speakerPanelRight);
        }
        else
        {
            HideAvatars();
            ShowPanel(speakerPanelCenter);
        }
    }

    private void UpdateAvatar(Image avatar, string characterName)
    {
        Sprite sprite = Resources.Load<Sprite>("Characters/" + characterName);
        if (sprite != null)
        {
            avatar.sprite = sprite;
            avatar.gameObject.SetActive(true);
        }
        else
        {
            avatar.gameObject.SetActive(false);
            Debug.LogWarning("Character sprite not found: " + characterName);
        }
    }

    public void HideAvatars()
    {
        leftAvatar.gameObject.SetActive(false);
        rightAvatar.gameObject.SetActive(false);
    }

    private void ShowPanel(GameObject panel)
    {
        speakerPanelLeft.SetActive(false);
        speakerPanelCenter.SetActive(false);
        speakerPanelRight.SetActive(false);

        if (panel != null)
        {
            panel.SetActive(true);
        }
    }
}

