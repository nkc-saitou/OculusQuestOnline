using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nakajima.Player
{
    public class DisplayPlayerProvider : MonoBehaviour
    {
        // オブジェクトリスト
        private Dictionary<string, PlayerHand> handList = new Dictionary<string, PlayerHand>();
        private Dictionary<string, GameObject> objList = new Dictionary<string, GameObject>();

        // 自身のID
        public int MyID {
            set; get;
        }

        // 自身のオブジェクト
        [SerializeField]
        private GameObject myHead;
        [SerializeField]
        private GameObject myBody;

        // 自身のHand
        [SerializeField]
        private PlayerHand myHand_R;
        [SerializeField]
        private PlayerHand myHand_L;

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
            // Objectの登録
            objList.Add("Head", myHead);
            objList.Add("Body", myBody);
            
            // 自身の手
            myHand_R.GetMyProvider = this;
            handList.Add("Hand_R", myHand_R);
            myHand_L.GetMyProvider = this;
            handList.Add("Hand_L", myHand_L);
        }

        /// <summary>
        /// 自身のHandを返す
        /// </summary>
        /// <param name="_key">Key</param>
        /// <returns></returns>
        public PlayerHand GetMyHand(string _key)
        {
            if (handList.ContainsKey(_key)) {
                return handList[_key];
            }

            // なにもないならnull
            return null;
        }

        /// <summary>
        /// 自身のObjを返す
        /// </summary>
        /// <param name="_key">Key</param>
        /// <returns></returns>
        public GameObject GetMyObj(string _key)
        {
            if (objList.ContainsKey(_key)) {
                return objList[_key];
            }

            // なにもないならnull
            return null;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}