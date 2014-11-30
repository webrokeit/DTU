using System;
using System.IO;
using Assets.Scripts.Lib;
using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class Buttonize : MonoBehaviour {
    public string Text = string.Empty;
    public Font Font = null;
    public int FontSize = 20;
    private int _actualFontSize = -1;
    private int _desiredFontSize = -1;
    public Color FontColor = Color.black;
    public TextAnchor TextAnchor = TextAnchor.MiddleCenter;
    public Color BackgroundColor = Color.white;
    private Color _prevBackgroundColor = Color.white;

    [Range(1, 10000)]
    public int Width = 100;
    [Range(1, 10000)]
    public int Height = 30;
    private int _actualWidth = -1;
    private int _actualHeight = -1;
    private int _desiredWidth = -1;
    private int _desiredHeight = -1;

    public int X = 10;
    public int Y = 10;
    private int _desiredX = 0;
    private int _desiredY = 0;

    [Range(1, 25)]
    public int BorderSize = 2;
    public Color BorderColor = Color.black;

    public int BaseScreenWidth = 1024;
    public int BaseScreenHeight = 768;
    public bool Scale = true;
    private float _minRatio = -1.0f;
    private float _widthRatio = -1.0f;
    private float _heightRatio = 1.0f;

    private Texture2D _buttonTexture;
    private Rect _buttonRect;

    private HoverMe _hover = null;
    private GUIStyle _prevStyle = null;
    private bool _updatedSomething = false;

    public event Action ButtonClicked;

	// Use this for initialization
	void Start () {
	    _actualWidth = Width;
	    _actualHeight = Height;
        _buttonRect = new Rect(X, Y, _actualWidth, _actualHeight);
        if (Scale) {
	        CheckScaling();
	    }

	    _hover = GetComponent<HoverMe>();
	    if (_hover) {
	        _hover.HoverActivated += sender => { if (ButtonClicked != null) ButtonClicked(); };
	    }
	}
	
	// Update is called once per frame
	void Update () {
	    if (Scale) {
	        CheckScaling();
	    }
	    if (!_updatedSomething) {
	        CheckStylings();
	    }
	    if (_updatedSomething) {
            UpdateTextures();
	    }
	}

    void CheckScaling() {
        var newWidthRatio = (float)Screen.width / BaseScreenWidth;
        var newHeightRatio = (float)Screen.height / BaseScreenHeight;
        var newMinRatio = Mathf.Min(newWidthRatio, newHeightRatio);

        if (Mathf.Abs(newMinRatio - _minRatio) > Mathf.Epsilon || Width != _desiredWidth || Height != _desiredHeight || FontSize != _desiredFontSize) {
            var ratio = Width/(float)Height;
            _actualWidth = (int)Mathf.Max(Width*newMinRatio, ratio/1.0f);
            _actualHeight = (int) Mathf.Max(Height*newMinRatio, 1.0f/ratio);
            _desiredWidth = Width;
            _desiredHeight = Height;

            _actualFontSize = Mathf.Max((int)(FontSize * newMinRatio), 1);
            _desiredFontSize = FontSize;

            _minRatio = newMinRatio;
            _updatedSomething = true;
        }

        
        if (Mathf.Abs(newWidthRatio - _widthRatio) > Mathf.Epsilon || Mathf.Abs(newHeightRatio - _heightRatio) > Mathf.Epsilon || X != _desiredX || Y != _desiredY) {
            var actualX = X * newWidthRatio;
            var actualY = Y * newHeightRatio;
            _desiredX = X;
            _desiredY = Y;

            _widthRatio = newWidthRatio;
            _heightRatio = newHeightRatio;
            _buttonRect = new Rect(actualX, actualY, _actualWidth, _actualHeight);
            _updatedSomething = true;
        }
    }

    void CheckStylings() {
        if (_prevStyle == null) return;
        if (FontColor != _prevStyle.normal.textColor || TextAnchor != _prevStyle.alignment || BackgroundColor != _prevBackgroundColor || Font != _prevStyle.font) {
            _updatedSomething = true;
        }
    }

    void OnGUI() {
        if (_updatedSomething) {
            GUI.skin.button.fixedWidth = _actualWidth;
            GUI.skin.button.fixedHeight = _actualHeight;
            GUI.skin.button.fontSize = _actualFontSize;
            GUI.skin.button.normal.textColor = FontColor;
            GUI.skin.button.normal.background = _buttonTexture;
            GUI.skin.button.active.textColor = FontColor;
            GUI.skin.button.active.background = _buttonTexture;
            GUI.skin.button.hover.textColor = FontColor;
            GUI.skin.button.hover.background = _buttonTexture;
            GUI.skin.button.font = Font;
            GUI.skin.button.alignment = TextAnchor;
            _prevStyle = GUI.skin.button;
        }

        if (GUI.Button(_buttonRect, Text)) {
            if (ButtonClicked != null) ButtonClicked();
        }

        _updatedSomething = false;
    }

    void UpdateTextures() {
        _buttonTexture = new Texture2D(_actualWidth, _actualHeight);
        var bgFill = new Color[_buttonTexture.width * _buttonTexture.height];
        var bgColor = BackgroundColor;
        var brdrColor = BorderColor;

        if (_hover != null && _hover.Progressing) {
            bgColor = new Color(bgColor.r, bgColor.g, bgColor.b, bgColor.a - bgColor.a * _hover.Progress);
            brdrColor = new Color(brdrColor.r, brdrColor.g, brdrColor.b, brdrColor.a - brdrColor.a * _hover.Progress);
        }

        bgFill.Fill(bgColor);
        _buttonTexture.SetPixels(bgFill);
        _prevBackgroundColor = BackgroundColor;

        var bs = Math.Min(Math.Min(_buttonTexture.width, _buttonTexture.height), BorderSize);

        if (bs > 0) {
            for (var j = 0; j < bs; j++) {
                for (var i = j; i < _buttonTexture.width - j; i++) {
                    _buttonTexture.SetPixel(i, j, BorderColor);
                    _buttonTexture.SetPixel(i, _buttonTexture.height - j - 1, brdrColor);
                }
                for (var i = j + 1; i < _buttonTexture.height - 1; i++) {
                    _buttonTexture.SetPixel(j, i, BorderColor);
                    _buttonTexture.SetPixel(_buttonTexture.width - j - 1, i, brdrColor);
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
