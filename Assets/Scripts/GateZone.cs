using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// The area boxes fall through to provide money to the player. Can be upgraded.
/// </summary>
public class GateZone : MonoBehaviour
{
	[SerializeField]
    private float multiplier;
	[SerializeField]
	private int zoneNum;
	[SerializeField]
	private bool canPassThrough;
	[SerializeField]
	private TextMeshProUGUI multiplierText;

	/// <summary>
	/// Value multiplier for the gate
	/// </summary>
	public float Multiplier { get => multiplier; set => setMult(value); }
	/// <summary>
	/// Which zone this is, sequential from top to bottom starting at 0
	/// </summary>
	public int ZoneNum { get => zoneNum; }
	/// <summary>
	/// Can balls pass through this gate? If not, they get deleted after evaluating and granting their value
	/// </summary>
	public bool CanPassThrough { get => canPassThrough; set => setPassthrough(value); }

	void setMult(float val)
	{
		multiplier = val;
		multiplierText.text = multiplier.ToString();
	}

	void setPassthrough(bool val)
	{
		canPassThrough = val;
		gameObject.GetComponent<SpriteRenderer>().color = canPassThrough ? Color.green : Color.red;
	}

	private void Start()
	{
		setMult(multiplier);
		setPassthrough(canPassThrough);
	}
}
