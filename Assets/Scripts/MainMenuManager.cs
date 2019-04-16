using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MainMenuManager : Singleton<MainMenuManager>
{
    [SerializeField]
    private GameManager gameManager;
    [SerializeField]
    private GameObject mainMenu;
	[SerializeField]
    private GameObject shopMenu;
	[SerializeField]
    private ShopMenuManager shopScript;
    [SerializeField]
    private Button playButton;
    [SerializeField]
    private Button shopButton;
    [SerializeField]
    private Button exitButton;

    private void Start()
    {
        Assert.IsNotNull(gameManager);
        Assert.IsNotNull(mainMenu);
        Assert.IsNotNull(shopMenu);

        Assert.IsNotNull(shopScript);

        Assert.IsNotNull(playButton);
        Assert.IsNotNull(shopButton);
        Assert.IsNotNull(exitButton);

        playButton.onClick.AddListener(Play);
        shopButton.onClick.AddListener(Shop);
        exitButton.onClick.AddListener(Exit);
    }

    private void OnDestroy()
    {
        playButton.onClick.RemoveListener(Play);
        shopButton.onClick.RemoveListener(Shop);
        exitButton.onClick.RemoveListener(Exit);
    }

    private void Play()
    {
        mainMenu.SetActive(false);
        shopMenu.SetActive(false);
		gameManager.InitGame();
    }

    private void Shop()
    {
		shopScript.UpdateText();
        mainMenu.SetActive(false);
    }

    private void Exit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
