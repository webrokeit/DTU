using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using System.Text;

// Taken from Asset Store: https://www.assetstore.unity3d.com/en/#!/content/7747
public class KinectManager : MonoBehaviour
{
	public enum Smoothing : int { None, Default, Medium, Aggressive }
	
	
	// Public Bool to determine how many players there are. Default of one user.
	public bool TwoUsers = false;
	
//	// Public Bool to determine if the sensor is used in near mode.
//	public bool NearMode = false;

	// Public Bool to determine whether to receive and compute the user map
	public bool ComputeUserMap = false;
	
	// Public Bool to determine whether to receive and compute the color map
	public bool ComputeColorMap = false;

    public float DisplayMapsWidthPercent = 20f;

	// How high off the ground is the sensor (in meters).
	public float SensorHeight = 1.0f;

	// Kinect elevation angle (in degrees)
	public int SensorAngle = 0;
	
	// Minimum user distance in order to process skeleton data
	public float MinUserDistance = 1.0f;
	
	// Maximum user distance, if any. 0 means no max-distance limitation
	public float MaxUserDistance = 0f;
	
	// Public Bool to determine whether to detect only the closest user or not
	public bool DetectClosestUser = true;
	
	// Public Bool to determine whether to use only the tracked joints (and ignore the inferred ones)
	public bool IgnoreInferredJoints = true;
	
	// Selection of smoothing parameters
	public Smoothing smoothing = Smoothing.Default;
	
	// Bool to keep track of whether Kinect has been initialized
	private bool _kinectInitialized = false; 
	
	// Bools to keep track of who is currently calibrated.
	private bool _player1Calibrated = false;
	private bool _player2Calibrated = false;
	
	private bool _allPlayersCalibrated = false;
	
	// Values to track which ID (assigned by the Kinect) is player 1 and player 2.
	private uint _player1Id;
	private uint _player2Id;
	
	private int _player1Index;
	private int _player2Index;
	
	// User Map vars.
	private Texture2D _usersLblTex;
	private Color32[] _usersMapColors;
	private ushort[] _usersPrevState;
	private Rect _usersMapRect;
	private int _usersMapSize;

	private Texture2D _usersClrTex;
	//Color[] usersClrColors;
	private Rect _usersClrRect;

    //short[] usersLabelMap;
	private ushort[] _usersDepthMap;
	private float[] _usersHistogramMap;
	
	// List of all users
	private List<uint> _allUsers;
	
	// Image stream handles for the kinect
	private IntPtr _colorStreamHandle;
	private IntPtr _depthStreamHandle;
	
	// Color image data, if used
	private Color32[] _colorImage;
	private byte[] _usersColorMap;
	
	// Skeleton related structures
	private KinectWrapper.NuiSkeletonFrame _skeletonFrame;
	private KinectWrapper.NuiTransformSmoothParameters _smoothParameters;
	private int _player1SkeletonIndex;
    private int _player2SkeletonIndex;

    // Skeleton tracking states, positions and joints' orientations
	private Vector3 _player1Pos, _player2Pos;
	private Matrix4x4 _player1Ori, _player2Ori;
	private bool[] _player1JointsTracked, _player2JointsTracked;
	private bool[] _player1PrevTracked, _player2PrevTracked;
	private Vector3[] _player1JointsPos, _player2JointsPos;
	private Matrix4x4[] _player1JointsOri, _player2JointsOri;
	private KinectWrapper.NuiSkeletonBoneOrientation[] _jointOrientations;
	
	private Matrix4x4 _kinectToWorld, _flipMatrix;

    // Timer for controlling Filter Lerp blends.
    private float _lastNuiTime;
	
	// returns the single KinectManager instance
    public static KinectManager Instance { get; private set; }

    // checks if Kinect is initialized and ready to use. If not, there was an error during Kinect-sensor initialization
	public static bool IsKinectInitialized()
	{
		return Instance != null && Instance._kinectInitialized;
	}
	
	// checks if Kinect is initialized and ready to use. If not, there was an error during Kinect-sensor initialization
	public bool IsInitialized()
	{
		return _kinectInitialized;
	}
	
	// this function is used internally by AvatarController
	public static bool IsCalibrationNeeded()
	{
		return false;
	}
	
	// returns the raw depth/user data, if ComputeUserMap is true
	public ushort[] GetRawDepthMap()
	{
		return _usersDepthMap;
	}
	
	// returns the depth data for a specific pixel, if ComputeUserMap is true
	public ushort GetDepthForPixel(int x, int y)
	{
		var index = y * KinectWrapper.Constants.DepthImageWidth + x;
		
		if(index >= 0 && index < _usersDepthMap.Length) return _usersDepthMap[index];
		return 0;
	}
	
