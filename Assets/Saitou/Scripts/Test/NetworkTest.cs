using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UniRx;
using System;
using UniRx.Triggers;
using UniRx.Async;
using UnityEngine.UI;


namespace Saitou.Network
{
    public class NetworkTest : MonoBehaviourPunCallbacks
    {
        // 最大人数
        [SerializeField] int maxPlayers = 4;

        // ルームの基本設定
        RoomOptions roomOptions;
        public ReactiveProperty<bool> IsInRoom { get; private set; } = new ReactiveProperty<bool>();

        Subject<Unit> onInRoomSub = new Subject<Unit>();
        public IObservable<Unit> OnInRoom { get { return onInRoomSub; } }

        public Text text;
        public Text playerText;

        string roomName = "myRoomName";

        void Start()
        {
            IsInRoom.Value = false;
            // Photonに接続する(引数でゲームのバージョンを指定できる)
            Connect("1.0");

            InitRoomSetting();

            text.text = "未接続";

            // 入っているかどうかを確認したい
            IsInRoom.Subscribe(_ => Debug.Log("isOpen" + IsInRoom.Value));

            this.UpdateAsObservable()
                .TakeUntilDestroy(this)
                .Where(_ => IsInRoom.Value)
                .Subscribe(_ => 
                {
                    playerText.text = PhotonNetwork.PlayerList.Length.ToString();
                });
        }

        /// <summary>
        /// ルームの設定
        /// </summary>
        void InitRoomSetting()
        {
            // ルームオプションの基本設定
            RoomOptions roomOptions = new RoomOptions
            {
                // 部屋の最大人数
                MaxPlayers = (byte)maxPlayers,

                // 公開
                IsVisible = true,

                // 入室可
                IsOpen = true
            };
        }

        // Photonに接続する
        void Connect(string gameVersion)
        {
            if (PhotonNetwork.IsConnected == false)
            {
                PhotonNetwork.GameVersion = gameVersion;
                PhotonNetwork.ConnectUsingSettings();
            }
        }

        // ロビーに入る
        void JoinLobby()
        {
            if (PhotonNetwork.IsConnected == false) return;
            PhotonNetwork.JoinLobby();
        }

        //------------------------------------------------
        // Photon
        //------------------------------------------------


        /// <summary>
        /// マスターサーバーに接続した時
        /// </summary>
        public override void OnConnectedToMaster()
        {
            Debug.Log("OnConnectedToMaster");

            text.text = "サーバー接続中";
            // ロビーに入る
            JoinLobby();
        }

        /// <summary>
        /// ロビーに入ると呼ばれる
        /// </summary>
        public override void OnJoinedLobby()
        {
            Debug.Log("ロビーに入りました。");
            text.text = "ロビー接続接続中";
            JoinOrCreateRoom().Forget();
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("ルームへ入室しました。");

            TestOnlineData.PlayerID = PhotonNetwork.PlayerList.Length;

            text.text = "ルーム入室済";
            onInRoomSub.OnNext(Unit.Default);
        }

        /// <summary>
        ///  2. 部屋に入室する （存在しなければ作成して入室する）
        /// </summary>
        public async UniTask JoinOrCreateRoom()
        {
            IsInRoom.Value = true;

            // 入室 (存在しなければ部屋を作成して入室する)
            if (PhotonNetwork.InLobby)
            {
                await UniTask.WaitUntil(() => PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default));
            }
        }
    }
}