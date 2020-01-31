using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Nakajima.Main;

public class GameTimerUI : MonoBehaviour
{
    // MainManagerの参照(Time取得)
    private MainManager mainMgr;

    float gameTimer = 246;

    [SerializeField]
    Text timerText;

    float minute = 0;
    float seconds = 0;

    float oldSeconds = 0;

    // Start is called before the first frame update
    void Start()
    {
        mainMgr = FindObjectOfType<MainManager>();
    }

    // Update is called once per frame
    void Update()
    {
        gameTimer = mainMgr.GameTime;

        if (gameTimer < 0) return;

        minute = (int)gameTimer / 60;
        seconds = gameTimer - minute * 60;

        if ((int)seconds != (int)oldSeconds)
        {
            timerText.text = minute.ToString("00") + ":" + ((int)seconds).ToString("00");
        }
        oldSeconds = seconds;
    }
}
