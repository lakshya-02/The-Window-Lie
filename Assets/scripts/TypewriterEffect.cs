using System.Collections;
using TMPro;
using UnityEngine;

public class TypewriterEffect : MonoBehaviour
{
    public float typingSpeed = 0.05f;

    private TextMeshProUGUI textMesh;
    private string fullText;

    void Awake()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        fullText = textMesh.text;
        textMesh.text = "";
    }

    void Start()
    {
        StartCoroutine(TypeText());
    }

    IEnumerator TypeText()
    {
        foreach (char c in fullText)
        {
            textMesh.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
    }
}
