using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using UniRx.Async;
using Photon.Pun;
using Saitou.Player;
using System;
using Nakajima.Player;

namespace Saitou.Network
{
    public class TestPlayerCreate : MonoBehaviourPunCallbacks
    {

        NetworkTest _network;
        public Transform[] CreatePos;

        public GameObject playerPrefab;

        public Action<PlayerHand[]> OnPlayerCreate;

        void Start()
        {
            _network = FindObjectOfType<NetworkTest>();

            _network.OnInRoom
                .Subscribe(_ =>
                {

                });


            this.UpdateAsObservable()
                .TakeUntilDestroy(this)
                .Where(_ => PhotonNetwork.PlayerList.Length >= 2)
                .Take(1)
                .Subscribe(async _ =>
                {
                    Transform pos = CreatePos[TestOnlineData.PlayerID - 1];
                    //Debug.Log(TestOnlineData.PlayerID);
                    // オブジェクトの生成
                    GameObject obj = Instantiate(playerPrefab, pos.position, pos.rotation);
                    GameObject displayObj = PhotonNetwork.Instantiate("DisplayPlayer", pos.position, pos.rotation);
                    displayObj.GetComponent<DisplayPlayerProvider>().MyID = TestOnlineData.PlayerID;

                    SetDisplay setDisp = obj.GetComponent<SetDisplay>();
                    GameObject[] setPos = setDisp.Display;

                    // 生成オブジェクトの手を格納する配列
                    PlayerHand[] setWeaponHandArray = new PlayerHand[2];

                    for(int i = setPos.Length - 1; i >= 0; i--)
                    {
                        // オブジェクトにPlayerHandが付いていたら、そのオブジェクトを武器に知らせる
                        PlayerHand tmp = displayObj.transform.GetChild(i).GetComponent<PlayerHand>();

                        // オブジェクトの親を変更
                        // PlayerHandの処理を先にしないと、親子関係の数が狂っておかしくなるため、取得を先にする
                        displayObj.transform.GetChild(i).transform.SetParent(setPos[i].transform, false);

                        // PlayerHandがついていなければここで処理を終了
                        if (tmp == null) continue;

                        // 0が右手、１が左手
                        if (tmp.myTouch == OVRInput.RawButton.RHandTrigger) setWeaponHandArray[0] = tmp;
                        else setWeaponHandArray[1] = tmp;
                    }

                    // Playerの生成を通知
                    await UniTask.Delay(1000);

                    // イベントを発行
                    OnPlayerCreate?.Invoke(setWeaponHandArray);

                });
        }
    }
}