	// returns the depth map position for a 3d joint position
	public Vector2 GetDepthMapPosForJointPos(Vector3 posJoint)
	{
		var vDepthPos = KinectWrapper.MapSkeletonPointToDepthPoint(posJoint);
		var vMapPos = new Vector2(vDepthPos.x, vDepthPos.y);
		
		return vMapPos;
	}
	
	// returns the color map position for a depth 2d position
	public Vector2 GetColorMapPosForDepthPos(Vector2 posDepth)
	{
		int cx, cy;

		KinectWrapper.NuiImageViewArea pcViewArea = new KinectWrapper.NuiImageViewArea 
		{
            eDigitalZoom = 0,
            lCenterX = 0,
            lCenterY = 0
        };
		
		KinectWrapper.NuiImageGetColorPixelCoordinatesFromDepthPixelAtResolution(
			KinectWrapper.Constants.ColorImageResolution,
			KinectWrapper.Constants.DepthImageResolution,
			ref pcViewArea,
			(int)posDepth.x, (int)posDepth.y, GetDepthForPixel((int)posDepth.x, (int)posDepth.y),
			out cx, out cy);
		
		return new Vector2(cx, cy);
	}
	
	// returns the depth image/users histogram texture,if ComputeUserMap is true
    public Texture2D GetUsersLblTex()
    { 
		return _usersLblTex;
	}
	
	// returns the color image texture,if ComputeColorMap is true
    public Texture2D GetUsersClrTex()
    { 
		return _usersClrTex;
	}
	
	// returns true if at least one user is currently detected by the sensor
	public bool IsUserDetected()
	{
		return _kinectInitialized && (_allUsers.Count > 0);
	}
	
	// returns the UserID of Player1, or 0 if no Player1 is detected
	public uint GetPlayer1ID()
	{
		return _player1Id;
	}
	
	// returns the UserID of Player2, or 0 if no Player2 is detected
	public uint GetPlayer2ID()
	{
		return _player2Id;
	}
	
	// returns the index of Player1, or 0 if no Player2 is detected
	public int GetPlayer1Index()
	{
		return _player1Index;
	}
	
	// returns the index of Player2, or 0 if no Player2 is detected
	public int GetPlayer2Index()
	{
		return _player2Index;
	}
	
	// returns true if the User is calibrated and ready to use
	public bool IsPlayerCalibrated(uint UserId)
	{
		if(UserId == _player1Id) return _player1Calibrated;
		if(UserId == _player2Id) return _player2Calibrated;
		
		return false;
	}
	
	// returns the raw unmodified joint position, as returned by the Kinect sensor
	public Vector3 GetRawSkeletonJointPos(uint UserId, int joint)
	{
		if(UserId == _player1Id)
			return joint >= 0 && joint < _player1JointsPos.Length ? (Vector3)_skeletonFrame.SkeletonData[_player1SkeletonIndex].SkeletonPositions[joint] : Vector3.zero;
		else if(UserId == _player2Id)
			return joint >= 0 && joint < _player2JointsPos.Length ? (Vector3)_skeletonFrame.SkeletonData[_player2SkeletonIndex].SkeletonPositions[joint] : Vector3.zero;
		
		return Vector3.zero;
	}
	
	// returns the User position, relative to the Kinect-sensor, in meters
	public Vector3 GetUserPosition(uint UserId)
	{
		if(UserId == _player1Id)
			return _player1Pos;
		else if(UserId == _player2Id)
			return _player2Pos;
		
		return Vector3.zero;
	}
	
	// returns the User rotation, relative to the Kinect-sensor
	public Quaternion GetUserOrientation(uint UserId, bool flip)
	{
		if(UserId == _player1Id && _player1JointsTracked[(int)KinectWrapper.NuiSkeletonPositionIndex.HipCenter])
			return ConvertMatrixToQuat(_player1Ori, (int)KinectWrapper.NuiSkeletonPositionIndex.HipCenter, flip);
		else if(UserId == _player2Id && _player2JointsTracked[(int)KinectWrapper.NuiSkeletonPositionIndex.HipCenter])
			return ConvertMatrixToQuat(_player2Ori, (int)KinectWrapper.NuiSkeletonPositionIndex.HipCenter, flip);
		
		return Quaternion.identity;
	}
	
	// returns true if the given joint of the specified user is being tracked
	public bool IsJointTracked(uint UserId, int joint)
	{
		if(UserId == _player1Id)
			return joint >= 0 && joint < _player1JointsTracked.Length ? _player1JointsTracked[joint] : false;
		else if(UserId == _player2Id)
			return joint >= 0 && joint < _player2JointsTracked.Length ? _player2JointsTracked[joint] : false;
		
		return false;
	}
	
	// returns the joint position of the specified user, relative to the Kinect-sensor, in meters
	public Vector3 GetJointPosition(uint UserId, int joint)
	{
		if(UserId == _player1Id)
			return joint >= 0 && joint < _player1JointsPos.Length ? _player1JointsPos[joint] : Vector3.zero;
		else if(UserId == _player2Id)
			return joint >= 0 && joint < _player2JointsPos.Length ? _player2JointsPos[joint] : Vector3.zero;
		
		return Vector3.zero;
	}
	
