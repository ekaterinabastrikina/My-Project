using System.Collections.Generic;
using UnityEngine;

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
    public List<string> texts;
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
