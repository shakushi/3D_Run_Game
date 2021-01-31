using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCtlr : MonoBehaviour
{
    [SerializeField]
    public GameObject Player;

    private const float cameraHigh = 2.0f;
    private Vector3 playerPosBefore;

    // Start is called before the first frame update
    void Start()
    {
        playerPosBefore = Player.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 playerCurPos = Player.transform.position;

        //Playerの動きに追従
        followPlayer(playerCurPos);

        /* Update Before Value */
        playerPosBefore = playerCurPos;

    }

    void followPlayer(Vector3 playerCurPos)
    {
        /* 移動中は重心を参照しない　カメラ揺れを防ぐため */
        Vector3 moveDiff = playerCurPos - playerPosBefore;

        /* y軸移動は少し遅らせる */
        float move_y = (playerCurPos.y + cameraHigh) - this.transform.position.y;
        //float maxy = 0.06f;
        //if (move_y > 0 && move_y < 2.5f)
        //{
        //    move_y = Mathf.Min(move_y, maxy);
        //}
        //else if(move_y < 0 && move_y > -2.5f)
        //{
        //    move_y = Mathf.Max(move_y, -maxy);
        //}
        moveDiff.y = move_y;
        
        transform.Translate(moveDiff, Space.World);
    }
}