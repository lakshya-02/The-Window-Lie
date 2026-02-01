using UnityEngine;

/// <summary>
/// Dialogue node structure used by all dialogue systems
/// </summary>
[System.Serializable]
public class DialogueNode
{
    public string text;
    public string[] choices;
    
    public DialogueNode(string text, string[] choices)
    {
        this.text = text;
        this.choices = choices;
    }
}
