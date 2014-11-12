/************************************************************************
*                                                                       *
*				      KinectSDK-Unity3D C# Wrapper:                     *
*	                     Attach to a GameObject                         *
*																		*
*						  Author: Andrew DeVine							*
*				  University of Central Florida ISUE Lab				*
*								  2012									*
*		   (see included BSD license for licensing information)         *
************************************************************************/

using UnityEngine;
using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections;

/// <summary>
/// Kinect method access
/// </summary>
public class KUInterface : MonoBehaviour {
    //public variables
    /// <summary>
    /// scales all joint positions by given amount. Do not set to zero.
    /// </summary>
    public int scaleFactor = 1000;
    /// <summary>
    /// set to true to track two skeletons
    /// </summary>
    public bool twoPlayer = false;
    /// <summary>
    /// set to false to optimize performance if RGB camera is not being used
    /// </summary>
    public bool useRGB = false;
    /// <summary>
    /// set to false to optimize performance if depth camera is not being used
    /// </summary>
    public bool useDepth = false;
    /// <summary>
    /// displays joint position data on screen
    /// </summary>
    public bool displayJointInformation = false;
    /// <summary>
    /// displays RGB texture image on screen
    /// </summary>
    public bool displayTextureImage = false;
    /// <summary>
    /// displays depth image on screen
    /// </summary>
    public bool displayDepthImage = false;

    //constants
    public const int ColorImageWidth = 640;
    public const int ColorImageHeight = 480;

    //private variables
    private bool NUIisReady = false;
    private float lastCameraAngleChange = -30.0f;
    private byte[] seqTex;
    private Color32[] cols;
    private Texture2D texture;
    private byte[] seqDepth;
    private Color32[] dcols;
    private Texture2D depthImg;
    private short[][] depth;

	public bool IsCameryReady {
		get{ return NUIisReady; }
	}

/********************************************************************************
*           USER METHODS -> Call these methods from your scripts
* ******************************************************************************/

    /// <summary>
    /// main joint position get function
    /// </summary>
    /// <param name="player">player number (1,2)</param>
    /// <param name="joint">KinectWrapper.Joints enum</param>
    /// <returns>position of given joint for given player</returns>
    public Vector3 GetJointPos(int player, KinectWrapper.Joints joint) {

        KinectWrapper.SkeletonTransform trans = new KinectWrapper.SkeletonTransform();
        if (NUIisReady && (player == 1 || player == 2))
        {
            KinectWrapper.GetSkeletonTransform(player, (int)joint, ref trans);
            return new Vector3(trans.x * scaleFactor, trans.y * scaleFactor, trans.z * scaleFactor);
        }
        else
        {
            return Vector3.zero;
        }
    }


    /// <summary>
    /// one-player overload of joint position get function
    /// </summary>
    /// <param name="joint">KinectWrapper.Joints enum</param>
    /// <returns>position of given joint</returns>
    public Vector3 GetJointPos(KinectWrapper.Joints joint) {

        return GetJointPos(1, joint);
    }

	public bool IsJointTracked(int player, KinectWrapper.Joints joint) {
		return !GetJointPos(player, joint).Equals (Vector3.zero);
	}

	public bool IsJointTracked(KinectWrapper.Joints joint) {
		return IsJointTracked (1, joint);
	}

	public bool IsUserDetected(){
		return IsJointTracked (1, KinectWrapper.Joints.HIP_CENTER) && IsJointTracked (1, KinectWrapper.Joints.SHOULDER_CENTER);
	}

	public Vector2 GetDepthMapPosForJointPos(int player, KinectWrapper.Joints joint){
		var pos3 = GetJointPos (player, joint);
		var pos2 = new Vector2 (pos3.x, pos3.y);
		return pos2;
	}

	public Vector2 GetDepthMapPosForJointPos(KinectWrapper.Joints joint){
		return GetDepthMapPosForJointPos (1, joint);
	}

