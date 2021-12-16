using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The falling box
/// </summary>
public class Ball : MonoBehaviour
{
    [SerializeField]
    private BallData ballData;
    private float value;
    [SerializeField]
    private bool isVIP;
    private int farthestZone = 0;

    /// <summary>
    /// The base data the ball in constructed with
    /// </summary>
    public BallData BallData { get => ballData; }

	// Start is called before the first frame update
	void Start()
    {
        transform.localScale = new Vector3(ballData.SizeMult, ballData.SizeMult, ballData.SizeMult);
        GetComponent<Rigidbody2D>().gravityScale = ballData.GravityMult;
		if (ballData.BouncyMult != 1.0f)
        {
            PhysicsMaterial2D oldMat = GetComponent<Rigidbody2D>().sharedMaterial;
			PhysicsMaterial2D newMat = new PhysicsMaterial2D
			{
				friction = oldMat.friction,
				bounciness = oldMat.bounciness * ballData.BouncyMult
            };
			GetComponent<Rigidbody2D>().sharedMaterial = newMat;
        }
        value = ballData.BaseValue; //Gotta do stuff with the current value mult
        GetComponent<SpriteRenderer>().color = ballData.Color;
		if (isVIP)
		{
            transform.GetChild(0).gameObject.SetActive(true);
		}
    }

	private void OnTriggerEnter2D(Collider2D collision)
	{
		GateZone zone = collision.gameObject.GetComponent<GateZone>();
		if (!zone)
		{
            return;
		}
		if (!zone.CanPassThrough)
		{
            Debug.LogFormat("Passed gate with multiplier {0}, would credit ${1}. Impassible gate, now destroying.", zone.Multiplier, value * zone.Multiplier);
            Destroy(gameObject);
            return;
        }
        else if(zone.ZoneNum > farthestZone)
		{
            Debug.LogFormat("Passed gate with multiplier {0}, would credit ${1}", zone.Multiplier, value*zone.Multiplier);
		}
        farthestZone = zone.ZoneNum;
	}
}
