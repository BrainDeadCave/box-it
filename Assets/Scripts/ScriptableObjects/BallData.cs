using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New BallData", menuName = "Ball Data", order = 51)]
public class BallData : ScriptableObject
{
	[SerializeField]
	private float sizeMult;
	[SerializeField]
	private float gravityMult;
	[SerializeField]
	private float bouncyMult;
	[SerializeField]
	private float baseCost;
	[SerializeField]
	private float baseValue;
	[SerializeField]
	private Color color;

	public float SizeMult
	{
		get
		{
			return sizeMult;
		}
	}
	public float GravityMult
	{
		get
		{
			return gravityMult;
		}
	}
	public float BouncyMult
	{
		get
		{
			return bouncyMult;
		}
	}
	public float BaseCost
	{
		get
		{
			return baseCost;
		}
	}
	public Color Color
	{
		get
		{
			return color;
		}
	}
	public float BaseValue
	{
		get
		{
			return baseValue;
		}
	}
}