using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager instance;
    
    // Think about goals in steps
    // 2. Move character based on those values 

    PlayerControls playerControls;
    
    [SerializeField] Vector2 movement;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        
        // When the scene changes, run this logic
        SceneManager.activeSceneChanged += OnSceneChange;
        
        instance.enabled = false;
    }

    private void OnSceneChange(Scene oldScene, Scene newScene)
    {
        // if we are loading into our world scene, enable our player controls
        if (newScene.buildIndex == WorldSaveGameManager.instance.GetWorldSceneIndex())
        {
            instance.enabled = true;
        }
        // Otherwise we must be at the main menu, disable our player controls
        // This is so our player cant move around if we enter things like a character creation menu, ect
        else
        {
            instance.enabled = false; 
        }
    }

    private void OnEnable()
    {
        if (playerControls == null)
        {
            playerControls = new PlayerControls();

            playerControls.PlayerMovement.Movement.performed += i => movement = i.ReadValue<Vector2>();
        }
        
        playerControls.Enable();
    }

    private void OnDestroy()
    {
        // If we destroy this object, unsubscribe from this event
        SceneManager.activeSceneChanged -= OnSceneChange;
    }
}
