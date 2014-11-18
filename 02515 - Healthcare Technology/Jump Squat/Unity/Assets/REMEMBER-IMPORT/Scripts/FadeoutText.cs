using TreeEditor;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(GUIText))]
public class FadeoutText : MonoBehaviour {
    public Vector3 MoveSpeed = Vector3.zero;

    public int WaitBeforeFade = 250;
    public int FadeDuration = 1000;
    private float _fadeRatio = 1.0f;

    public bool RemoveWhenFaded;

    public bool HasBorderScript;
    [Range(1.0f, 5.0f)]
    public float BorderFadeRatio = 1.0f;
    private TextBorder _borderScript;

    private float _displayed;

    public delegate void TextFadedEvent(FadeoutText fadeScript, GUIText textObj);
    public event TextFadedEvent TextFaded;

	// Use this for initialization
	void Start () {
	    if (HasBorderScript) {
	        _borderScript = GetComponent<TextBorder>();
	    }
	    _fadeRatio = 1.0f/FadeDuration;
	    _displayed = Time.time*1000;
	}
	
	// Update is called once per frame
	void Update () {
	    var pos = transform.position;
	    var timePassed = Time.time*1000 - _displayed;
	    if (timePassed > WaitBeforeFade + FadeDuration) {
	        if (RemoveWhenFaded) {
	            Destroy(gameObject);
	        } else {
	            guiText.color = new Color(0f, 0f, 0f, 0f);
	            gameObject.SetActive(false);
	        }

	        if (TextFaded != null) {
	            TextFaded(this, guiText);
	        }

	        return;
	    }

	    if (timePassed > WaitBeforeFade) {
	        var fadeAmount = Mathf.Clamp(1.0f - _fadeRatio*(timePassed - WaitBeforeFade), 0.0f, 1.0f);
            guiText.color = new Color(guiText.color.r, guiText.color.g, guiText.color.b, fadeAmount);

	        if (HasBorderScript && _borderScript) {
	            var fadeBorderAmount = fadeAmount/BorderFadeRatio;
	            _borderScript.BorderColor = new Color(_borderScript.BorderColor.r, _borderScript.BorderColor.g, _borderScript.BorderColor.b, fadeBorderAmount);
	        }
	    }

	    if (MoveSpeed != Vector3.zero) {
	        transform.position = new Vector3(pos.x + MoveSpeed.x*Time.deltaTime, pos.y + MoveSpeed.y*Time.deltaTime, pos.z + MoveSpeed.z*Time.deltaTime);
	    }
	}
}
