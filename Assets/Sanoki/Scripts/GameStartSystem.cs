using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UniRx.Async;
using UniRx.Triggers;
using UniRx;

namespace Sanoki.Online
{
    public class GameStartSystem : MonoBehaviourPunCallbacks
    {
        PhotonView photonView;
        public NetworkManager networkManager;
        bool[] isStart = new bool[2];

        public Text[] flgText = new Text[2];
        
        private void Start()
        {
            networkManager = FindObjectOfType<NetworkManager>();
            photonView = networkManager.GetComponent<PhotonView>();

            // すべてのクライアントが準備完了したら、ゲームシーンに移行
            this.UpdateAsObservable()
                .TakeUntilDestroy(this)
                .Where(_ => isCompleted())
                .Take(1)
                .Subscribe(_ => 
                {
                    SceneChange();
                });
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.G)) Entry();
            if (Input.GetKeyDown(KeyCode.F))
            {
                networkManager.JoinOrCreateRoom();
            }
            flgText[0].text = "マスタークライアント：" + isStart[0].ToString();
            flgText[1].text = "その他：" + isStart[1].ToString();


        }
        
        /// <summary>
        /// マスタークライアント以外の参加判定
        /// </summary>
        [PunRPC]
        void OtherClientEntry()
        {
            isStart[1] = true;
        }

        /// <summary>
        /// 参加判定
        /// </summary>
        public void Entry()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                photonView.RPC(nameof(OtherClientEntry), RpcTarget.All);
            }
            else isStart[0] = true;
        }

        /// <summary>
        /// Scene移動処理
        /// </summary>
        [PunRPC]
        void SceneChange()
        {
            SceneManager.LoadScene("Online_test");
        }

        /// <summary>
        /// 全クライアントが準備できたかを確認
        /// </summary>
        /// <returns>準備できていたらtrue</returns>
        bool isCompleted()
        {
            for(int i = 0; i < isStart.Length; i++)
            {
                if (!isStart[i]) return false;
            }
            return true;
        }
    }
}
