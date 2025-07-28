using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestImput : MonoBehaviour
{
    [SerializeField]
    private float speed = 3;
    private Vector2 moveInput;
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    public void WalkRight(InputAction.CallbackContext context)
    {
        Debug.Log("Walk right");
        moveInput = context.ReadValue<Vector2>();
        rb.velocity = moveInput * speed;

    }
}
