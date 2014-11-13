using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class BrainScript : MonoBehaviour {

    public Rigidbody PlayerAvatar;
    public GameObject PlayerPlatform;

    private readonly Vector3 _playerResetPosition = new Vector3(0f, 1.235f, 0f);

    // Use this for initialization
    private void Start() {
        
    }

    public void Reset() {
        PlayerAvatar.transform.position = _playerResetPosition;
        PlayerAvatar.transform.eulerAngles = Vector3.zero;
        PlayerAvatar.angularVelocity = Vector3.zero;
        PlayerAvatar.velocity = Vector3.zero;
    }

    // Update is called once per frame
    private void Update() {}
}