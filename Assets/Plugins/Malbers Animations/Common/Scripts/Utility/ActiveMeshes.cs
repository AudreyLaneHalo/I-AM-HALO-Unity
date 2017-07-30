using UnityEngine;
using System.Collections.Generic;
using System;

namespace MalbersAnimations.Utilities
{

    [Serializable]
    public class ActiveSMesh
    {
        [HideInInspector]
        public string Name = "NameHere";
        public Transform[] meshes;
        [HideInInspector]
        [SerializeField]
        public int current;

        public virtual void ToggleMesh()
        {
            current++;
            if (current >= meshes.Length) current = 0;

            foreach (var item in meshes)
            {
                if (item) item.gameObject.SetActive(false);
            }

            if (meshes[current]) meshes[current].gameObject.SetActive(true);
        }
    }



    public class ActiveMeshes : MonoBehaviour
    {
        [SerializeField] public List<ActiveSMesh> ToggleMeshes =  new List<ActiveSMesh>();
        [HideInInspector]
        [SerializeField] public bool showMeshesList = true;

        public virtual void ToggleMesh(int index)
        {
            ToggleMeshes[index].ToggleMesh();
        }
    }
}