using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nakajima.Main
{
    public class MainManager : MonoBehaviour
    {
        // プレイヤーの得点
        [HideInInspector]
        public int[] playerScore;
        // 残り時間
        private float gameTime;
        public float GameTime {
            set { gameTime = value; }
            get { return gameTime; }
        }

        // ステージサイズ
        private const int stageSize = 24;

        void Start()
        {

        }
        
        void Update()
        {

        }

        /// <summary>
        /// ステージの最端かどうか
        /// </summary>
        /// <param name="_playerPos">プレイヤー座標</param>
        /// <param name="_moveVec">進行ベクター</param>
        /// <returns></returns>
        public bool GetStageEdge(Vector3 _playerPos, Vector3 _moveVec)
        {
            // ステージからの距離
            var myCenter = new Vector3(transform.position.x, 0.0f, transform.position.z);
            var playerCenter = new Vector3(_playerPos.x, 0.0f, _playerPos.z) + _moveVec;

            float distance = Vector3.Distance(myCenter, playerCenter);
            Debug.Log("ステージ中央からの距離 : " + distance);
            // ステージギリギリだったらtrue
            if (distance > stageSize) return true;

            // 最短でないならfalse
            return false;
        }
    }
}