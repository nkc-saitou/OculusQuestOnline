using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using UniRx.Async;
using Photon.Pun;
using Saitou.Player;

namespace Saitou.Network
{
    public class TestPlayerCreate : MonoBehaviourPunCallbacks
    {

        NetworkTest _network;
        public Transform[] CreatePos;

        public GameObject playerPrefab;


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
                .Subscribe(_ =>
                {
                    Transform pos = CreatePos[TestOnlineData.PlayerID - 1];
                    //Debug.Log(TestOnlineData.PlayerID);
                    GameObject obj = Instantiate(playerPrefab, pos.position, pos.rotation);
                    GameObject displayObj = PhotonNetwork.Instantiate("DisplayPlayer", pos.position, pos.rotation);

                    SetDisplay setDisp = obj.GetComponent<SetDisplay>();
                    GameObject[] setPos = setDisp.Display;

                    for(int i = setPos.Length - 1; i >= 0; i--)
                    {
                        displayObj.transform.GetChild(i).transform.SetParent(setPos[i].transform, false);
                    }

                });
        }
    }
}