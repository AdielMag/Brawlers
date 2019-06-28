using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody rb;
    Animator anim;
    ObjectPooler objPooler;
    PlayerInput pInput;

    public float movementSpeed = 3.5f, movementSmoothMultiplier = 9;
    public float fallMultiplier = 3.5f, lowJumpMultiplier = 2.5f;
    public float jumpForce = 9f;

    bool isWalledRight, isWalledLeft;
    bool pushingWall;

    bool cantWall;
    bool canDash = true;
    bool canMove = true;
    bool canChangeDir = true;

    public Vector2 dirInput,targetInput;

    // Animator variables
    float targetHorizontal, targetVertical;
    float lastDir;
    Transform rightFoot, leftFoot;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        pInput = GetComponent<PlayerInput>();

        rightFoot = anim.GetBoneTransform(HumanBodyBones.RightFoot);
        leftFoot = anim.GetBoneTransform(HumanBodyBones.LeftFoot);

        allButPlayer = 55; // set layer mask for box casting!

        objPooler = ObjectPooler.instance;
    }

    void Update()
    {
        // Movement Input
        dirInput = new Vector2(Input.GetAxis(pInput.horizontal), Input.GetAxis(pInput.vertical)) * movementSpeed;

        if (!canMove)
            return;

        // Jump Input
        if (Input.GetButtonDown(pInput.jump))
        {
            if (isWalledLeft || isWalledRight)
            {
                WallJump();
                return;
            }
            if (!IsGrounded())
                return;
                
            Jump();
        }

        if (Input.GetButtonDown(pInput.dash))
        {
            if (canDash)
                Dash(dirInput);
        }
    }

    private void FixedUpdate()
    {
        Movement(dirInput);
        AdjustingJump();
        CheckForWalls();

        targetHorizontal = Mathf.Lerp(targetHorizontal, rb.velocity.x / 8, Time.fixedDeltaTime * 8);
        anim.SetFloat("Horizontal", targetHorizontal);

        targetVertical = Mathf.Lerp(targetVertical, rb.velocity.y /8, Time.fixedDeltaTime * 7);
        anim.SetFloat("Vertical", targetVertical);

        anim.SetBool("Grounded", IsGrounded());
        anim.SetBool("WalledRight", isWalledRight);
        anim.SetBool("WalledLeft", isWalledLeft);

    }

    void Movement(Vector2 dir) 
    {
        if (!canMove)
            return;

        pushingWall = (isWalledRight && dir.x > 0) || (isWalledLeft && dir.x < 0);
        
        if (!cantWall && (isWalledRight || isWalledLeft))
        {

            if (Input.GetButton(pInput.grabWall))
            {
                rb.velocity = new Vector2( rb.velocity.x, dir.y * movementSpeed / 2);
                rb.useGravity = false;
            }
            else
            {
                rb.velocity = new Vector2(rb.velocity.x, -.7f);
                rb.useGravity = true;
            }
        }
        else
            rb.useGravity = true;

        rb.drag = Mathf.Lerp(rb.drag,1, Time.deltaTime * 6);
        rb.velocity = Vector2.Lerp(rb.velocity, new Vector2(pushingWall ? 0 : dir.x * movementSpeed, Mathf.Clamp(rb.velocity.y, -jumpForce * 2, jumpForce * 2)), Time.fixedDeltaTime * (IsGrounded() ? movementSmoothMultiplier : movementSmoothMultiplier / 2.5f));
    }

    void Jump()
    {
        objPooler.SpawnFromPool("Jump", transform.position - Vector3.up * .9f, Quaternion.identity);

        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.velocity += Vector3.up * jumpForce;
    }

    void WallJump() 
    {
        StartCoroutine(CantGrabWallTimer(.1f));

        Vector3 jumpDirection;

        if (isWalledRight) 
        {
            jumpDirection = Vector2.left * 1.5f + Vector2.up / 1.5f;
            objPooler.SpawnFromPool("Jump", transform.position - Vector3.up * .9f, Quaternion.identity);

        }
        else 
        {
            jumpDirection = Vector2.right * 1.5f + Vector2.up / 1.5f;
            objPooler.SpawnFromPool("Jump", transform.position - Vector3.up * .9f, Quaternion.identity);

        }


        rb.drag = 5;

        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.velocity += jumpDirection * 17;
    }

    void Dash(Vector3 direction)
    {
        if (direction == Vector3.zero || pushingWall)
            return;

        StartCoroutine(DashTimer(1));

        if (isWalledLeft || isWalledRight)
            StartCoroutine(CantGrabWallTimer(.1f));

        rb.drag = 5;

        if (IsGrounded())
        {
            objPooler.SpawnFromPool("Jump", transform.position - Vector3.up * .9f, Quaternion.identity);
            rb.velocity = direction.normalized * 40;
        }
        else
            rb.velocity = direction.normalized * 30;

        objPooler.SpawnFromPool("Dash", transform.position, Quaternion.LookRotation(direction, Vector3.up));


        if (!IsGrounded())
            anim.SetTrigger("Dash"); // Nedd to get animations! do this at home
    }

    #region Called from animation events methods
    public void Footstep(int right) 
    {
        Ray ray = new Ray(right == 1 ? rightFoot.position : leftFoot.position, -Vector3.up);
        if (Physics.Raycast(ray, out RaycastHit hit, .5f))
            objPooler.SpawnFromPool("FootStep", hit.point, Quaternion.identity);
    }
    public void HardLanding()
    {
        objPooler.SpawnFromPool("HardLanding", transform.position - Vector3.up * .9f, Quaternion.identity);

        StartCoroutine(StopMoving(1f));
    }
    #endregion

    void AdjustingJump()
    {
        if (rb.velocity.y < 0)
            rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        else if (rb.velocity.y > 0 && !Input.GetButton(pInput.jump))
            rb.velocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
    }

    LayerMask allButPlayer;
    Vector3 boxCastingExtends = new Vector3(0.45f,.2f, 1);
    void CheckForWalls()
    {
        if (Physics.BoxCast(transform.position + Vector3.up / 5, boxCastingExtends, Vector2.right, Quaternion.identity, .14f, allButPlayer)) // check right
        {
            isWalledRight = true;
            isWalledLeft = false;
        }
        else if (Physics.BoxCast(transform.position + Vector3.up / 5, boxCastingExtends, -Vector2.right, Quaternion.identity, .14f, allButPlayer)) // check left
        {
            isWalledLeft = true;
            isWalledRight = false;
        }
        else
        {
            isWalledLeft = false;
            isWalledRight = false;
        }
    }

    bool IsGrounded() 
    {
        if (Physics.BoxCast(transform.position, boxCastingExtends, -Vector2.up, Quaternion.identity, 1f))
            return true;

        return false;
    }

    #region Enumerators
    public IEnumerator CantGrabWallTimer(float time)
    {
        cantWall = true;
        yield return new WaitForSeconds(time);
        cantWall = false;
    }
    public IEnumerator DashTimer(float time)
    {
        canDash = false;
        yield return new WaitForSeconds(time);
        canDash = true;
    }
    public IEnumerator StopMoving(float time)
    {
        canMove = false;
        yield return new WaitForSeconds(time);
        canMove = true;
    }
    public IEnumerator ChangeMovementSpeed(float time,float amount)
    {
        movementSpeed /= amount;
        yield return new WaitForSeconds(time);
        movementSpeed *= amount;
    }
    public IEnumerator CantChangeDircetion(float time)
    {
        canChangeDir = false;
        yield return new WaitForSeconds(time);
        canChangeDir = true;
    }
    #endregion
}
