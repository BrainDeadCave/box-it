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
			Debug.Log($"Attempting sign in. UN: {usernameTMPro.text}  PW: {passwordTMPro.text}");
			//Show spinny boi
		}
		else 
		{ 
			//Show error
		}
	}
}