	// returns the local joint position of the specified user, relative to the parent joint, in meters
	public Vector3 GetJointLocalPosition(uint UserId, int joint)
	{
        int parent = KinectWrapper.GetSkeletonJointParent(joint);

		if(UserId == _player1Id)
			return joint >= 0 && joint < _player1JointsPos.Length ? 
				(_player1JointsPos[joint] - _player1JointsPos[parent]) : Vector3.zero;
		else if(UserId == _player2Id)
			return joint >= 0 && joint < _player2JointsPos.Length ? 
				(_player2JointsPos[joint] - _player2JointsPos[parent]) : Vector3.zero;
		
		return Vector3.zero;
	}
	
	// returns the joint rotation of the specified user, relative to the Kinect-sensor
	public Quaternion GetJointOrientation(uint UserId, int joint, bool flip)
	{
		if(UserId == _player1Id)
		{
			if(joint >= 0 && joint < _player1JointsOri.Length && _player1JointsTracked[joint])
				return ConvertMatrixToQuat(_player1JointsOri[joint], joint, flip);
		}
		else if(UserId == _player2Id)
		{
			if(joint >= 0 && joint < _player2JointsOri.Length && _player2JointsTracked[joint])
				return ConvertMatrixToQuat(_player2JointsOri[joint], joint, flip);
		}
		
		return Quaternion.identity;
	}
	
	// returns the joint rotation of the specified user, relative to the parent joint
	public Quaternion GetJointLocalOrientation(uint UserId, int joint, bool flip)
	{
        int parent = KinectWrapper.GetSkeletonJointParent(joint);

		if(UserId == _player1Id)
		{
			if(joint >= 0 && joint < _player1JointsOri.Length && _player1JointsTracked[joint])
			{
				Matrix4x4 localMat = (_player1JointsOri[parent].inverse * _player1JointsOri[joint]);
				return Quaternion.LookRotation(localMat.GetColumn(2), localMat.GetColumn(1));
			}
		}
		else if(UserId == _player2Id)
		{
			if(joint >= 0 && joint < _player2JointsOri.Length && _player2JointsTracked[joint])
			{
				Matrix4x4 localMat = (_player2JointsOri[parent].inverse * _player2JointsOri[joint]);
				return Quaternion.LookRotation(localMat.GetColumn(2), localMat.GetColumn(1));
			}
		}
		
		return Quaternion.identity;
	}
	
	// returns the direction between baseJoint and nextJoint, for the specified user
	public Vector3 GetDirectionBetweenJoints(uint UserId, int baseJoint, int nextJoint, bool flipX, bool flipZ)
	{
		Vector3 jointDir = Vector3.zero;
		
		if(UserId == _player1Id)
		{
			if(baseJoint >= 0 && baseJoint < _player1JointsPos.Length && _player1JointsTracked[baseJoint] &&
				nextJoint >= 0 && nextJoint < _player1JointsPos.Length && _player1JointsTracked[nextJoint])
			{
				jointDir = _player1JointsPos[nextJoint] - _player1JointsPos[baseJoint];
			}
		}
		else if(UserId == _player2Id)
		{
			if(baseJoint >= 0 && baseJoint < _player2JointsPos.Length && _player2JointsTracked[baseJoint] &&
				nextJoint >= 0 && nextJoint < _player2JointsPos.Length && _player2JointsTracked[nextJoint])
			{
				jointDir = _player2JointsPos[nextJoint] - _player2JointsPos[baseJoint];
			}
		}
		
		if(jointDir != Vector3.zero)
		{
			if(flipX)
				jointDir.x = -jointDir.x;
			
			if(flipZ)
				jointDir.z = -jointDir.z;
		}
		
		return jointDir;
	}
	
	// removes the currently detected kinect users, allowing a new detection/calibration process to start
	public void ClearKinectUsers()
	{
		if(!_kinectInitialized)
			return;

		// remove current users
		for(int i = _allUsers.Count - 1; i >= 0; i--)
		{
			uint userId = _allUsers[i];
			RemoveUser(userId);
		}
		
		ResetFilters();
	}
	
	// clears Kinect buffers and resets the filters
	public void ResetFilters()
	{
		if(!_kinectInitialized)
			return;
		
		// clear kinect vars
		_player1Pos = Vector3.zero; _player2Pos = Vector3.zero;
		_player1Ori = Matrix4x4.identity; _player2Ori = Matrix4x4.identity;
		
		int skeletonJointsCount = (int)KinectWrapper.NuiSkeletonPositionIndex.Count;
		for(int i = 0; i < skeletonJointsCount; i++)
		{
			_player1JointsTracked[i] = false; _player2JointsTracked[i] = false;
			_player1PrevTracked[i] = false; _player2PrevTracked[i] = false;
			_player1JointsPos[i] = Vector3.zero; _player2JointsPos[i] = Vector3.zero;
			_player1JointsOri[i] = Matrix4x4.identity; _player2JointsOri[i] = Matrix4x4.identity;
		}
	}
	
	
	//----------------------------------- end of public functions --------------------------------------//

