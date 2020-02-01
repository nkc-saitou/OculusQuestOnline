using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Saitou.Network;
using UniRx;
using UniRx.Triggers;
using UniRx.Async;
using Nakajima.Main;

namespace Saitou.GameScene
{
    public class GameStartDirecting : MonoBehaviour
    {
    
        [SerializeField]TestPlayerCreate _playerCreate;

        [SerializeField] Animator[] _startUI;
        [SerializeField] CountUI _countUI;


        [SerializeField] MainManager _mainManager;

        void Start()
        {
            _playerCreate.OnPlayerCreate = (tmp) => 
            {
                CountStart().Forget();
            };


            _countUI.CountEnd += () =>
            {
                _mainManager.battleStart(60.0f);
            };

        }

        async UniTask CountStart()
        {
            await UniTask.Delay(500);
            foreach (Animator anim in _startUI)
            {
                anim.SetTrigger("IsStart");
            }

            _mainManager.Ready = true;
        }
    }
}
