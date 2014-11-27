using System;
using System.IO;
using Assets.Scripts.Lib;
using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent(typeof(BoxCollider))]
public class Buttonize : MonoBehaviour {
    private BoxCollider _theCollider;

    public string Text = string.Empty;
    public Font Font = null;
    public int FontSize = 20;
    private int _actualFontSize = -1;
    private int _desiredFontSize = -1;
    public Color FontColor = Color.black;
    public TextAnchor ButtonAnchor = TextAnchor.MiddleCenter;
    public TextAnchor TextAnchor = TextAnchor.MiddleCenter;
    public Color BackgroundColor = Color.white;
    private Color _prevBackgroundColor = Color.white;

    [Range(1, 10000)]
    public int Width = 100;
    [Range(1, 10000)]
    public int Height = 30;

    public int PositionX = 10;
    public int PositionY = 10;
    private RectOffset _positionOffset;

    [Range(1, 25)]
    public int BorderSize = 2;
    public Color BorderColor = Color.black;

    public int BaseScreenWidth = 1024;
    public int BaseScreenHeight = 768;
    public bool Scale = true;
    private float _minRatio = -1.0f;
    private int _actualWidth = -1;
    private int _actualHeight = -1;
    private int _desiredWidth = -1;
    private int _desiredHeight = -1;

    private Texture2D _buttonTexture;
    private Rect _buttonRect;

    private int _prevX = 0;
    private int _prevY = 0;
    private GUIStyle _prevStyle = null;
    private bool _updatesSomething = false;

	// Use this for initialization
	void Start () {
	    _theCollider = GetComponent<BoxCollider>();
	    _prevX = PositionX - 1;
	    _prevY = PositionY - 1;
        _positionOffset = new RectOffset(PositionX, BaseScreenWidth - PositionX + Width, BaseScreenHeight - PositionY, PositionY + Height);
        CheckScaling();
        CheckReposition();
	}
	
	// Update is called once per frame
	void Update () {
        CheckScaling();
        CheckReposition();
	    if (!_updatesSomething) {
	        CheckStylings();
	    }
	    if (_updatesSomething) {
	        _buttonRect = CalcRect();
            UpdateTextures();
	    }
	}

    void CheckScaling() {
        var newMinRatio = Mathf.Min((float)Screen.width / BaseScreenWidth, (float)Screen.height / BaseScreenHeight);
        if (Mathf.Abs(newMinRatio - _minRatio) > Mathf.Epsilon || Width != _desiredWidth || Height != _desiredHeight || FontSize != _desiredFontSize) {
            var ratio = Width/(float)Height;
            var newWidth = Mathf.Max(Width*newMinRatio, ratio/1.0f);
            var newHeight = Mathf.Max(Height*newMinRatio, 1.0f/ratio);
            var newFontSize = Mathf.Max((int)(FontSize * newMinRatio), 1);

            PositionX = (int) (PositionX*newMinRatio);
            PositionY = (int) (PositionY*newMinRatio);
            _actualFontSize = newFontSize;
            _actualWidth = (int)newWidth;
            _actualHeight = (int) newHeight;
            _desiredWidth = Width;
            _desiredHeight = Height;
            _desiredFontSize = FontSize;
            _minRatio = newMinRatio;
            _updatesSomething = true;
        }
    }

    void CheckReposition() {
        if (_prevX != PositionX || _prevY != PositionY) {
            _positionOffset = new RectOffset(
                PositionX,
                _positionOffset.right + (PositionX - _prevX),
                PositionY,
                _positionOffset.bottom + (PositionY - _prevY)
            );
            _prevX = PositionX;
            _prevY = PositionY;
            _updatesSomething = true;
        }
    }

    void CheckStylings() {
        if (_prevStyle == null) return;
        if (FontColor != _prevStyle.normal.textColor || TextAnchor != _prevStyle.alignment ||
            BackgroundColor != _prevBackgroundColor || Font != _prevStyle.font) {
            _updatesSomething = true;
        }
    }

    Rect CalcRect() {
        Debug.Log(_positionOffset);
        switch (ButtonAnchor) {
            case TextAnchor.UpperLeft:
                return new Rect(_positionOffset.left, _positionOffset.top, _actualWidth, _actualHeight);
            case TextAnchor.UpperRight:
                return new Rect(_positionOffset.right - _actualWidth, _positionOffset.top, _actualWidth, _actualHeight);
            default:
                return new Rect(_positionOffset.left - _actualWidth/2.0f, _positionOffset.top - _actualHeight/2.0f, _actualWidth, _actualHeight);
        }
    }

    void OnGUI() {
        if (_updatesSomething) {
            GUI.skin.button.fixedWidth = _actualWidth;
            GUI.skin.button.fixedHeight = _actualHeight;
            GUI.skin.button.fontSize = _actualFontSize;
            GUI.skin.button.normal.textColor = FontColor;
            GUI.skin.button.font = Font;
            GUI.skin.button.normal.background = _buttonTexture;
            GUI.skin.button.alignment = TextAnchor;
            _prevStyle = GUI.skin.button;
        }

        if (GUI.Button(_buttonRect, Text)) {
            Debug.Log("Clicked the button with text");
        }

        _updatesSomething = false;
    }

    void UpdateTextures() {
        _buttonTexture = new Texture2D(_actualWidth, _actualHeight);
        var bgFill = new Color[_buttonTexture.width * _buttonTexture.height];
        bgFill.Fill(BackgroundColor);
        _buttonTexture.SetPixels(bgFill);
        _prevBackgroundColor = BackgroundColor;

        var bs = Math.Min(Math.Min(_buttonTexture.width, _buttonTexture.height), BorderSize);

        if (bs > 0) {
            for (var j = 0; j < bs; j++) {
                for (var i = j; i < _buttonTexture.width - j; i++) {
                    _buttonTexture.SetPixel(i, j, BorderColor);
                    _buttonTexture.SetPixel(i, _buttonTexture.height - j - 1, BorderColor);
                }
                for (var i = j + 1; i < _buttonTexture.height - 1; i++) {
                    _buttonTexture.SetPixel(j, i, BorderColor);
                    _buttonTexture.SetPixel(_buttonTexture.width - j - 1, i, BorderColor);
                }
            }
        }

        _buttonTexture.Apply();
        var bytes = _buttonTexture.EncodeToPNG();
        using (var fs = new FileStream("TextureGens/" + Environment.TickCount + ".png", FileMode.Create, FileAccess.Write)) {
            fs.Write(bytes, 0, bytes.Length);
        }
    }
}
