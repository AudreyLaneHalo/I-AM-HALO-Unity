using UnityEngine;
using UnityEditor;

namespace MalbersAnimations.HAP
{
    [CustomEditor(typeof(Mountable), true)]
    public class MHorseEditor : Editor
    {
        Mountable MAnimal;
        bool CallHelp;

        private void OnEnable()
        {
            MAnimal = (Mountable)target;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.BeginVertical(MalbersEditor.StyleBlue);
            EditorGUILayout.HelpBox("Makes this GameObject mountable. Need Mount Triggers and IK Links", MessageType.None);
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(MalbersEditor.StyleGray);
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                MAnimal.mountType = (MountType)EditorGUILayout.EnumPopup(new GUIContent("Type"), MAnimal.mountType);
                MAnimal.Active = EditorGUILayout.Toggle(new GUIContent("Active", "If the Mount can be mounted. Deactivate if the mount is death or destroyed or is not ready to be mountable"), MAnimal.Active);
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(MalbersEditor.StyleGray);
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                {
                    MAnimal.straightSpine = EditorGUILayout.ToggleLeft(new GUIContent("Straight Mount Point", "Straighten the Mount Point to fix the Rider Animation"), MAnimal.straightSpine);
                    MAnimal.spineOffset = EditorGUILayout.Vector3Field(new GUIContent("Point Offset", ""), MAnimal.spineOffset);
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndVertical();


            EditorGUILayout.BeginVertical(MalbersEditor.StyleGray);
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.BeginHorizontal();
                MAnimal.ShowLinks = EditorGUILayout.Foldout(MAnimal.ShowLinks, "Links");
                CallHelp = GUILayout.Toggle(CallHelp, "?", EditorStyles.miniButton, GUILayout.Width(18));
                EditorGUILayout.EndHorizontal();

                EditorGUI.indentLevel--;
                if (MAnimal.ShowLinks)
                {
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    {
                        if (CallHelp) EditorGUILayout.HelpBox("'Mount Point' is obligatory, the rest are optional", MessageType.None);
                      
                        MAnimal.ridersLink = (Transform)EditorGUILayout.ObjectField(new GUIContent("Mount Point", "Reference for the Mount Point"), MAnimal.ridersLink, typeof(Transform), true);
                        MAnimal.leftIK = (Transform)EditorGUILayout.ObjectField(new GUIContent("Left IK", "Reference for the Left Foot correct position on the mount"), MAnimal.leftIK, typeof(Transform), true);
                        MAnimal.rightIK = (Transform)EditorGUILayout.ObjectField(new GUIContent("Right IK", "Reference for the Right Foot correct position on the mount"), MAnimal.rightIK, typeof(Transform), true);
                        MAnimal.leftKnee = (Transform)EditorGUILayout.ObjectField(new GUIContent("Left Knee", "Reference for the Left Knee correct position on the mount"), MAnimal.leftKnee, typeof(Transform), true);
                        MAnimal.rightKnee = (Transform)EditorGUILayout.ObjectField(new GUIContent("Right Knee", "Reference for the Right Knee correct position on the mount"), MAnimal.rightKnee, typeof(Transform), true);
                    }
                    EditorGUILayout.EndVertical();
                }
            }
            EditorGUILayout.EndVertical();

            if (MAnimal.ridersLink == null)
            {
                EditorGUILayout.HelpBox("'Mount Point'  is empty, please set a reference", MessageType.Warning);
            }

            EditorUtility.SetDirty(target);

            serializedObject.ApplyModifiedProperties();
        }
    }
}