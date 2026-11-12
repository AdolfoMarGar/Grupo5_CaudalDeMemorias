using UnityEngine;

[System.Serializable]
public class MinigameItemData
{
    public string id;
    public string title;
    [TextArea(2, 4)]
    public string description;
    public string sceneName;
    public Sprite thumbnail;
    public bool isUnlocked;
}
