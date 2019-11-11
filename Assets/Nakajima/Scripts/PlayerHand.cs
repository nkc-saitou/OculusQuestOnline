using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nakajima.Weapon;
using Matsumoto.Weapon;

/// <summary>
/// プレイヤーハンドクラス
/// </summary>
namespace Nakajima.Player
{
    public class PlayerHand : MonoBehaviour
    {
        // WeaponCreateの参照
        private WeaponCreate weaponCreate;
        private WeaponManager weaponMgr;

        // どっちの手か
        [SerializeField]
        private OVRInput.RawButton myTouch;

        [SerializeField]
        private GameObject GunObj;

        // 触れたオブジェクト
        private GameObject hasObj;

        // 武器を所持しているか
        public bool HasWeapon {
            get; private set;
        }

        /// <summary>
        /// 初回処理
        /// </summary>
        void Start()
        {
            HasWeapon = false;
            weaponCreate = FindObjectOfType<WeaponCreate>();
            weaponMgr = FindObjectOfType<WeaponManager>();
        }

        /// <summary>
        /// 更新処理
        /// </summary>
        void Update()
        {
            // 武器を掴む
            if (OVRInput.GetDown(myTouch))
            {
                if (HasWeapon) return;

                GetHasWeapon(); 
            }

            WeaponAction();
        }

        /// <summary>
        /// 武器を掴む
        /// </summary>
        private void GetHasWeapon()
        {
            // 何も触れていないならリターン
            if (hasObj == null) return;

            // 生成中の武器だったら装備する
            foreach (GameObject weapon in weaponCreate.createWeaponList)
            {
                if (weapon.name == hasObj.name)
                {
                    // 掴んだなら他の武器は削除
                    weaponCreate.DeleteWeapon();
                    weaponCreate.WeaponUnfold = false;

                    // 武器のデータを持ってくる
                    weaponMgr.LoadWeapon();
                    hasObj = weaponMgr.CreateWeapon(GetWeaponName(weapon.name)).GetBody();
                    hasObj.transform.parent = transform;
                    hasObj.transform.localPosition = Vector3.zero;
                    hasObj.transform.localRotation = Quaternion.identity;

                    HasWeapon = true;
                    break;
                }
            }
        }

        /// <summary>
        /// 武器の名前を抜き出す
        /// </summary>
        /// <param name="_objName">武器のオブジェクト</param>
        /// <returns>武器の名前</returns>
        private string GetWeaponName(string _objName)
        {
            // オブジェクト名から(Clone)を抜く
            string[] weaponName = _objName.Split('(');
            return weaponName[0];
        }

        /// <summary>
        /// 武器ごとのアクション実行
        /// </summary>
        private void WeaponAction()
        {
            // 武器がないならリターン
            if (HasWeapon == false) return;

            var weapon = hasObj.GetComponent<IWeapon>();

            // 武器使用
            switch (myTouch) {
                case OVRInput.RawButton.RHandTrigger:
                    if (OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger)) {
                        weapon.OnButtonDown(OVRInput.Button.One);
                    }
                    break;
                case OVRInput.RawButton.LHandTrigger:
                    if (OVRInput.GetDown(OVRInput.RawButton.LIndexTrigger)) {
                        weapon.OnButtonDown(OVRInput.Button.One);
                    }
                    break;
            }
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