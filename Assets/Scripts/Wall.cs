using System.Collections;
using UnityEngine.Assertions;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Wall : MonoBehaviour
{             
    [SerializeField]
    private Sprite damageSprite;                
    [SerializeField]
    private int HP = 5;                          
    [SerializeField]
    private AudioClip chopSound1;
    [SerializeField]
    private AudioClip chopSound2;
    private SpriteRenderer spriteRenderer;   
    
    void Awake ()
    {
        Assert.IsNotNull(damageSprite);
        Assert.IsNotNull(chopSound1);
        Assert.IsNotNull(chopSound2);

        spriteRenderer = GetComponent<SpriteRenderer> ();
		Assert.IsNotNull(spriteRenderer);
    }
    
    public void DamageWall (int loss)
    {   
        SoundManager.Instance.RandomizeSfx(chopSound1, chopSound2);
        spriteRenderer.sprite = damageSprite;

        HP -= loss;
        if(HP <= 0)
            gameObject.SetActive (false);
    }
}