
using UnityEngine.InputSystem;
using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerController controller;
    public bool IsHoldingBreath = false;

    private void Awake()
    {
        controller = GetComponent<PlayerController>();
    }

    // ========= InputSystem Callbacks =========
    public void OnMove(InputAction.CallbackContext context)
    {
        controller.SetMoveInput(context.ReadValue<Vector2>());
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.started)
            controller.SetRunning(true);
        else if (context.canceled)
            controller.SetRunning(false);
    }

    public void OnHoldBreath(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            controller.SetHoldBreath(true);
            IsHoldingBreath = true;
        }
        else if (context.canceled)
        {
            controller.SetHoldBreath(false);
            IsHoldingBreath = false;
        }
    }

    public void OnUseMendama(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            controller.UseHideAbility();
            
        }
    }

    public void OnCollect(InputAction.CallbackContext context)
    {
        if (context.performed && IsHoldingBreath == false)  // 玩家按下 Z 開始長按
        {
            controller.SetStartCollect(true);
        }
        else if (context.canceled) // 玩家放開 Z
        {
            controller.SetStartCollect(false);
        }
    }
}
