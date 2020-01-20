using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Photon.Pun;
using Nakajima.Weapon;
using Matsumoto.Weapon;

namespace Nakajima.Player
{
    public class DemoHand : HandMaster
    {
        public event Action<HandMaster, GameObject> oppositeWeapon;
        public event Action<HandMaster> deleteWeapon;

        /// <summary>
        /// 初回処理
        /// </summary>
        public override void Start()
        {
            HasWeapon = false;
            weaponCreate = GetComponent<WeaponCreate>();
            weaponMgr = FindObjectOfType<WeaponManager>();
        }

        /// <summary>
        /// 更新処理
        /// </summary>
        public override void Update()
        {
            // 武器所持時のみ実行
            if (HasWeapon) {
                WeaponAction();
                return;
            }

            // 武器を掴む
            if (OVRInput.GetDown(myTouch) && HasWeapon == false) GrabWeapon();
        }

        /// <summary>
        /// 武器を手にセットする
        /// </summary>
        private void SetHandWeapon()
        {
            // 何も触れていないならリターン
            if (hasObj == null) return;
        }

        /// <summary>
        /// 武器を掴む(ローカル)
        /// </summary>
        public override void GrabWeapon()
        {
            // 何も触れていないならリターン
            if (hasObj == null) return;

            // 生成中の武器だったら装備する
            // 掴んだなら他の武器は削除
            weaponCreate.DeleteWeapon();

            // 武器のデータを持ってくる
            weaponMgr.LoadWeapon();
            var handList = weaponMgr.CreateWeapon(GetWeaponName(hasObj.name));
            hasObj = handList[0].GetBody();
            hasObj.transform.parent = transform;
            hasObj.transform.localPosition = Vector3.zero;
            hasObj.transform.localRotation = Quaternion.identity;

            HasWeapon = true;
            weaponCreate.CanCreate = false;

            // 同期したい
            Debug.Log("ID ; " + TestOnlineData.PlayerID + " 通過");

            // 両手武器の場合装備
            if (weaponMgr.HasOtherWeapon(GetWeaponName(hasObj.name)))
            {
                var obj = handList[1].GetBody();
                oppositeWeapon?.Invoke(this, obj);
            }
        }

        /// <summary>
        /// 武器をセットする(両手武器用)
        /// </summary>
        /// <param name="_weapon">セットする武器オブジェクト</param>
        public override void SetWeapon(GameObject _weapon)
        {
            if (HasWeapon && hasObj == _weapon) return;

            DeleteWeapon(true);
            hasObj = _weapon;
            hasObj.transform.parent = transform;
            hasObj.transform.localPosition = Vector3.zero;
            hasObj.transform.localRotation = Quaternion.identity;
            HasWeapon = true;
        }

        /// <summary>
        /// 武器生成(まだ所持ではない)
        /// </summary>
        public override void Create()
        {
            weaponCreate.ActiveHand = this;
            weaponCreate.Create();
        }

        /// <summary>
        /// 武器の削除
        /// </summary>
        /// <param name="_flag">逆の手をチェックするか</param>
        /// <returns></returns>
        public override bool CheckDelete()
        {
            // 武器を所持していないならfalse
            if (HasWeapon == false) return false;

            // 両手武器の場合逆の手も削除する
            if(weaponMgr.HasOtherWeapon(GetWeaponName(hasObj.name))) {
                deleteWeapon?.Invoke(this);
            }

            // 削除
            Destroy(hasObj);
            HasWeapon = false;
            weaponCreate.Reset();
            return true;
        }

        public override void DeleteWeapon()
        {
            // なにもないならリターン
            if (HasWeapon == false) return;

            // 削除
            Destroy(hasObj);
            HasWeapon = false;
            weaponCreate.Reset();
        }

        /// <summary>
        /// 触れている状態
        /// </summary>
        /// <param name="_col">コリジョン</param>
        void OnTriggerStay(Collider _col)
        {
            // 武器を持っているならリターン
            if (HasWeapon) return;

            var obj = _col.gameObject.GetComponent<ProvisionalWeapon>();
            if (obj == null) return;
            hasObj = _col.gameObject;
        }

        /// <summary>
        /// 離れた瞬間
        /// </summary>
        /// <param name="_col">コリジョン</param>
        void OnTriggerExit(Collider _col)
        {
            // 武器を持っているならリターン
            if (HasWeapon) return;

            var obj = _col.gameObject.GetComponent<ProvisionalWeapon>();
            if (obj == null) return;
        }
    }
}
