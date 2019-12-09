using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TestRPC : MonoBehaviour
{
    [SerializeField] int _playerID;
    PhotonView _photonView;

    // Start is called before the first frame update
    void Start()
    {
        _photonView = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(_playerID))
        {
            _photonView.RPC(nameof(OutputLog), RpcTarget.All, _playerID);
        }
    }

    [PunRPC]
    public void OutputLog(int _receiveID)
    {
        Debug.Log("オブジェクト名　" +  gameObject.name +"  自分の値  "+ _playerID + "  送られてきた値  " + _receiveID);
    }
}
