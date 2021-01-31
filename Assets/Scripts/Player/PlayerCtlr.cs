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

    private PlayerStateEnum pstate = PlayerStateEnum.Pause;
    private PlayerEventEnum pevent = PlayerEventEnum.None;

    private const float moveSpeed = 6f;
    private const float jumpFirstSpeed = 18f;
    private const float jumpAcceleration = -5f;
    private const float maxJumpHight = 2.0f;

    private Vector3 inputdir;
    private float currentSpeedY;

    private const float animationTime = 0.8f;
    private const float damageTime = 1.2f;

    private LaneMoveOrderEnum laneOrder = LaneMoveOrderEnum.None;
    private int nowlane = 0;
    private bool laneMoveEnable = true;

    private bool isgrounded;
    private int layerMask;

    /* IPlayerCtlr */
    public float IPSpeed
    {
        get { return character.velocity.magnitude; }
    }
    public void IPlayerGameStart()
    {
        if (pevent != PlayerEventEnum.Clear)
        {
            pevent = PlayerEventEnum.GameStart;
        }
    }
    public void IPlayerGameClear()
    {
        pevent = PlayerEventEnum.Clear;
    }
    public void IPOnButtonJumpAct()
    {
        if (pevent != PlayerEventEnum.GameStart
            || pevent != PlayerEventEnum.Damage
            || pevent != PlayerEventEnum.Clear)
        {
            pevent = PlayerEventEnum.Jump;
        }
    }
    public void IPOnButtonCrouchAct()
    {
        if (pevent != PlayerEventEnum.GameStart
            || pevent != PlayerEventEnum.Jump
            || pevent != PlayerEventEnum.Damage
            || pevent != PlayerEventEnum.Clear)
        {
            pevent = PlayerEventEnum.Crouch;
        }
    }
    public void IPDamage()
    {
        if (pevent != PlayerEventEnum.GameStart
            || pevent != PlayerEventEnum.Clear)
        {
            pevent = PlayerEventEnum.Damage;
        }
    }
    public void IPMove(Vector3 normalInput)
    {
        if (normalInput == null) { return; }
        inputdir = normalInput;
    }
    public void IPMoveLane(LaneMoveOrderEnum order)
    {
        laneOrder = order;
    }

    /* MonoBehaviour */
    void Awake()
    {
        character = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        upCollider = GetComponent<CapsuleCollider>();
        initFlags();
        layerMask = LayerMask.GetMask(new string[] { "Field" });
    }

    private void Start()
    {
        currentSpeedY = 0;
    }

    void Update()
    {
        Vector3 moveDirHolizontal = Vector3.zero;
        Vector3 moveDirVertical = Vector3.zero;

        isgrounded = checkGrounded();
        animator.SetBool("IsGrounded", isgrounded);

        if (isgrounded)
        {
            currentSpeedY = 0;
        }

        /* Update States */
        switch (pevent)
        {
            case PlayerEventEnum.None:
                if(pstate == PlayerStateEnum.Jump && isgrounded)
                {
                    pstate = PlayerStateEnum.Normal;
                    animator.SetInteger("Move", 1); //Run
                }
                break;
            case PlayerEventEnum.GameStart:
                if(pstate == PlayerStateEnum.Pause)
                {
                    pstate = PlayerStateEnum.Normal;
                    animator.SetInteger("Move", 1); //Run
                }
                break;
            case PlayerEventEnum.Jump:
                if (pstate == PlayerStateEnum.Normal && isgrounded)
                {
                    pstate = PlayerStateEnum.Jump;
                    animator.SetTrigger("Jump");
                    /* Only when just start jumping */
                    currentSpeedY = jumpFirstSpeed;
                }
                break;
            case PlayerEventEnum.Crouch:
                if (pstate == PlayerStateEnum.Normal)
                {
                    upColliderDisable();
                    animator.SetTrigger("ScrewKick");
                    pstate = PlayerStateEnum.Crouch;
                    StartCoroutine("endCrouchManage");
                }
                break;
            case PlayerEventEnum.Damage:
                if (pstate == PlayerStateEnum.Normal
                    || pstate == PlayerStateEnum.Jump
                    || pstate == PlayerStateEnum.Crouch)
                {
                    pstate = PlayerStateEnum.Down;
                    animator.SetTrigger("Damage");
                    pstate = PlayerStateEnum.Down;
                    StartCoroutine("endDamageManage");
                }
                break;
            case PlayerEventEnum.Clear:
                pstate = PlayerStateEnum.Clear;
                animator.SetTrigger("Clear");
                break;
            default:
                Debug.Log("Event Error : Unknown event occured");
                break;
        }

        /* Move */
        if (pstate == PlayerStateEnum.Clear
            || pstate == PlayerStateEnum.Pause
            || pstate == PlayerStateEnum.Down)
        {
            moveDirHolizontal *= 0;
            moveDirVertical = -5.0f * Vector3.up;
        }
        else
        {
            if (laneOrder != LaneMoveOrderEnum.None)
            {
                laneMoveStart();
            }
            moveDirHolizontal += inputdir * moveSpeed;
            if (!isgrounded)
            {
                currentSpeedY += jumpAcceleration * Time.deltaTime * 10f;
                moveDirVertical = currentSpeedY * Vector3.up;
            }
            else if(pstate == PlayerStateEnum.Jump)
            {
                moveDirVertical = currentSpeedY * Vector3.up;
            }
            else
            {
                moveDirVertical = Vector3.zero;
            }
        }

        /* 一定以上の高さに上がらせない */
        if (transform.position.y > maxJumpHight)
        {
            moveDirVertical = -0.5f * Vector3.up;
        }
        moveWithRotation(moveDirHolizontal + moveDirVertical);

        deinitFlags();
    }

    /* private */
    private bool checkGrounded()
    {
        if (character.isGrounded)
        {
            return true;
        }
        var ray = new Ray(this.transform.position + Vector3.up * 0.1f, Vector3.down);
        var tolerance = 0.3f;
        return Physics.Raycast(ray, tolerance, layerMask);
    }
    private void initFlags()
    {
        animator.SetInteger("Move", 0); //Idle

        pevent = PlayerEventEnum.None;
        laneOrder = LaneMoveOrderEnum.None;
    }
    private void deinitFlags()
    {
        pevent = PlayerEventEnum.None;
        laneOrder = LaneMoveOrderEnum.None;
    }

    private void laneMoveStart()
    {
        if (laneMoveEnable && laneOrder == LaneMoveOrderEnum.Right && nowlane != 1)
        {
            nowlane += 1;
            StartCoroutine(laneMoving(laneOrder));            
        }
        else if (laneMoveEnable && laneOrder == LaneMoveOrderEnum.Left && nowlane != -1)
        {
            nowlane += -1;
            StartCoroutine(laneMoving(laneOrder));
        }
    }
    private IEnumerator laneMoving(LaneMoveOrderEnum order)
    {
        laneMoveEnable = false;
        float goalPosX = nowlane * 2f;
        while (true)
        {
            if (Mathf.Abs(this.transform.position.x - goalPosX) < 0.2f)
            {
                this.transform.position = new Vector3(goalPosX, this.transform.position.y, this.transform.position.z);
                laneMoveEnable = true;
                yield break;
            }
            if(order == LaneMoveOrderEnum.Right)
            {
                character.Move(Vector3.right * 10f * Time.deltaTime);
            }
            else
            {
                character.Move(Vector3.left * 10f * Time.deltaTime);
            }
            yield return null;
        }
    }

    private IEnumerator endDamageManage()
    {
        yield return new WaitForSeconds(damageTime);
        pstate = PlayerStateEnum.Normal;
    }

    private IEnumerator endCrouchManage()
    {
        yield return new WaitForSeconds(animationTime);
        pstate = PlayerStateEnum.Normal;
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
        character.Move(moveDirection * Time.deltaTime);

        lookRotation = Quaternion.LookRotation(moveDirection);

        lookRotation.x = 0;
        lookRotation.z = 0;
        this.transform.rotation = Quaternion.Lerp(this.transform.rotation, lookRotation, 0.2f);
    }
}