	void Start()
	{
		//CalibrationText = GameObject.Find("CalibrationText");
		int hr = 0;
		
		try
		{
			hr = KinectWrapper.NuiInitialize(KinectWrapper.NuiInitializeFlags.UsesSkeleton |
				KinectWrapper.NuiInitializeFlags.UsesDepthAndPlayerIndex |
				(ComputeColorMap ? KinectWrapper.NuiInitializeFlags.UsesColor : 0));
            if (hr != 0)
			{
            	throw new Exception("NuiInitialize Failed");
			}
			
			hr = KinectWrapper.NuiSkeletonTrackingEnable(IntPtr.Zero, 8);  // 0, 12,8
			if (hr != 0)
			{
				throw new Exception("Cannot initialize Skeleton Data");
			}
			
			_depthStreamHandle = IntPtr.Zero;
			if(ComputeUserMap)
			{
				hr = KinectWrapper.NuiImageStreamOpen(KinectWrapper.NuiImageType.DepthAndPlayerIndex, 
					KinectWrapper.Constants.DepthImageResolution, 0, 2, IntPtr.Zero, ref _depthStreamHandle);
				if (hr != 0)
				{
					throw new Exception("Cannot open depth stream");
				}
			}
			
			_colorStreamHandle = IntPtr.Zero;
			if(ComputeColorMap)
			{
				hr = KinectWrapper.NuiImageStreamOpen(KinectWrapper.NuiImageType.Color, 
					KinectWrapper.Constants.ColorImageResolution, 0, 2, IntPtr.Zero, ref _colorStreamHandle);
				if (hr != 0)
				{
					throw new Exception("Cannot open color stream");
				}
			}

			// set kinect elevation angle
			KinectWrapper.NuiCameraElevationSetAngle(SensorAngle);
			
			// init skeleton structures
			_skeletonFrame = new KinectWrapper.NuiSkeletonFrame() 
							{ 
								SkeletonData = new KinectWrapper.NuiSkeletonData[KinectWrapper.Constants.NuiSkeletonCount] 
							};
			
			// values used to pass to smoothing function
			_smoothParameters = new KinectWrapper.NuiTransformSmoothParameters();
			
			switch(smoothing)
			{
				case Smoothing.Default:
					_smoothParameters.fSmoothing = 0.5f;
					_smoothParameters.fCorrection = 0.5f;
					_smoothParameters.fPrediction = 0.5f;
					_smoothParameters.fJitterRadius = 0.05f;
					_smoothParameters.fMaxDeviationRadius = 0.04f;
					break;
				case Smoothing.Medium:
					_smoothParameters.fSmoothing = 0.5f;
					_smoothParameters.fCorrection = 0.1f;
					_smoothParameters.fPrediction = 0.5f;
					_smoothParameters.fJitterRadius = 0.1f;
					_smoothParameters.fMaxDeviationRadius = 0.1f;
					break;
				case Smoothing.Aggressive:
					_smoothParameters.fSmoothing = 0.7f;
					_smoothParameters.fCorrection = 0.3f;
					_smoothParameters.fPrediction = 1.0f;
					_smoothParameters.fJitterRadius = 1.0f;
					_smoothParameters.fMaxDeviationRadius = 1.0f;
					break;
			}
			
			// create arrays for joint positions and joint orientations
			int skeletonJointsCount = (int)KinectWrapper.NuiSkeletonPositionIndex.Count;
			
			_player1JointsTracked = new bool[skeletonJointsCount];
			_player2JointsTracked = new bool[skeletonJointsCount];
			_player1PrevTracked = new bool[skeletonJointsCount];
			_player2PrevTracked = new bool[skeletonJointsCount];
			
			_player1JointsPos = new Vector3[skeletonJointsCount];
			_player2JointsPos = new Vector3[skeletonJointsCount];
			
			_player1JointsOri = new Matrix4x4[skeletonJointsCount];
			_player2JointsOri = new Matrix4x4[skeletonJointsCount];
			
			//create the transform matrix that converts from kinect-space to world-space
			Quaternion quatTiltAngle = new Quaternion();
			quatTiltAngle.eulerAngles = new Vector3(-SensorAngle, 0.0f, 0.0f);
			
			// transform matrix - kinect to world
			_kinectToWorld.SetTRS(new Vector3(0.0f, SensorHeight, 0.0f), quatTiltAngle, Vector3.one);
			_flipMatrix = Matrix4x4.identity;
			_flipMatrix[2, 2] = -1;
			
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
		catch(DllNotFoundException e)
		{
			string message = "Please check the Kinect SDK installation.";
			Debug.LogError(message);
			Debug.LogError(e.ToString());
				
			return;
		}
		catch (Exception e)
		{
			string message = e.Message + " - " + KinectWrapper.GetNuiErrorString(hr);
			Debug.LogError(message);
			Debug.LogError(e.ToString());
				
			return;
		}
		
		// get the main camera rectangle
		Rect cameraRect = Camera.main.pixelRect;
		if(ComputeUserMap)
		{
			var displayMapsWidthPercent = DisplayMapsWidthPercent / 100f;
			var displayMapsHeightPercent = displayMapsWidthPercent * KinectWrapper.GetDepthHeight() / KinectWrapper.GetDepthWidth();

			var displayWidth = cameraRect.width * displayMapsWidthPercent;
			var displayHeight = cameraRect.width * displayMapsHeightPercent;

	        // Initialize depth & label map related stuff
	        _usersMapSize = KinectWrapper.GetDepthWidth() * KinectWrapper.GetDepthHeight();
	        _usersLblTex = new Texture2D(KinectWrapper.GetDepthWidth(), KinectWrapper.GetDepthHeight());
	        _usersMapColors = new Color32[_usersMapSize];
			_usersPrevState = new ushort[_usersMapSize];
			_usersMapRect = new Rect(cameraRect.width - displayWidth, cameraRect.height, displayWidth, -displayHeight);
			
	        _usersDepthMap = new ushort[_usersMapSize];
	        _usersHistogramMap = new float[8192];
		}
		
		if(ComputeColorMap)
		{
			var displayMapsWidthPercent = DisplayMapsWidthPercent / 100f;
			var displayMapsHeightPercent = displayMapsWidthPercent * KinectWrapper.GetColorHeight() / KinectWrapper.GetColorWidth();
			
			var displayWidth = cameraRect.width * displayMapsWidthPercent;
			var displayHeight = cameraRect.width * displayMapsHeightPercent;
			
			// Initialize color map related stuff
	        _usersClrTex = new Texture2D(KinectWrapper.GetColorWidth(), KinectWrapper.GetColorHeight());
	        _usersClrRect = new Rect(cameraRect.width - displayWidth, cameraRect.height, displayWidth, -displayHeight);
			
			_colorImage = new Color32[KinectWrapper.GetColorWidth() * KinectWrapper.GetColorHeight()];
			_usersColorMap = new byte[_colorImage.Length << 2];
		}
		
		
        // Initialize user list to contain ALL users.
        _allUsers = new List<uint>();
        
		Debug.Log("Waiting for users.");
			
		_kinectInitialized = true;
	}
	
	void Update()
	{
		if(_kinectInitialized)
		{
			// needed by the KinectExtras' native wrapper to check for next frames
			// uncomment the line below, if you use the Extras' wrapper, but none of the Extras' managers
			//KinectWrapper.UpdateKinectSensor();
			
	        // If the players aren't all calibrated yet, draw the user map.
			if(ComputeUserMap)
			{
				if(_depthStreamHandle != IntPtr.Zero &&
					KinectWrapper.PollDepth(_depthStreamHandle, KinectWrapper.Constants.IsNearMode, ref _usersDepthMap))
				{
		        	UpdateUserMap();
				}
			}
			
			if(ComputeColorMap)
			{
				if(_colorStreamHandle != IntPtr.Zero &&
					KinectWrapper.PollColor(_colorStreamHandle, ref _usersColorMap, ref _colorImage))
				{
					UpdateColorMap();
				}
			}
			
			if(KinectWrapper.PollSkeleton(ref _smoothParameters, ref _skeletonFrame))
			{
				ProcessSkeleton();
			}
		}
		
		// Kill the program with ESC.
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			Application.Quit();
		}
	}
	
