using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCtlr : MonoBehaviour
{
    public GameObject Player;
    public GameObject PlayerRoot;
    [SerializeField]
    public float cameraHigh = 1.8f;

    private PlayerCtlr pctlr;
    private Vector3 playerPosBefore;

    private bool tmpFlagRotate = false;
    private Vector2 inputDir;

    // TODO
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
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 playerCurPos = Player.transform.position;
        Vector3 playerCurRootPos = PlayerRoot.transform.position;

        //Playerの動きに追従
        followPlayer(playerCurPos, playerCurRootPos);

        /* Update Before Value */
        playerPosBefore = playerCurPos;

    }

    void followPlayer(Vector3 playerCurPos, Vector3 playerCurRootPos)
    {
        /* 移動中は重心を参照しない　カメラ揺れを防ぐため */
        Vector3 moveDiff = playerCurPos - playerPosBefore;

        /* y軸移動は少し遅らせる */
        float move_y = (playerCurRootPos.y + cameraHigh) - this.transform.position.y;
        float maxy = 0.06f;
        if (move_y > 0 && move_y < 2.5f)
        {
            move_y = Mathf.Min(move_y, maxy);
        }
        else if(move_y < 0 && move_y > -2.5f)
        {
            move_y = Mathf.Max(move_y, -maxy);
        }
        moveDiff.y = move_y;
        
        transform.Translate(moveDiff, Space.World);
    }
}