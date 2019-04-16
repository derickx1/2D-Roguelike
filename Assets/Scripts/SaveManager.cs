using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SaveManager
{
    private static class Keys
    {
        public const string CoinCount = "Coins";
        public const string WallDamage = "WallDamage";
        public const string FoodDamage = "FoodDamage";
    }

    private static class DefaultValues
    {
        public const int CoinCount = 0;
        public const int WallDamage = 1;
        public const int FoodDamage = 5;
    }

    public static int Coins => PlayerPrefs.GetInt(Keys.CoinCount, DefaultValues.CoinCount);
    public static int WallDamage => PlayerPrefs.GetInt(Keys.WallDamage, DefaultValues.WallDamage);
    public static int FoodDamage => PlayerPrefs.GetInt(Keys.FoodDamage, DefaultValues.FoodDamage);

    public static void Save(int Coins, int WallDamage, int FoodDamage) 
    {
        PlayerPrefs.SetInt(Keys.CoinCount, Coins);
        PlayerPrefs.SetInt(Keys.WallDamage, WallDamage);
        PlayerPrefs.SetInt(Keys.FoodDamage, FoodDamage);
    }
}
