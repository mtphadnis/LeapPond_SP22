using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimationManager : MonoBehaviour
{
    // Start is called before the first frame update
    public Animator mAnimator;
    
    void Start()
    {
        mAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    public void Throw(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            //whatever runs when ani starts
            //Debug.Log("hey");

            //Debug.Log("Hey");

            //mAnimator.SetBool("Throw", true);

            mAnimator.SetTrigger("Throw");

            //Debug.Log(mAnimator.gameObject.name); 
        }
        else if(context.canceled)
        {
            //whatever runs when ani is ended
        }
    }
}