	// Make sure to kill the Kinect on quitting.
	void OnApplicationQuit()
	{
		if(_kinectInitialized)
		{
			// Shutdown OpenNI
			KinectWrapper.NuiShutdown();
			Instance = null;
		}
	}
	
	// Update the User Map
    void UpdateUserMap()
    {
        int numOfPoints = 0;
		Array.Clear(_usersHistogramMap, 0, _usersHistogramMap.Length);

        // Calculate cumulative histogram for depth
        for (int i = 0; i < _usersMapSize; i++)
        {
            // Only calculate for depth that contains users
            if ((_usersDepthMap[i] & 7) != 0)
            {
				ushort userDepth = (ushort)(_usersDepthMap[i] >> 3);
                _usersHistogramMap[userDepth]++;
                numOfPoints++;
            }
        }
		
        if (numOfPoints > 0)
        {
            for (int i = 1; i < _usersHistogramMap.Length; i++)
	        {   
		        _usersHistogramMap[i] += _usersHistogramMap[i - 1];
	        }
			
            for (int i = 0; i < _usersHistogramMap.Length; i++)
	        {
                _usersHistogramMap[i] = 1.0f - (_usersHistogramMap[i] / numOfPoints);
	        }
        }
		
		// dummy structure needed by the coordinate mapper
        KinectWrapper.NuiImageViewArea pcViewArea = new KinectWrapper.NuiImageViewArea 
		{
            eDigitalZoom = 0,
            lCenterX = 0,
            lCenterY = 0
        };
		
        // Create the actual users texture based on label map and depth histogram
		Color32 clrClear = Color.clear;
        for (int i = 0; i < _usersMapSize; i++)
        {
	        // Flip the texture as we convert label map to color array
            int flipIndex = i; // usersMapSize - i - 1;
			
			ushort userMap = (ushort)(_usersDepthMap[i] & 7);
			ushort userDepth = (ushort)(_usersDepthMap[i] >> 3);
			
			ushort nowUserPixel = userMap != 0 ? (ushort)((userMap << 13) | userDepth) : userDepth;
			ushort wasUserPixel = _usersPrevState[flipIndex];
			
			// draw only the changed pixels
			if(nowUserPixel != wasUserPixel)
			{
				_usersPrevState[flipIndex] = nowUserPixel;
				
	            if (userMap == 0)
	            {
	                _usersMapColors[flipIndex] = clrClear;
	            }
	            else
	            {
					if(_colorImage != null)
					{
						int x = i % KinectWrapper.Constants.DepthImageWidth;
						int y = i / KinectWrapper.Constants.DepthImageWidth;
	
						int cx, cy;
						int hr = KinectWrapper.NuiImageGetColorPixelCoordinatesFromDepthPixelAtResolution(
							KinectWrapper.Constants.ColorImageResolution,
							KinectWrapper.Constants.DepthImageResolution,
							ref pcViewArea,
							x, y, _usersDepthMap[i],
							out cx, out cy);
						
						if(hr == 0)
						{
							int colorIndex = cx + cy * KinectWrapper.Constants.ColorImageWidth;
							//colorIndex = usersMapSize - colorIndex - 1;
							if(colorIndex >= 0 && colorIndex < _usersMapSize)
							{
								Color32 colorPixel = _colorImage[colorIndex];
								_usersMapColors[flipIndex] = colorPixel;  // new Color(colorPixel.r / 256f, colorPixel.g / 256f, colorPixel.b / 256f, 0.9f);
								_usersMapColors[flipIndex].a = 230; // 0.9f
							}
						}
					}
					else
					{
		                // Create a blending color based on the depth histogram
						float histDepth = _usersHistogramMap[userDepth];
		                Color c = new Color(histDepth, histDepth, histDepth, 0.9f);
		                
						switch(userMap % 4)
		                {
		                    case 0:
		                        _usersMapColors[flipIndex] = Color.red * c;
		                        break;
		                    case 1:
		                        _usersMapColors[flipIndex] = Color.green * c;
		                        break;
		                    case 2:
		                        _usersMapColors[flipIndex] = Color.blue * c;
		                        break;
		                    case 3:
		                        _usersMapColors[flipIndex] = Color.magenta * c;
		                        break;
		                }
					}
	            }
				
			}
        }
		
		// Draw it!
        _usersLblTex.SetPixels32(_usersMapColors);
	}
	
