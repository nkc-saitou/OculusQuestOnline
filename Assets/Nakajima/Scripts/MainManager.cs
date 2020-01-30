using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UniRx;
using UniRx.Async;

namespace Nakajima.Main
{
    public class MainManager : MonoBehaviour
    {
        // バトル開始イベント
        public Action<float> battleStart;
        // バトル終了イベント
        public Action<bool> battleEnd;

        // スコアの更新イベント
        public Action<int, int> updateScore;

        // バトル開始したかどうか
        private bool battle = false;
        public bool Battle
        {
            get { return battle; }
        }

        // エントリー状態かどうか
        private bool entry = false;
        public bool Entry
        {
            set { entry = value; }
            get { return entry; }
        }

        // プレイヤーの得点
        [HideInInspector]
        public int[] playerScore = new int[2];
        // 残り時間
        private float gameTime = 60.0f;
        public float GameTime
        {
            set { gameTime = value; }
            get { return gameTime; }
        }

        // ステージサイズ
        private const int stageSize = 24;

        /// <summary>
        /// 初回処理
        /// </summary>
        void Start()
        {
            // イベントにバインド
            battleStart += BattleStart;
            battleEnd += BattleEnd;
            updateScore += UpdateScore;
        }
        
        /// <summary>
        /// 更新処理
        /// </summary>
        void Update()
        {
            if (battle) CountDown();
        }

        // デバッグ用
        [ContextMenu("start")]
        private void GameStart()
        {
            battleStart?.Invoke(10.0f);
        }

        /// <summary>
        /// バトル開始イベント
        /// </summary>
        /// <param name="_time"></param>
        private async void BattleStart(float _time)
        {
            // フラグを立てる
            battle = true;
            GameTime = _time;

            // 時間終了まで待機
            await UniTask.WaitUntil(() => GameTime <= 0.0f);

            // 終了したあとの処理
            battleEnd?.Invoke(Battle);
        }

        /// <summary>
        /// バトル終了イベント
        /// </summary>
        /// <param name="_battle"></param>
        private void BattleEnd(bool _battle)
        {
            battle = false;
        }

        /// <summary>
        /// スコアの更新イベント
        /// </summary>
        /// <param name="_ID">プレイヤーID</param>
        /// <param name="_score">更新スコア</param>
        private void UpdateScore(int _ID, int _score)
        {
            // 相手プレイヤースコアのインデックスに変更
            int index = _ID + 1 * (_ID - 1) * -2;

            playerScore[index] = _score;
        }

        /// <summary>
        /// カウントダウン(時間計測)
        /// </summary>
        private void CountDown()
        {
            // マイナス制限
            if (gameTime <= 0.0f) {
                gameTime = 0.0f;
                return;
            }

            gameTime -= Time.deltaTime;
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

            // ステージギリギリだったらtrue
            if (distance > stageSize) return true;

            // 最短でないならfalse
            return false;
        }
    }
}