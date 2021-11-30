using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SignInButton : MonoBehaviour
{
	[SerializeField]
	private TMP_InputField usernameTMPro;
	[SerializeField]
	private TMP_InputField passwordTMPro;

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
