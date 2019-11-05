using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nakajima.Weapon;

/// <summary>
/// プレイヤーハンドクラス
/// </summary>
namespace Nakajima.Player
{
    public class PlayerHand : MonoBehaviour
    {
        // WeaponCreateの参照
        private WeaponCreate weaponCreate;

        // どっちの手か
        [SerializeField]
        private OVRInput.RawButton myTouch;

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
        }

        void Update()
        {
            if (OVRInput.GetDown(myTouch))
            {
                if (HasWeapon) return;

                // 生成中の武器だったら装備する
                foreach (GameObject weapon in weaponCreate.createWeaponList)
                {
                    if (weapon.name == touchObj.name)
                    {
                        weapon.transform.parent = transform;
                        weapon.transform.localPosition = Vector3.zero;
                        weaponCreate.createWeaponList.Remove(weapon);
                        weaponCreate.DeleteWeapon();
                        HasWeapon = true;
                        break;
                    }
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

            var obj = _col.gameObject;
            if (touchObj != null && touchObj == obj) touchObj = null;
        }
    }
}