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
            hand.syncWeapon += (ID, Hand, weaponName) => {
                myPhotonView.RPC(nameof(PlayerGrab), RpcTarget.All, ID, Hand, weaponName);
            };

            hand.setOppesite += (ID, Hand, obj, weaponName) => {
                SetOpposite(ID, Hand, obj, weaponName);
            };

            hand.netDeleteWeapon += (ID, handName, Flag) => {
                SetDestroy(ID, handName, Flag);
            };
        }
    }

    /// <summary>
    /// 武器の同期
    /// </summary>
    /// <param name="_playerID"></param>
    /// <param name="_weaponName"></param>
    [PunRPC]
    public void PlayerGrab(int _playerID, string _handName, string _weaponName)
    {
        weapon = null;

        // IDで処理分け
        var handHash = new ExitGames.Client.Photon.Hashtable();
        handHash[_playerID + _handName] = _weaponName;
        PhotonNetwork.LocalPlayer.SetCustomProperties(handHash);
    }

    /// <summary>
    /// 反対の手の設定
    /// </summary>
    /// <param name="_hand">利き手</param>
    /// <param name="_weapon">武器</param>
    [PunRPC]
    private void SetOpposite(int _playerID, string _handName, GameObject _weapon, string _weaponName)
    {
        // IDで処理分け
        weapon = _weapon;
        var handHash = new ExitGames.Client.Photon.Hashtable();
        if (_handName == "_right") handHash[_playerID + "_left"] = _weaponName;
        else if (_handName == "_left") handHash[_playerID + "_right"] = _weaponName;
        PhotonNetwork.LocalPlayer.SetCustomProperties(handHash);
    }

    /// <summary>
    /// 武器の削除
    /// </summary>
    /// <param name="_hand">武器を所持している手</param>
    /// <param name="_playerID">ID</param>
    [PunRPC]
    private void SetDestroy(int _playerID, string _handName, bool _flag)
    {
        var handHash = new ExitGames.Client.Photon.Hashtable();
        handHash[_playerID + _handName] = "None";
        PhotonNetwork.LocalPlayer.SetCustomProperties(handHash);
    }

    /// <summary>
    /// カスタムPropertiesに変更があった際のCallback
    /// </summary>
    /// <param name="target"></param>
    /// <param name="changedProps">ハッシュテーブル</param>
    public override void OnPlayerPropertiesUpdate(Player target, ExitGames.Client.Photon.Hashtable changedProps)
    {
        // 更新されたキーと値のペアを、デバッグログに出力する
        foreach (var p in changedProps)
        {
            SetPlayerWeapon(p);
        }
    }

    /// <summary>
    /// 武器のセッティング
    /// </summary>
    /// <param name="_dic">ハッシュテーブル</param>
    private void SetPlayerWeapon(DictionaryEntry _dic)
    {
        // IDを抜き出す
        string[] hash = SplitID(_dic.Key.ToString());

        // 武器削除
        if (_dic.Value.ToString() == "None") {
            DeletePlayerWeapon(_dic);
            return;
        }

        // 右手の処理
        if (hash[1] == "right") {
            switch (hash[0])
            {
                case "1":
                    if (HandList[1].HasWeapon == false)
                        HandList[0].GrabWeapon(_dic.Value.ToString());
                    else if(HandList[1].HasWeapon && weapon != null)
                        HandList[0].SetWeapon(weapon);
                    else if(HandList[1].HasWeapon && weapon == null)
                        HandList[0].GrabWeapon(_dic.Value.ToString());
                    break;
                case "2":
                    if (HandList[3].HasWeapon == false)
                        HandList[2].GrabWeapon(_dic.Value.ToString());
                    else if (HandList[3].HasWeapon && weapon != null)
                        HandList[2].SetWeapon(weapon);
                    else if (HandList[3].HasWeapon && weapon == null)
                        HandList[2].GrabWeapon(_dic.Value.ToString());
                    break;
            }
        }
        // 左手の処理
        else if(hash[1] == "left") {
            switch (hash[0])
            {
                case "1":
                    if (HandList[0].HasWeapon == false)
                        HandList[1].GrabWeapon(_dic.Value.ToString());
                    else if (HandList[0].HasWeapon && weapon != null)
                        HandList[1].SetWeapon(weapon);
                    else if (HandList[0].HasWeapon && weapon == null)
                        HandList[1].GrabWeapon(_dic.Value.ToString());
                    break;
                case "2":
                    if (HandList[2].HasWeapon == false)
                        HandList[3].GrabWeapon(_dic.Value.ToString());
                    else if (HandList[2].HasWeapon && weapon != null)
                        HandList[3].SetWeapon(weapon);
                    else if (HandList[2].HasWeapon && weapon == null)
                        HandList[3].GrabWeapon(_dic.Value.ToString());
                    break;
            }
        }
    }

    /// <summary>
    /// 武器の削除
    /// </summary>
    /// <param name="_dic">ハッシュテーブル</param>
    private void DeletePlayerWeapon(DictionaryEntry _dic)
    {
        // IDを抜き出す
        string[] hash = SplitID(_dic.Key.ToString());

        // 右手の処理
        if (hash[1] == "right") {
            // IDで処理わけ
            switch (hash[0])
            {
                case "1":
                    HandList[0].DeleteWeapon();
                    break;
                case "2":
                    HandList[2].DeleteWeapon();
                    break;
            }
        }
        // 左手の処理
        else if (hash[1] == "left") {
            // IDで処理わけ
            switch (hash[0])
            {
                case "1":
                    HandList[1].DeleteWeapon();
                    break;
                case "2":
                    HandList[3].DeleteWeapon();
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
