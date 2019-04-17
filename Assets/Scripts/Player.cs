using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;
using UnityEngine.UI;    

public class Player : MovingObject
{
    [SerializeField]
    private int defaultFood = 100;
	[SerializeField]                  
    private int food;     
    [SerializeField]
    private int pointsPerFood = 10;           
    [SerializeField]
    private int pointsPerSoda = 20;
    [SerializeField]
    private float restartLevelDelay = 1f;
	[SerializeField]
    private GameManager gameManager;
	[SerializeField]
    private SoundManager soundManager;
	[SerializeField]	
	private Text foodText;
    [SerializeField]	
	private AudioClip moveSound1;
    [SerializeField]	
	private AudioClip moveSound2;
    [SerializeField]	
	private AudioClip eatSound1;
    [SerializeField]	
	private AudioClip eatSound2; 
    [SerializeField]	
	private AudioClip drinkSound1;
    [SerializeField]	
	private AudioClip drinkSound2;
    [SerializeField]	
	private AudioClip gameOverSound;                 
    private Animator animator;

    public static int Coins;
    public static int WallDamage;
    public static int FoodDamage;

#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
    // Used to store location of screen touch origin for mobile controls.
    private Vector2 touchOrigin = -Vector2.one;	
#endif                        

    protected override void Start ()
    {
        Assert.IsNotNull(gameManager);
        Assert.IsNotNull(soundManager);

        Assert.IsNotNull(foodText);

        Assert.IsNotNull(moveSound1);
        Assert.IsNotNull(moveSound2);
        Assert.IsNotNull(eatSound1);
        Assert.IsNotNull(eatSound2);
        Assert.IsNotNull(drinkSound1);
        Assert.IsNotNull(drinkSound2);
        Assert.IsNotNull(gameOverSound);

        animator = GetComponent<Animator>();
        Assert.IsNotNull(animator);

        Coins = SaveManager.Coins;
        WallDamage = SaveManager.WallDamage;
        FoodDamage = SaveManager.FoodDamage;
        food = defaultFood;
        foodText.text = $"Food: {food}";
        base.Start ();
    }
    
    private void Update ()
    {
        if (!gameManager.PlayersTurn || gameManager.FreezePlayer)
        {
            return;
        }
        int horizontal = 0;
        int vertical = 0;       
        
#if UNITY_STANDALONE || UNITY_WEBPLAYER
        
        // Get input from the input manager, round it to an integer and store in horizontal to set x axis move direction.
        horizontal = (int) (Input.GetAxisRaw ("Horizontal"));
        
        // Get input from the input manager, round it to an integer and store in vertical to set y axis move direction.
        vertical = (int) (Input.GetAxisRaw ("Vertical"));
        
        // Check if moving horizontally, if so set vertical to zero.
        if(horizontal != 0)
        {
            vertical = 0;
        }

#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHON

        if (Input.touchCount > 0)
        {
            Touch myTouch = Input.touches[0];

            if (myTouch.phase == TouchPhase.Began)
            {
                touchOrigin = myTouch.position;
            }

            else if (myTouch.phase == TouchPhase.Ended && touchOrigin.x >= 0)
            {
                Vector2 touchEnd = myTouch.position;
                float x = touchEnd.x - touchOrigin.x;
                float y = touchEnd.y - touchOrigin.y;
                touchOrigin.x = -1;
                if (Mathf.Abs(x) > Mathf.Abs(y))
                    horizontal = x > 0 ? 1 : -1;
                else
                    vertical = y > 0 ? 1 : -1;
            }
        }
        
        #endif
        
        if((horizontal != 0) || (vertical != 0))
        {
            AttemptMove<Wall> (horizontal, vertical);
        }
    }

    protected override void AttemptMove <T> (int xDir, int yDir)
    {
        food = food - FoodDamage;
        foodText.text = $"Food: {food}";
        
        base.AttemptMove <T> (xDir, yDir);
        RaycastHit2D hit;
        if (Move (xDir, yDir, out hit)) 
        {
            soundManager.RandomizeSfx(moveSound1, moveSound2);
        }
        CheckIfGameOver ();
        gameManager.PlayersTurn = false;
    }
    
    protected override void OnCantMove <T> (T component)
    {
        Wall hitWall = component as Wall;
        hitWall.DamageWall (WallDamage);
        animator.SetTrigger ("playerChop");
    }

    private void OnTriggerEnter2D (Collider2D other)
    {
        if(other.tag == "Exit")
        {
			Coins++;
            StartCoroutine(Restart());
            enabled = false;
        }        
        else if(other.tag == "Food")
        {
            food += pointsPerFood;
            foodText.text = $"+{pointsPerFood} Food: {food}";
            soundManager.RandomizeSfx(eatSound1, eatSound2);
            other.gameObject.SetActive (false);
        }
        else if(other.tag == "Soda")
        {
            food += pointsPerSoda;
            foodText.text = $"+{pointsPerSoda} Food: {food}";
            soundManager.RandomizeSfx(drinkSound1, drinkSound2);
            other.gameObject.SetActive (false);
        }
    }
    
    private IEnumerator Restart ()
    {
		yield return new WaitForSeconds(restartLevelDelay);
        this.transform.position = Vector3.zero;	
		gameManager.BeatLevel();
		enabled = true;
    }
    
    public void LoseFood (int loss)
    {
        animator.SetTrigger ("playerHit");
        food -= loss;
        foodText.text = $"-{loss} Food: {food}";
        CheckIfGameOver ();
    }
    
    private void CheckIfGameOver ()
    {
        if (food <= 0) 
        {
            soundManager.PlaySingle(gameOverSound);
            gameManager.GameOver();
			SaveManager.Save(Coins, WallDamage, FoodDamage);
			this.transform.position = Vector3.zero;
			food = defaultFood;
			foodText.text = $"Food: {food}";
        }
    }
}