	// Update the Color Map
	void UpdateColorMap()
	{
        _usersClrTex.SetPixels32(_colorImage);
        _usersClrTex.Apply();
	}
	
	// Assign UserId to player 1 or 2.
    void CalibrateUser(uint UserId, int UserIndex, ref KinectWrapper.NuiSkeletonData skeletonData)
    {
		// If player 1 hasn't been calibrated, assign that UserID to it.
		if(!_player1Calibrated)
		{
			// Check to make sure we don't accidentally assign player 2 to player 1.
			if (!_allUsers.Contains(UserId))
			{
				_player1Calibrated = true;
				_player1Id = UserId;
				_player1Index = UserIndex;
					
				_allUsers.Add(UserId);
					
				// reset skeleton filters
				ResetFilters();
					
				// If we're not using 2 users, we're all calibrated.
				//if(!TwoUsers)
				{
					_allPlayersCalibrated = !TwoUsers ? _allUsers.Count >= 1 : _allUsers.Count >= 2; // true;
				}
				
			}
		}
		// Otherwise, assign to player 2.
		else if(TwoUsers && !_player2Calibrated)
		{
			if (!_allUsers.Contains(UserId))
			{
				_player2Calibrated = true;
				_player2Id = UserId;
				_player2Index = UserIndex;
					
				_allUsers.Add(UserId);
					
				// reset skeleton filters
				ResetFilters();
					
				// All users are calibrated!
				_allPlayersCalibrated = !TwoUsers ? _allUsers.Count >= 1 : _allUsers.Count >= 2; // true;
			}
		}
		
		// If all users are calibrated, stop trying to find them.
		if(_allPlayersCalibrated)
		{
			Debug.Log("All players calibrated.");
		}
    }
	
