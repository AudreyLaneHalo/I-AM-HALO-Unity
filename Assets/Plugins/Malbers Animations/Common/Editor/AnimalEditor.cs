using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;

namespace MalbersAnimations
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Animal))]
    public class AnimalEditor : Editor
    {
        protected Animal myAnimal;
        protected SerializedProperty
            animalTypeID, GroundLayer, StartSpeed, height, WalkSpeed, TrotSpeed, RunSpeed, TurnSpeed, FallRayDistance,
            maxAngleSlope, GotoSleep, SnapToGround, swapSpeed, waterLine, swimSpeed, swimTurn,
            life, defense, attackStrength, debug;
        protected bool
            ground = true,
            water = true,
            advanced = true,
            atributes = true;
        

        GUIStyle currentStyle;

        private void OnEnable()
        {
            myAnimal = (Animal)target;
            FindProperties();
        }

        protected void FindProperties()
        {
            animalTypeID = serializedObject.FindProperty("animalTypeID");
            GroundLayer =  serializedObject.FindProperty("GroundLayer");
            StartSpeed =   serializedObject.FindProperty("StartSpeed");
            height =       serializedObject.FindProperty("height");
            WalkSpeed = serializedObject.FindProperty("WalkSpeed");
            TrotSpeed = serializedObject.FindProperty("TrotSpeed");
            RunSpeed = serializedObject.FindProperty("RunSpeed");
            TurnSpeed = serializedObject.FindProperty("TurnSpeed");
            maxAngleSlope = serializedObject.FindProperty("maxAngleSlope");
            GotoSleep = serializedObject.FindProperty("GotoSleep");
            SnapToGround = serializedObject.FindProperty("SnapToGround");
            swapSpeed = serializedObject.FindProperty("swapSpeed");
            waterLine = serializedObject.FindProperty("waterLine");
            swimSpeed = serializedObject.FindProperty("swimSpeed");
            swimTurn = serializedObject.FindProperty("swimTurn");
            life = serializedObject.FindProperty("life");
            defense = serializedObject.FindProperty("defense");
            attackStrength = serializedObject.FindProperty("attackStrength");
            FallRayDistance = serializedObject.FindProperty("FallRayDistance");
            debug = serializedObject.FindProperty("debug");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawAnimalInspector();

            serializedObject.ApplyModifiedProperties();
        }

        protected void DrawAnimalInspector()
        {
            EditorGUILayout.BeginVertical(MalbersEditor.StyleBlue);
            EditorGUILayout.HelpBox("Locomotion System", MessageType.None,true);
            EditorGUILayout.EndVertical();

            currentStyle = MalbersEditor.StyleGray;

            EditorGUILayout.BeginVertical(currentStyle);
            myAnimal.animalTypeID = EditorGUILayout.IntField(new GUIContent("Animal Type ID", "Activate the correct additive Animation to offset the Bones"), myAnimal.animalTypeID);
            EditorGUILayout.EndVertical();

            //────────────────────────────────── Ground ──────────────────────────────────
            EditorGUILayout.BeginVertical(currentStyle);
            EditorGUI.indentLevel++;
            ground = EditorGUILayout.Foldout(ground, "Ground");
            EditorGUI.indentLevel--;
            if (ground)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.PropertyField(GroundLayer, new GUIContent("Ground Layer", "Specify wich layer are Ground"));
                EditorGUILayout.PropertyField(StartSpeed, new GUIContent("Start Speed", "Activate the correct additive Animation to offset the Bones"));
                EditorGUILayout.PropertyField(height, new GUIContent("Height", "Distance from the ground"));
                EditorGUILayout.EndVertical();

                EditorGUILayout.Space();

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.PropertyField(WalkSpeed, new GUIContent("Walk Speed", "Add Speed to the walk... Greater than 0 the animal will Slide"));
                EditorGUILayout.PropertyField(TrotSpeed, new GUIContent("Trot Speed", "Add Speed to the trot... Greater than 0 the animal will Slide"));
                EditorGUILayout.PropertyField(RunSpeed, new GUIContent("Run Speed", "Add Speed to the run... Greater than 0 the animal will Slide"));
                EditorGUILayout.PropertyField(TurnSpeed, new GUIContent("Turn Speed", "Add Speed to the turn"));

                EditorGUILayout.EndVertical();
 
            }
            
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(currentStyle);
            EditorGUI.indentLevel++;
            advanced = EditorGUILayout.Foldout(advanced, "Advanced");
            EditorGUI.indentLevel--;
            if (ground)
            {
              

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.PropertyField(maxAngleSlope, new GUIContent("Max Angle Slope", "Max Angle that the animal can walk"));
                EditorGUILayout.PropertyField(GotoSleep, new GUIContent("Go to Sleep", "Number of Idles before going to sleep (AFK)"));
                EditorGUILayout.PropertyField(FallRayDistance, new GUIContent("Fall Ray Multiplier", "Multiplier to set the Fall Ray in front of the animal"));
                EditorGUILayout.PropertyField(SnapToGround, new GUIContent("Snap to ground", "Smoothness to aling to terrain"));
                EditorGUILayout.PropertyField(swapSpeed, new GUIContent("Swap Speed", "Swap the Speed with Shift instead of 1 2 3"));

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(new GUIContent("Locomotion Speed", "This are the values for the Animator Locomotion Blend Tree when the velocity is changed"), GUILayout.MaxWidth(120));
                myAnimal.movementS1 = EditorGUILayout.FloatField(myAnimal.movementS1, GUILayout.MaxWidth(28));
                myAnimal.movementS2 = EditorGUILayout.FloatField(myAnimal.movementS2, GUILayout.MaxWidth(28));
                myAnimal.movementS3 = EditorGUILayout.FloatField(myAnimal.movementS3, GUILayout.MaxWidth(28));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndVertical();


            //────────────────────────────────── Water ──────────────────────────────────

            EditorGUILayout.BeginVertical(currentStyle);
            EditorGUI.indentLevel++;
            water = EditorGUILayout.Foldout(water, "Water");
            EditorGUI.indentLevel--;
            if (water)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.PropertyField(waterLine, new GUIContent("Water Line", "Aling the animal to the Water Surface"));
                EditorGUILayout.PropertyField(swimSpeed, new GUIContent("Swim Speed", "Add Speed to the Swim... Greater than 0 the animal will Slide"));
                EditorGUILayout.PropertyField(swimTurn, new GUIContent("Swim Turn", "Add Speed to the run... Greater than 0 the animal will Slide"));
                EditorGUILayout.EndVertical();

            }
            EditorGUILayout.EndVertical();

            //────────────────────────────────── Atributes ──────────────────────────────────

            EditorGUILayout.BeginVertical(currentStyle);
            EditorGUI.indentLevel++;
            atributes = EditorGUILayout.Foldout(atributes, "Atributes");
            EditorGUI.indentLevel--;
            if (atributes)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.PropertyField(life, new GUIContent("Life", "Life Points"));
                EditorGUILayout.PropertyField(defense, new GUIContent("Defense", "Defense Points"));
                EditorGUILayout.PropertyField(attackStrength, new GUIContent("Attack", "Attack Points"));
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndVertical();


            EditorGUILayout.BeginVertical(currentStyle);
            EditorGUILayout.PropertyField(debug, new GUIContent("Debug"));
            EditorGUILayout.EndVertical();
        }
    }
}