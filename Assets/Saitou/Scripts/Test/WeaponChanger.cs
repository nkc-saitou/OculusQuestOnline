using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using OVR;

namespace Saitou.Weapon
{
    public class WeaponChanger : MonoBehaviour
    {
        void Start()
        {
            // Right押した瞬間
            this.UpdateAsObservable()
                .TakeUntilDestroy(this)
                .Where(_ => IsGetDownRight())
                .Subscribe(_ =>
                {

                });

            // Left押した瞬間
            this.UpdateAsObservable()
                .TakeUntilDestroy(this)
                .Where(_ => IsGetDownLeft())
                .Subscribe(_ =>
                {

                });



            // Right押されていたら
            this.UpdateAsObservable()
                .TakeUntilDestroy(this)
                .Where(_ => IsGetRight())
                .Subscribe(_ =>
                {
                     
                });

            // Left押されていたら
            this.UpdateAsObservable()
                .TakeUntilDestroy(this)
                .Where(_ => IsGetLeft())
                .Subscribe(_ =>
                {
                     
                });



            // Right離した瞬間
            this.UpdateAsObservable()
                .TakeUntilDestroy(this)
                .Where(_ => IsGetUpRight())
                .Subscribe(_ =>
                {

                });

            // Left話した瞬間
            this.UpdateAsObservable()
                .TakeUntilDestroy(this)
                .Where(_ => IsGetUpLeft())
                .Subscribe(_ =>
                {

                });

        }

        public bool IsGetRight()
        {
            if (OVRInput.Get(OVRInput.RawButton.A) || OVRInput.Get(OVRInput.RawTouch.B)) return true;
            return false;
        }

        public bool IsGetLeft()
        {
            if (OVRInput.Get(OVRInput.RawTouch.X) || OVRInput.Get(OVRInput.RawTouch.Y)) return true;
            return false;
        }

        public bool IsGetDownRight()
        {
            if (OVRInput.GetDown(OVRInput.RawButton.A) || OVRInput.GetDown(OVRInput.RawTouch.B)) return true;
            return false;
        }

        public bool IsGetDownLeft()
        {
            if (OVRInput.GetDown(OVRInput.RawTouch.X) || OVRInput.GetDown(OVRInput.RawTouch.Y)) return true;
            return false;
        }

        public bool IsGetUpRight()
        {
            if (OVRInput.GetUp(OVRInput.RawButton.A) || OVRInput.GetUp(OVRInput.RawTouch.B)) return true;
            return false;
        }

        public bool IsGetUpLeft()
        {
            if (OVRInput.GetUp(OVRInput.RawTouch.X) || OVRInput.GetUp(OVRInput.RawTouch.Y)) return true;
            return false;
        }
    }
}