	public Vector2 GetColorMapPosForDepthPos(Vector2 posDepth) {
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
	
	/// <summary>
	/// gets color texture image from Kinect RGB camera
    /// </summary>
    /// <returns>RGB image</returns>
    public Texture2D GetTextureImage() {

        return this.texture;
    }


    /// <summary>
    /// gets depth data from Kinect depth camera
    /// </summary>
    /// <returns>2D array of pixel depths as bytes</returns>
    /// <remarks>depth[x=0][y=0] corresponds to top-left corner of image</remarks>
    public short[][] GetDepthData() {

        return this.depth;
    }


    /// <summary>
    /// returns current Kinect camera angle from horizontal
    /// </summary>
    /// <returns>camera angle from horizontal</returns>
    public float GetCameraAngle() {

        float cameraAngle = 0;

        KinectWrapper.GetCameraAngle(ref cameraAngle);
        return cameraAngle;
    }


    /// <summary>
    /// sets Kinect camera angle
    /// </summary>
    /// <param name="angle">range: -27 -> 27</param>
    /// <returns>returns true if successful</returns>
    /// <remarks>do not change angle more than once every 30 sec</remarks>
    public bool SetCameraAngle(int angle) {

        /* DO NOT CHANGE CAMERA ANGLE MORE OFTEN THAN ONCE
         * EVERY 30 SECONDS, IT COULD DAMAGE THE KINECT
         * SENSOR (SEE KINECT SDK DOCUMENTATION).
         */

        if (Time.time - lastCameraAngleChange > 30 && NUIisReady) {
            lastCameraAngleChange = Time.time;
            return KinectWrapper.SetCameraAngle(angle);
        } else {
            return false;
        }
    }


/********************************************************************************
*       DEVICE MANAGEMENT -> initialize, uninitialize, and update sensor
* ******************************************************************************/

    //called on application start
    private void Start() {

        NUIisReady = false;

        //initialize Kinect sensor
        NUIisReady = KinectWrapper.NuiContextInit(twoPlayer);

        //display messages
        if (NUIisReady)
            Debug.Log("Sensor Initialized.");
        else
            Debug.Log("Could Not Initialize Sensor.");

        if (scaleFactor == 0)
            Debug.Log("WARNING: KUInterface.scaleFactor is set to zero. All joint positions will be the zero vector.");

        //set up image memory
        seqTex = new byte[ColorImageWidth * ColorImageHeight * 4];
        cols = new Color32[ColorImageWidth * ColorImageHeight];
        texture = new Texture2D(ColorImageWidth, ColorImageHeight);

        seqDepth = new byte[ColorImageWidth * ColorImageHeight * 2];
        dcols = new Color32[ColorImageWidth * ColorImageHeight];
        depthImg = new Texture2D(ColorImageWidth, ColorImageHeight);
        depth = new short[ColorImageWidth][];
        for (int i = 0; i < depth.Length; i++) {
            depth[i] = new short[ColorImageHeight];
        }
    }


    //called on application stop
    private void OnApplicationQuit() {

        NUIisReady = false;
        KinectWrapper.NuiContextUnInit();
        Debug.Log("Sensor Off.");
    }


    //called every Unity frame (frame-rate dependent)
    private void Update() {

        if (NUIisReady)
        {
            //update Kinect
            KinectWrapper.NuiUpdate();

            //update RGB texture
            if (useRGB) UpdateTextureImage();

            //update Depth data
            if (useDepth) UpdateDepth();
        }
    }


    //update RGB texture
    private void UpdateTextureImage() {

        int size = 0;
		var bytes = ColorImageWidth * ColorImageHeight * 4;

        //copy pixel data from unmanaged memory
        IntPtr ptr = KinectWrapper.GetTextureImage(ref size);

        if (ptr != IntPtr.Zero) {
            Marshal.Copy(ptr, seqTex, 0, size);
            //Debug.Log(size.ToString());

            //create color matrix
			for (int i = 0; i < bytes; i += 4) {
				//cols[(IM_W * IM_H) - (i / 4) - 1] = new Color32(seqTex[i + 2], seqTex[i + 1], seqTex[i], 255);
				cols[i/4] = new Color32(seqTex[i + 2], seqTex[i + 1], seqTex[i], 255);
			}
			
			//set texture
            texture.SetPixels32(cols);
            texture.Apply();
        }
    }


    //update Depth array and texture
    private void UpdateDepth() {

        int size = 0;

        //copy pixel data from unmanaged memory
        IntPtr ptr = KinectWrapper.GetDepthImage(ref size);

        if (ptr != IntPtr.Zero)
        {
            Marshal.Copy(ptr, seqDepth, 0, size);

            //create depth array
            for (int x = 0; x < ColorImageWidth; x++)
            {
                for (int y = 0; y < ColorImageHeight; y++)
                {
                    depth[ColorImageWidth - x - 1][ColorImageHeight - y - 1] = BitConverter.ToInt16(seqDepth, (x*2)+(y*ColorImageWidth*2));
                }
            }

            if (displayDepthImage)
            {
                //create color matrix as bytes (loops resolution)
                for (int x = 0; x < ColorImageWidth; x++)
                {
                    for (int y = 0; y < ColorImageHeight; y++)
                    {
                        dcols[x + (y * ColorImageWidth)] = new Color32((byte)depth[x][y], (byte)depth[x][y], (byte)depth[x][y], 255);
                    }
                }

                //set texture
                depthImg.SetPixels32(dcols);
                depthImg.Apply();
            }
        }
    }


    //update function for GUI (if enabled)
    private void OnGUI() {

        if (displayJointInformation && NUIisReady) {
            //display joint info in upper-left corner
            if (twoPlayer) {
                for (int i = 0; i < (int)KinectWrapper.Joints.COUNT; i++) {
                    DisplayPlayerData(1, i);
                    DisplayPlayerData(2, i);
                }
            } else {
                for (int i = 0; i < (int)KinectWrapper.Joints.COUNT; i++) {
                    DisplayPlayerData(1, i);
                }
            }
        }

        if (displayTextureImage && useRGB && NUIisReady) {
            //scaled to half-res
            GUI.DrawTexture(new Rect(600, 50, ColorImageWidth / 2, ColorImageHeight / 2), texture, ScaleMode.ScaleToFit, false);
        }

        if (displayDepthImage && useDepth && NUIisReady) {
            //scaled to half-res
            GUI.DrawTexture(new Rect(600, 300, ColorImageWidth / 2, ColorImageHeight / 2), depthImg, ScaleMode.ScaleToFit, false);
        }
    }


    //displays joint position data in GUI (if enabled)
    private void DisplayPlayerData(int player, int place) {

        Vector3 joint = GetJointPos(player, (KinectWrapper.Joints)place);

        GUI.Label(new Rect((player - 1) * 300, place * 30, 200, 30), joint.ToString());
    }
}


//------------------------------------------------------------------------------------------------
//------------------------------------------------------------------------------------------------

/// <summary>
/// KUInterface.dll wrapper
/// </summary>
public class KinectWrapper {

