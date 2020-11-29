using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCtlr : MonoBehaviour
{
    public GameObject Player;
    public GameObject PlayerRoot;
    [SerializeField]
    public float cameraScale = 1.0f; /* <1.0>通常・人型 <1.5>:大型 */

    private float rotateSpeed_h = 2.0f;
    private float rotateSpeed_v = 3.0f;

    private PlayerCtlr pctlr;
    private Vector3 playerPosBefore;
    private Vector3 playerRootPosBefore;
    private float defCameraRootY;
    private float cameraDis;
    private float maxCameraY = 3.7f;
    private float minCameraY = -0.5f;
    private Vector3 wallHitPosition;

    private bool tmpFlagRotate = false;
    private Vector2 inputDir;

    public void CameraRotate(Vector2 input)
    {
        tmpFlagRotate = true;
        inputDir = input;

    }

    // Start is called before the first frame update
    void Start()
    {
        pctlr = Player.GetComponent<PlayerCtlr>();
        playerPosBefore = Player.transform.position;
        playerRootPosBefore = PlayerRoot.transform.position;
        defCameraRootY = playerRootPosBefore.y + (cameraScale - 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 playerCurPos = Player.transform.position;
        Vector3 playerCurRootPos = PlayerRoot.transform.position;

        /* 将来的にカメラスケールを動的変化させるためUpdateで更新 */
        cameraDis = cameraScale * 3.0f;

        //Playerの動きに追従
        followPlayer(playerCurPos, playerCurRootPos);

        /* Update Before Value */
        playerPosBefore = playerCurPos;
        playerRootPosBefore = playerCurRootPos;

        if (pctlr.IPCameraEnable && tmpFlagRotate)
        {
            if (inputDir.magnitude > 1.0f)
            {
                Debug.Log("invalid args");
                return;
            }
            rotateByInput(inputDir);

            tmpFlagRotate = false;
        }
    }

    void followPlayer(Vector3 playerCurPos, Vector3 playerCurRootPos)
    {
        /* 移動中は重心を参照しない　カメラ揺れを防ぐため */
        Vector3 moveDiff = playerCurPos - playerPosBefore;

        /* y軸移動は少し遅らせる */
        float move_y = (playerCurRootPos.y + 1.5f + cameraScale -0.5f) - this.transform.position.y;
        float maxy = 0.4f;
        if (move_y > 0 && move_y < 1.5f)
        {
            move_y = Mathf.Min(move_y, maxy);
        }
        else if(move_y < 0 && move_y > -1.5f)
        {
            move_y = Mathf.Max(move_y, -maxy);
        }
        moveDiff.y = move_y;
        
        transform.Translate(moveDiff, Space.World);
    }

    void rotateByInput(Vector2 input)
    {
        float h, v;

        h = input.x;
        v = input.y;

        if (Mathf.Abs(h) < 0.2) h = 0.0f;
        if (Mathf.Abs(v) < 0.2) v = 0.0f;
        //Debug.Log("(h,v) = ("+h+","+v+")");
        float angle_h = h * rotateSpeed_h;
        float angle_v = v * rotateSpeed_v;

        float posy = this.transform.position.y;
        Vector3 playerPos = PlayerRoot.transform.position;
        /* 横回転 */
        transform.RotateAround(playerPos, Vector3.up, angle_h);
        /* 縦回転 */
        if (posy > maxCameraY) /* too high */
        {
            //Debug.Log("camera high adjust. posy = " + posy);
            transform.RotateAround(playerPos, crossVector(Camera.main.transform.forward, Vector3.up), 0.1f);
        }
        else if (posy < minCameraY) /* too low */
        {
            //Debug.Log("camera low adjust. posy = " + posy);
            transform.RotateAround(playerPos, crossVector(Camera.main.transform.forward, Vector3.up), -0.1f);
        }
        else /* normal */
        {
            transform.RotateAround(playerPos, crossVector(Camera.main.transform.forward, Vector3.up), angle_v);
        }
    }

    //外積を返す
    public static Vector3 crossVector(Vector3 a, Vector3 b)
    {
        Vector3 c = new Vector3();
        c.x = a.y * b.z - a.z * b.y;
        c.y = a.z * b.x - a.x * b.z;
        c.z = a.x * b.y - a.y * b.x;
        return c;
    }
}