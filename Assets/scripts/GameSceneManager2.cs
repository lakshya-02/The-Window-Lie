using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSceneManager2 : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Reference to the camera intro script for Scene 2")]
    public CameraIntro1 cameraIntro;
    
    [Tooltip("Reference to the dialogue UI for Scene 2")]
    public DialogueUIScene2 dialogueUI;
    
    [Tooltip("Fade panel for scene transition (optional - create a black full-screen Image)")]
    public Image fadePanel;
    
    [Header("Timing")]
    [Tooltip("Duration of fade-in transition")]
    public float fadeInDuration = 1f;
    
    [Tooltip("Delay after fade before starting cutscene")]
    public float sceneLoadDelay = 0.5f;
    
    [Tooltip("Delay after cutscene before showing dialogue")]
    public float dialogueStartDelay = 1f;
    
    private bool sequenceStarted = false;
    
    void Start()
    {
        // Start the intro sequence when this scene loads
        StartIntroSequence();
    }
    
    /// <summary>
    /// Starts the intro sequence for Scene 2
    /// </summary>
    public void StartIntroSequence()
    {
        if (sequenceStarted) return;
        
        sequenceStarted = true;
        StartCoroutine(IntroSequence());
    }
    
    /// <summary>
    /// Main intro sequence for Scene 2: Fade in -> Play cutscene -> Show dialogue
    /// </summary>
    private IEnumerator IntroSequence()
    {
        Debug.Log("Scene 2 intro sequence starting...");
        
        // LOCK CURSOR - No mouse input during transition/cutscene
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        // Disable MouseMovement script if it exists on camera
        MouseMovement mouseMovement = FindObjectOfType<MouseMovement>();
        if (mouseMovement != null)
        {
            mouseMovement.DisableMouseLook();
        }
        
        // Find references if not assigned
        if (cameraIntro == null)
        {
            cameraIntro = FindObjectOfType<CameraIntro1>();
        }
        
        if (dialogueUI == null)
        {
            dialogueUI = FindObjectOfType<DialogueUIScene2>();
        }
        
        // Disable dialogue UI initially - NO TEXT during transition
        if (dialogueUI != null)
        {
            dialogueUI.gameObject.SetActive(false);
        }
        
        // Fade in from black (instant transition effect)
        if (fadePanel != null)
        {
            Debug.Log("Fading in Scene 2...");
            yield return StartCoroutine(FadeIn());
        }
        
        yield return new WaitForSeconds(sceneLoadDelay);
        
        // Play camera cutscene AFTER fade completes
        if (cameraIntro != null)
        {
            Debug.Log("Starting Scene 2 camera cutscene...");
            yield return StartCoroutine(cameraIntro.PlayIntroCutscene());
            Debug.Log("Scene 2 camera cutscene completed");
        }
        else
        {
            Debug.LogWarning("CameraIntro1 not found! Make sure it's on the Main Camera in Scene 2.");
        }
        
        // Enable MouseMovement script if it exists
        if (mouseMovement != null)
        {
            mouseMovement.EnableMouseLook();
        }
        
        // Wait before showing dialogue
        yield return new WaitForSeconds(dialogueStartDelay);
        
        // UNLOCK CURSOR - Now player can interact
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        // NOW show dialogue after transition is complete
        if (dialogueUI != null)
        {
            Debug.Log("Starting Scene 2 dialogue...");
            dialogueUI.gameObject.SetActive(true);
            dialogueUI.StartDialogue(); // Start the dialogue and enable interaction
        }
        else
        {
            Debug.LogWarning("DialogueUIScene2 not found! Make sure it exists in Scene 2.");
        }
        
        Debug.Log("Scene 2 intro sequence complete! Core game functionality active.");
    }
    
    /// <summary>
    /// Fades in from black
    /// </summary>
    private IEnumerator FadeIn()
    {
        if (fadePanel == null) yield break;
        
        float elapsed = 0f;
        Color color = fadePanel.color;
        color.a = 1f; // Start fully black
        fadePanel.color = color;
        fadePanel.gameObject.SetActive(true);
        
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            color.a = 1f - (elapsed / fadeInDuration);
            fadePanel.color = color;
            yield return null;
        }
        
        color.a = 0f;
        fadePanel.color = color;
        fadePanel.gameObject.SetActive(false); // Hide when done
    }
}
