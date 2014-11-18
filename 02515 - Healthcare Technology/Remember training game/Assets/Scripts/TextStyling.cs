using TreeEditor;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(GUIText))]
public class TextStyling : MonoBehaviour {
    [Range(0, 3)]
    public int BorderWidth = 0;
    public Color BorderColor = Color.black;
    private GameObject[] _borders;
    private string _prevText = string.Empty;
    private Vector3 _prevPos = new Vector3(-1.234f, -1.345f, -1.456f);

	// Use this for initialization
	void Start () {
	    if (BorderWidth > 0) {
	        _borders = new GameObject[4*BorderWidth];
	        for (var i = 0; i < _borders.Length; i++) {
                _borders[i] = new GameObject(name + "_border[" + i + "]", typeof(GUIText));
	            _borders[i].guiText.text = string.Empty;
	            _borders[i].guiText.font = guiText.font;
	            _borders[i].guiText.fontSize = guiText.fontSize;
	            _borders[i].guiText.fontStyle = guiText.fontStyle;
	            _borders[i].guiText.richText = guiText.richText;
	            _borders[i].guiText.anchor = guiText.anchor;
	            _borders[i].guiText.alignment = guiText.alignment;
                _borders[i].transform.parent = transform;
                _borders[i].transform.localPosition = Vector3.zero;
	        }
	    }
	}
	
	// Update is called once per frame
	void Update () {
	    if (_borders != null) {
	        if (guiText.text.Length > 0) {
	            if (!guiText.text.Equals(_prevText) || transform.position != _prevPos) {
	                foreach (var border in _borders) {
	                    border.guiText.text = guiText.text;
                        border.guiText.color = BorderColor;
	                }

                    for (var i = 0; i < _borders.Length / 4; i++) {
                        var offset = i + 1;
                        _borders[i * 4].guiText.pixelOffset = new Vector2(guiText.pixelOffset.x - offset, guiText.pixelOffset.y);
                        _borders[i * 4 + 1].guiText.pixelOffset = new Vector2(guiText.pixelOffset.x + offset, guiText.pixelOffset.y);
                        _borders[i * 4 + 2].guiText.pixelOffset = new Vector2(guiText.pixelOffset.x, guiText.pixelOffset.y - offset);
                        _borders[i * 4 + 3].guiText.pixelOffset = new Vector2(guiText.pixelOffset.x, guiText.pixelOffset.y + offset);
                    }

	                _prevText = guiText.text;
	                _prevPos = transform.position;
	            }
	        } else if (_borders[0].guiText.text.Length > 0) {
	            foreach (var border in _borders) {
	                border.guiText.text = string.Empty;
	            }
	        }
	    }
    }
}
