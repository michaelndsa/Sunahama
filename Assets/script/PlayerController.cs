using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float speed = 3;
    private Vector2 moveInput;
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        Debug.Log("Walk right");
        moveInput = context.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        rb.velocity = moveInput * speed;
    }
}
