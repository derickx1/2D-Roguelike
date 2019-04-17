using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class Enemy : MovingObject
{
    private Animator animator;                
    private bool skipMove;
    [SerializeField]
    private int playerDamage;
    [SerializeField]
    private AudioClip enemyAttack1;
    [SerializeField]
    private AudioClip enemyAttack2;

    public Transform Target;
    public int PlayerDamage
    {
        get
        {
            return playerDamage;
        }
        set
        {
            if (value < 0)
            {
                playerDamage = 0;
            }
            else
            {
                playerDamage = value;
            }
        }
    }   

    protected override void Start ()
    {   
        Assert.IsNotNull(enemyAttack1);
        Assert.IsNotNull(enemyAttack2);

        Assert.IsNotNull(Target);

        GameManager.Instance.AddEnemyToList(this);
        
        animator = GetComponent<Animator> ();
        Assert.IsNotNull(animator);

        base.Start ();
    }
    
    // Override the AttemptMove function of MovingObject to include functionality needed for Enemy to skip turns.
    protected override void AttemptMove <T> (int xDir, int yDir)
    {
        if(skipMove)
        {
            skipMove = false;
            return;
            
        }
    
        base.AttemptMove <T> (xDir, yDir);
        skipMove = true;
    }
    
    // MoveEnemy is called by the GameManger each turn to tell each Enemy to try to move towards the player.
    public void MoveEnemy ()
    {
        int xDir = 0;
        int yDir = 0;
        
        if(Mathf.Abs (Target.position.x - transform.position.x) < float.Epsilon)
        {
            yDir = Target.position.y > transform.position.y ? 1 : -1;
        }
        else
        {
            xDir = Target.position.x > transform.position.x ? 1 : -1;
        }
        AttemptMove <Player> (xDir, yDir);
    }
    
    // OnCantMove is called if Enemy attempts to move into a space occupied by a Player.
    protected override void OnCantMove <T> (T component)
    {
        Player hitPlayer = component as Player;        
        hitPlayer.LoseFood (playerDamage);        
        animator.SetTrigger ("enemyAttack");
        SoundManager.Instance.RandomizeSfx(enemyAttack1, enemyAttack2);
    }
}