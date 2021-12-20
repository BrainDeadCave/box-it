using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.SceneManagement;

/*! \mainpage Box'it index page
 *
 * \section intro_sec Introduction
 *
 * Welcome to the Box'it documentation!
 * To get started, look at the classes and files in the menu above!
 *
 */

/// <summary>
/// Controls overarching game logic and cross-component data flows.
/// </summary>
public class GameManager : MonoBehaviour
{
	/// <summary>
	/// The singleton instance of the GameManager
	/// </summary>
	public static GameManager Instance;
    [SerializeField]
	private Camera mainCam;
	[SerializeField]
	private GameObject errorPanel;
	[SerializeField]
	private GameObject loadingPanel;
	[SerializeField]
	private GameObject connectErrorPanel;
	[SerializeField]
	private GameObject viewBlocker;
	//These bools are to ensure we received all we needed to from the network manager before letting the user in
	private bool gotCoins;
	private bool gotSpeedUpgrade;
	private bool gotValueUpgrade;

	[SerializeField]
	private UpgradeButton valueButton;
	private int valueUpgradeLevel;
	[SerializeField]
	private UpgradeButton speedButton;
	private int speedUpgradeLevel;
	[SerializeField]
	private TextMeshProUGUI coinsTMP;
	[SerializeField]
	private TextMeshProUGUI userIDTMP;

	private int coins;

	[Serializable]
	private struct SpeedUpgrade
	{
		public int cost;
		public float secRemoval;
	}
	[SerializeField]
	private List<SpeedUpgrade> speedUpgrades;

	[Serializable]
	private struct ValueUpgrade
	{
		public int cost;
		public float valueIncrease;
	}
	[SerializeField]
	private List<ValueUpgrade> valueUpgrades;

	[SerializeField]
	private List<Spawner> spawners;

	private float timer = 0f;
	private bool inGame = false;

	private bool setCoinsNextFrame, setSpeedNextFrame, setValueNextFrame = false;
	private string speedUp, coinSet;
	private int upgradeLevel;

	/// <summary>
	/// The main camera being used
	/// </summary>
	public Camera MainCam { get => mainCam; set => mainCam = value; }

	// Start is called before the first frame update
	void Start()
    {
		if (!Instance)
		{
            Instance = this;
			RunStart();
		}
		else
		{
			Debug.LogWarning("Only one GameManager should exist! You should remove this instance of the component!");
			this.enabled = false;
			return;
		}
	}

	private void RunStart()
	{
		//try to get all data
		NetworkManager.Instance.GetCoins();
		NetworkManager.Instance.GetSpeedUpgrade();
		NetworkManager.Instance.GetValueUpgrade();
	}

	public void Update()
	{
		//if not, heck out
		if (!inGame)
		{
			timer += Time.deltaTime;

			if(gotCoins && setCoinsNextFrame)
			{
				SetCoins(coinSet);
				setCoinsNextFrame = false;
			}

			if (gotSpeedUpgrade && setSpeedNextFrame)
			{
				SetSpeedUpgradeLevel(speedUp);
				setSpeedNextFrame = false;
			}

			if (gotValueUpgrade && setValueNextFrame)
			{
				SetValueUpgradeLevel(upgradeLevel);
				setValueNextFrame = false;
			}

			if (gotCoins && gotSpeedUpgrade && gotValueUpgrade)
			{
				HideViewBlocker();
				CloseLoadingWindow();
				userIDTMP.text = $"User ID:{NetworkManager.Instance.userID}";
				inGame = true;
				Debug.Log("we're goin in hot");
			}
			else if (timer > 15.0f)
			{
				CloseLoadingWindow();
				ConnectionErrorWindow();
				inGame = true;
			}
		}

	}

	private void HideViewBlocker()
	{
		viewBlocker.SetActive(false);
	}

