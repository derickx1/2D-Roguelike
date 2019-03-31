using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
// Allows us to use Lists. 
using System.Collections.Generic;		
// Allows us to use UI.
using UnityEngine.UI;					

public class GameManager : MonoBehaviour
{
    // Time to wait before starting level, in seconds.
    public float LevelStartDelay = 2f;					
    // Delay between each Player turn.	
    public float TurnDelay = 0.1f;		
    // Starting value for Player food points.					
    public int PlayerFoodPoints = 100;
    // Amount of coins Player has.
    public int Coins;
    // Static Instance of GameManager which allows it to be accessed by any other script.						
    public static GameManager Instance = null;
    // Boolean to check if it's players turn, hidden in inspector but public.				
    [HideInInspector] public bool PlayersTurn = true;		

    // Menu Image.			
    private GameObject mainMenu;
    // Shop Image.			
    private GameObject shopMenu;
    // Text to display current Coins.
    private Text coinText;
    // Text to display current Wall Damage level and cost.
    private Text upgradeDamageText;
    // Text to display current Food Preservation level and cost.
    private Text upgradeFoodText;
    // Text to display current level number.
    private Text levelText;						
    // Image to block out level as levels are being set up, background for levelText.			
    private GameObject levelImage;						
    // Store a reference to our BoardManager which will set up the level.	
    private BoardManager boardScript;					
    // Current level number, expressed in game as "Day 1".	
    private int level = 1;									
    // List of all Enemy units, used to issue them move commands.
    private List<Enemy> enemies;						
    // Boolean to check if enemies are moving.	
    private bool enemiesMoving;								
    // Boolean to check if we're setting up board, prevent Player from moving during setup.
    private bool freezePlayer = true;							

    // Awake is always called before any Start functions.
    void Awake ()
    {
        // Check if Instance already exists.
        if (Instance == null)
            // if not, set Instance to this.
            Instance = this;
        // If Instance already exists and it's not this.
        else if (Instance != this)
            // Then destroy this. This enforces our singleton pattern, meaning there can only ever be one Instance of a GameManager.
            Destroy(gameObject);

        // Sets this to not be destroyed when reloading scene.
        DontDestroyOnLoad(gameObject);
        // Assign enemies to a new List of Enemy objects.  
        enemies = new List<Enemy>();
        // Get a component reference to the attached BoardManager script.
        boardScript = GetComponent<BoardManager>();
        Coins = PlayerPrefs.GetInt("Coins", 0);
    }

    // This is called only once, and the parameter tell it to be called only after the scene was loaded
    // (otherwise, our Scene Load callback would be called the very first load, and we don't want that).
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static public void CallbackInitialization()
    {
        // Register the callback to be called everytime the scene is loaded.
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // This is called each time a scene is loaded.
    static private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        Instance.level++;
        Instance.InitGame();
    }

    // Get a reference to MainMenu
    void InitMenu()
    {
        // While freezePlayer is true the player can't move, prevent player from moving while menu is up.
        freezePlayer = true;
        
        // Get a reference to our image MainMenu by finding it by name.
        mainMenu = GameObject.Find("MainMenu");
    }

    // Get a reference to ShopMenu
    void InitShop()
    {        
        // Get a reference to our image MainMenu by finding it by name.
        shopMenu = GameObject.Find("ShopMenu");
        
        // Get a reference to our text UpgradeDamageText by finding it by name.
        upgradeDamageText = GameObject.Find("UpgradeDamageText").GetComponent<Text>();

        // Get a reference to our text UpgradeFoodText by finding it by name.
        upgradeFoodText = GameObject.Find("UpgradeFoodText").GetComponent<Text>();

        coinText = GameObject.Find("CoinText").GetComponent<Text>();

        SetUpgradeDamageText();
        SetUpgradeFoodText();
        SetCoinText();
    }

    // Covers ShopMenu.
    private void HideShopMenu()
    {
        // Enable the MainMenu gameObject to cover the ShopMenu gameObject.
        mainMenu.SetActive(true);
    }

    // Checks if Play button is pressed.
    public void PlayButton()
    {
        // Call the InitGame function to initialize the first level. 
        InitGame();
    }

    // Checks if Shop button is pressed.
    public void ShopButton()
    {
        // Get MainMenu reference.
        InitMenu();
        // Deactivate MainMenu. 
        mainMenu.SetActive(false);
        // Get ShopMenu references.
        InitShop();
    }

    // Checks if Exit button is pressed.
    public void ExitButton()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    private void SetUpgradeDamageText() 
    {
        if (PlayerPrefs.GetInt("WallDamage", 1) < 5)
        {
            upgradeDamageText.text = "Wall Damage Level " + PlayerPrefs.GetInt("WallDamage", 1) + "\nCost: 5";
        }
        else
        {
            upgradeDamageText.text = "Wall Damage Max Level";
        }
    }

    private void SetUpgradeFoodText()
    {
        if (PlayerPrefs.GetInt("FoodDamage", 5) > 1)
        {
            upgradeFoodText.text = "Food Preservation Level " + (6 - PlayerPrefs.GetInt("FoodDamage", 5)) + "\nCost: 5";
        }
        else
        {
            upgradeFoodText.text = "Food Preservation Max Level";
        }
    }

    private void SetCoinText() 
    {
        coinText.text = "\nCoins:" + PlayerPrefs.GetInt("Coins", 0);
    }

