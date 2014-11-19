using System.Text.RegularExpressions;
using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent(typeof(GUIText))]
public class TextBorder : MonoBehaviour {
    [Range(1, 3)]
    public int BorderWidth = 1;
    public Color BorderColor = Color.black;

    public bool EditorMode;

    private GameObject[] _borders;
    private Vector3 _prevPos = new Vector3(-1.234f, -1.345f, -1.456f);
    private int _prevBorderWidth;

    private bool MustUpdate {
        get {
            return !guiText.text.Equals(_borders[0].guiText.text)
                   || BorderColor != _borders[0].guiText.color
                   || guiText.enabled != _borders[0].guiText.enabled
                   || guiText.font != _borders[0].guiText.font
                   || guiText.fontSize != _borders[0].guiText.fontSize
                   || guiText.fontStyle != _borders[0].guiText.fontStyle
                   || guiText.richText != _borders[0].guiText.richText
                   || guiText.anchor != _borders[0].guiText.anchor
                   || guiText.alignment != _borders[0].guiText.alignment;
        }
    }

	// Use this for initialization
    void Start() {
        if (!Application.isPlaying && !EditorMode && Application.isEditor) return;
        RemoveBorders();
        InitBorders();
        _prevBorderWidth = BorderWidth;
    }
	
	// Update is called once per frame
	void Update () {
        if (!Application.isPlaying && !EditorMode && Application.isEditor) {
	        if (transform.childCount > 0) {
	            RemoveBorders();
	        }
            return;
        }

	    if (_borders != null) {
	        if (guiText.text.Length > 0) {
                var createBorders = transform.position != _prevPos;

	            var forceUpdate = false;
	            if (_prevBorderWidth != BorderWidth || _borders.Length < 1 || _borders[0] == null) {
	                RemoveBorders();
                    InitBorders();
	                forceUpdate = true;
	            }

	            if (forceUpdate || MustUpdate) {
                    foreach (var border in _borders) {
                        border.guiText.text = guiText.text;
                        border.guiText.font = guiText.font;
                        border.guiText.color = BorderColor;
                        border.guiText.fontSize = guiText.fontSize;
                        border.guiText.fontStyle = guiText.fontStyle;
                        border.guiText.richText = guiText.richText;
                        border.guiText.anchor = guiText.anchor;
                        border.guiText.alignment = guiText.alignment;
                        border.guiText.enabled = guiText.enabled;
                    }
	                createBorders = true;
	            }

                if (createBorders) {
                    for (var i = 0; i < _borders.Length / 4; i++) {
                        var offset = i + 1;
                        _borders[i * 4].guiText.pixelOffset = new Vector2(guiText.pixelOffset.x - offset, guiText.pixelOffset.y);
                        _borders[i * 4 + 1].guiText.pixelOffset = new Vector2(guiText.pixelOffset.x + offset, guiText.pixelOffset.y);
                        _borders[i * 4 + 2].guiText.pixelOffset = new Vector2(guiText.pixelOffset.x, guiText.pixelOffset.y - offset);
                        _borders[i * 4 + 3].guiText.pixelOffset = new Vector2(guiText.pixelOffset.x, guiText.pixelOffset.y + offset);
                    }
	                _prevPos = transform.position;
                    _prevBorderWidth = BorderWidth;
                }
	        } else if (_borders[0].guiText.text.Length > 0) {
	            foreach (var border in _borders) {
	                border.guiText.text = string.Empty;
	            }
	        }
	    }
    }

    void InitBorders() {
        Debug.Log("INit borders");
        _borders = new GameObject[4 * BorderWidth];
        for (var i = 0; i < _borders.Length; i++) {
            _borders[i] = new GameObject(name + "_border[" + i + "]", typeof(GUIText));
            _borders[i].guiText.text = string.Empty;
            _borders[i].guiText.font = guiText.font;
            _borders[i].guiText.color = BorderColor;
            _borders[i].guiText.fontSize = guiText.fontSize;
            _borders[i].guiText.fontStyle = guiText.fontStyle;
            _borders[i].guiText.richText = guiText.richText;
            _borders[i].guiText.anchor = guiText.anchor;
            _borders[i].guiText.alignment = guiText.alignment;
            _borders[i].transform.parent = transform;
            _borders[i].transform.localPosition = new Vector3(0, 0, -1);
            _borders[i].guiText.enabled = guiText.enabled;
        }
    }

    void RemoveBorders() {
        Debug.Log("remove borders");
        var children = transform.childCount;
        if (children < 1) return;

        var properChildRegex = new Regex(Regex.Escape(name) + @"(\(Clone\))?_border\[[0-9]+\]");
        for (var i = children - 1; i >= 0; i--) {
            var child = transform.GetChild(i);
            if (child.GetComponent(typeof (GUIText)) == null) continue;
            if (!properChildRegex.IsMatch(child.name)) continue;
            DestroyImmediate(child.gameObject);
        }
    }
}
