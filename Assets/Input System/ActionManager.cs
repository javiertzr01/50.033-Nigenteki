using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class ActionManager : MonoBehaviour
{

    public UnityEvent<Vector2> movement;
    public UnityEvent<Vector2> look;
    public UnityEvent<bool> rightArmUse;
    public UnityEvent<bool> leftArmUse;


    public void OnMovementAction(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Vector2 moveDir = context.ReadValue<Vector2>();
            movement.Invoke(moveDir);

            // Debug.Log("MOVING");
        }
        else if (context.performed)
        {
            Vector2 moveDir = context.ReadValue<Vector2>();
            movement.Invoke(moveDir);

            // Debug.Log("MOVING");
        }
        else if (context.canceled)
        {
            movement.Invoke(new Vector2(0, 0));

            // Debug.Log("STOPPED MOVING");
        }
    }

    public void OnLookAction(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Vector2 mousePos = context.ReadValue<Vector2>();
            look.Invoke(mousePos);

            // Debug.Log("LOOKING");
        }
        else if (context.performed)
        {
            Vector2 mousePos = context.ReadValue<Vector2>();
            look.Invoke(mousePos);

            // Debug.Log("LOOKING");
        }
        else if (context.canceled)
        {
            Vector2 mousePos = context.ReadValue<Vector2>();
            look.Invoke(mousePos);

            // Debug.Log("STOPPED LOOKING");
        }
    }

    public void OnRightArmUseAction(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            // Debug.Log("USING RIGHT ARM");
        }
        else if (context.performed)
        {
            rightArmUse.Invoke(true);

            // Debug.Log("USING RIGHT ARM");
        }
        else if (context.canceled)
        {
            rightArmUse.Invoke(false);

            // Debug.Log("STOP USING RIGHT ARM");
        }
    }

    public void OnLeftArmUseAction(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            // Debug.Log("USING LEFT ARM");
        }
        else if (context.performed)
        {
            leftArmUse.Invoke(true);

            // Debug.Log("USING LEFT ARM");
        }
        else if (context.canceled)
        {
            leftArmUse.Invoke(false);

            // Debug.Log("STOP USING LEFT ARM");
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
