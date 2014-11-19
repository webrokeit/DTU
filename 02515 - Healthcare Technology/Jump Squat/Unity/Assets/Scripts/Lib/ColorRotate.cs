using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[RequireComponent(typeof(GUIText))]
public class ColorRotate : MonoBehaviour {
    [Range(1.0f, 250.0f)]
    public float Speed = 50.0f;
    [Range(0, 255)]
    public int MinRed = 0;
    [Range(0, 255)]
    public int MaxRed = 255;
    [Range(0, 255)]
    public int MinGreen = 0;
    [Range(0, 255)]
    public int MaxGreen = 255;
    [Range(0, 255)]
    public int MinBlue = 0;
    [Range(0, 255)]
    public int MaxBlue = 255;
    [Range(0, 255)]
    public int Alpha = 255;

    private float _red = 255;
    private float _green;
    private float _blue;
    private ColorDirection _direction;
    

    void Start() {
        _red = Mathf.Min(_red, MaxRed);
        _green = Mathf.Max(_green, MinGreen);
        _blue = Mathf.Max(_blue, MinBlue);
        SetColor();
        _direction = ColorDirection.RedToGreen;
        if (Speed < 0.00f) Speed = 1.0f;
    }

    void Update() {
        switch (_direction) {
            case ColorDirection.RedToGreen:
                if (_green < MaxGreen) {
                    _green = Mathf.Min(MaxGreen, _green + Time.deltaTime*Speed);
                } else {
                    _red = Mathf.Max(MinRed, _red - Time.deltaTime*Speed);
                    if (_red <= MinRed) {
                        _direction = ColorDirection.GreenToBlue;
                    }
                }
                break;
            case ColorDirection.GreenToBlue:
                if (_blue < MaxBlue) {
                    _blue = Mathf.Min(MaxBlue, _blue + Time.deltaTime * Speed);
                } else {
                    _green = Mathf.Max(MinGreen, _green - Time.deltaTime * Speed);
                    if (_green <= MinGreen) {
                        _direction = ColorDirection.BlueToRed;
                    }
                }
                break;
            case ColorDirection.BlueToRed:
                if (_red < MaxRed) {
                    _red = Mathf.Min(MaxRed, _red + Time.deltaTime * Speed);
                } else {
                    _blue = Mathf.Max(MinBlue, _blue - Time.deltaTime * Speed);
                    if (_blue <= MinBlue) {
                        _direction = ColorDirection.RedToGreen;
                    }
                }
                break;
        }
        SetColor();
    }

    void SetColor() {
        guiText.color = new Color32((byte)_red, (byte)_green, (byte)_blue, (byte)Mathf.Min(255, Alpha));
    }

    enum ColorDirection {
        RedToGreen,
        GreenToBlue,
        BlueToRed
    }
}

