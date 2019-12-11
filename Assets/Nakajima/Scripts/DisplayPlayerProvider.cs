using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nakajima.Player
{
    public class DisplayPlayerProvider : MonoBehaviour
    {
        // オブジェクトリスト
        private Dictionary<string, PlayerHand> objList = new Dictionary<string, PlayerHand>();

        // 自身のID
        public int MyID {
            set; get;
        }

        // 自身のオブジェクト
        [SerializeField]
        GameObject myHead;
        [SerializeField]
        PlayerHand myHand_R;
        [SerializeField]
        PlayerHand myHand_L;

        // Start is called before the first frame update
        void Start()
        {
            Register();
        }

        /// <summary>
        /// 自身のObjを登録
        /// </summary>
        private void Register()
        {
            myHand_R.myProvider = this;
            objList.Add("Hand_R", myHand_R);
            myHand_L.myProvider = this;
            objList.Add("Hand_L", myHand_L);
        }

        /// <summary>
        /// 自身のObjを返す
        /// </summary>
        /// <param name="_key">Key</param>
        /// <returns></returns>
        public PlayerHand GetMyHand(string _key)
        {
            Debug.Log("指定したオブジェクトがあるかどうか" + objList.ContainsKey(_key));
            if (objList.ContainsKey(_key)) {
                return objList[_key];
            }

            return null;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}