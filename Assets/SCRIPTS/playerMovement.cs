using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMovement : MonoBehaviour
{
    private InputSystem_Actions playerActionControls;
    private Rigidbody2D rb;
    [SerializeField] private float speed = 15f;

    private void Awake()
    {
        playerActionControls = new InputSystem_Actions();
        rb = GetComponent<Rigidbody2D>();

    }

    private void OnEnable()
    {
        playerActionControls.Enable();
    }

    private void OnDisable()
    {
        playerActionControls.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 moveInput = playerActionControls.Player.Move.ReadValue<Vector2>();
        rb.linearVelocity = moveInput * speed;
    }
}