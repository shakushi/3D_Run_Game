using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/* Dependencies */
[RequireComponent(typeof(UIManager))]

public class GameLoopManager : MonoBehaviour
{
    private PlayerCtlr playerCtlr;
    private UIManager uiManager;
    private ObstacleFactory factory;
    private bool inGame = false;
    private float startTime;

    private void Awake()
    {
        playerCtlr = GameObject.Find("Player").GetComponent<PlayerCtlr>();
        uiManager = GetComponent<UIManager>();
        factory = GameObject.Find("ObstacleFactory").GetComponent<ObstacleFactory>();
    }

    // Start is called before the first frame update
    void Start()
    {
        playerCtlr.IPlayerSetPause(true);
        factory.Activate = false;
    }

    private void Update()
    {
        if (inGame)
        {
            uiManager.SetTimeText(Time.time - startTime);
        }
    }

    public void GameStart()
    {
        inGame = true;
        startTime = Time.time;
        playerCtlr.IPlayerSetPause(false);
        factory.Activate = true;
    }

    public void GameClear()
    {
        inGame = false;
        playerCtlr.IPlayerGameClear();
        uiManager.EnableClearUI();
        factory.Activate = false;
    }

    public void GameRestart()
    {
        SceneManager.LoadScene("Main");
    }
}
