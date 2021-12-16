using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Attached to a panel that can be closed by clicking anywhere on screen
/// </summary>
public class ClickToClosePanel : MonoBehaviour
{
    /// <summary>
	/// Hide the attached panel when its outro animation completes
	/// </summary>
    public bool hideOnAnimComplete = true;
    /// <summary>
	/// Can the user click to close this panel currently?
	/// </summary>
    public bool canClickToClose = true;

    private Animator animator;
    private bool goingOut;
    void Awake()
    {
        animator = GetComponent<Animator>();
    }

	private void OnEnable()
    {
        goingOut = false;
    }

	// Update is called once per frame
	void Update()
    {
		if(canClickToClose && !goingOut && Input.GetMouseButtonDown(0)){
            ClosePanel();
		}
		else if(goingOut)
		{
            CheckClose();
		}
    }
    /// <summary>
	/// Closes the panel (playing animation first)
	/// </summary>
    public void ClosePanel()
	{
        goingOut = true;
	}

    private void CheckClose()
	{
        if (animator)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsTag("In_Done"))
            {
                animator.Play("Base Layer.Out");
                goingOut = true;
            }

            if (hideOnAnimComplete && animator.GetCurrentAnimatorStateInfo(0).IsTag("Out_Done"))
            {
                gameObject.SetActive(false);
            }
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
