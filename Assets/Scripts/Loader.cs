using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour
{
    // GameManager prefab to instantiate.
    public GameObject gameManager;
    // SoundManager prefab to instantiate.			
    public GameObject soundManager;			

    void Awake()
    {
    	// Check if a GameManager has already been assigned to static variable GameManager.Instance or if it's still null.
        if (GameManager.Instance == null)
            
            // Instantiate gameManager prefab.
            Instantiate(gameManager);
        
        // Check if a SoundManager has already been assigned to static variable GameManager.Instance or if it's still null.
        if (SoundManager.Instance == null)
            
            // Instantiate SoundManager prefab.
            Instantiate(soundManager);
    }
}
