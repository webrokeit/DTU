using System;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class HoverMe : MonoBehaviour {
	[Range(0.0f, 60.0f)]
	public float TimeToActivate = 2.0f;

	private float _hoverStart = 0.000f;

    public bool Progressing {
        get { return _hoverStart > 0.00f; }
    }

    public float Progress { get; set; }

    public delegate void HoverActivatedEvent(GameObject sender);
	public event HoverActivatedEvent HoverActivated;

	public delegate void HoverProgessedEvent(GameObject sender, float progress);
	public event HoverProgessedEvent HoverProgressed;

    public delegate void HoverResetEvent(GameObject sender);
    public event HoverResetEvent HoverReset;

	void Update(){
        if (Progressing) {
			var delta = Time.time - _hoverStart;
			if(HoverProgressed != null) {
				Progress = Mathf.Min (delta / TimeToActivate, 1.0f);
				HoverProgressed(gameObject, Progress);
			}

			if (delta >= TimeToActivate) {
				if (HoverActivated != null) HoverActivated (gameObject);
			}
        } else if (_hoverStart < 1.0f) {
            if (HoverReset != null) HoverReset(gameObject);
            _hoverStart = 0.0f;
        }
	}

	void OnTriggerEnter(){
		_hoverStart = Time.time;
	}

	void OnTriggerExit(){
		_hoverStart = -1.1f;
	    Progress = 0.0f;
	}
}