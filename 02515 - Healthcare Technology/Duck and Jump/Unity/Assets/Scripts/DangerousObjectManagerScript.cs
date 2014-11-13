using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Managers;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class DangerousObjectManagerScript : MonoBehaviour {
    public MovingObjectScript[] DangerousObjects = {};

    public Transform StartTopLeft;
    public Transform StartTopMiddle;
    public Transform StartTopRight;
    public Transform StartBottomLeft;
    public Transform StartBottomMiddle;
    public Transform StartBottomRight;

    public Transform HalfwaySpot;
    public Transform EndSpot;

    public float InitialMovingSpeed = 100.0f;
    public float IncrementMovingSpeed = 0.75f;
    private float _movingSpeed;

    public int ObjectsPassed = 0;
    private readonly Queue<MovingObjectScript> _startingObjects = new Queue<MovingObjectScript>();
    private readonly Queue<MovingObjectScript> _endingObjects = new Queue<MovingObjectScript>();

    private static readonly Vector3 CreatePosition = new Vector3(50f, 250f, 0f);
    private static readonly Quaternion CreateRotation = new Quaternion(0f, 0f, 0f, 0f);

	// Use this for initialization
	void Start () {
	    GameEventManager.GameStart += GameStart;
	    GameEventManager.GameOver += GameOver;
	    /*DangerousObjects = DangerousObjects.Where(obj => obj != null).ToArray();
	    foreach (var obj in DangerousObjects) {
	        obj.gameObject.SetActive(false);
	    }
	    _movingSpeed = InitialMovingSpeed;*/
	}
	
	// Update is called once per frame
	void Update () {
	    if (DangerousObjects.Length < 1) return;

	    if (_startingObjects.Count < 1 || _startingObjects.Peek().transform.localPosition.z < HalfwaySpot.transform.localPosition.z) {
	        if (_startingObjects.Count > 0) {
	            var obj = _startingObjects.Dequeue();
	            _endingObjects.Enqueue(obj);
	        }
	        CreateNewObject();
	    }

	    if (_endingObjects.Count > 0 && _endingObjects.Peek().transform.localPosition.z < EndSpot.transform.localPosition.z) {
	        var obj = _endingObjects.Dequeue();
	        Destroy(obj.gameObject);
	        ObjectsPassed++;
	    }
	}

    void CreateNewObject() {
        var randIndex = Random.Range(0, DangerousObjects.Length);
        var baseObj = DangerousObjects[randIndex];
        var obj = (MovingObjectScript)Instantiate(baseObj, CreatePosition, CreateRotation);
        var startPos = PickStartPosition(obj);

        obj.transform.position = startPos;
        obj.transform.rotation = baseObj.transform.rotation;
        obj.gameObject.SetActive(true);
        obj.StartMoving(_movingSpeed);

        _startingObjects.Enqueue(obj);

        _movingSpeed += IncrementMovingSpeed;
    }

    Vector3 PickStartPosition(MovingObjectScript obj) {
        if (obj.IsWide) {
            switch (obj.StartPositions) {
                case StartPosition.Top:
                case StartPosition.TopLeft:
                case StartPosition.TopMiddle:
                case StartPosition.TopRight:
                    return StartTopMiddle.transform.position;
                case StartPosition.Bottom:
                case StartPosition.BottomLeft:
                case StartPosition.BottomMiddle:
                case StartPosition.BottomRight:
                    return StartBottomMiddle.transform.position;
                default:
                    return PickAny(StartBottomMiddle, StartTopMiddle).transform.position;
            }
        }

        switch (obj.StartPositions) {
            case StartPosition.BottomLeft:
                return StartBottomLeft.transform.position;
            case StartPosition.BottomMiddle:
                return StartBottomMiddle.transform.position;
            case StartPosition.BottomRight:
                return StartBottomRight.transform.position;
            case StartPosition.TopLeft:
                return StartTopLeft.transform.position;
            case StartPosition.TopMiddle:
                return StartTopMiddle.transform.position;
            case StartPosition.TopRight:
                return StartTopRight.transform.position;
            case StartPosition.Top:
                return PickAny(StartTopLeft, StartTopMiddle, StartTopRight).transform.position;
            case StartPosition.Bottom:
                return PickAny(StartBottomLeft, StartBottomMiddle, StartBottomRight).transform.position;
            case StartPosition.Left:
                return PickAny(StartBottomLeft, StartTopLeft).transform.position;
            case StartPosition.Right:
                return PickAny(StartBottomRight, StartTopRight).transform.position;
            case StartPosition.LeftOrRight:
                return PickAny(StartBottomLeft, StartTopLeft, StartBottomRight, StartTopRight).transform.position;
            case StartPosition.BottomLeftOrRight:
                return PickAny(StartBottomLeft, StartBottomRight).transform.position;
            case StartPosition.TopLeftOrRight:
                return PickAny(StartTopLeft, StartTopRight).transform.position;
            case StartPosition.TopOrBottomMiddle:
                return PickAny(StartBottomMiddle, StartTopMiddle).transform.position;
            default:
                return PickAny(StartBottomLeft, StartBottomMiddle, StartBottomRight, StartTopLeft, StartTopMiddle, StartTopRight).transform.position;
        }
    }

    static T PickAny<T>(params T[] args) {
        if (args == null || args.Length < 1) return default(T);
        return args[Random.Range(0, args.Length)];
    }

    public enum StartPosition {
        Any,
        Top,
        Bottom,
        Left,
        Right,

        TopLeftOrRight,
        TopLeft,
        TopMiddle,
        TopRight,

        BottomLeftOrRight,
        BottomLeft,
        BottomMiddle,
        BottomRight,

        LeftOrRight,
        TopOrBottomMiddle
    }

    private void GameStart() {
        DangerousObjects = DangerousObjects.Where(obj => obj != null).ToArray();
        foreach (var obj in DangerousObjects) {
            obj.gameObject.SetActive(false);
        }
        _movingSpeed = InitialMovingSpeed;
        ObjectsPassed = 0;
        this.enabled = true;
    }

    private void GameOver() {
        this.enabled = false;
    }
}
