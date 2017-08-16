using UnityEngine;
using System.Collections.Generic;
using System;

namespace MalbersAnimations.Utilities
{

    public class ActiveMeshes : MonoBehaviour
    {
        [SerializeField]
        public List<ActiveSMesh> ToggleMeshes = new List<ActiveSMesh>();
        [HideInInspector]
        [SerializeField]
        public bool showMeshesList = true;

        public virtual void ToggleMesh(int index)
        {
            ToggleMeshes[index].ToggleMesh();
        }

        public virtual void ToggleMesh(int index, bool next)
        {
            ToggleMeshes[index].ToggleMesh(next);
        }

        /// <summary>
        /// Toogle all meshes on the list
        /// </summary>
        public virtual void ToggleMesh(bool next = true)
        {
            foreach (var mesh in ToggleMeshes)
            {
                mesh.ToggleMesh(next);
            }
        }

        public virtual ActiveSMesh GetActiveMesh(string name)
        {
            if (ToggleMeshes.Count == 0) return null;

            return ToggleMeshes.Find(item => item.Name == name);
        }

        public virtual ActiveSMesh GetActiveMesh(int index)
        {
            if (ToggleMeshes.Count == 0) return null;

            if (index >= ToggleMeshes.Count) index = 0;
            if (index < 0) index = ToggleMeshes.Count - 1;

            return ToggleMeshes[index];
        }
    }


    [Serializable]
    public class ActiveSMesh
    {
        [HideInInspector]
        public string Name = "NameHere";
        public Transform[] meshes;
        [HideInInspector]
        [SerializeField]
        public int current;

        public virtual void ToggleMesh(bool next = true)
        {
            if (next)
                current++;
            else
                current--;

            if (current >= meshes.Length) current = 0;
            if (current < 0) current = meshes.Length - 1;

            foreach (var item in meshes)
            {
                if (item) item.gameObject.SetActive(false);
            }

            if (meshes[current]) meshes[current].gameObject.SetActive(true);
        }

        public virtual void ToggleMesh(int Index)
        {
            current = Index - 1;
            ToggleMesh();
        }
    }
}