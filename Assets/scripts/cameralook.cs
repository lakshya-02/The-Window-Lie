using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseMovement : MonoBehaviour
{
    public float mouseSensitivity = 360f;

    float xRotation = 0f;
    float yRotation = 0f;

    public float topClamp = 90f;
    public float bottomClamp = -90f;
    
    private bool isEnabled = false; // Start disabled

    void Start()
    {
        // Don't lock cursor here - GameSceneManager will handle it
        isEnabled = false;
    }
    
    /// <summary>
    /// Enable mouse look after cutscene
    /// </summary>
    public void EnableMouseLook()
    {
        isEnabled = true;
        // Initialize rotation from current camera rotation
        Vector3 currentRotation = transform.localEulerAngles;
        xRotation = currentRotation.x;
        yRotation = currentRotation.y;
    }
    
    /// <summary>
    /// Disable mouse look during cutscene
    /// </summary>
    public void DisableMouseLook()
    {
        isEnabled = false;
    }

    void Update()
    {
        // Don't run if disabled (during cutscene)
        if (!isEnabled) return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;

        xRotation = Mathf.Clamp(xRotation, bottomClamp, topClamp);

        yRotation += mouseX;

        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }
}
