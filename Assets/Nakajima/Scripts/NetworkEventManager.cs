﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;
using Photon.Realtime;
using Nakajima.Player;
using Saitou.Network;

public class NetworkEventManager : MonoBehaviourPunCallbacks
{
    // 手の情報
    ExitGames.Client.Photon.Hashtable hand;

    // Hand
    PlayerHand[] HandList = new PlayerHand[4];
    private GameObject weapon;

    private PhotonView myPhotonView;

    // オンライン用のプレイヤーの生成
    private TestPlayerCreate testPlayerCreate;

    // Start is called before the first frame update
    void Start()
    {
        myPhotonView = GetComponent<PhotonView>();
        testPlayerCreate = FindObjectOfType<TestPlayerCreate>();

        hand = new ExitGames.Client.Photon.Hashtable();
        hand["1_right"] = "";
        hand["1_left"] = "";
        hand["2_right"] = "";
        hand["2_left"] = "";

        testPlayerCreate.OnPlayerCreate += (myHandArray) =>
        {
            DisplayPlayerProvider[] playerList = FindObjectsOfType<DisplayPlayerProvider>();
            Debug.Log("プレイヤー  " + playerList.Length);
            foreach(var player in playerList) {
                switch (player.MyID)
                {
                    case 0:
                        if(TestOnlineData.PlayerID == 1) {
                            player.MyID = 2;
                            HandList[2] = player.GetMyHand("Hand_R");
                            HandList[3] = player.GetMyHand("Hand_L");
                        }
                        else {
                            player.MyID = 1;
                            HandList[0] = player.GetMyHand("Hand_R");
                            HandList[1] = player.GetMyHand("Hand_L");
                        }
                        break;
                    case 1:
                        HandList[0] = player.GetMyHand("Hand_R");
                        HandList[1] = player.GetMyHand("Hand_L");
                        break;
                    case 2:
                        HandList[2] = player.GetMyHand("Hand_R");
                        HandList[3] = player.GetMyHand("Hand_L");
                        break;
                }
            }
            EventBind(HandList);
        };
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// イベントにバインド
    /// </summary>
    public void EventBind(PlayerHand[] _hand)
    {
        //HandList.Add(_hand);

        foreach(var hand in _hand)
        {
            hand.syncWeapon += (ID, weaponName) => {
                myPhotonView.RPC(nameof(PlayerGrab), RpcTarget.All, ID, weaponName);
            };

            hand.setOppesite += SetOpposite;
        }
    }

    /// <summary>
    /// 武器の同期
    /// </summary>
    /// <param name="_playerID"></param>
    /// <param name="_weaponName"></param>
    [PunRPC]
    public void PlayerGrab(int _playerID, string _weaponName)
    {
        // IDで処理分け
        hand[_playerID + "_left"] = _weaponName;
        PhotonNetwork.LocalPlayer.SetCustomProperties(hand);
    }

    /// <summary>
    /// 反対の手の設定
    /// </summary>
    /// <param name="_hand">利き手</param>
    /// <param name="_weapon">武器</param>
    [PunRPC]
    private void SetOpposite(int _playerID, GameObject _weapon, string _weaponName)
    {
        weapon = _weapon;
        hand[_playerID + "_right"] = _weaponName;
        PhotonNetwork.LocalPlayer.SetCustomProperties(hand);
    }

    /// <summary>
    /// カスタムPropertiesに変更があった際のCallback
    /// </summary>
    /// <param name="target"></param>
    /// <param name="changedProps"></param>
    public override void OnPlayerPropertiesUpdate(Player target, ExitGames.Client.Photon.Hashtable changedProps)
    {
        // 更新されたキーと値のペアを、デバッグログに出力する
        foreach (var p in changedProps)
        {
            SetPlayerWeapon(p);
        }
    }

    private void SetPlayerWeapon(DictionaryEntry _dic)
    {
        // IDを抜き出す
        string[] hash = SplitID(_dic.Key.ToString());
        Debug.Log(hash[1]);

        if(hash[1] == "right")
        {
            switch (hash[0])
            {
                case "1":
                    if (_dic.Value != hand[hash[0] + "_left"])
                        HandList[0].GrabWeapon(_dic.Value.ToString());
                    else
                        HandList[0].SetWeapon(weapon);
                    break;
                case "2":
                    if (_dic.Value != hand[hash[0] + "_left"])
                        HandList[2].GrabWeapon(_dic.Value.ToString());
                    else
                        HandList[2].SetWeapon(weapon);
                    break;
            }
        }
        else if(hash[1] == "left")
        {
            switch (hash[0])
            {
                case "1":
                    if (_dic.Value != hand[hash[0] + "_right"])
                        HandList[1].GrabWeapon(_dic.Value.ToString());
                    else
                        HandList[1].SetWeapon(weapon);
                    break;
                case "2":
                    if (_dic.Value != hand[hash[0] + "_right"])
                        HandList[3].GrabWeapon(_dic.Value.ToString());
                    else
                        HandList[3].SetWeapon(weapon);
                    break;
            }
        }
        
    }

    /// <summary>
    /// IDを抜き出す
    /// </summary>
    /// <param name="_name"></param>
    /// <returns></returns>
    private string[] SplitID(string _name)
    {
        // ID抜き出し
        string[] hash = _name.Split('_');
        return hash;
    }
}
