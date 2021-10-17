using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RegisterButton : MonoBehaviour
{
	[SerializeField]
	private TMP_InputField usernameTMPro;
	[SerializeField]
	private TMP_InputField passwordTMPro;

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
