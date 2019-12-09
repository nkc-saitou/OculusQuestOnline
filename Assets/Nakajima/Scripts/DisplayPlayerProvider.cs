using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nakajima.Player
{
    public class DisplayPlayerProvider : MonoBehaviour
    {
        // オブジェクトリスト
        private Dictionary<string, GameObject> objList = new Dictionary<string, GameObject>();

        // 自身のID
        public int MyID {
            set; get;
        }

        // 自身のオブジェクト
        [SerializeField]
        GameObject myHead;
        [SerializeField]
        GameObject myHand_R;
        [SerializeField]
        GameObject myHand_L;

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
            objList.Add("Head", myHead);
            objList.Add("Hand_R", myHand_R);
            objList.Add("Hand_L", myHand_L);
        }

        /// <summary>
        /// 自身のObjを返す
        /// </summary>
        /// <param name="_key">Key</param>
        /// <returns></returns>
        public GameObject GetMyObj(string _key)
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