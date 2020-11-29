using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text TimeText;
    public GameObject ClearText;
    public GameObject RestartButton;

    public void SetTimeText(float time)
    {
        TimeText.text = "Time:" + time.ToString("f1");
    }

    public void EnableClearUI()
    {
        ClearText.SetActive(true);
        RestartButton.SetActive(true);
        TimeText.color = Color.yellow;
    }
}
