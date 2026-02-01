using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class DialogueUIScene2 : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI dialogueText;
    public GameObject choiceButtonsPanel; // Panel containing choice buttons
    public Button choiceButton1;
    public Button choiceButton2;
    public TextMeshProUGUI choiceText1;
    public TextMeshProUGUI choiceText2;
    
    [Header("Settings")]
    public float typingSpeed = 0.04f;

    // Scene 2 Dialogue structure: null = automatic, choices = player picks
    private List<DialogueNode> dialogue = new List<DialogueNode>()
{
    new DialogueNode("Nurse: The rain came down hard yesterday", null),
    new DialogueNode("Nurse: Washing everything away, like the world wanted a fresh start", null),
    new DialogueNode("", new string[] { "Rain is just rain", "Did it help?" }),
    new DialogueNode("Nurse: Maybe. But the streets smell different after.", null),
    new DialogueNode("Nurse: Like something old was pulled up from underneath", null),
    new DialogueNode("", new string[] { "That sounds ominous", "What do you mean?" }),
    new DialogueNode("Nurse: Not ominous. Just... honest", null),
    new DialogueNode("Nurse: Things hide when it's dry. When it's wet, you see what's really there", null),
    new DialogueNode("", new string[] { "I don't care about rain", "Go on" }),
    new DialogueNode("Nurse: There was a dog. Small thing, sitting under an awning", null),
    new DialogueNode("Nurse: Someone left it there. It waited anyway", null),
    new DialogueNode("", new string[] { "Pathetic", "That's sad" }),
    new DialogueNode("Nurse: Or loyal", null),
    new DialogueNode("Nurse: Depends how you look at it", null),
    new DialogueNode("", new string[] { "Same thing", "I guess" }),
    new DialogueNode("Nurse: A woman eventually took it home. Wrapped it in her coat", null),
    new DialogueNode("", new string[] { "Lucky dog", "Why are you telling me this?" }),
    new DialogueNode("Nurse: Because sometimes things work out", null),
    new DialogueNode("Nurse: Not always. But sometimes", null),
    new DialogueNode("", new string[] { "That's rare", "I want to believe that" }),
    new DialogueNode("Nurse: The lighthouse was on last night. First time in months", null),
    new DialogueNode("", new string[] { "So what?", "Why?" }),
    new DialogueNode("Nurse: Don't know. Maybe someone needed it", null),
    new DialogueNode("Nurse: Maybe someone was coming home", null),
    new DialogueNode("", new string[] { "Or maybe they were leaving", "That's hopeful" }),
    new DialogueNode("Nurse: Both are movement,", null),
    new DialogueNode("Nurse: Better than being stuck", null),
    new DialogueNode("...", null),
    new DialogueNode("Nurse: You're still here", null),
    new DialogueNode("Nurse: That means something", null),
    new DialogueNode("", new string[] { "Not really", "Maybe" }),
    new DialogueNode("Nurse: The rain will come again...", null),
    new DialogueNode("...", null),
    new DialogueNode("The nurse's breathing becomes shallow.", null),
    new DialogueNode("<sketchy>Her eyes close.</sketchy>", null),
    new DialogueNode("...", null),
    new DialogueNode("<sketchy>Silence.</sketchy>", null),
    new DialogueNode("...", null),
    new DialogueNode("<sketchy>The machines stop beeping.</sketchy>", null),
    new DialogueNode("...", null),
    new DialogueNode("<sketchy>[Time passes]</sketchy>", null),
    new DialogueNode("...", null),
    new DialogueNode("<sketchy>You wake up.</sketchy>", null),
    new DialogueNode("<sketchy>You're in her bed now.</sketchy>", null),
    new DialogueNode("<sketchy>The view from this side is different.</sketchy>", null),
    new DialogueNode("...", null),
    new DialogueNode("<sketchy>A new patient lies where you used to be.</sketchy>", null),
    new DialogueNode("<sketchy>Younger. Quiet. Staring at nothing.</sketchy>", null),
    new DialogueNode("...", null),
    new DialogueNode("", new string[] { "What happened to her?", "..." }),
    new DialogueNode("Doctor: She passed three days ago.", null),
    new DialogueNode("Doctor: Peacefully. In her sleep.", null),
    new DialogueNode("", new string[] { "She told me stories", "I barely knew her" }),
    new DialogueNode("Doctor: She told everyone stories.", null),
    new DialogueNode("Doctor: Different ones, depending on who was listening.", null),
    new DialogueNode("", new string[] { "Were they real?", "What do you mean?" }),
    new DialogueNode("Doctor: She made it all up.", null),
    new DialogueNode("Doctor: So you'd have something to hold onto.", null),
    new DialogueNode("Doctor: People give up faster than their bodies fail.", null),
    new DialogueNode("", new string[] { "That's cruel", "That's kind" }),
    new DialogueNode("Doctor: Maybe both.", null),
    new DialogueNode("Doctor: But you're still here, aren't you?", null),
    new DialogueNode("...", null),
    new DialogueNode("<sketchy>The new patient shifts slightly.</sketchy>", null),
    new DialogueNode("<sketchy>You can feel their eyes on you.</sketchy>", null),
    new DialogueNode("<sketchy>Waiting.</sketchy>", null),
    new DialogueNode("...", null),
    new DialogueNode("", new string[] { "Someone's selling cotton candy today", "The sea’s restless today" }),
    new DialogueNode("...", null),
};

    int index = 0;
    bool isTyping = false;
    bool forceComplete = false;
    private bool dialogueStarted = false;
    private bool canInteract = false;
    private bool waitingForChoice = false;

    void Awake()
    {
        canInteract = false;
        dialogueStarted = false;
        waitingForChoice = false;
        
        if (dialogueText != null)
        {
            dialogueText.text = "";
            dialogueText.maxVisibleCharacters = 0;
        }
        
        // Hide choice buttons initially
        if (choiceButtonsPanel != null)
        {
            choiceButtonsPanel.SetActive(false);
        }
        
        // Setup button listeners
        if (choiceButton1 != null)
        {
            choiceButton1.onClick.AddListener(() => OnChoiceSelected(0));
        }
        if (choiceButton2 != null)
        {
            choiceButton2.onClick.AddListener(() => OnChoiceSelected(1));
        }
    }

    void Start()
    {
        // Don't auto-start - wait for GameSceneManager2
        Debug.Log("DialogueUIScene2 Start() called");
    }
    
    void OnEnable()
    {
        // This runs when the GameObject is enabled
        Debug.Log("DialogueUIScene2 OnEnable() called");
    }
    
    /// <summary>
    /// Called by GameSceneManager2 to start dialogue
    /// </summary>
    public void StartDialogue()
    {
        Debug.Log("DialogueUIScene2 StartDialogue() called");
        
        if (!dialogueStarted)
        {
            dialogueStarted = true;
            canInteract = true; // NOW allow interaction
            index = 0; // Reset to beginning
            ShowLine();
        }
    }

    void Update()
    {
        // CRITICAL: Block ALL input until dialogue is explicitly started
        if (!canInteract || !dialogueStarted) return;
        
        // Don't accept clicks if waiting for button choice
        if (waitingForChoice) return;
        
        if (!Input.GetMouseButtonDown(0)) return;

        if (isTyping)
        {
            // GBA behavior: instantly finish line
            forceComplete = true;
        }
        else
        {
            AdvanceDialogue();
        }
    }

    void AdvanceDialogue()
    {
        index++;
        if (index < dialogue.Count)
        {
            ShowLine();
        }
        else
        {
            // Dialogue finished
            Debug.Log("Scene 2 Dialogue complete!");
        }
    }

    void ShowLine()
    {
        StopAllCoroutines();
        forceComplete = false;

        DialogueNode currentNode = dialogue[index];
        
        // Check if this is a choice node
        if (currentNode.choices != null && currentNode.choices.Length > 0)
        {
            // Hide dialogue text, show choice buttons
            dialogueText.text = "";
            ShowChoices(currentNode.choices);
        }
        else
        {
            // Normal dialogue line
            HideChoices();
            dialogueText.text = currentNode.text;
            dialogueText.ForceMeshUpdate();
            dialogueText.maxVisibleCharacters = 0;
            StartCoroutine(TypeLine());
        }
    }

    void ShowChoices(string[] choices)
    {
        waitingForChoice = true;
        
        if (choiceButtonsPanel != null)
        {
            choiceButtonsPanel.SetActive(true);
        }
        
        if (choiceText1 != null && choices.Length > 0)
        {
            choiceText1.text = choices[0];
            choiceButton1.gameObject.SetActive(true);
        }
        
        if (choiceText2 != null && choices.Length > 1)
        {
            choiceText2.text = choices[1];
            choiceButton2.gameObject.SetActive(true);
        }
    }

    void HideChoices()
    {
        waitingForChoice = false;
        
        if (choiceButtonsPanel != null)
        {
            choiceButtonsPanel.SetActive(false);
        }
    }

    void OnChoiceSelected(int choiceIndex)
    {
        // Get the selected text and show it as dialogue
        DialogueNode currentNode = dialogue[index];
        string selectedText = "You: " + currentNode.choices[choiceIndex];
        
        // Hide choices
        HideChoices();
        
        // Show selected choice as dialogue
        dialogueText.text = selectedText;
        dialogueText.ForceMeshUpdate();
        dialogueText.maxVisibleCharacters = 0;
        
        StartCoroutine(TypeChoiceAndContinue());
    }

    IEnumerator TypeChoiceAndContinue()
    {
        isTyping = true;
        int totalChars = dialogueText.textInfo.characterCount;

        for (int i = 0; i <= totalChars; i++)
        {
            dialogueText.maxVisibleCharacters = i;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        
        // Wait a moment then continue to next line
        yield return new WaitForSeconds(0.5f);
        AdvanceDialogue();
    }

    IEnumerator TypeLine()
    {
        isTyping = true;
        int totalChars = dialogueText.textInfo.characterCount;

        for (int i = 0; i <= totalChars; i++)
        {
            if (forceComplete)
            {
                dialogueText.maxVisibleCharacters = totalChars;
                break;
            }

            dialogueText.maxVisibleCharacters = i;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }
    
    void OnDestroy()
    {
        // Clean up button listeners
        if (choiceButton1 != null)
        {
            choiceButton1.onClick.RemoveAllListeners();
        }
        if (choiceButton2 != null)
        {
            choiceButton2.onClick.RemoveAllListeners();
        }
    }
}
