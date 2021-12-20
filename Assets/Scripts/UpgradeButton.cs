using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeButton : MonoBehaviour
{
	[SerializeField]
	private Image background;
	[SerializeField]
	private TextMeshProUGUI cost;
	[SerializeField]
	private TextMeshProUGUI effect;


	[SerializeField]
	private Color purchaseableColor;
	[SerializeField]
	private Color unpurchaseableColor;
	[SerializeField]
	private Color maxedColor;

	public enum UpgradeBackgroundType
	{
		Purchaseable,
		Not_Purchaseable,
		Maxed
	}

    public void SetCost(int newCost)
	{
		if (newCost >= 0)
		{
			cost.text = newCost.ToString("C");
		}
		else
		{
			cost.text = "";
		}
	}
	public void SetEffect(string newEffect)
	{
		effect.text = newEffect;
	}
	public void SetBackground(UpgradeBackgroundType backgroundType)
	{
		switch (backgroundType)
		{
			case UpgradeBackgroundType.Purchaseable:
				background.color = purchaseableColor;
				break;
			case UpgradeBackgroundType.Not_Purchaseable:
				background.color = unpurchaseableColor;
				break;
			case UpgradeBackgroundType.Maxed:
				background.color = maxedColor;
				break;
			default:
				break;
		}
	}
}
