using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
public class GizmoVisualizer : MonoBehaviour
{
   public enum GizmoType
    {
        Cube,
        Sphere,
    }
    public bool UseColliders;
    public GizmoType gizmoType;
    [Range(0,1f)]
    public float alpha = 1;
    public float debugSize = 0.03f;
    public Color DebugColor = Color.blue;

    private Collider _collider;

    Collider _Collider
    {
        get
        {
            if (_collider == null)
            {
                _collider = GetComponent<Collider>();
            }
            return _collider;
        }
    }
  

  void OnDrawGizmos()
    {
        Gizmos.color = DebugColor;
        Gizmos.matrix = transform.localToWorldMatrix;

        if (_Collider && UseColliders)
        {
            UsesColliders(false);
            return;
        }

        switch (gizmoType)
        {
            case GizmoType.Cube:
                Gizmos.DrawWireCube(Vector3.zero, new Vector3(debugSize, debugSize, debugSize));
                Gizmos.color = new Color(DebugColor.r, DebugColor.g, DebugColor.b, alpha);
                Gizmos.DrawCube(Vector3.zero, Vector3.one * debugSize);
                break;
            case GizmoType.Sphere:
               
                Gizmos.DrawWireSphere(Vector3.zero, debugSize);
                Gizmos.color = new Color(DebugColor.r, DebugColor.g, DebugColor.b, alpha);
                Gizmos.DrawSphere(Vector3.zero, debugSize);
                break;
            default:
                break;
        }
       
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1,1,0,1);
        Gizmos.matrix = transform.localToWorldMatrix;

        if (UseColliders && _Collider)
        {
            UsesColliders(true);
            return;
        }
        switch (gizmoType)
        {
            case GizmoType.Cube:
                Gizmos.DrawWireCube(Vector3.zero, Vector3.one * debugSize);       
                break;
            case GizmoType.Sphere:
                Gizmos.DrawWireSphere(Vector3.zero, debugSize);       
                break;
        }
    }

    void UsesColliders(bool sel)
    {
        if (_Collider is BoxCollider)
        {
            BoxCollider box = _Collider as BoxCollider;

            var sizeX = transform.lossyScale.x * box.size.x;
            var sizeY = transform.lossyScale.y * box.size.y;
            var sizeZ = transform.lossyScale.z * box.size.z;
            Matrix4x4 rotationMatrix = Matrix4x4.TRS(box.bounds.center, transform.rotation, new Vector3(sizeX, sizeY, sizeZ));

            Gizmos.matrix = rotationMatrix;
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
            if (!sel)
            {
                Gizmos.color = new Color(DebugColor.r, DebugColor.g, DebugColor.b, alpha);
                Gizmos.DrawCube(Vector3.zero, Vector3.one);
            }

        }
        else if (_Collider is SphereCollider)
        {
            SphereCollider _C = _Collider as SphereCollider;

            Gizmos.matrix = transform.localToWorldMatrix;

            Gizmos.DrawWireSphere(Vector3.zero + _C.center, _C.radius);
            if (!sel)
            {
                Gizmos.color = new Color(Gizmos.color.r, Gizmos.color.g, Gizmos.color.b, alpha);
                Gizmos.DrawSphere(Vector3.zero + _C.center, _C.radius);
            }
        }
    }
}
#endif