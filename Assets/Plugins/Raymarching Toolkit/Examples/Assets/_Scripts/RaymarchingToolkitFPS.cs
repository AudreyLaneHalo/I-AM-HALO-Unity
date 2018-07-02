// FPS Controller
// 1. Create a Parent Object like a 3D model
// 2. Make the Camera the user is going to use as a child and move it to the height you wish. 
// 3. Attach a Rigidbody to the parent
// 4. Drag the Camera into the m_Camera public variable slot in the inspector
// Escape Key: Escapes the mouse lock
// Mouse click after pressing escape will lock the mouse again


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RaymarchingToolkit.Examples {
[RequireComponent(typeof(Rigidbody))]
public class RaymarchingToolkitFPS : MonoBehaviour
{
    [SerializeField] private float speed = 5.0f;
    private float m_MovX;
    private float m_MovY;
    private Vector3 m_moveHorizontal;
    private Vector3 m_movVertical;
    private Vector3 m_velocity;
    private Rigidbody m_Rigid;
    private float m_yRot;
    private float m_xRot;
    private Vector3 m_rotation;
    private Vector3 m_cameraRotation;
    private bool m_cursorIsLocked = true;

    public Camera m_Camera;

    // Use this for initialization
    private void Start()
    {
        m_Rigid = GetComponent<Rigidbody>();
        if (!m_Camera)
            m_Camera = GetComponent<Camera>();
        if (!m_Camera)
            m_Camera = GetComponentInChildren<Camera>();
        if (!m_Camera)
            m_Camera = Camera.main;
    }

    public float sensitivityX = 32f;
    public float sensitivityY = 32f;

    public float minimumY = -60F;
    public float maximumY = 60F;

    float rotationY;

    public void Update()
    {
        float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX * Time.deltaTime;

        rotationY += Input.GetAxis("Mouse Y") * sensitivityY * Time.deltaTime;
        rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);

        transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
    }

    public void FixedUpdate()
    {
        m_MovX = Input.GetAxis("Horizontal");
        m_MovY = Input.GetAxis("Vertical");

        m_moveHorizontal = transform.right * m_MovX;
        m_movVertical = transform.forward * m_MovY;

        m_velocity = (m_moveHorizontal + m_movVertical).normalized * (Input.GetButton("Fire3") ? 3 : 1) * speed * Time.fixedDeltaTime;

        //move the actual player here
        if (m_velocity != Vector3.zero)
            m_Rigid.MovePosition(m_Rigid.position + m_velocity);

        InternalLockUpdate();
    }

    void OnDisable() {
        UnlockCursor();
    }

    //controls the locking and unlocking of the mouse
    private void InternalLockUpdate()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            m_cursorIsLocked = false;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            m_cursorIsLocked = true;
        }

        if (m_cursorIsLocked)
        {
            UnlockCursor();
        }
        else if (!m_cursorIsLocked)
        {
            LockCursor();
        }
    }

    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

}
}