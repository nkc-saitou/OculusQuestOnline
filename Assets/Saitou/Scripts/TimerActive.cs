using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nakajima.Main;

namespace Saitou.GameScene
{
    public class TimerActive : MonoBehaviour
    {

        [SerializeField] MainManager _mainManager;

        [SerializeField] GameObject[] _timeUI;

        // Start is called before the first frame update
        void Start()
        {
            _mainManager.battleEnd = (end) =>
            {
                for(int i = 0; i < _timeUI.Length; i++)
                {
                    _timeUI[i].SetActive(false);
                }
            };
        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }
}
