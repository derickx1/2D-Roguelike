using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using System.Collections.Generic;		
using UnityEngine.UI;					

public class GameManager : Singleton<GameManager>
{
    private int level = 1;	
    [SerializeField]
    private float levelStartDelay = 2f;				
    [SerializeField]
    private float turnDelay = 0.1f;						
	[SerializeField]
    private Text levelText;						
	[SerializeField]
    private GameObject levelImage;
	[SerializeField]
    private GameObject mainMenu;
	[SerializeField]
    private GameObject shopMenu;
	[SerializeField]
    private BoardManager boardScript;					
    // List of all Enemy units, used to issue them move commands.
    private List<Enemy> enemies;						
    private bool enemiesMoving;								
		
    [HideInInspector] 
    public bool PlayersTurn = true;
	[HideInInspector] 
    public bool FreezePlayer = true;	

    private void Awake ()
    { 
        Assert.IsNotNull(levelText);

        Assert.IsNotNull(levelImage);
        Assert.IsNotNull(mainMenu);
        Assert.IsNotNull(shopMenu);

        Assert.IsNotNull(boardScript);

        enemies = new List<Enemy>();
    }

    public void BeatLevel()
    {
        level++;
		enemies.Clear();
		boardScript.Reset();
        InitGame();
    }

    public void InitGame()
    {
        FreezePlayer = true;
        levelText.text = $"Day {level}";
        levelImage.SetActive(true);
        StartCoroutine(HideLevelImage());
        enemies.Clear();
        boardScript.SetupScene(level);
        
    }

    private IEnumerator HideLevelImage()
    {
		yield return new WaitForSeconds(levelStartDelay);
        levelImage.SetActive(false);
        FreezePlayer = false;
    }

    public void GameOver()
    {
		FreezePlayer = true;
		enemies.Clear();
		boardScript.Reset();
        StartCoroutine(EndScreen());
		level = 1;
    }

	private IEnumerator EndScreen()
	{
        levelText.text = $"After {level} days, you starved.";
        levelImage.SetActive(true);
		yield return new WaitForSeconds(levelStartDelay);
		mainMenu.SetActive(true);
		shopMenu.SetActive(true);
	}

    private void Update()
    {
        if(PlayersTurn || enemiesMoving || FreezePlayer)
            {
            return;
			}
        
        StartCoroutine (MoveEnemies ());
    }

    public void AddEnemyToList(Enemy script)
    {
        enemies.Add(script);
    }

    // Coroutine to move enemies in sequence.
    private IEnumerator MoveEnemies()
    {
        enemiesMoving = true;
        yield return new WaitForSeconds(turnDelay);
        
        if (enemies.Count == 0) 
        {
            yield return new WaitForSeconds(turnDelay);
        }
        
        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].MoveEnemy ();    
            yield return new WaitForSeconds(enemies[i].MoveTime);
        }
        PlayersTurn = true;
        enemiesMoving = false;
    }
}
