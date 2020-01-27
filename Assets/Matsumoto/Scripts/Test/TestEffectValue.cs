using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestEffectValue : MonoBehaviour
{

	[SerializeField]
	private Renderer _target;

	[SerializeField]
	private Text _view;

	[SerializeField]
	private float _value;

	[SerializeField]
	private int _paramValue;

    // Start is called before the first frame update
    void Start()
    {
		_paramValue = Shader.PropertyToID("_Value");
	}

	// Update is called once per frame
	void Update()
    {
		if(OVRInput.GetDown(OVRInput.RawButton.X) || Input.GetKeyDown(KeyCode.D)) {
			_value += .1f;
		}
		if(OVRInput.GetDown(OVRInput.RawButton.Y) || Input.GetKeyDown(KeyCode.A)) {
			_value -= .1f;
		}

		_target.material.SetFloat(_paramValue, _value);
		_view.text = "value=" + _value;
	}
}
