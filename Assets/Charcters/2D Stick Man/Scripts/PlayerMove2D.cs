using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace memstow
{
    public class PlayerMove2D : MonoBehaviour
    {
        // Player Movement 2D as in up/down and right/left

        private float horizontal;
        public float speedWalk = 8f;  // The speed for walking
        private float speedRunFactor = 2f;  // The factor to be applied to the walk speed for running
        private float jumpingPower = 20f;
        private bool isFacingRight = true;
        private bool permitRunWalk = false;
        private bool permitJump = false;
        private string clipNameL0;  // clip name in Base Layer of animator
        private float fallTime = 0f;
        private bool fallingIs = false;

        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private Transform groundCheck;
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private Collider2D cb;  // collider box

        public int level;
        public int health;

        public Animator animator;
        public AnimatorClipInfo[] currentClipInfo;

        private InputSystem_Actions inputActions;
        private bool jumpPressed = false;
        private bool jumpReleased = false;
        private bool attackPressed = false;
        private bool attackReleased = false;
        private bool sprintPressed = false;

        void Start()
        {
            inputActions = new InputSystem_Actions();
            inputActions.Player.Enable();

            // Setup Jump callbacks
            inputActions.Player.Jump.started += ctx => jumpPressed = true;
            inputActions.Player.Jump.canceled += ctx => { jumpPressed = false; jumpReleased = true; };

            // Setup Attack callbacks
            inputActions.Player.Attack.started += ctx => attackPressed = true;
            inputActions.Player.Attack.canceled += ctx => { attackPressed = false; attackReleased = true; };
        }

        void OnDestroy()
        {
            if (inputActions != null)
            {
                inputActions.Dispose();
            }
        }

        // Update is called once per frame
        void Update()
        {
            bool isGrounded = IsGrounded();

            horizontal = inputActions.Player.Move.ReadValue<Vector2>().x;  // returns -1, 0, 1  Depends on direction moving. -1=left, 0=none, 1=right (programmer's persective, not avatar's)

            animator = gameObject.GetComponent<Animator>();
            currentClipInfo = animator.GetCurrentAnimatorClipInfo(0);
            clipNameL0 = currentClipInfo[0].clip.name;

            //permitRunWalk = !(animator.GetBool("doSpecial") || (clipNameL0 == "Flex"));  // if doing this action, don't allow running or walking.
            permitRunWalk = !(clipNameL0 == "Flex");  // if doing this action, don't allow running or walking.
            permitJump = !(animator.GetBool("doAttack") || animator.GetBool("doSpecial"));  // if doing this action, don't allow jumping.


            // Jump Update
            if (jumpPressed && isGrounded && permitJump)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpingPower);
                animator.SetBool("doJumpUp", true);
                jumpPressed = false;
            }

            if (jumpReleased && rb.linearVelocity.y > 0.0f)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
                jumpReleased = false;
            }

            if (!(Input.GetButtonUp("Jump")) && rb.linearVelocity.y == 0.0f)
            {
                animator.SetBool("doJumpUp", false);
                animator.SetBool("doJumpDown", false);
            }

            if ((rb.linearVelocity.y < -0.2) && !isGrounded)
            {
                fallTime += Time.deltaTime;

                if (fallTime > 0.1) fallingIs = true;
                else fallingIs = false;

                if (fallingIs)
                {
                    animator.SetBool("doJumpUp", false);
                    animator.SetBool("doJumpDown", true);
                }
            }
            else
            {
                fallTime = 0f;
                fallingIs = false;
            }


            // Walk & Run Updating
            if (Mathf.Abs(rb.linearVelocity.x) > 0)
            {
                if (Mathf.Abs(rb.linearVelocity.x) > (speedWalk + 0.02))  // Add 0.02 just in case there is some fluctuation for something.
                {
                    animator.SetBool("doRun", true);
                    animator.SetBool("doWalk", false);
                }
                else
                {
                    animator.SetBool("doRun", false);
                    animator.SetBool("doWalk", true);
                }
            }
            else
            {
                animator.SetBool("doRun", false);
                animator.SetBool("doWalk", false);
            }

            // Attack Updating
            if (attackPressed)
            {

                if ((rb.linearVelocity.x != 0) || (rb.linearVelocity.x == 0f && (rb.linearVelocity.y != 0f)))
                {
                    animator.SetBool("doSpecial", true);
                    animator.SetBool("doAttack", false);
                }
                else
                {
                    animator.SetBool("doSpecial", false);
                    animator.SetBool("doAttack", true);
                }
                attackPressed = false;
            }
            //else
            if (attackReleased)
            {
                animator.SetBool("doSpecial", false);
                animator.SetBool("doAttack", false);
                attackReleased = false;
            }

            Flip();
        }

        private void FixedUpdate()
        {
            sprintPressed = inputActions.Player.Sprint.IsPressed();

            if (permitRunWalk) // if permitted to run or walk, then update velicity
            {
                // Check if the button for running is pressed
                if (sprintPressed)
                {
                    rb.linearVelocity = new Vector2(horizontal * speedWalk * speedRunFactor, rb.linearVelocity.y);
                }
                else
                {
                    rb.linearVelocity = new Vector2(horizontal * speedWalk, rb.linearVelocity.y);
                }
            }
            else // Else set the velocity.x to 0 as the character may be performing an act that does not permit x-axis movement
            {
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            }
        }

        private bool IsGrounded()
        {
            bool isOverlap = false;

            if (groundLayer == 0)  // If specific layer is not set
            {
                isOverlap = Physics2D.OverlapCircle(groundCheck.position, 0.2f);
            }
            else
            {
                isOverlap = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
            }

            if (isOverlap)
            {
                return true;
            }

            return false;
        }

        private void Flip()
        {
            if (permitRunWalk)
            {
                // Player to be flipped if true
                if ((isFacingRight && horizontal < 0.0f) || (!isFacingRight && horizontal > 0.0f))
                {
                    isFacingRight = !isFacingRight;
                    Vector3 localScale = transform.localScale;
                    localScale.x *= -1.0f;
                    transform.localScale = localScale;
                }
            }
        }
    }

}   // end of memstow namespace