﻿using UnityEngine;

namespace BL.Vive
{
	public enum DPadDirection
	{
		Up,
		Left,
		Right,
		Down
	}

	[RequireComponent(typeof(SteamVR_TrackedObject))]
	public class ViveController : MonoBehaviour
	{
		public bool useDPadPress;
		public bool useDPadHover;
		public bool useTriggerInput;
        public bool useGripInput;

        SteamVR_TrackedObject trackedObj;
		SteamVR_Controller.Device controller
		{
			get
			{
				if (trackedObj == null)
				{
					trackedObj = GetComponent<SteamVR_TrackedObject>();
				}
				return SteamVR_Controller.Input( (int)trackedObj.index );
			}
		}

		Vector2 dPadPosition;
		bool[] dPadHovering = new bool[4];

		void Update ()
		{
			GetInput();
		}

		void GetInput ()
		{
			if (controller == null)
			{
				Debug.LogWarning("Vive controller not initialized");
				return;
			}

			if (useDPadHover)
			{
				GetDPadHover();
			}
			if (useDPadPress)
			{
				GetDPadPress();
			}
			if (useTriggerInput)
			{
                GetTrigger();
			}
            if (useGripInput)
            {
                GetGrip();
            }
		}

		void GetDPadHover ()
		{
			dPadPosition = controller.GetAxis();

			if (dPadPosition.y > 0.4f)
			{
				if (!dPadHovering[(int)DPadDirection.Up])
				{
					GetDPadExit();
					dPadHovering[(int)DPadDirection.Up] = true;
					OnDPadUpEnter();
				}
			}
			else if (dPadPosition.y < -0.4f)
			{
				if (!dPadHovering[(int)DPadDirection.Down])
				{
					GetDPadExit();
					dPadHovering[(int)DPadDirection.Down] = true;
					OnDPadDownEnter();
				}
			}
			else if (dPadPosition.x < -0.4f)
			{
				if (!dPadHovering[(int)DPadDirection.Left])
				{
					GetDPadExit();
					dPadHovering[(int)DPadDirection.Left] = true;
					OnDPadLeftEnter();
				}
			}
			else if (dPadPosition.x > 0.4f)
			{
				if (!dPadHovering[(int)DPadDirection.Right])
				{
					GetDPadExit();
					dPadHovering[(int)DPadDirection.Right] = true;
					OnDPadRightEnter();
				}
			}
		}

		void GetDPadExit ()
		{
			for (int i = 0; i < dPadHovering.Length; i++)
			{
				if (dPadHovering[i])
				{
					dPadHovering[i] = false;
					OnDPadExit();
				}
			}
		}

		void GetDPadPress ()
		{
			if (controller.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad))
			{
				if (dPadHovering[(int)DPadDirection.Up])
				{
					OnDPadUpPressed();
				}
				else if (dPadHovering[(int)DPadDirection.Left])
				{
					OnDPadLeftPressed();
				}
				else if (dPadHovering[(int)DPadDirection.Right])
				{
					OnDPadRightPressed();
				}
				else if (dPadHovering[(int)DPadDirection.Down])
				{
					OnDPadDownPressed();
				}
			}
		}

		void GetTrigger ()
		{
            if (controller.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
			{
                OnTriggerDown();
			}
	        else if (controller.GetPress(SteamVR_Controller.ButtonMask.Trigger))
	        {
                OnTriggerStay();
	        }
			if (controller.GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
			{
                OnTriggerUp();
			}
		}

        void GetGrip ()
        {
            if (controller.GetPressDown(SteamVR_Controller.ButtonMask.Grip))
            {
                OnGripDown();
            }
            else if (controller.GetPress(SteamVR_Controller.ButtonMask.Grip))
            {
                OnGripStay();
            }
            else if (controller.GetPressUp(SteamVR_Controller.ButtonMask.Grip))
            {
                OnGripUp();
            }
        }

		public virtual void OnDPadUpEnter () { }

		public virtual void OnDPadLeftEnter () { }

		public virtual void OnDPadRightEnter () { }

		public virtual void OnDPadDownEnter () { }

		public virtual void OnDPadExit () { }

		public virtual void OnDPadUpPressed () { }

		public virtual void OnDPadLeftPressed () { }

		public virtual void OnDPadRightPressed () { }

		public virtual void OnDPadDownPressed () { }

		public virtual void OnTriggerDown () { }

		public virtual void OnTriggerStay () { }

		public virtual void OnTriggerUp () { }

        public virtual void OnGripDown() { }

        public virtual void OnGripStay() { }

        public virtual void OnGripUp() { }
    }
}