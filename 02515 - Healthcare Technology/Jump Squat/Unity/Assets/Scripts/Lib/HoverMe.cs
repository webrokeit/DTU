using System;
using UnityEngine;

public class HoverMe : MonoBehaviour {
	[Range(0.0f, 60.0f)]
	public float TimeToActivate = 2.0f;

	private float _hoverStart = -1.0f;

	public delegate void HoverActivatedEvent(GameObject sender);
	public event HoverActivatedEvent HoverActivated;

	public delegate void HoverProgessedEvent(GameObject sender, float progress);
	public event HoverProgessedEvent HoverProgressed;

	void Update(){
		if (_hoverStart > 0.0f) {
			var delta = Time.time - _hoverStart;
			if(HoverProgressed != null) {
				var progress = Mathf.Min (TimeToActivate / delta, 1.0f);
				HoverProgressed(this, progress);
			}

			if (delta >= TimeToActivate) {
				if (HoverActivated != null) {
					HoverActivated (this);
				}
			}
		}
	}

	void OnTriggerEnter(){
		_hoverStart = Time.time;
	}

	void OnTriggerExit(){
		_hoverStart = -1.0f;
	}
}