	/// <summary>
	/// Show the error panel with an error message
	/// </summary>
	public void Error(string message)
	{
		errorPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = message;
		errorPanel.SetActive(true);
		CloseLoadingWindow();
	}
	/// <summary>
	/// Show the loading panel with a message
	/// </summary>
	public void LoadingWindow(string message)
	{
		if (!loadingPanel.activeSelf)
		{
			loadingPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = message;
			loadingPanel.SetActive(true);
		}
		else
		{
			Debug.LogWarning($"Tried to open loading window for message \"{message}\", but it is already open for message \"{loadingPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text}\"!");
		}
	}
	/// <summary>
	/// Close the loading window
	/// </summary>
	public void CloseLoadingWindow()
	{
		if (!loadingPanel.activeSelf)
		{
			return;
		}
		ClickToClosePanel click = loadingPanel.GetComponent<ClickToClosePanel>();
		if (click)
		{
			click.ClosePanel();
		}
		else
		{
			loadingPanel.SetActive(false);
		}
	}

	/// <summary>
	/// Show the connection error panel
	/// </summary>
	public void ConnectionErrorWindow()
	{
		if (!connectErrorPanel.activeSelf)
		{
			connectErrorPanel.SetActive(true);
		}
		else
		{
			Debug.LogWarning("Tried to open connection error window, but it is already open! How did I mess this up THIS much ;w;");
		}
	}

	public void SetSpeedUpgradeLevel(string speedup)
	{
		int upgradeLevel = int.Parse(speedup);
		if (upgradeLevel < speedUpgrades.Count - 1)
		{
			//remove coins of old cost
			//SetCoins((coins - speedUpgrades[upgradeLevel + 1].cost).ToString());
			//calculate and set cost
			int newCost = speedUpgrades[upgradeLevel + 1].cost;
			//update button
			speedButton.SetCost(newCost);
			speedButton.SetEffect($"-{speedUpgrades[upgradeLevel + 1].secRemoval} sec. per ball");
			//upgrade spawners
			foreach (Spawner spawner in spawners)
			{
				spawner.SetSpawnTime(speedUpgrades[upgradeLevel].secRemoval);
			}
			speedUpgradeLevel = upgradeLevel;

			if (speedUpgradeLevel == speedUpgrades.Count - 1)
			{
				speedButton.SetBackground(UpgradeButton.UpgradeBackgroundType.Maxed);
			}
			else if (coins < speedUpgrades[speedUpgradeLevel+1].cost)
			{
				speedButton.SetBackground(UpgradeButton.UpgradeBackgroundType.Not_Purchaseable);
			}
			else
			{
				speedButton.SetBackground(UpgradeButton.UpgradeBackgroundType.Purchaseable);
			}
		}
		else if (upgradeLevel == speedUpgrades.Count - 1)
		{
			//gray out button and say it's maxed out
			speedButton.SetCost(-1);
			speedButton.SetEffect("Maxed out!");
			speedButton.SetBackground(UpgradeButton.UpgradeBackgroundType.Maxed);
		}
	}

	public void SetNetworkSpeedUpgradeLevel(string speedup)
	{
		Debug.LogWarning($"Setting speed level to {speedup}");
		speedUp = speedup;
		setSpeedNextFrame = true;
		gotSpeedUpgrade = true;
	}

	public void SetCoins(string coinsString)
	{
		int coinNum = int.Parse(coinsString);
		coins = coinNum;
		//update money text
		coinsTMP.text = coins.ToString();
		//update button backgrounds
		if(speedUpgradeLevel == speedUpgrades.Count - 1)
		{
			speedButton.SetBackground(UpgradeButton.UpgradeBackgroundType.Maxed);
		}
		else if(coinNum < speedUpgrades[speedUpgradeLevel+1].cost)
		{
			speedButton.SetBackground(UpgradeButton.UpgradeBackgroundType.Not_Purchaseable);
		}
		else
		{
			speedButton.SetBackground(UpgradeButton.UpgradeBackgroundType.Purchaseable);
		}

		if (valueUpgradeLevel == valueUpgrades.Count - 1)
		{
			valueButton.SetBackground(UpgradeButton.UpgradeBackgroundType.Maxed);
		}
		else if (coinNum < valueUpgrades[valueUpgradeLevel+1].cost)
		{
			valueButton.SetBackground(UpgradeButton.UpgradeBackgroundType.Not_Purchaseable);
		}
		else
		{
			valueButton.SetBackground(UpgradeButton.UpgradeBackgroundType.Purchaseable);
		}
	}


