using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
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

    // Dialogue structure: null = automatic, choices = player picks
    private List<DialogueNode> dialogue = new List<DialogueNode>()
    {
        new DialogueNode("Stranger: <sketchy>Someone's selling cotton candy today</sketchy>", null),
        new DialogueNode("Stranger: <sketchy>Pink and blue clouds on a stick. Kids chasing him like... they've forgotten pain exists</sketchy>", null),
        new DialogueNode("", new string[] { "So what?", "That's... actually kind of beautiful" }),
        new DialogueNode("Stranger: <sketchy>It's not the sugar..</sketchy>", null),
        new DialogueNode("Stranger: <sketchy>It's the way the kids don't think about tomorrow while they're eating it</sketchy>", null),
        new DialogueNode("", new string[] { "That's because they're stupid, Life hasn't taught them anything yet", "Maybe ignorance is bliss" }),
        new DialogueNode("Stranger: <sketchy>Maybe. Or maybe they already learned something we forgot</sketchy>", null),
        new DialogueNode("...", null),
        new DialogueNode("", new string[] { "What else? Let me guess, birds, lovers holding hands, some nonsense like that?", "Tell me more" }),
        new DialogueNode("Stranger: <sketchy>Actually.., there's an old couple arguing over a map. They've been arguing for ten minutes. Neither of them knows where they're going,</sketchy>", null),
        new DialogueNode("Stranger: <sketchy>but they're going Together</sketchy>", null),
        new DialogueNode("", new string[] { "So?", "That's kind of sweet, I guess" }),
        new DialogueNode("...", null),
        new DialogueNode("Stranger: <sketchy>So they're still choosing,</sketchy>", null),
        new DialogueNode("Stranger: <sketchy>That matters</sketchy>", null),
        new DialogueNode("", new string[] { "...", "I don't get it" }),
        new DialogueNode("", new string[] { "Sounds miserable", "Maybe you're right" }),
        new DialogueNode("Stranger: <sketchy>Most good things do</sketchy>", null),
        new DialogueNode("Stranger: <sketchy>The sea’s restless today. Dark blue. There’s a ship just sitting there, like it’s scared to leave</sketchy>", null),
        new DialogueNode("", new string[] {"Why would it be scared?", "What do you mean?"}),
        new DialogueNode("Stranger: <sketchy>I don’t know. Maybe it’s waiting for something. Someone...</sketchy>", null),
        new DialogueNode("", new string[] {"Like you?", "Huh,"}),
        new DialogueNode("Stranger: <sketchy>Because staying feels safer than risking storms</sketchy>", null),
        new DialogueNode("Stranger: <sketchy>Even if the sea is scary.</sketchy>", null),
        new DialogueNode("", new string[] {"Ships are built for storms", "Sounds about right" }),
        new DialogueNode("Stranger: <sketchy>People are too,</sketchy>", null),
        new DialogueNode("", new string[] {"You don’t know anything about me", "You are not a philosopher"}),        
        new DialogueNode("Stranger: <sketchy>I know you listen,</sketchy>", null),
        new DialogueNode("Stranger: <sketchy>Even when you pretend not to</sketchy>", null),
        new DialogueNode("...", null),
        new DialogueNode("", new string[] {"How far is the sea?", "Stop with the Allusions"}),
        new DialogueNode("", new string[] {"Do the kids come every day?", "I don't care"}),
        new DialogueNode("", new string[] {"What color are the sunsets really?", "Why does any of this matter?"}),
        new DialogueNode("...", null),
        new DialogueNode("", new string[] {"Why do you tell me all this?", "Just leave me alone"}),
        new DialogueNode("...", null),
        new DialogueNode("Stranger: <sketchy>Because,</sketchy>", null),
        new DialogueNode("Stranger: <sketchy>when you're stuck somewhere small, you need to believe the world is still big</sketchy>", null),
        new DialogueNode("...", null),
        new DialogueNode("Stranger: <sketchy>Tomorrow,</sketchy>", null),
        new DialogueNode("Stranger: <sketchy>there's supposed to be a clear sky. You'll like it</sketchy>", null),
        new DialogueNode("", new string[] {"Whatever", "I doubt it"}),          
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
        
        // Disable this component entirely until needed
        this.enabled = false;
    }

    void Start()
    {
        // Don't auto-start - wait for GameSceneManager
        // Component is disabled by default
    }
    
    void OnEnable()
    {
        // This runs when the GameObject is enabled
        // Still wait for explicit StartDialogue() call
        // Do NOT auto-start here
    }
    
    /// <summary>
    /// Called by GameSceneManager to start dialogue
    /// </summary>
    public void StartDialogue()
    {
        if (!dialogueStarted)
        {
            // Re-enable the component
            this.enabled = true;
            
            dialogueStarted = true;
            canInteract = true; // NOW allow interaction
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
            Debug.Log("Dialogue complete!");
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
