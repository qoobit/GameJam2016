using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Portal))]
[CanEditMultipleObjects]
public class PortalEditor : Editor
{
    Portal myTarget;
    public static Color DirectionColor;
    public static bool ShowGizmos;

    //Tool LastTool = Tool.None;

    void OnEnable()
    {
        myTarget = (Portal)target;

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
        

        EditorGUILayout.LabelField("Spawn", EditorStyles.boldLabel);
        myTarget.Direction = EditorGUILayout.Vector3Field("Direction", myTarget.Direction);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Teleport Targets", EditorStyles.boldLabel);
        myTarget.NextSceneName = EditorGUILayout.TextField("Scene Name", myTarget.NextSceneName);
        myTarget.NextPortalName = EditorGUILayout.TextField("Portal Name", myTarget.NextPortalName);


        EditorGUILayout.Separator();
        ShowGizmos = EditorGUILayout.Foldout(ShowGizmos, "Gizmos", myFoldoutStyle);
        if (ShowGizmos)
        {
            myTarget.GizmosDirectionColor = EditorGUILayout.ColorField("Spawn Direction Color", myTarget.GizmosDirectionColor);
            myTarget.GizmosDirectionLength = EditorGUILayout.FloatField("Spawn Direction Length", myTarget.GizmosDirectionLength);
        }

        EditorUtility.SetDirty(target);
    }


    [DrawGizmo(GizmoType.Selected | GizmoType.NonSelected)]
    static void DrawSpawnDirection(Portal p, GizmoType gizmoType)
    {
        Gizmos.color = p.GizmosDirectionColor;
        Gizmos.DrawLine(p.gameObject.transform.position, p.gameObject.transform.position + p.Direction * p.GizmosDirectionLength);
    }
}
