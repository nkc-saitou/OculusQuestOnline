using UnityEngine;
using System.Collections;
using System;
using UniRx;

namespace Matsumoto.Weapon {

	public class WeaponOtherInput : WeaponBase {

		private Subject<OVRInput.Button> _onButton = new Subject<OVRInput.Button>();
		public IObservable<OVRInput.Button> OnButtonRecieved {
			get { return _onButton; }
		}

		private Subject<OVRInput.Button> _onButtonDown = new Subject<OVRInput.Button>();
		public IObservable<OVRInput.Button> OnButtonDownRecieved {
			get { return _onButtonDown; }
		}

		private Subject<OVRInput.NearTouch> _onButtonNear = new Subject<OVRInput.NearTouch>();
		public IObservable<OVRInput.NearTouch> OnButtonNearRecieved {
			get { return _onButtonNear; }
		}

		private Subject<OVRInput.Touch> _onButtonTouch = new Subject<OVRInput.Touch>();
		public IObservable<OVRInput.Touch> OnButtonTouchRecieved {
			get { return _onButtonTouch; }
		}

		private Subject<OVRInput.Button> _onButtonUp = new Subject<OVRInput.Button>();
		public IObservable<OVRInput.Button> OnButtonUpRecieved {
			get { return _onButtonUp; }
		}

		private Subject<(OVRInput.Axis2D, Vector2)> _onStickAnalogValue = new Subject<(OVRInput.Axis2D, Vector2)>();
		public IObservable<(OVRInput.Axis2D type, Vector2 axis)> OnStickAnalogValueRecieved {
			get { return _onStickAnalogValue; }
		}

		private Subject<(OVRInput.Axis1D, float)> _onTriggerAnalogValue = new Subject<(OVRInput.Axis1D, float)>();
		public IObservable<(OVRInput.Axis1D type, float axis)> OnTriggerAnalogValueRecieved {
			get { return _onTriggerAnalogValue; }
		}

		public void Update() {
			if(Input.GetKeyDown(KeyCode.I)) {
				OnButtonDown(OVRInput.Button.One);
			}
		}


		public override void OnButton(OVRInput.Button button) {
			_onButton.OnNext(button);
		}

		public override void OnButtonDown(OVRInput.Button button) {
			_onButtonDown.OnNext(button);

		}

		public override void OnButtonNear(OVRInput.NearTouch nearTouch) {
			_onButtonNear.OnNext(nearTouch);
		}

		public override void OnButtonTouch(OVRInput.Touch touch) {
			_onButtonTouch.OnNext(touch);
		}

		public override void OnButtonUp(OVRInput.Button button) {
			_onButtonUp.OnNext(button);
		}

		public override void OnStickAnalogValue(OVRInput.Axis2D type, Vector2 axis) {
			_onStickAnalogValue.OnNext((type, axis));
		}

		public override void OnTriggerAnalogValue(OVRInput.Axis1D type, float axis) {
			_onTriggerAnalogValue.OnNext((type, axis));
		}

	}
}