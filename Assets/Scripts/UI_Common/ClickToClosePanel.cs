using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickToClosePanel : MonoBehaviour
{
    public bool hideOnAnimComplete = true;
    private Animator animator;
    [SerializeField]
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
		if(!goingOut && Input.GetMouseButtonDown(0)){
            ClosePanel();
		}
		else if(goingOut)
		{
            CheckClose();
		}
    }

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
