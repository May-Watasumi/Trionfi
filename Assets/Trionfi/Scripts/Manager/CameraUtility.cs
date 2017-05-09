using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class CameraUtility : MonoBehaviour
{

	private Vector2 aspect = new Vector2(4,3);
	private Color32 backgroundColor = Color.black;
	private  float aspectRate ;
	private Camera _camera;
	private static Camera _backgroundCamera;

	void Start ()
	{
		aspectRate = (float)aspect.x / aspect.y;
		_camera = GetComponent<Camera>();

		JokerSetting jokerSetting = GameObject.Find ("JOKER_SETTING").GetComponent<JokerSetting> ();
		this.aspect = jokerSetting.aspect;

		CreateBackgroundCamera ();
		UpdateScreenRate ();

		//enabled = false;
	}

	void CreateBackgroundCamera ()
	{
		#if UNITY_EDITOR
		if(! UnityEditor.EditorApplication.isPlaying )
			return;
		#endif

		if (_backgroundCamera != null)
			return;

		var backGroundCameraObject = new GameObject ("Background Color Camera");
		_backgroundCamera = backGroundCameraObject.AddComponent<Camera> ();
		_backgroundCamera.depth = -99;
		_backgroundCamera.fieldOfView = 1;
		_backgroundCamera.farClipPlane = 1.1f;
		_backgroundCamera.nearClipPlane = 1; 
		_backgroundCamera.cullingMask = 0;
		_backgroundCamera.depthTextureMode = DepthTextureMode.None;
		_backgroundCamera.backgroundColor = backgroundColor;
		_backgroundCamera.renderingPath = RenderingPath.VertexLit;
		_backgroundCamera.clearFlags = CameraClearFlags.SolidColor;
		_backgroundCamera.useOcclusionCulling = false;
		backGroundCameraObject.hideFlags = HideFlags.NotEditable;
	}

	void UpdateScreenRate ()
	{
		float baseAspect = aspect.y / aspect.x;
		float nowAspect = (float)Screen.height / Screen.width;

		if (baseAspect > nowAspect) {
			var changeAspect = nowAspect / baseAspect;
			_camera.rect = new Rect ((1 - changeAspect) * 0.5f, 0, changeAspect, 1);
		} else {
			var changeAspect = baseAspect / nowAspect;
			_camera.rect = new Rect (0, (1 - changeAspect) * 0.5f, 1, changeAspect);
		}
	}

	bool IsChangeAspect ()
	{
		return _camera.aspect == aspectRate;
	}

	void Update ()
	{
		if (IsChangeAspect ())
			return;

		UpdateScreenRate ();
		_camera.ResetAspect ();
	}
}