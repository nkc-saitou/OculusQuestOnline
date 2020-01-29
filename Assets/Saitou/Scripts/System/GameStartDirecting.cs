using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Saitou.Network;
using UniRx;
using UniRx.Triggers;
using UniRx.Async;

namespace Saitou.GameScene
{
    public class GameStartDirecting : MonoBehaviour
    {
    
        [SerializeField]TestPlayerCreate _playerCreate;

        [SerializeField] Animator[] _startUI;
        [SerializeField] CountUI _countUI;

        async void Start()
        {
            _playerCreate.OnPlayerCreate = (tmp) => 
            {
                foreach(Animator anim in _startUI)
                {
                    anim.SetTrigger("IsStart");
                }
            };


            await _countUI.CountStart();

        }
    }
}
