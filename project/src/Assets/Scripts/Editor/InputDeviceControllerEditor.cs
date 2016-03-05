using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(InputDeviceController))]
public class InputDeviceControllerEditor : Editor
{
    InputDeviceController myTarget;

    //Tool LastTool = Tool.None;

    void OnEnable()
    {
        myTarget = (InputDeviceController)target;

        //LastTool = Tools.current;
        //Tools.current = Tool.None;
    }

    void OnDisable()
    {
        //Tools.current = LastTool;
    }
    public override void OnInspectorGUI()
    {

        GUIStyle myFoldoutStyle = new GUIStyle(EditorStyles.foldout);
        myFoldoutStyle.fontStyle = FontStyle.Bold;
        

        EditorGUILayout.LabelField("Controller State", EditorStyles.boldLabel);
        myTarget.HorizontalAxis = EditorGUILayout.FloatField("Horizontal", myTarget.HorizontalAxis);
        myTarget.VerticalAxis = EditorGUILayout.FloatField("Vertical", myTarget.VerticalAxis);
        myTarget.A = EditorGUILayout.Toggle("A", myTarget.A);
        myTarget.B = EditorGUILayout.Toggle("B", myTarget.B);
        myTarget.X = EditorGUILayout.Toggle("X", myTarget.X);
        myTarget.Y = EditorGUILayout.Toggle("Y", myTarget.Y);
        myTarget.LB = EditorGUILayout.Toggle("LB", myTarget.LB);
        myTarget.RB = EditorGUILayout.Toggle("RB", myTarget.RB);
        
        myTarget.LT = EditorGUILayout.FloatField("LT", myTarget.LT);
        myTarget.RT = EditorGUILayout.FloatField("RT", myTarget.RT);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Keyboard Mappings", EditorStyles.boldLabel);

        myTarget.UpKey = (KeyCode)EditorGUILayout.EnumPopup("Up", myTarget.UpKey);
        myTarget.DownKey = (KeyCode)EditorGUILayout.EnumPopup("Down", myTarget.DownKey);
        myTarget.LeftKey = (KeyCode)EditorGUILayout.EnumPopup("Left", myTarget.LeftKey);
        myTarget.RightKey = (KeyCode)EditorGUILayout.EnumPopup("Right", myTarget.RightKey);
        myTarget.JumpKey = (KeyCode)EditorGUILayout.EnumPopup("Jump Key", myTarget.JumpKey);
        myTarget.Fire1Key = (KeyCode)EditorGUILayout.EnumPopup("Fire 1", myTarget.Fire1Key);
        myTarget.DashKey = (KeyCode)EditorGUILayout.EnumPopup("Dash", myTarget.DashKey);
        myTarget.OtherKey = (KeyCode)EditorGUILayout.EnumPopup("Other", myTarget.OtherKey);
        EditorGUILayout.Space();
        EditorUtility.SetDirty(target);
    }
    
}
