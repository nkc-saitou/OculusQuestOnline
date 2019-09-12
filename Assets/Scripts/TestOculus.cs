using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System;
using UniRx.Async;
using Photon.Pun;
using Photon.Realtime;
using Saitou.Network;
using UnityEngine.UI;

namespace Saitou.OculusInput
{
    public class TestOculus : MonoBehaviour, Photon.Pun.IPunObservable
    {
        NetworkTest network;

        public Text text;

        // Start is called before the first frame update
        void Start()
        {
            network = FindObjectOfType<NetworkTest>();

            network.OnInRoom
                .TakeUntilDestroy(this)
                .Subscribe(_ => 
                {
                    Debug.Log("にゅーしゅつ！処理できます");
                });

            this.UpdateAsObservable()
                .TakeUntilDestroy(this)
                .Where(_ => network.IsInRoom.Value)
                .Subscribe(_ => 
                {
                    if (OVRInput.GetDown(OVRInput.RawButton.A) || Input.GetMouseButtonDown(0))
                    {
                        text.text = "Aボタン";
                    }
                    if (OVRInput.GetDown(OVRInput.RawButton.B) || Input.GetMouseButtonDown(1))
                    {
                        text.text = "Bボタン";
                    }
                    if (OVRInput.GetDown(OVRInput.RawButton.X))
                    {
                        text.text = "Xボタン";
                    }
                    if (OVRInput.GetDown(OVRInput.RawButton.Y))
                    {
                        text.text = "Yボタン";
                    }
                    if (OVRInput.GetDown(OVRInput.RawButton.Start))
                    {
                        text.text = "メニューボタン";
                    }

                    if (OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger))
                    {
                        text.text = "右人差し指トリガー";
                    }
                    if (OVRInput.GetDown(OVRInput.RawButton.RHandTrigger))
                    {
                        text.text = "右中指トリガー";
                    }
                    if (OVRInput.GetDown(OVRInput.RawButton.LIndexTrigger))
                    {
                        text.text = "左人差し指トリガー";
                    }
                    if (OVRInput.GetDown(OVRInput.RawButton.LHandTrigger))
                    {
                        text.text = "左中指トリガー";
                    }
                });
                

        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(text.text);
            }
            else
            {
                this.text.text = (string)stream.ReceiveNext();
            }
        }
    }
}