    /// <summary>
    /// defines Kinect skeleton
    /// </summary>
    public enum Joints {
        HIP_CENTER = 0,
        SPINE,
        SHOULDER_CENTER,
        HEAD,
        SHOULDER_LEFT,
        ELBOW_LEFT,
        WRIST_LEFT,
        HAND_LEFT,
        SHOULDER_RIGHT,
        ELBOW_RIGHT,
        WRIST_RIGHT,
        HAND_RIGHT,
        HIP_LEFT,
        KNEE_LEFT,
        ANKLE_LEFT,
        FOOT_LEFT,
        HIP_RIGHT,
        KNEE_RIGHT,
        ANKLE_RIGHT,
        FOOT_RIGHT,
        COUNT
    };


    [StructLayout(LayoutKind.Sequential)]
    public struct SkeletonTransform {

        public float x, y, z, w;
    }


    //NUI Context Management
    [DllImport("/Assets/Plugins/KUInterface.dll")]
    public static extern bool NuiContextInit(bool twoPlayer);
    [DllImport("/Assets/Plugins/KUInterface.dll")]
    public static extern void NuiUpdate();
    [DllImport("/Assets/Plugins/KUInterface.dll")]
    public static extern void NuiContextUnInit();
    //Get Methods
    [DllImport("/Assets/Plugins/KUInterface.dll")]
    public static extern void GetSkeletonTransform(int player, int joint, ref SkeletonTransform trans);
    [DllImport("/Assets/Plugins/KUInterface.dll")]
    public static extern IntPtr GetTextureImage(ref int size);
    [DllImport("/Assets/Plugins/KUInterface.dll")]
    public static extern IntPtr GetDepthImage(ref int size);
    [DllImport("/Assets/Plugins/KUInterface.dll")]
    public static extern void GetCameraAngle(ref float angle);
    //Set Methods
    [DllImport("/Assets/Plugins/KUInterface.dll")]
    public static extern bool SetCameraAngle(int angle);
}