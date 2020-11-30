using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Dependencies */
[RequireComponent(typeof(CharacterController))]

public class PlayerCtlr : MonoBehaviour
{
    private CharacterController character;
    private Animator animator;
    private CapsuleCollider upCollider;
    private bool isPause = false;
    private bool isClear = false;

    private bool tmpFlagMove = false;
    private float moveSpeed = 1f;
    private Vector3 inputdir;
    private Vector3 gravity = new Vector3(0, -0.6f, 0);
    private Vector3 inertiaDir;

    private float jumpPower = 13f;
    private bool tmpFlagJump = false;
    private bool tmpFlagSliding = false;

    private Vector3 screw = new Vector3(0, 0, 5.0f);
    private bool inAnimation = false;
    private float animationTime = 0.8f;

    private bool inDamage = false;
    private float damageTime = 1.2f;

    private MoveOrderEnum moveOrder = MoveOrderEnum.None;
    private int nowlane = 0;

    /* Property */

    /* IPlayerCtlr */
    public bool IPCameraEnable
    {
        get { return !isPause; }
    }
    public float IPSpeed
    {
        get { return character.velocity.magnitude; }
    }
    public void IPlayerSetPause(bool value)
    {
        if (value && !isPause)
        {
            animator.SetInteger("Move", 0); //Idle
        }
        isPause = value;
    }
    public void IPlayerGameClear()
    {
        if (!isClear)
        {
            isClear = true;
            animator.SetTrigger("Clear");
        }
    }
    public void IPOnButtonJumpAct()
    {
        tmpFlagJump = true;
    }
    public void IPOnButtonSlidingAct()
    {
        tmpFlagSliding = true;
    }
    public void IPOnStickInput(Vector3 normalInput)
    {
        if (normalInput == null) { return; }

        tmpFlagMove = true;
        inputdir = normalInput;
    }
    public void IPDamage()
    {
        if (isClear || isPause)
        {
            return;
        }
        animator.SetTrigger("Damage");
        inDamage = true;
        StartCoroutine("endDamageManage");
    }
    public void IPMoveLane(MoveOrderEnum order)
    {
        moveOrder = order;
    }

    /* MonoBehaviour */
    void Awake()
    {
        character = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        upCollider = GetComponent<CapsuleCollider>();
    }

    private void Start()
    {
        inertiaDir = Vector3.zero;
    }

    void Update()
    {
        if (isClear || isPause)
        {
            moveWithRotation(gravity);
            animator.SetBool("IsGrounded", character.isGrounded);
            return;
        }
        if (inDamage)
        {
            moveWithRotation(gravity);
            inertiaDir *= 0;
            deinitFlags();
            return;
        }

        if (moveOrder != MoveOrderEnum.None)
        {
            if (moveOrder == MoveOrderEnum.Right && nowlane != 1)
            {
                character.Move(Vector3.right * 2f);
                nowlane += 1;
            }
            else if (moveOrder == MoveOrderEnum.Left && nowlane != -1)
            {
                character.Move(Vector3.left * 2f);
                nowlane += -1;
            }
        }
        Vector3 moveDirThisFrame = Vector3.zero;
        if (inAnimation)
        {
            moveWithRotation(screw);
            inertiaDir = updateInertiaDir(moveDirThisFrame, inertiaDir, true);
            deinitFlags();
            return;
        }

        bool isgrounded = character.isGrounded;
        animator.SetBool("IsGrounded", isgrounded);
        if (isgrounded)
        {
            if (tmpFlagMove)
            {
                if (!tmpFlagJump && !tmpFlagSliding)
                {
                    animator.SetInteger("Move", 1); //Run
                }
                moveDirThisFrame += inputdir;
            }
            if (tmpFlagJump)
            {
                animator.SetTrigger("Jump");
                moveDirThisFrame += Vector3.up * 2.0f * jumpPower;
            }
            else if (tmpFlagSliding)
            {
                upColliderDisable();
                animator.SetTrigger("ScrewKick");
                inAnimation = true;
                StartCoroutine("animationLifeManage");
            }
            if (!tmpFlagMove && !tmpFlagJump && !tmpFlagSliding)
            {
                animator.SetInteger("Move", 0); //Idle
            }
        }
        else
        {
            if (tmpFlagMove)
            {
                moveDirThisFrame += inputdir;
            }
        }

        moveWithRotation(moveDirThisFrame + gravity + inertiaDir);

        inertiaDir = updateInertiaDir(moveDirThisFrame, inertiaDir, isgrounded);
        deinitFlags();
    }

    /* private */
    private void deinitFlags()
    {
        tmpFlagJump = false;
        tmpFlagMove = false;
        tmpFlagSliding = false;
        moveOrder = MoveOrderEnum.None;
    }
    private Vector3 updateInertiaDir(Vector3 thisFrame, Vector3 old, bool isGrounded)
    {
        if (isGrounded)
        {
            return (old * 0.8f) + thisFrame;
        }
        else
        {
            return (old * 0.8f) + thisFrame + gravity;
        }
    }

    private IEnumerator animationLifeManage()
    {
        yield return new WaitForSeconds(animationTime);
        inAnimation = false;
    }
    private IEnumerator endDamageManage()
    {
        yield return new WaitForSeconds(damageTime);
        inDamage = false;
    }

    private void upColliderDisable()
    {
        upCollider.enabled = false;
        StartCoroutine("restoreCollider");
    }
    private IEnumerator restoreCollider()
    {
        yield return new WaitForSeconds(animationTime);
        upCollider.enabled = true;
    }

    private void moveWithRotation(Vector3 moveDirection)
    {
        Quaternion lookRotation;
        character.Move(moveDirection * moveSpeed * Time.deltaTime);

        lookRotation = Quaternion.LookRotation(moveDirection);

        lookRotation.x = 0;
        lookRotation.z = 0;
        this.transform.rotation = Quaternion.Lerp(this.transform.rotation, lookRotation, 0.2f);
    }
}
