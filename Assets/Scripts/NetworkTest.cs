using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UniRx;
using System;
using UniRx.Triggers;


namespace Saitou.Network
{
    public class NetworkTest : MonoBehaviourPunCallbacks
    {
        // 最大人数
        [SerializeField] int maxPlayers = 2;

        // ルームの基本設定
        RoomOptions roomOptions;

        // 現在部屋に入っているかどうか
        ReactiveProperty<bool> isInRoom = new ReactiveProperty<bool>();

        

        void Start()
        {
            isInRoom.Value = false;
            // Photonに接続する(引数でゲームのバージョンを指定できる)
            PhotonNetwork.ConnectUsingSettings();

            InitRoomSetting();

            // 入っているかどうかを確認したい
            isInRoom.Subscribe(_ => Debug.Log("isOpen" + isInRoom.Value));
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

        // ロビーに入ると呼ばれる
        public override void OnJoinedLobby()
        {
            Debug.Log("ロビーに入りました。");

            // ルームに入室する
            PhotonNetwork.JoinRandomRoom();
        }

        // ルームに入室すると呼ばれる
        public override void OnJoinedRoom()
        {
            Debug.Log("ルームへ入室しました。");

            isInRoom.Value = true;
        }

        // ルームの入室に失敗すると呼ばれる
        void OnPhotonRandomJoinFailed()
        {
            Debug.Log("ルームの入室に失敗しました。");

            // ルームがないと入室に失敗するため、その時は自分で作る
            // 引数でルーム名を指定できる
            PhotonNetwork.CreateRoom("myRoomName", roomOptions);
        }
    }
}