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
    public class EntrySystem : MonoBehaviourPunCallbacks
    {
        PhotonView photonView;
        NetworkManager networkManager;
        bool[] isStart = new bool[2];

        public bool[] IsStart()
        {
            return isStart;
        }

        private void Start()
        {
            networkManager = FindObjectOfType<NetworkManager>();
            photonView = GetComponent<PhotonView>();

            // すべてのクライアントが準備完了したら、ゲームシーンに移行
            this.UpdateAsObservable()
                .TakeUntilDestroy(this)
                .Where(_ => isCompleted())
                .Take(1)
                .Subscribe(_ => 
                {
                    photonView.RPC(nameof(SceneChange), RpcTarget.All);
                });
        }

        private void Update()
        {
            

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
            SceneChanger.Instance.MoveScene("TestSaito", 1.0f, 1.0f, SceneChangeType.BlackFade, true);
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