	// Remove a lost UserId
	void RemoveUser(uint UserId)
	{
		// If we lose player 1...
		if(UserId == _player1Id)
		{
			// Null out the ID and reset all the models associated with that ID.
			_player1Id = 0;
			_player1Index = 0;
			_player1Calibrated = false;
			
		}
		
		// If we lose player 2...
		if(UserId == _player2Id)
		{
			// Null out the ID and reset all the models associated with that ID.
			_player2Id = 0;
			_player2Index = 0;
			_player2Calibrated = false;
		}

        // remove from global users list
        _allUsers.Remove(UserId);
		_allPlayersCalibrated = !TwoUsers ? _allUsers.Count >= 1 : _allUsers.Count >= 2; // false;
		
		// Try to replace that user!
		Debug.Log("Waiting for users.");
	}
	
	// Some internal constants
	private const int stateTracked = (int)KinectWrapper.NuiSkeletonPositionTrackingState.Tracked;
	private const int stateNotTracked = (int)KinectWrapper.NuiSkeletonPositionTrackingState.NotTracked;
	
	private int [] mustBeTrackedJoints = { 
		(int)KinectWrapper.NuiSkeletonPositionIndex.AnkleLeft,
		(int)KinectWrapper.NuiSkeletonPositionIndex.FootLeft,
		(int)KinectWrapper.NuiSkeletonPositionIndex.AnkleRight,
		(int)KinectWrapper.NuiSkeletonPositionIndex.FootRight,
	};

    public KinectManager(Matrix4x4 kinectToWorld) {
        this._kinectToWorld = kinectToWorld;
    }

    // Process the skeleton data
	void ProcessSkeleton()
	{
		List<uint> lostUsers = new List<uint>();
		lostUsers.AddRange(_allUsers);
		
		// calculate the time since last update
		float currentNuiTime = Time.realtimeSinceStartup;
		float deltaNuiTime = currentNuiTime - _lastNuiTime;
		
		for(int i = 0; i < KinectWrapper.Constants.NuiSkeletonCount; i++)
		{
			KinectWrapper.NuiSkeletonData skeletonData = _skeletonFrame.SkeletonData[i];
			uint userId = skeletonData.dwTrackingID;
			
			if(skeletonData.eTrackingState == KinectWrapper.NuiSkeletonTrackingState.SkeletonTracked)
			{
				// get the skeleton position
				Vector3 skeletonPos = _kinectToWorld.MultiplyPoint3x4(skeletonData.Position);
				
				if(!_allPlayersCalibrated)
				{
					// check if this is the closest user
					bool bClosestUser = true;
					
					if(DetectClosestUser)
					{
						for(int j = 0; j < KinectWrapper.Constants.NuiSkeletonCount; j++)
						{
							if(j != i)
							{
								KinectWrapper.NuiSkeletonData skeletonDataOther = _skeletonFrame.SkeletonData[j];
								
								if((skeletonDataOther.eTrackingState == KinectWrapper.NuiSkeletonTrackingState.SkeletonTracked) &&
									(Mathf.Abs(_kinectToWorld.MultiplyPoint3x4(skeletonDataOther.Position).z) < Mathf.Abs(skeletonPos.z)))
								{
									bClosestUser = false;
									break;
								}
							}
						}
					}
					
					if(bClosestUser)
					{
						CalibrateUser(userId, i + 1, ref skeletonData);
					}
				}
				
				if(userId == _player1Id && Mathf.Abs(skeletonPos.z) >= MinUserDistance &&
				   (MaxUserDistance <= 0f || Mathf.Abs(skeletonPos.z) <= MaxUserDistance))
				{
					_player1SkeletonIndex = i;

					// get player position
					_player1Pos = skeletonPos;
					
	
					// get joints' position and rotation
					for (int j = 0; j < (int)KinectWrapper.NuiSkeletonPositionIndex.Count; j++)
					{
						bool playerTracked = IgnoreInferredJoints ? (int)skeletonData.eSkeletonPositionTrackingState[j] == stateTracked :
							(Array.BinarySearch(mustBeTrackedJoints, j) >= 0 ? (int)skeletonData.eSkeletonPositionTrackingState[j] == stateTracked :
							(int)skeletonData.eSkeletonPositionTrackingState[j] != stateNotTracked);
						_player1JointsTracked[j] = _player1PrevTracked[j] && playerTracked;
						_player1PrevTracked[j] = playerTracked;
						
						if(_player1JointsTracked[j])
						{
							_player1JointsPos[j] = _kinectToWorld.MultiplyPoint3x4(skeletonData.SkeletonPositions[j]);
						}
					}
					
					// calculate joint orientations
					KinectWrapper.GetSkeletonJointOrientation(ref _player1JointsPos, ref _player1JointsTracked, ref _player1JointsOri);
	
					// get player rotation
					_player1Ori = _player1JointsOri[(int)KinectWrapper.NuiSkeletonPositionIndex.HipCenter];
				}
				else if(userId == _player2Id && Mathf.Abs(skeletonPos.z) >= MinUserDistance &&
				        (MaxUserDistance <= 0f || Mathf.Abs(skeletonPos.z) <= MaxUserDistance))
				{
					_player2SkeletonIndex = i;

					// get player position
					_player2Pos = skeletonPos;

					// get joints' position and rotation
					for (int j = 0; j < (int)KinectWrapper.NuiSkeletonPositionIndex.Count; j++)
					{
						bool playerTracked = IgnoreInferredJoints ? (int)skeletonData.eSkeletonPositionTrackingState[j] == stateTracked :
							(Array.BinarySearch(mustBeTrackedJoints, j) >= 0 ? (int)skeletonData.eSkeletonPositionTrackingState[j] == stateTracked :
							(int)skeletonData.eSkeletonPositionTrackingState[j] != stateNotTracked);
						_player2JointsTracked[j] = _player2PrevTracked[j] && playerTracked;
						_player2PrevTracked[j] = playerTracked;
						
						if(_player2JointsTracked[j])
						{
							_player2JointsPos[j] = _kinectToWorld.MultiplyPoint3x4(skeletonData.SkeletonPositions[j]);
						}
					}
					
					// calculate joint orientations
					KinectWrapper.GetSkeletonJointOrientation(ref _player2JointsPos, ref _player2JointsTracked, ref _player2JointsOri);
	
					// get player rotation
					_player2Ori = _player2JointsOri[(int)KinectWrapper.NuiSkeletonPositionIndex.HipCenter];
				}
				
				lostUsers.Remove(userId);
			}
		}
		
		// update the nui-timer
		_lastNuiTime = currentNuiTime;
		
		// remove the lost users if any
		if(lostUsers.Count > 0)
		{
			foreach(uint userId in lostUsers)
			{
				RemoveUser(userId);
			}
			
			lostUsers.Clear();
		}
	}
	
