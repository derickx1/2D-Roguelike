using UnityEngine;
using System;
using System.Collections.Generic;   
using Random = UnityEngine.Random;      
    
public class BoardManager : MonoBehaviour
{
    [Serializable]
    private class Count
    {
        public int minimum;          
        public int maximum;             
        
        public Count (int min, int max)
        {
            minimum = min;
            maximum = max;
        }
    }

    [SerializeField]
    private int columns = 8;
    [SerializeField]
    private int rows = 8;
    [SerializeField]
    private Count wallCount = new Count(5, 9);
    [SerializeField]
    private Count foodCount = new Count(1, 5);
    [SerializeField]
    private GameObject exit;
    // Arrays holding the variants of each type of tile.
    [SerializeField]
    private GameObject[] floorTiles;
    [SerializeField]
    private GameObject[] wallTiles;
    [SerializeField]
    private GameObject[] foodTiles;
    [SerializeField]
    private Enemy[] enemyTiles;
    [SerializeField]
    private int playerDamage = 5;
    [SerializeField]
    private GameObject[] outerWallTiles;
    [SerializeField]
    private Transform target;
    private Transform boardHolder;
    [SerializeField]
    private LayerMask blockingLayer;
    // A list of possible locations to place tiles.
    private List<Vector3> gridPositions = new List<Vector3>();
    private const string boardName = "Board";


    private void InitialiseList ()
    {
        gridPositions.Clear ();
        for(int x = 1; x < columns-1; x++)
        {
            for(int y = 1; y < rows-1; y++)
            {
                gridPositions.Add (new Vector3(x, y, 0f));
            }
        }
    }
        
    private void BoardSetup ()
    {
        boardHolder = new GameObject (boardName).transform;
        
        // Loop along x and y axis, starting from -1 place floor or outerwall edge tiles.
        for(int x = -1; x < columns + 1; x++)
        {
            for(int y = -1; y < rows + 1; y++)
            {
                GameObject toInstantiate = floorTiles[Random.Range (0,floorTiles.Length)];

                if( x < 0 || x >= columns || y < 0 || y >= rows)
                {
                    toInstantiate = outerWallTiles[Random.Range(0, outerWallTiles.Length)];
                }
                GameObject instance = 
                    Instantiate (toInstantiate, new Vector3 (x, y, 0f), Quaternion.identity) as GameObject;                
                instance.transform.SetParent (boardHolder);
            }
        }
    }
    
    private Vector3 RandomPosition ()
    {
        int randomIndex = Random.Range (0, gridPositions.Count);        
        Vector3 randomPosition = gridPositions[randomIndex];        
        gridPositions.RemoveAt (randomIndex);
        return randomPosition;
    }
        
    private void LayoutObjectAtRandom (GameObject[] tileArray, int minimum, int maximum)
    {
        int objectCount = Random.Range (minimum, maximum+1);
        
        for(int i = 0; i < objectCount; i++)
        {
            Vector3 randomPosition = RandomPosition();
            GameObject tileChoice = tileArray[Random.Range (0, tileArray.Length)];
			GameObject instance = 
				Instantiate(tileChoice, randomPosition, Quaternion.identity);
			instance.transform.SetParent (boardHolder);
        }
    }

    private void LayoutObjectAtRandom (Enemy[] tileArray, int minimum, int maximum)
    {
        int objectCount = Random.Range (minimum, maximum+1);
        
        for(int i = 0; i < objectCount; i++)
        {
            Vector3 randomPosition = RandomPosition();
            Enemy tileChoice = tileArray[Random.Range (0, tileArray.Length)];
		    Enemy enemy = Instantiate<Enemy>(tileChoice, randomPosition, Quaternion.identity);
		    enemy.transform.SetParent(boardHolder);
		    enemy.Target = target;
            enemy.PlayerDamage = playerDamage;
            enemy.blockingLayer = blockingLayer;
        }
    }
        
    public void SetupScene (int level)
    {
        BoardSetup ();        
        InitialiseList ();
        LayoutObjectAtRandom (wallTiles, wallCount.minimum, wallCount.maximum);
        LayoutObjectAtRandom (foodTiles, foodCount.minimum, foodCount.maximum);        
        int enemyCount = (int)Mathf.Log(level, 2f);        
        LayoutObjectAtRandom (enemyTiles, enemyCount, enemyCount);        
        GameObject instance =
			Instantiate (exit, new Vector3 (columns - 1, rows - 1, 0f), Quaternion.identity);
		instance.transform.SetParent (boardHolder);
    }

	public void Reset()
	{
		Destroy(boardHolder.gameObject);
	}
}