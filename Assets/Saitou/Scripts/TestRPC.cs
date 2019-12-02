using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TestRPC : MonoBehaviour
{
    [SerializeField] int _rpcValue;
    PhotonView _photonView;

    // Start is called before the first frame update
    void Start()
    {
        _photonView = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(_rpcValue))
        {
            _photonView.RPC(nameof(OutputLog), RpcTarget.All,TestOnlineData.PlayerID);
        }
    }

    [PunRPC]
    public void OutputLog(int _playerID)
    {
        Debug.Log("オブジェクト名　" +  gameObject.name +"  あたい  "+ _rpcValue + "  PlayerID  " + _playerID);
    }
}
