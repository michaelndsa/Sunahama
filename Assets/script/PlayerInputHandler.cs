
using UnityEngine.InputSystem;
using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerController controller;

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
            controller.SetHoldBreath(true);
        else if (context.canceled)
            controller.SetHoldBreath(false);
    }

    public void OnCollect(InputAction.CallbackContext context)
    {
        if (context.performed)  // 玩家按下 Z 開始長按
        {
            controller.SetStartCollect(true);
        }
        else if (context.canceled) // 玩家放開 Z
        {
            controller.SetStartCollect(false);
        }
    }
}
