using UnityEngine;
using System.Collections;

public class Wall : MonoBehaviour
{             
    // Alternate sprite to display after Wall has been attacked by player. 
    public Sprite DmgSprite;          
    // hit points for the wall.          
    public int HP = 5;                          
    public AudioClip ChopSound1;
    public AudioClip ChopSound2;

    // Store a component reference to the attached SpriteRenderer.
    private SpriteRenderer spriteRenderer;      
    
    
    void Awake ()
    {
        // Get a component reference to the SpriteRenderer.
        spriteRenderer = GetComponent<SpriteRenderer> ();
    }
    
    
    // DamageWall is called when the player attacks a wall.
    public void DamageWall (int loss)
    {   
        SoundManager.Instance.RandomizeSfx(ChopSound1, ChopSound2);
        
        // Set spriteRenderer to the damaged wall sprite.
        spriteRenderer.sprite = DmgSprite;
        
        // Subtract loss from hit point total.
        HP -= loss;
        
        // If hit points are less than or equal to zero.
        if(HP <= 0)
            // Disable the gameObject.
            gameObject.SetActive (false);
    }
}