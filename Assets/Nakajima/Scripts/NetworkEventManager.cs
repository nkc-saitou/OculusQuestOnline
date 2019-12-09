using System.Collections;
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
    //public List<PlayerHand> HandList = new List<PlayerHand>();

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
                            HandList[2] = player.GetMyObj("Hand_R").GetComponent<PlayerHand>();
                            HandList[3] = player.GetMyObj("Hand_L").GetComponent<PlayerHand>();
                        }
                        else {
                            player.MyID = 1;
                            HandList[0] = player.GetMyObj("Hand_R").GetComponent<PlayerHand>();
                            HandList[1] = player.GetMyObj("Hand_L").GetComponent<PlayerHand>();
                        }
                        break;
                    case 1:
                        HandList[0] = player.GetMyObj("Hand_R").GetComponent<PlayerHand>();
                        HandList[1] = player.GetMyObj("Hand_L").GetComponent<PlayerHand>();
                        break;
                    case 2:
                        HandList[2] = player.GetMyObj("Hand_R").GetComponent<PlayerHand>();
                        HandList[3] = player.GetMyObj("Hand_L").GetComponent<PlayerHand>();
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
        var handHash = new ExitGames.Client.Photon.Hashtable();
        handHash[_playerID + "_left"] = _weaponName;
        PhotonNetwork.LocalPlayer.SetCustomProperties(handHash);
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
            // IDを抜き出す
            string ID = SplitID(p.Key.ToString());

            switch (ID)
            {
                case "1":
                    HandList[1].GrabWeapon(p.Value.ToString());
                    break;
                case "2":
                    HandList[3].GrabWeapon(p.Value.ToString());
                    break;
            }
        }
    }

    private string SplitID(string _name)
    {
        // ID抜き出し
        string ID = _name.Split('_')[0];
        if (ID != "") return ID;

        return "";
    }
}
