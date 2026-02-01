using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    [Header("Scene Settings")]
    [Tooltip("Name of the game scene to load after pressing Start")]
    public string gameSceneName = "GameScene";
    
    [Header("UI References")]
    [Tooltip("Start button reference")]
    public Button startButton;
    
    [Tooltip("Quit button reference")]
    public Button quitButton;
    
    [Header("Optional UI")]
    [Tooltip("Title text (optional)")]
    public TextMeshProUGUI titleText;
    
    [Tooltip("Fade panel for transitions (optional)")]
    public CanvasGroup fadePanel;
    
    [Header("Settings")]
    [Tooltip("Fade duration when transitioning")]
    public float fadeDuration = 1f;
    
    private bool isTransitioning = false;
    
    void Start()
    {
        // Setup button listeners
        if (startButton != null)
        {
            startButton.onClick.AddListener(OnStartButtonClicked);
        }
        else
        {
            Debug.LogWarning("Start button reference is missing!");
        }
        
        if (quitButton != null)
        {
            quitButton.onClick.AddListener(OnQuitButtonClicked);
        }
        else
        {
            Debug.LogWarning("Quit button reference is missing!");
        }
        
        // Fade in if fade panel exists
        if (fadePanel != null)
        {
            StartCoroutine(FadeIn());
        }
    }
    
    /// <summary>
    /// Called when Start button is clicked
    /// </summary>
    public void OnStartButtonClicked()
    {
        if (isTransitioning) return;
        
        isTransitioning = true;
        Debug.Log("Start button clicked - Loading game scene");
        
        // Disable buttons
        if (startButton != null) startButton.interactable = false;
        if (quitButton != null) quitButton.interactable = false;
        
        // Start transition
        if (fadePanel != null)
        {
            StartCoroutine(FadeOutAndLoadScene());
        }
        else
        {
            LoadGameScene();
        }
    }
    
    /// <summary>
    /// Called when Quit button is clicked
    /// </summary>
    public void OnQuitButtonClicked()
    {
        Debug.Log("Quit button clicked - Exiting game");
        
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    
    /// <summary>
    /// Loads the game scene
    /// </summary>
    private void LoadGameScene()
    {
        SceneManager.LoadScene(gameSceneName);
    }
    
    /// <summary>
    /// Fades in the menu
    /// </summary>
    private System.Collections.IEnumerator FadeIn()
    {
        float elapsed = 0f;
        fadePanel.alpha = 1f;
        
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            fadePanel.alpha = 1f - (elapsed / fadeDuration);
            yield return null;
        }
        
        fadePanel.alpha = 0f;
    }
    
    /// <summary>
    /// Fades out and loads the game scene
    /// </summary>
    private System.Collections.IEnumerator FadeOutAndLoadScene()
    {
        float elapsed = 0f;
        fadePanel.alpha = 0f;
        
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            fadePanel.alpha = elapsed / fadeDuration;
            yield return null;
        }
        
        fadePanel.alpha = 1f;
        LoadGameScene();
    }
    
    void OnDestroy()
    {
        // Clean up button listeners
        if (startButton != null)
        {
            startButton.onClick.RemoveListener(OnStartButtonClicked);
        }
        
        if (quitButton != null)
        {
            quitButton.onClick.RemoveListener(OnQuitButtonClicked);
        }
    }
}
