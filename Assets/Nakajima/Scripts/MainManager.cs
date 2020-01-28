﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UniRx;
using UniRx.Async;

namespace Nakajima.Main
{
    public class MainManager : MonoBehaviour
    {
        public event Action<float> battleStart;

        // バトル開始したかどうか
        private bool battle;
        public bool Battle
        {
            get { return battle; }
        }

        // プレイヤーの得点
        [HideInInspector]
        public int[] playerScore;
        // 残り時間
        private float gameTime = 60.0f;
        public float GameTime
        {
            set { gameTime = value; }
            get { return gameTime; }
        }

        // ステージサイズ
        private const int stageSize = 24;

        void Start()
        {
            // イベントにバインド
            battleStart += BattleStart;
        }
        
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
            battle = true;
            GameTime = _time;

            // 時間終了まで待機
            await UniTask.WaitUntil(() => GameTime <= 0.0f);

            // 終了したあとの処理
            battle = false;
        }

        /// <summary>
        /// カウントダウン(時間計測)
        /// </summary>
        private void CountDown()
        {
            if (gameTime <= 0.0f) return;

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