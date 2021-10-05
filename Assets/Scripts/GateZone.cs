using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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

	public float Multiplier { get => multiplier; set => setMult(value); }
	public int ZoneNum { get => zoneNum; }
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
