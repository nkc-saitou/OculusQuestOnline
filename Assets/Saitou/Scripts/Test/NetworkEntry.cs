using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;
using System;
using UniRx.Async;

namespace Saitou.Network
{

    public class NetworkEntry : MonoBehaviour
    {

        public Text PlayerCountText;
        public Text MatchingText;

        int matchingIndex = 0;

        // マッチングが完了したかどうか
        bool IsMatchingCompleate = false;

        // Start is called before the first frame update
        void Start()
        {

            TestOnlineData.PlayerCount
                .TakeUntilDestroy(this)
                .Subscribe(async _ =>
                {
                    PlayerCountText.text = TestOnlineData.PlayerCount.Value.ToString() + " / 2";

                    if(TestOnlineData.PlayerCount.Value >= 2)
                    {
                        IsMatchingCompleate = true;

                        await UniTask.Delay(500);

                        MatchingText.text = "CompleteMatching!";

                        await UniTask.Delay(1000);

                        SceneChanger.Instance.MoveScene("LobyTest", 1.0f, 1.0f, SceneChangeType.WhiteFade, true);
                    }
                });

            Observable.Interval(TimeSpan.FromMilliseconds(500))
                .Where(_ => IsMatchingCompleate == false)
                .Subscribe(_ => 
                {
                    MatchingText.text = "Matching";
                    for (int i = 0; i < matchingIndex; i++) MatchingText.text += ".";

                    if (matchingIndex < 3) matchingIndex++;
                    else matchingIndex = 0;
                });
        }
    }
}
