using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultUI : MonoBehaviour
{
    int playerScore = 0;

    int enemyScore = 0;

    [SerializeField]
    Text playerScoreText;

    [SerializeField]
    Text enemyScoreText;

    [SerializeField]
    Text resultText;

    // Start is called before the first frame update
    void Start()
    {

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// TextにScoreをセット
    /// </summary>
    /// <param name="playerScore_">1Pのスコア</param>
    /// <param name="enemyScore_">2Pのスコア</param>
    public void SetScoreText(int playerScore_, int enemyScore_)
    {

        playerScoreText.text = playerScore_.ToString();

        enemyScoreText.text = enemyScore_.ToString();
    }

    /// <summary>
    /// 結果の表示
    /// </summary>
    /// <param name="playerResult">勝ち負け判定</param>
    public void ResultDisplay(int playerResult)
    {
        switch (playerResult)
        {
            case 0:
                resultText.text = "Draw";
                break;
            case 1:
                resultText.text = "You Win";
                break;
            case 2:
                resultText.text = "You Lose";
                break;
        }
    }
}