    // Checks if Upgrade Damage button is pressed.
    public void UpgradeDamageButton()
    {
        if ((PlayerPrefs.GetInt("Coins", 0) >= 5) && (PlayerPrefs.GetInt("WallDamage", 1) < 5))
        {
            DecreaseCoin();
            UpgradeWallDamage();
            SetUpgradeDamageText();
            SetCoinText();
        }
    }

    public void UpgradeFoodButton()
    {
        if ((PlayerPrefs.GetInt("Coins", 0) >= 5) && (PlayerPrefs.GetInt("FoodDamage", 5) > 1))
        {
            DecreaseCoin();
            UpgradeFoodPreservation();
            SetUpgradeFoodText();
            SetCoinText();
        }
    }

    // Checks if Back button is pressed.
    public void BackButton()
    {
        mainMenu.SetActive(true);
    }

    // Upgrades WallDamage.
    private void UpgradeWallDamage()
    {
        PlayerPrefs.SetInt("WallDamage", PlayerPrefs.GetInt("WallDamage", 1) + 1);
        GameObject.Find("Player").GetComponent<Player>().UpdateStats();
    }

    // Upgrades FoodPreservation.
    private void UpgradeFoodPreservation()
    {
        PlayerPrefs.SetInt("FoodDamage", PlayerPrefs.GetInt("FoodDamage", 5) - 1);
        GameObject.Find("Player").GetComponent<Player>().UpdateStats();
    }

    // Subtracts Coins by 5
    private void DecreaseCoin()
    {
        PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins", 0) - 5);
    }

    // Increments Coins.
    private void IncrementCoin(int level)
    {
        if (level > 1) 
        {
            PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins", 0) + 1);
        }
    }

    // Initializes the game for each level.
    void InitGame()
    {
        // Increment Coin count if needed.
        IncrementCoin(level);

        // Get a reference to MainMenu.
        InitMenu();

        // Disable the MainMenu gameObject.
        mainMenu.SetActive(false);

        // Get a reference to ShopMenu.
        InitShop();

        // Disable the ShopMenu gameObject.
        shopMenu.SetActive(false);

        // While freezePlayer is true the player can't move, prevent player from moving while title card is up.
        freezePlayer = true;
        
        // Get a reference to our image LevelImage by finding it by name.
        levelImage = GameObject.Find("LevelImage");
        
        // Get a reference to our text LevelText's text component by finding it by name and calling GetComponent.
        levelText = GameObject.Find("LevelText").GetComponent<Text>();
        
        // Set the text of levelText to the string "Day" and append the current level number.
        levelText.text = "Day " + level;
        
        // Set levelImage to active blocking player's view of the game board during setup.
        levelImage.SetActive(true);
        
        // Call the HideLevelImage function with a delay in seconds of LevelStartDelay.
        Invoke("HideLevelImage", LevelStartDelay);
        
        // Clear any Enemy objects in our List to prepare for next level.
        enemies.Clear();
        
        // Call the SetupScene function of the BoardManager script, pass it current level number.
        boardScript.SetupScene(level);
        
    }

    // Disables LevelImage and unlocks player movement.
    private void HideLevelImage()
    {
        // Disable the levelImage gameObject.
        levelImage.SetActive(false);
			
        // Set freezePlayer to false allowing player to move again.
        freezePlayer = false;
    }

    // GameOver is called when the player reaches 0 food points.
    public void GameOver()
    {
        // Set levelText to display number of levels passed and game over message.
        levelText.text = "After " + level + " days, you starved.";
        
        // Enable black background image gameObject.
        levelImage.SetActive(true);
        
        // Disable this GameManager.
        enabled = false;

        // freezePlayer = true;
        // PlayerFoodPoints = 100;
        // level = 0;
        // shopMenu.SetActive(true);
        // mainMenu.SetActive(true);
    }

    // Update is called every frame.
    void Update()
    {
        // Check that PlayersTurn or enemiesMoving or freezePlayer are not currently true.
        if(PlayersTurn || enemiesMoving || freezePlayer)
            
            // If any of these are true, return and do not start MoveEnemies.
            return;
        
        // Start moving enemies.
        StartCoroutine (MoveEnemies ());
    }

    // Call this to add the passed in Enemy to the List of Enemy objects.
    public void AddEnemyToList(Enemy script)
    {
        // Add Enemy to List enemies.
        enemies.Add(script);
    }

    // Coroutine to move enemies in sequence.
    IEnumerator MoveEnemies()
    {
        // While enemiesMoving is true player is unable to move.
        enemiesMoving = true;
        
        // Wait for TurnDelay seconds, defaults to .1 (100 ms).
        yield return new WaitForSeconds(TurnDelay);
        
        // If there are no enemies spawned (IE in first level).
        if (enemies.Count == 0) 
        {
            // Wait for TurnDelay seconds between moves, replaces delay caused by enemies moving when there are none.
            yield return new WaitForSeconds(TurnDelay);
        }
        
        // Loop through List of Enemy objects.
        for (var i = 0; i < enemies.Count; i++)
        {
            // Call the MoveEnemy function of Enemy at index i in the enemies List.
            enemies[i].MoveEnemy ();
            
            // Wait for Enemy's moveTime before moving next Enemy, 
            yield return new WaitForSeconds(enemies[i].moveTime);
        }
        // Once Enemies are done moving, set PlayersTurn to true so player can move.
        PlayersTurn = true;
        
        // Enemies are done moving, set enemiesMoving to false.
        enemiesMoving = false;
    }
}
