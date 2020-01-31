﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Nakajima.Player;
using UniRx;
using UniRx.Async;
using Photon.Pun;
using UnityEngine.SceneManagement;

namespace Nakajima.Main
{
    public class MainManager : MonoBehaviour
    {
        // バトル開始イベント
        public Action<float> battleStart;
        // バトル終了イベント
        public Action<bool> battleEnd;
        // エントリーイベント
        public Action playerEntry;
        // リザルトイベント
        public Action resultEvent;

        // photonView
        private PhotonView myPhotonView;

        // ローカルプレイヤー
        private PlayerMaster playerController;

        // ResultCanvasの設定
        [SerializeField]
        private GameObject resultCanvas;
        public GameObject ResultCanvas {
            get { return resultCanvas; }
        }

        [SerializeField]
        private GameObject[] rootPos;
        
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

        void Awake()
        {
        }

        /// <summary>
        /// 初回処理
        /// </summary>
        void Start()
        {
            myPhotonView = GetComponent<PhotonView>();

            // イベントにバインド
            battleStart += BattleStart;
            battleEnd += BattleEnd;
            updateScore += UpdateScore;
            playerEntry += () => {
                myPhotonView.RPC(nameof(PlayerEntry), RpcTarget.All);
            };

            playerScore = new int[2];

            if (SceneManager.GetActiveScene().name == "LobbyTest") playerEntry();
        }
        
        /// <summary>
        /// 更新処理
        /// </summary>
        void Update()
        {
            // CanvasTracking();

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

            playerController = FindObjectOfType<PlayerMaster>();

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

            // イベント実行
            resultEvent?.Invoke();

            // 10秒後ロビー画面へ
            Observable.Timer(TimeSpan.FromMilliseconds(10000))
                .Subscribe(_ => SceneChanger.Instance.MoveScene("LobbyTest", 1.0f, 1.0f, SceneChangeType.BlackFade, true));
        }

        /// <summary>
        /// プレイヤーエントリーイベント
        /// </summary>
        [PunRPC]
        private void PlayerEntry()
        {
            Entry = true;
        }

        /// <summary>
        /// スコアの更新イベント
        /// </summary>
        /// <param name="_ID">プレイヤーID</param>
        /// <param name="_score">更新スコア</param>
        private void UpdateScore(int _ID, int _score)
        {
            // 相手プレイヤースコアのインデックスに変更
            if(_ID == 1) playerScore[1] = _score;
            else if(_ID == 2) playerScore[0] = _score;
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
        /// キャンバスの位置を調整する
        /// </summary>
        private void CanvasTracking()
        {
            if (playerController == null) return;

            // プレイヤーとのベクトルを取得
            //Vector3 playerVec = (transform.position - playerController.transform.position).normalized;
            for(int i = 0;i < rootPos.Length; i++)
            {
                Vector3 playerVec = playerController.myMovement.GetMyCamera.transform.forward * 30;
                playerVec = new Vector3(playerVec.x, rootPos[i].transform.position.y, playerVec.z);
                // プレイヤーとの反対のベクトル
                Vector3 reverseVec = playerVec * -1;

                // トラッキングした位置
                Vector3 trackingPos = reverseVec * (stageSize + 10);
                rootPos[i].transform.position = Vector3.Lerp(rootPos[i].transform.position, Vector3.zero + playerVec, 10.0f);
                rootPos[i].transform.rotation = Quaternion.Euler(0.0f, playerController.myMovement.GetMyCamera.transform.eulerAngles.y, 0.0f);
            }
        }

        /// <summary>
        /// 勝敗判定
        /// </summary>
        /// <param name="_ID"></param>
        /// <returns>1 勝ち 2 負け 0 引き分け</returns>
        public void WinOrLose(int _ID, ResultUI _resultObj)
        {
            // デフォルトは引き分け
            var result = 0;

            // IDに応じて勝敗判定
            if (_ID == 1)
            {
                // 勝ち
                if (playerScore[0] > playerScore[1]) {
                    result = 1;
                }
                // 負け
                else if (playerScore[0] < playerScore[1]) {
                    result = 2;
                }
            }
            else if (_ID == 2)
            {
                // 勝ち
                if (playerScore[1] > playerScore[0]) {
                    result = 1;
                }
                // 負け
                else if (playerScore[1] < playerScore[0]) {
                    result = 2;
                }
            }

            // ResultCanvasの処理
            var canvas = _resultObj.GetComponent<ResultUI>();
            canvas.SetScoreText(playerScore[0], playerScore[1]);
            canvas.ResultDisplay(result);
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