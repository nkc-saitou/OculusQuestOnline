using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using UniRx.Async;
using System;
using Photon.Pun;

namespace Saitou.Player
{
    public class TestPlayer : MonoBehaviourPunCallbacks
    {
        // Start is called before the first frame update
        void Start()
        {
            this.UpdateAsObservable()
                .TakeUntilDestroy(this)
                .Where(_ => Input.GetMouseButtonDown(0))
                .Subscribe(_ =>
                {

                });
        }
    }
}

