using UnityEngine;
using System;
using System.Collections.Generic;
using MalbersAnimations;

namespace MalbersAnimations.Utilities
{
    [Serializable]
    public class MaterialItem
    {
        [SerializeField]
        [HideInInspector]
        public string Name;
        public Renderer mesh;
        public Material[] materials;
        [HideInInspector]
        [SerializeField]        public int current = 0;

        public MaterialItem()
        {
            mesh = null;
            materials = new Material[0];
            Name = "NameHere";
        }

        public virtual void ChangeMaterial()
        {
            current++;
            if (current >= materials.Length) current = 0;
            mesh.material = materials[current];
        }
    }

    public class MaterialChanger : MonoBehaviour
    {
        [SerializeField]
        public List<MaterialItem> materialList =  new List<MaterialItem>();
        [HideInInspector]
        [SerializeField] public bool showMeshesList = true;
    }
}
