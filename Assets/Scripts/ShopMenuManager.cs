using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ShopMenuManager : MonoBehaviour
{
    [SerializeField]
    private GameObject mainMenu;
    [SerializeField]
    private Button wallDamageButton;
    [SerializeField]
    private Button foodPreservationButton;
    [SerializeField]
    private Button backButton;
	[SerializeField]
    private Text coinText;
    [SerializeField]
    private Text wallDamageText;
    [SerializeField]
    private Text foodPreservationText;
	[SerializeField]
	private int maxWallDamage = 5;
    [SerializeField]
	private int minFoodDamage = 1;
    [SerializeField]
	private int cost = 5;
    private bool MaxWallDamage = false;
	private bool MaxFoodPreservation = false;

	private static class TextStrings
    {
		public static string CoinText => $"Coins: {Player.Coins}";
        public static string WallDamageLevelText => $"Wall Damage Level {Player.WallDamage}\nCost: 5";
		public static string WallDamageMaxLevelText => $"Wall Damage Max Level";
        public static string FoodPreservationLevelText => $"Food Preservation Level {6 - Player.FoodDamage}\nCost: 5";
		public static string FoodPreservationMaxLevelText => $"Food Preservation Max Level";
	}

    private void Start()
    {
        Assert.IsNotNull(mainMenu);

        Assert.IsNotNull(wallDamageButton);
        Assert.IsNotNull(foodPreservationButton);
        Assert.IsNotNull(backButton);

		Assert.IsNotNull(wallDamageText);
		Assert.IsNotNull(foodPreservationText);

		wallDamageButton.onClick.AddListener(WallDamage);
		foodPreservationButton.onClick.AddListener(FoodPreservation);
		backButton.onClick.AddListener(Back);
		
		CheckMax();
		UpdateText();
    }

	private void WallDamage()
	{
		if (MaxWallDamage || Player.Coins < cost)
		{
			return;
		}

		Player.WallDamage++;
		Player.Coins -= cost;
		
		coinText.text = TextStrings.CoinText;
		CheckMax();
		UpdateText();
	}

	private void FoodPreservation()
	{
		if (MaxFoodPreservation || Player.Coins < cost)
		{
			return;
		}

		Player.FoodDamage--;
		Player.Coins -= cost;
		
		coinText.text = TextStrings.CoinText;
		CheckMax();
		UpdateText();
	}

    private void Back()
    {
		CheckMax();
        mainMenu.SetActive(true);
    }

	private void CheckMax()
	{
		if (Player.WallDamage >= maxWallDamage) 
		{
			MaxWallDamage = true;
		}
		if (Player.FoodDamage <= minFoodDamage)
		{
			MaxFoodPreservation = true;
		}
	}

	public void UpdateText()
	{
		coinText.text = TextStrings.CoinText;
		if (!MaxWallDamage)
		{
			wallDamageText.text = TextStrings.WallDamageLevelText;
		}
		else
		{
			wallDamageText.text = TextStrings.WallDamageMaxLevelText;
		}
		if (!MaxFoodPreservation)
		{
			foodPreservationText.text = TextStrings.FoodPreservationLevelText;
		}
		else
		{
			foodPreservationText.text = TextStrings.FoodPreservationMaxLevelText;
		}
	}
}
