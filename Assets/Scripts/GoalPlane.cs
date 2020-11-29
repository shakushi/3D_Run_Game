using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalPlane : MonoBehaviour
{
    public GameObject GameLoopManager;
    // Start is called before the first frame update
    void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Goal!");
            GameLoopManager.GetComponent<GameLoopManager>().GameClear();
        }
    }
}
