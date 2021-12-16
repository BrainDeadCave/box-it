using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
/// <summary>
/// Handles behavior of sign in button press
/// </summary>

public class SignInButton : MonoBehaviour
{
	[SerializeField]
	private TMP_InputField usernameTMPro;
	[SerializeField]
	private TMP_InputField passwordTMPro;
	/// <summary>
	/// Tells NetworkManager Instance to send a login request with supplied username and password, if both are filled
	/// </summary>
	public void Login()
	{
		if (usernameTMPro.text.Length > 0 && passwordTMPro.text.Length > 0)
		{
			NetworkManager.Instance.Login(usernameTMPro.text, passwordTMPro.text);
		}
		else 
		{
			MainMenuController.Instance.Error("Please fill in both boxes!");
		}
	}
}
