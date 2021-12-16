using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Data that balls are created with
/// </summary>
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

	/// <summary>
	/// How large the ball should be, multiplicative.
	/// </summary>
	public float SizeMult
	{
		get
		{
			return sizeMult;
		}
	}
	/// <summary>
	/// How much the ball is affected by gravity (how "heavy" it is), multiplicative.
	/// </summary>
	public float GravityMult
	{
		get
		{
			return gravityMult;
		}
	}
	/// <summary>
	/// How bouncy the balls are, multiplicative.
	/// </summary>
	public float BouncyMult
	{
		get
		{
			return bouncyMult;
		}
	}
	/// <summary>
	/// How expensive the first upgrade tier for this ball is
	/// </summary>
	public float BaseCost
	{
		get
		{
			return baseCost;
		}
	}
	/// <summary>
	/// The color of the ball 
	/// </summary>
	public Color Color
	{
		get
		{
			return color;
		}
	}
	/// <summary>
	/// How much money this ball gives before other factors are taken into account
	/// </summary>
	public float BaseValue
	{
		get
		{
			return baseValue;
		}
	}
}