using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UniRx.Async;
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
            if (HasWeapon) return;

            // 生成中の武器だったら装備する
            // 掴んだなら他の武器は削除
            weaponCreate.DeleteWeapon();

            // 武器のデータを持ってくる
            weaponMgr.LoadWeapon();
            var handList = weaponMgr.CreateWeapon(GetWeaponName(hasObj.name), 0.5f);
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

            if(HasWeapon) DeleteWeapon(CheckDelete());
            hasObj = _weapon;
            hasObj.transform.parent = transform;
            hasObj.transform.localPosition = Vector3.zero;
            hasObj.transform.localRotation = Quaternion.identity;
            HasWeapon = true;
            weaponCreate.CanCreate = false;
            isBoth = true;
        }

        public override void WeaponAction(bool _getButton, bool _UpOrDown)
        {
            var weapon = hasObj.GetComponent<IWeapon>();

            // 武器使用
            if (_UpOrDown)
            {
                weapon.OnButtonUp(OVRInput.Button.One);
            }
            else
            {
                weapon.OnButtonDown(OVRInput.Button.One);
            }
        }

        /// <summary>
        /// 武器生成(まだ所持ではない)
        /// </summary>
        public async override void Create()
        {
            // 武器所持中なら武器を削除する
            if (isBoth) return;

            // 削除するまで待機
            if (HasWeapon) DeleteWeapon(CheckDelete());

            await UniTask.WaitUntil(() => hasObj == null);

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
            if (HasWeapon == false || hasObj == null) return false;

            // 両手武器の場合逆の手も削除する
            if (weaponMgr.HasOtherWeapon(GetWeaponName(hasObj.name))) return true;

            return false;
        }

        /// <summary>
        /// 武器の削除
        /// </summary>
        /// <param name="_flag">両手武器かどうか</param>
        public override void DeleteWeapon(bool _flag)
        {
            // なにもないならリターン
            if (HasWeapon == false) return;

            // 両手武器の場合
            if (_flag) deleteWeapon?.Invoke(this);

            // 削除
            var weapon = hasObj.GetComponent<IWeapon>();
            if (weapon == null) return;

            weapon.Destroy(0.5f);
            HasWeapon = false;
            isBoth = false;
            weaponCreate.Reset();
        }

        public override void DeleteWeapon()
        {
            // なにもないならリターン
            if (HasWeapon == false) return;

            // 削除
            var weapon = hasObj.GetComponent<IWeapon>();
            weapon.Destroy(0.5f);
            HasWeapon = false;
            isBoth = false;
            weaponCreate.Reset();
        }

        /// <summary>
        /// 触れている状態
        /// </summary>
        /// <param name="_col">コリジョン</param>
        private void OnTriggerStay(Collider _col)
        {
            // 武器を持っているならリターン
            if (HasWeapon) return;

            var obj = _col.gameObject.GetComponent<ProvisionalWeapon>();
            if (obj == null) return;
            hasObj = _col.gameObject;
        }
    }
}
