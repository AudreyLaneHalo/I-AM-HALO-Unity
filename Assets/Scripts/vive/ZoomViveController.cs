using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace BL.Vive
{
	public class ZoomViveController : ViveController
	{
		public bool triggerDown;

		ViveInputManager _inputManager;
		ViveInputManager inputManager
		{
			get
			{
				if (_inputManager == null)
				{
					_inputManager = GetComponentInParent<ViveInputManager>();
				}
				return _inputManager;
			}
		}

		public override void OnTriggerDown()
		{
			triggerDown = true;
			inputManager.StartPinch( this );
		}

		public override void OnTriggerUp()
		{
			triggerDown = false;
			inputManager.StopPinch();
		}
	}
}