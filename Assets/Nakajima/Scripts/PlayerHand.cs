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
        private GameObject touchObj;

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

        void Update()
        {
            // 武器を掴む
            if (OVRInput.GetDown(myTouch))
            {
                if (HasWeapon) return;

                GetHasWeapon();
            }
        }

        /// <summary>
        /// 武器を掴む
        /// </summary>
        private void GetHasWeapon()
        {
            // 生成中の武器だったら装備する
            foreach (GameObject weapon in weaponCreate.createWeaponList)
            {
                if (weapon.name == touchObj.name)
                {
                    GameObject weaponObj = Instantiate(GunObj, transform);
                    weaponObj.transform.localPosition = Vector3.zero;
                    weaponObj.transform.localRotation = Quaternion.identity;
                    weaponCreate.DeleteWeapon();
                    HasWeapon = true;

                    weaponMgr.CreateWeapon("TestGun");
                    break;
                }
            }
        }

        /// <summary>
        /// 触れている状態
        /// </summary>
        /// <param name="_col"></param>
        void OnTriggerStay(Collider _col)
        {
            // 武器を持っているならリターン
            if (HasWeapon) return;

            var obj = _col.gameObject.GetComponent<ProvisionalWeapon>();
            if (obj == null) return;
            touchObj = _col.gameObject;
        }

        /// <summary>
        /// 離れた瞬間
        /// </summary>
        /// <param name="_col"></param>
        void OnTriggerExit(Collider _col)
        {
            // 武器を持っているならリターン
            if (HasWeapon) return;

            var obj = _col.gameObject.GetComponent<ProvisionalWeapon>();
            if (obj == null) return;

            if (touchObj != null && touchObj == obj) touchObj = null;
        }
    }
}