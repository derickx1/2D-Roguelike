using UnityEngine;
using System.Collections;
// Allows us to use SceneManager.
using UnityEngine.SceneManagement;  
using UnityEngine.UI;    

// Player inherits from MovingObject, our base class for objects that can move, Enemy also inherits from this.
public class Player : MovingObject
{
    // Delay time in seconds to restart level.
    public float RestartLevelDelay = 1f;
    // Number of points to add to player food points when picking up a food object.        
    public int PointsPerFood = 10;
    // Number of points to add to player food points when picking up a soda object.              
    public int PointsPerSoda = 20;
    // How much damage a player does to a wall when chopping it.              
    public int WallDamage;
    // How much food a player loses per movement.
    public int FoodPerMovement;

    public Text FoodText;
    public AudioClip MoveSound1;
    public AudioClip MoveSound2;
    public AudioClip EatSound1;
    public AudioClip EatSound2; 
    public AudioClip DrinkSound1;
    public AudioClip DrinkSound2;
    public AudioClip GameOverSound;                 
    
    // Used to store a reference to the Player's animator component.
    private Animator animator;
    // Used to store player food points total during level.                  
    private int food;
#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
    // Used to store location of screen touch origin for mobile controls.
    private Vector2 touchOrigin = -Vector2.one;	
#endif                        
    
    
    // Start overrides the Start function of MovingObject.
    protected override void Start ()
    {
        // Get a component reference to the Player's animator component.
        animator = GetComponent<Animator>();
        
        // Get the current food point total stored in GameManager.Instance between levels.
        food = GameManager.Instance.PlayerFoodPoints;

        FoodText = GameObject.Find("FoodText").GetComponent<Text>();

        FoodText.text = "Food: " + food;
        
        // Call the Start function of the MovingObject base class.
        base.Start ();

        WallDamage = PlayerPrefs.GetInt("WallDamage", 1);
        FoodPerMovement = PlayerPrefs.GetInt("FoodDamage", 5);
    }

    public void UpdateStats()
    {
        WallDamage = PlayerPrefs.GetInt("WallDamage", 1);
        FoodPerMovement = PlayerPrefs.GetInt("FoodDamage", 5);
    }
    
    // This function is called when the behaviour becomes disabled or inactive.
    private void OnDisable ()
    {
        // When Player object is disabled, store the current local food total in the GameManager so it can be re-loaded in next level.
        GameManager.Instance.PlayerFoodPoints = food;
    }
    
    
    private void Update ()
    {
        // If it's not the player's turn, exit the function.
        if(!GameManager.Instance.PlayersTurn) return;
        
        // Used to store the horizontal move direction.
        int horizontal = 0;
        // Used to store the vertical move direction.     
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
        
        // Check if we have a non-zero value for horizontal or vertical.
        if((horizontal != 0) || (vertical != 0))
        {
            // Call AttemptMove passing in the generic parameter Wall, since that is what Player may interact with if they encounter one (by attacking it).
            // Pass in horizontal and vertical as parameters to specify the direction to move Player in.
            AttemptMove<Wall> (horizontal, vertical);
        }
    }
    
    // AttemptMove overrides the AttemptMove function in the base class MovingObject.
    // AttemptMove takes a generic parameter T which for Player will be of the type Wall, it also takes integers for x and y direction to move in.
    protected override void AttemptMove <T> (int xDir, int yDir)
    {
        // Every time player moves, subtract from food points total.
        food = food - FoodPerMovement;

        FoodText.text = "Food: " + food;
        
        // Call the AttemptMove method of the base class, passing in the component T (in this case Wall) and x and y direction to move.
        base.AttemptMove <T> (xDir, yDir);
        
        // Hit allows us to reference the result of the Linecast done in Move.
        RaycastHit2D hit;
        
        // If Move returns true, meaning Player was able to move into an empty space.
        if (Move (xDir, yDir, out hit)) 
        {
            // Call RandomizeSfx of SoundManager to play the move sound, passing in two audio clips to choose from.
            SoundManager.Instance.RandomizeSfx(MoveSound1, MoveSound2);
        }
        
        // Since the player has moved and lost food points, check if the game has ended.
        CheckIfGameOver ();
        
        // Set the PlayersTurn boolean of GameManager to false now that players turn is over.
        GameManager.Instance.PlayersTurn = false;
    }
    
    
    // OnCantMove overrides the abstract function OnCantMove in MovingObject.
    // It takes a generic parameter T which in the case of Player is a Wall which the player can attack and destroy.
    protected override void OnCantMove <T> (T component)
    {
        // Set hitWall to equal the component passed in as a parameter.
        Wall hitWall = component as Wall;
        
        // Call the DamageWall function of the Wall we are hitting.
        hitWall.DamageWall (WallDamage);
        
        // Set the attack trigger of the player's animation controller in order to play the player's attack animation.
        animator.SetTrigger ("playerChop");
    }
    
    
    // OnTriggerEnter2D is sent when another object enters a trigger collider attached to this object (2D physics only).
    private void OnTriggerEnter2D (Collider2D other)
    {
        // Check if the tag of the trigger collided with is Exit.
        if(other.tag == "Exit")
        {
            // Invoke the Restart function to start the next level with a delay of RestartLevelDelay (default 1 second).
            Invoke ("Restart", RestartLevelDelay);
            
            // Disable the player object since level is over.
            enabled = false;
        }
        
        // Check if the tag of the trigger collided with is Food.
        else if(other.tag == "Food")
        {
            // Add PointsPerFood to the players current food total.
            food += PointsPerFood;

            FoodText.text = "+" + PointsPerFood + " Food: " + food;

            SoundManager.Instance.RandomizeSfx(EatSound1, EatSound2);
            
            // Disable the food object the player collided with.
            other.gameObject.SetActive (false);
        }
        
        // Check if the tag of the trigger collided with is Soda.
        else if(other.tag == "Soda")
        {
            // Add PointsPerSoda to players food points total.
            food += PointsPerSoda;
            
            FoodText.text = "+" + PointsPerSoda + " Food: " + food;

            SoundManager.Instance.RandomizeSfx(DrinkSound1, DrinkSound2);
            
            // Disable the soda object the player collided with.
            other.gameObject.SetActive (false);
        }
    }
    
    
    // Restart reloads the scene when called.
    private void Restart ()
    {
        // Load the last scene loaded, in this case Main, the only scene in the game.
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
    }
    
    
    // LoseFood is called when an enemy attacks the player.
    // It takes a parameter loss which specifies how many points to lose.
    public void LoseFood (int loss)
    {
        // Set the trigger for the player animator to transition to the playerHit animation.
        animator.SetTrigger ("playerHit");
        
        // Subtract lost food points from the players total.
        food -= loss;

        FoodText.text = "-" + loss + " Food: " + food;
        
        // Check to see if game has ended.
        CheckIfGameOver ();
    }
    
    
    // CheckIfGameOver checks if the player is out of food points and if so, ends the game.
    private void CheckIfGameOver ()
    {
        // Check if food point total is less than or equal to zero.
        if (food <= 0) 
        {
            SoundManager.Instance.PlaySingle(GameOverSound);

            SoundManager.Instance.musicSource.Stop();

            // food = 100;

            // Call the GameOver function of GameManager.
            GameManager.Instance.GameOver ();
        }
    }
}