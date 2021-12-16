using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
/// <summary>
/// Handles behavior of register button press
/// </summary>
public class RegisterButton : MonoBehaviour
{
	[SerializeField]
	private TMP_InputField usernameTMPro;
	[SerializeField]
	private TMP_InputField passwordTMPro;
	/// <summary>
	/// Tells NetworkManager Instance to send a registration request with supplied username and password, if both are filled
	/// </summary>
	public void Register()
	{
		if (usernameTMPro.text.Length > 0 && passwordTMPro.text.Length > 0)
		{
			NetworkManager.Instance.Register(usernameTMPro.text, passwordTMPro.text);
			Debug.Log($"Attempting Register. UN: {usernameTMPro.text}  PW: {passwordTMPro.text}");
			//Show spinny boi
			MainMenuController.Instance.LoadingWindow("Registering...");
		}
		else
		{
			//Show error
			MainMenuController.Instance.Error("Please fill in both boxes!");
		}
	}
}
