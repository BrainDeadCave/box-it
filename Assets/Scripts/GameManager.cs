using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

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
		//if not, heck out
		float timer = 0f;
		while (timer < 5.0f)
		{
			timer += Time.deltaTime;
			if (gotCoins && gotSpeedUpgrade && gotValueUpgrade)
			{
				break;
			}
		}

		if (gotCoins && gotSpeedUpgrade && gotValueUpgrade)
		{
			HideViewBlocker();
			CloseLoadingWindow();
			userIDTMP.text = $"User ID:{NetworkManager.Instance.userID}";
		}
		else
		{
			CloseLoadingWindow();
			ConnectionErrorWindow();
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
			SetCoins((coins - speedUpgrades[upgradeLevel + 1].cost).ToString());
			//calculate and set new cost
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
		SetSpeedUpgradeLevel(speedup);
		gotSpeedUpgrade = true;
	}

	public void SetCoins(string coins)
	{
		int coinNum = int.Parse(coins);
		//update money text
		coinsTMP.text = coins.ToString();
		//update button backgrounds
		if(speedUpgradeLevel == speedUpgrades.Count - 1)
		{
			speedButton.SetBackground(UpgradeButton.UpgradeBackgroundType.Maxed);
		}
		else if(coinNum < speedUpgrades[speedUpgradeLevel].cost)
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
		else if (coinNum < valueUpgrades[valueUpgradeLevel].cost)
		{
			valueButton.SetBackground(UpgradeButton.UpgradeBackgroundType.Not_Purchaseable);
		}
		else
		{
			valueButton.SetBackground(UpgradeButton.UpgradeBackgroundType.Purchaseable);
		}
	}

	public void SetNetworkCoins(string coin)
	{
		SetCoins(coin);
		gotCoins = true;
	}

	public void SetValueUpgradeLevel(int upgradeLevel)
	{
		if (upgradeLevel < valueUpgrades.Count-1)
		{
			//remove coins of old cost
			SetCoins((coins - valueUpgrades[upgradeLevel + 1].cost).ToString());
			//calculate and set new cost
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
		int upgradeLevel = int.Parse(valuelevel);
		SetValueUpgradeLevel(upgradeLevel);
		gotValueUpgrade = true;
	}
}