	public void SetCoins(int newCoins)
	{
		int coinNum = newCoins;

		coins = coinNum;
		//update money text
		coinsTMP.text = coins.ToString();
		//update button backgrounds
		if (speedUpgradeLevel == speedUpgrades.Count - 1)
		{
			speedButton.SetBackground(UpgradeButton.UpgradeBackgroundType.Maxed);
		}
		else if (coinNum < speedUpgrades[speedUpgradeLevel+1].cost)
		{
			speedButton.SetBackground(UpgradeButton.UpgradeBackgroundType.Not_Purchaseable);
		}
		else
		{
			speedButton.SetBackground(UpgradeButton.UpgradeBackgroundType.Purchaseable);
		}

		if (valueUpgradeLevel == valueUpgrades.Count - 1)
		{
			valueButton.SetBackground(UpgradeButton.UpgradeBackgroundType.Maxed);
		}
		else if (coinNum < valueUpgrades[valueUpgradeLevel+1].cost)
		{
			valueButton.SetBackground(UpgradeButton.UpgradeBackgroundType.Not_Purchaseable);
		}
		else
		{
			valueButton.SetBackground(UpgradeButton.UpgradeBackgroundType.Purchaseable);
		}
	}

	public void AddCoins(int amount)
	{
		Debug.Log($"Adding {amount}");
		SetCoins(coins + amount);
	}

	public void SetNetworkCoins(string coin)
	{
		setCoinsNextFrame = true;
		Debug.LogWarning($"Setting coins to {coin}");
		gotCoins = true;
		coinSet = coin;
	}

	public void SetValueUpgradeLevel(int upgradeLevel)
	{
		if (upgradeLevel < valueUpgrades.Count-1)
		{
			//remove coins of old cost
			//SetCoins((coins - valueUpgrades[upgradeLevel + 1].cost).ToString());
			//calculate and set cost
			int newCost = valueUpgrades[upgradeLevel + 1].cost;
			//update button
			valueButton.SetCost(newCost);
			valueButton.SetEffect($"+${valueUpgrades[upgradeLevel + 1].valueIncrease}");
			//upgrade spawners
			foreach (Spawner spawner in spawners)
			{
				spawner.SetValueIncrease((int)valueUpgrades[upgradeLevel].valueIncrease);
			}
			valueUpgradeLevel = upgradeLevel;

			Debug.LogWarning(valueUpgradeLevel);
			Debug.LogWarning(coins);
			Debug.LogWarning(valueUpgrades[valueUpgradeLevel].cost);
			if (valueUpgradeLevel == valueUpgrades.Count - 1)
			{
				valueButton.SetBackground(UpgradeButton.UpgradeBackgroundType.Maxed);
			}
			else if (coins < valueUpgrades[valueUpgradeLevel+1].cost)
			{
				valueButton.SetBackground(UpgradeButton.UpgradeBackgroundType.Not_Purchaseable);
			}
			else
			{
				valueButton.SetBackground(UpgradeButton.UpgradeBackgroundType.Purchaseable);
			}
		}
		else if(upgradeLevel == valueUpgrades.Count - 1)
		{
			//gray out button and say it's maxed out
			valueButton.SetCost(-1);
			valueButton.SetEffect("Maxed out!");
			valueButton.SetBackground(UpgradeButton.UpgradeBackgroundType.Maxed);
		}
	}

	public void SetNetworkValueUpgradeLevel(string valuelevel)
	{
		Debug.LogWarning($"Setting value upgrade level to {valuelevel}");
		setValueNextFrame = true;
		upgradeLevel = int.Parse(valuelevel);
		gotValueUpgrade = true;
	}


	/// <summary>
	/// Reset the main menu scene
	/// </summary>
	public void ResetGame()
	{
		SceneManager.LoadScene(0);
	}
}
