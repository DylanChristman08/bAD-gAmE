using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

private float horizontalMvmt;
public float speed = 2f;
void Update()
        {
            bool isGrounded = IsGrounded();

            horizontalMvmt = inputActions.Player.Move.ReadValue<Vector2>().x; 
        }
 void Start()
        {
            inputActions = new InputSystem_Actions();
            inputActions.Player.Enable();
        }