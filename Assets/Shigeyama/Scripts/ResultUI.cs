using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultUI : MonoBehaviour
{
    int playerScore = 10;

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
        //-------------------------------------------
        // playerScoreとenemyScoreにスコアの値を入れる


        //-------------------------------------------

        playerScoreText.text = playerScore.ToString();

        enemyScoreText.text = enemyScore.ToString();

        if (playerScore > enemyScore)
        {
            resultText.text = "You Win";
        }
        else if (enemyScore > playerScore)
        {
            resultText.text = "You Lose";
        }
        else
        {
            resultText.text = "Draw";
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
