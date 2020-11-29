//#define DEBUG_INPUT_KEYBOARD
#define DEBUG_INPUT_MOUSE

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainInputCtlr : MonoBehaviour
{
    [SerializeField]
    public GameObject Player;
    [SerializeField]
    public GameObject Camera;

    private PlayerCtlr playerCtlr;
    private CameraCtlr cameraCtlr;

    // Start is called before the first frame update
    void Awake()
    {
        playerCtlr = Player.GetComponent<PlayerCtlr>();
        cameraCtlr = Camera.GetComponent<CameraCtlr>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
#if  DEBUG_INPUT_KEYBOARD //キーボード入力
        /* Player */
        if (Input.GetKeyDown(KeyCode.Space))
        {
            playerCtlr.IPOnButtonJumpAct();
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            playerCtlr.IPOnButtonSlidingAct();
        }

        Vector3 input = getMoveDir();
        if (input != Vector3.zero)
        {
            playerCtlr.IPOnStickInput(input);
        }

        /* Camera */
        float h = 0;
        if (Input.GetKey(KeyCode.RightArrow)) { h += 1f; }
        if (Input.GetKey(KeyCode.LeftArrow)) { h -= 1f; }
        if (h != 0)
        {
            Debug.Log("camera:" + h);
            cameraCtlr.CameraRotate(new Vector2(h, 0));
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            playerCtlr.IPOnButtonJumpAct();
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            playerCtlr.IPOnButtonSlidingAct();
        }
#elif DEBUG_INPUT_MOUSE //マウス入力
        /* Player */
        if (Input.GetMouseButtonDown(0))
        {
            float x = Input.mousePosition.x;
            float y = Input.mousePosition.y;
            if (y > (Screen.height * 2f / 3f))
            {
                Debug.Log("Upper");
                playerCtlr.IPOnButtonJumpAct();
            }
            else if(y < (Screen.height / 3f))
            {
                Debug.Log("Buttom");
                playerCtlr.IPOnButtonSlidingAct();
            }
            else if (x < (Screen.width * 2 / 5))
            {
                Debug.Log("Left");
                playerCtlr.IPMoveLane(MoveOrderEnum.Left);
            }
            else if (x > (Screen.width * 3 / 5))
            {
                Debug.Log("Right");
                playerCtlr.IPMoveLane(MoveOrderEnum.Right);
            }
        }
#else   //タッチ入力
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                float x = touch.position.x;
                float y = touch.position.y;
                if (y > (Screen.height * 3f / 4f))
                {
                    Debug.Log("Upper");
                    playerCtlr.IPOnButtonJumpAct();
                }
                else if (y < (Screen.height / 4f))
                {
                    Debug.Log("Buttom");
                    playerCtlr.IPOnButtonSlidingAct();
                }
                else if (x <= (Screen.width / 2))
                {
                    Debug.Log("Left");
                }
                else
                {
                    Debug.Log("Right");
                }
            }
        }
#endif
        Vector3 input = getMoveDir();
        if (input != Vector3.zero)
        {
            playerCtlr.IPOnStickInput(input);
        }

        /* Camera */
        //float h = 0;
        //if (Input.GetKey(KeyCode.RightArrow)) { h += 1f; }
        //if (Input.GetKey(KeyCode.LeftArrow)) { h -= 1f; }
        //if (h != 0)
        //{
        //    Debug.Log("camera:" + h);
        //    cameraCtlr.CameraRotate(new Vector2(h, 0));
        //}

    }

    private Vector3 getMoveDir()
    {
        /* 今は真っ直ぐにしか進めない */
        return Vector3.forward;
    }
}
