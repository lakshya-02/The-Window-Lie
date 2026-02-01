using System.Collections;
using UnityEngine;

public class CameraIntro : MonoBehaviour
{
    [Header("Rotation Settings")]
    [Tooltip("Duration of the first rotation (X-axis)")]
    public float firstRotationDuration = 2f;
    
    [Tooltip("Duration of the second rotation (Y-axis)")]
    public float secondRotationDuration = 2f;
    
    [Tooltip("Delay before starting the cutscene")]
    public float startDelay = 0.5f;
    
    [Tooltip("Animation curve for smooth rotation")]
    public AnimationCurve rotationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    [Header("Cutscene Control")]
    [Tooltip("Enable to play cutscene on Start")]
    public bool playOnStart = false; // Changed to false - will be controlled by GameSceneManager
    
    private bool cutsceneComplete = false;
    
    void Start()
    {
        if (playOnStart)
        {
            StartCoroutine(PlayIntroCutscene());
        }
    }
    
    /// <summary>
    /// Plays the intro cutscene with camera rotations
    /// </summary>
    public IEnumerator PlayIntroCutscene()
    {
        // Set initial rotation explicitly
        transform.rotation = Quaternion.Euler(0, -90, 0);
        
        // Wait for initial delay
        yield return new WaitForSeconds(startDelay);
        
        // First rotation: X-axis from 90 to 0
        yield return StartCoroutine(RotateCamera(
            new Vector3(90, 0, 0), 
            new Vector3(0, 0, 0), 
            firstRotationDuration
        ));
        
        // Second rotation: Y-axis from 0 to -90
        yield return StartCoroutine(RotateCamera(
            new Vector3(0, 0, 0), 
            new Vector3(0, -90, 0), 
            secondRotationDuration
        ));
        
        // Ensure final rotation is locked
        transform.rotation = Quaternion.Euler(0, -90, 0);
        
        cutsceneComplete = true;
        OnCutsceneComplete();
    }
    
    /// <summary>
    /// Smoothly rotates the camera from start to end rotation
    /// </summary>
    private IEnumerator RotateCamera(Vector3 startRotation, Vector3 endRotation, float duration)
    {
        float elapsed = 0f;
        Quaternion startQuat = Quaternion.Euler(startRotation);
        Quaternion endQuat = Quaternion.Euler(endRotation);
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float curveValue = rotationCurve.Evaluate(t);
            
            transform.rotation = Quaternion.Lerp(startQuat, endQuat, curveValue);
            
            yield return null;
        }
        
        // Ensure final rotation is exact
        transform.rotation = endQuat;
    }
    
    /// <summary>
    /// Called when the cutscene is complete
    /// </summary>
    private void OnCutsceneComplete()
    {
        Debug.Log("Camera intro cutscene completed!");
        // Add any additional logic here (e.g., enable player controls)
    }
    
    /// <summary>
    /// Manually trigger the cutscene from code
    /// </summary>
    public void PlayCutscene()
    {
        if (!cutsceneComplete)
        {
            StartCoroutine(PlayIntroCutscene());
        }
    }
    
    /// <summary>
    /// Check if cutscene is finished
    /// </summary>
    public bool IsCutsceneComplete()
    {
        return cutsceneComplete;
    }
}
