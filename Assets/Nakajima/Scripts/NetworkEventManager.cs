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

	private Dictionary<(int playerID, int eventID), System.Action<object>> _syncEventTable
		= new Dictionary<(int playerID, int eventID), System.Action<object>>();

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

            hand.setOppesite += (ID, obj, weaponName) => {
                SetOpposite(ID, obj, weaponName);
            };

            hand.deleteWeapon += (Hand, ID) => {
                SetDestroy(Hand, ID);
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
        Debug.Log( "ID : " + _playerID);
        // IDで処理分け
        var handHash = new ExitGames.Client.Photon.Hashtable();
        handHash[_playerID + "_left"] = _weaponName;
        PhotonNetwork.LocalPlayer.SetCustomProperties(handHash);
    }

    /// <summary>
    /// 反対の手の設定
    /// </summary>
    /// <param name="_hand">利き手</param>
    /// <param name="_weapon">武器</param>
    [PunRPC]
    private void SetOpposite(int _playerID, GameObject _weapon, string _weaponName)
    {
        Debug.Log("ID : " + _playerID);
        // IDで処理分け
        weapon = _weapon;
        var handHash = new ExitGames.Client.Photon.Hashtable();
        handHash[_playerID + "_right"] = _weaponName;
        PhotonNetwork.LocalPlayer.SetCustomProperties(handHash);
    }

    /// <summary>
    /// 武器の削除
    /// </summary>
    /// <param name="_hand">武器を所持している手</param>
    /// <param name="_playerID">ID</param>
    [PunRPC]
    private void SetDestroy(PlayerHand _hand, int _playerID)
    {
        var handHash = new ExitGames.Client.Photon.Hashtable();
        if(_hand.myTouch == OVRInput.RawButton.RHandTrigger) handHash[_playerID + "_right"] = "None";
        else handHash[_playerID + "_left"] = "None";
        PhotonNetwork.LocalPlayer.SetCustomProperties(handHash);
    }

	/// <summary>
	/// イベントを登録する
	/// </summary>
	/// <param name="playerID">呼び出されるときに判断するプレイヤーID</param>
	/// <param name="eventID">呼び出すためのイベント番号(固有)</param>
	/// <param name="action">実行されるイベント</param>
	public void AddSyncEvent(int playerID, int eventID, System.Action<object> action) {
		_syncEventTable.Add((playerID, eventID), action);
	}

	/// <summary>
	/// 全プレイヤーにイベントを実行させる
	/// </summary>
	/// <param name="eventID">呼び出すイベント番号(固有)</param>
	public void CallSyncEvent(int eventID, object data) {
		myPhotonView.RPC(nameof(ExecSyncEvent), RpcTarget.All, TestOnlineData.PlayerID, eventID, data);
	}

	/// <summary>
	/// 全プレイヤーにイベントを実行させる
	/// </summary>
	/// <param name="eventID">呼び出すイベント番号(固有)</param>
	[PunRPC]
	private void ExecSyncEvent(int playerID, int eventID, object data) {

		var eventPair = (playerID, eventID);
		// eventPairで参照するイベントは必ず存在するはず
		if(_syncEventTable.ContainsKey(eventPair)) {
			_syncEventTable[eventPair]?.Invoke(data);
		}
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

        var handL = PhotonNetwork.LocalPlayer.CustomProperties[hash[0] + "_left"];
        Debug.Log(handL.ToString());
        var handR = PhotonNetwork.LocalPlayer.CustomProperties[hash[0] + "_right"];
        Debug.Log(_dic.Value.ToString());

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
                    else
                        HandList[0].SetWeapon(weapon);
                    break;
                case "2":
                    if (HandList[3].HasWeapon == false)
                        HandList[2].GrabWeapon(_dic.Value.ToString());
                    else
                        HandList[2].SetWeapon(weapon);
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
                    else
                        HandList[1].SetWeapon(weapon);
                    break;
                case "2":
                    if (HandList[2].HasWeapon == false)
                        HandList[3].GrabWeapon(_dic.Value.ToString());
                    else
                        HandList[3].SetWeapon(weapon);
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

        var handL = PhotonNetwork.LocalPlayer.CustomProperties[hash[0] + "_left"];
        var handR = PhotonNetwork.LocalPlayer.CustomProperties[hash[0] + "_right"];

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
