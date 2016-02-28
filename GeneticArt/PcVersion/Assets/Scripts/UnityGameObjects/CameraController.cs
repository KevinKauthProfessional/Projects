//------------------------------------------------------------------
// <copyright file="EntBehaviorManager.cs">
//     Copyright (c) Kevin Joshua Kauth.  All rights reserved.
// </copyright>
//------------------------------------------------------------------
namespace AssemblyCSharp.Scripts.UnityGameObjects
{
	using System;

	using AssemblyCSharp.Scripts.GameLogic;
	using UnityEngine;

	/// <summary>
	/// Camera controller.
	/// </summary>
	public class CameraController : MonoBehaviour 
	{
		/// <summary>
		/// The zoom in max.
		/// </summary>
		public float ZoomInMax;

		/// <summary>
		/// The zoom out max.
		/// </summary>
		public float ZoomOutMax;

		/// <summary>
		/// The zoom delta.
		/// </summary>
		public float ZoomDelta;

		/// <summary>
		/// The camera field of view.
		/// Measureed in degrees from Y axis.
		/// </summary>
		private const float CameraFieldOfView = 90.0f;
		private const string CameraObjectName = "Main Camera";

		/// <summary>
		/// Resets the main camera.
		/// </summary>
		public static void ResetMainCamera()
		{
			GameObject cameraObject = GameObject.Find(CameraObjectName);
			CameraController controller = cameraObject.GetComponent<CameraController>();
			controller.Reset();
		}

		/// <summary>
		/// Reset this instance.
		/// </summary>
		public void Reset()
		{
			float x = (((float)GameObjectGrid.GridWidth * GameObjectGrid.CellScale) - GameObjectGrid.CellScale) / 2.0f;
			float z = (((float)GameObjectGrid.GridHeight * GameObjectGrid.CellScale) - GameObjectGrid.CellScale) / 2.0f;

			// 1 Rad = 180 degrees
			float y = ((float)GameObjectGrid.GridHeight / 3.56f) / (float)Math.Tan(CameraFieldOfView / 180.0f);

			// Tan(45) = (grid height / 2) / camera y
			// Camera y  = (gridheight / 2) / Tan(field of view / 2)
			Camera camera = this.GetComponent<Camera>();
			camera.fieldOfView = CameraFieldOfView;
			
			this.transform.position = new Vector3(x, y, z);
		}

		// Use this for initialization
		public void Start () 
		{
			this.Reset();
		}
		
		// Update is called once per frame
		public void Update () 
		{
			// Mouse wheel moving forward
			if(Input.GetAxis("Mouse ScrollWheel") > 0 && this.transform.position.y > ZoomInMax)
			{
				this.transform.position -= new Vector3(0.0f, ZoomDelta, 0.0f);
			}
			
			// Mouse wheel moving backward
			if(Input.GetAxis("Mouse ScrollWheel") < 0 && this.transform.position.y < ZoomOutMax)
			{
				this.transform.position += new Vector3(0.0f, ZoomDelta, 0.0f);
			}
		}
	}
}
