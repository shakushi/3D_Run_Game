using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleFactory : MonoBehaviour
{
    [SerializeField]
    public bool DisableForTest = false;

    private GameObject obstacle_wide;
    private GameObject obstacle_cell;
    private GameObject obstacle_tall;

    private int maxnum = 50;
    private int cnt = 0;
    private bool onCoolTime = false;
    private float coolTimeOnMove = 0.5f;
    private float coolTimeOnGoal = 3f;
    private bool activate = true;

    public bool Activate
    {
        set
        {
            activate = value;
            if (DisableForTest) activate = false;
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        Random.InitState(System.DateTime.Now.Millisecond);
        obstacle_wide = (GameObject)Resources.Load("Obstacle_wide");
        obstacle_cell = (GameObject)Resources.Load("Obstacle_cell");
        obstacle_tall = (GameObject)Resources.Load("Obstacle_tall");
    }

    // Update is called once per frame
    void Update()
    {
        if (!activate)
        {
            return;
        }
        if (this.transform.position.z < 211)
        {
            int posz = Mathf.CeilToInt(this.transform.position.z);
            if (posz % 10 == 0 && !onCoolTime)
            {
                makeObsByVec3(makeRandomInput());
                cnt++;
                onCoolTime = true;
                if (posz / 10 != 21)
                {
                    StartCoroutine("CoolTimeOnMove");
                }
                else
                {
                    StartCoroutine("CoolTimeOnGoal");
                }
            }
            transform.Translate(Vector3.forward * 10f * Time.deltaTime, Space.World);
        }
        else 
        {
            if (cnt < maxnum && !onCoolTime)
            {
                makeObsByVec3(makeRandomInput());
                cnt++;
                onCoolTime = true;
                StartCoroutine("CoolTimeOnGoal");
            }
        }
    }

    private Vector3 makeRandomInput()
    {
        //return notGoodRandam();
        int x, y, z;

        x = Random.Range(1, 5);
        y = Random.Range(1, 5);
        z = Random.Range(1, 5);
        if (x == 4 && y == 4 && z == 4)
        {
            y = 1;
        }
        else if (x == 1 && y == 1)
        {
            y = 3;
        }
        else if (y == 1 && z == 1)
        {
            y = 3;
        }

        Debug.Log("x=" + x + ", y=" + y + ", z=" + z);
        return new Vector3(x, y, z);
    }

    private void makeObsByVec3(Vector3 input)
    {
        if (input == new Vector3(2f, 2f, 2f))
        {
            makeObsBigUp();
        }
        else if (input == new Vector3(3f, 3f, 3f))
        {
            makeObsBigDown();
        }
        else
        {
            makeObsCell(input);
        }
    }

    private void makeObsBigUp()
    {
        Instantiate(obstacle_wide, this.transform.position + (Vector3.up * 2), this.transform.rotation);
    }
    private void makeObsBigDown()
    {
        Instantiate(obstacle_wide, this.transform.position + (Vector3.up * 0.5f), this.transform.rotation);
    }
    private void makeObsCell(Vector3 input)
    {
        switch (input.x)
        {
            case 1:
                break;
            case 2:
                Instantiate(obstacle_cell, this.transform.position + new Vector3(2, 2f, 0), this.transform.rotation);
                break;
            case 3:
                Instantiate(obstacle_cell, this.transform.position + new Vector3(2, 0.5f, 0), this.transform.rotation);
                break;
            case 4:
                Instantiate(obstacle_tall, this.transform.position + new Vector3(2, 0.5f, 0), this.transform.rotation);
                break;
        }
        switch (input.y)
        {
            case 1:
                break;
            case 2:
                Instantiate(obstacle_cell, this.transform.position + new Vector3(0, 2f, 0), this.transform.rotation);
                break;
            case 3:
                Instantiate(obstacle_cell, this.transform.position + new Vector3(0, 0.5f, 0), this.transform.rotation);
                break;
            case 4:
                Instantiate(obstacle_tall, this.transform.position + new Vector3(0, 0.5f, 0), this.transform.rotation);
                break;
        }
        switch (input.z)
        {
            case 1:
                break;
            case 2:
                Instantiate(obstacle_cell, this.transform.position + new Vector3(-2, 2f, 0), this.transform.rotation);
                break;
            case 3:
                Instantiate(obstacle_cell, this.transform.position + new Vector3(-2, 0.5f, 0), this.transform.rotation);
                break;
            case 4:
                Instantiate(obstacle_tall, this.transform.position + new Vector3(-2, 0.5f, 0), this.transform.rotation);
                break;
        }
    }

    private IEnumerator CoolTimeOnMove()
    {
        yield return new WaitForSeconds(coolTimeOnMove);
        onCoolTime = false;
    }
    private IEnumerator CoolTimeOnGoal()
    {
        yield return new WaitForSeconds(coolTimeOnGoal);
        onCoolTime = false;
    }

}