	// draws a line in a texture
	private void DrawLine(Texture2D a_Texture, int x1, int y1, int x2, int y2, Color a_Color)
	{
		int width = a_Texture.width;  // KinectWrapper.Constants.DepthImageWidth;
		int height = a_Texture.height;  // KinectWrapper.Constants.DepthImageHeight;
		
		int dy = y2 - y1;
		int dx = x2 - x1;
	 
		int stepy = 1;
		if (dy < 0) 
		{
			dy = -dy; 
			stepy = -1;
		}
		
		int stepx = 1;
		if (dx < 0) 
		{
			dx = -dx; 
			stepx = -1;
		}
		
		dy <<= 1;
		dx <<= 1;
	 
		if(x1 >= 0 && x1 < width && y1 >= 0 && y1 < height)
			for(int x = -1; x <= 1; x++)
				for(int y = -1; y <= 1; y++)
					a_Texture.SetPixel(x1 + x, y1 + y, a_Color);
		
		if (dx > dy) 
		{
			int fraction = dy - (dx >> 1);
			
			while (x1 != x2) 
			{
				if (fraction >= 0) 
				{
					y1 += stepy;
					fraction -= dx;
				}
				
				x1 += stepx;
				fraction += dy;
				
				if(x1 >= 0 && x1 < width && y1 >= 0 && y1 < height)
					for(int x = -1; x <= 1; x++)
						for(int y = -1; y <= 1; y++)
							a_Texture.SetPixel(x1 + x, y1 + y, a_Color);
			}
		}
		else 
		{
			int fraction = dx - (dy >> 1);
			
			while (y1 != y2) 
			{
				if (fraction >= 0) 
				{
					x1 += stepx;
					fraction -= dy;
				}
				
				y1 += stepy;
				fraction += dx;
				
				if(x1 >= 0 && x1 < width && y1 >= 0 && y1 < height)
					for(int x = -1; x <= 1; x++)
						for(int y = -1; y <= 1; y++)
							a_Texture.SetPixel(x1 + x, y1 + y, a_Color);
			}
		}
		
	}
	
	// convert the matrix to quaternion, taking care of the mirroring
	private Quaternion ConvertMatrixToQuat(Matrix4x4 mOrient, int joint, bool flip)
	{
		var vZ = mOrient.GetColumn(2);
		var vY = mOrient.GetColumn(1);

		if(!flip)
		{
			vZ.y = -vZ.y;
			vY.x = -vY.x;
			vY.z = -vY.z;
		}
		else
		{
			vZ.x = -vZ.x;
			vZ.y = -vZ.y;
			vY.z = -vY.z;
		}

	    if (vZ.x != 0.0f || vZ.y != 0.0f || vZ.z != 0.0f) {
	        return Quaternion.LookRotation(vZ, vY);
	    }
	    return Quaternion.identity;
	}
}


