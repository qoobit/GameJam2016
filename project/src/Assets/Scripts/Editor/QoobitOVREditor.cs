using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(QoobitOVR))]
public class QoobitOVREditor : Editor
{
    QoobitOVR myTarget;
    
    protected static bool ShowReticles = true;
    protected static bool DetectAllLayers = true;
    protected static bool ShowGizmos = false;

    void OnEnable()
    {
        myTarget = (QoobitOVR)target;
    }

    public override void OnInspectorGUI()
    {
        //DrawDefaultInspector();

        GUIStyle myFoldoutStyle = new GUIStyle(EditorStyles.foldout);
        myFoldoutStyle.fontStyle = FontStyle.Bold;


        EditorGUILayout.Separator();
        EditorGUILayout.LabelField("HMD", EditorStyles.boldLabel);
        myTarget.HmdModel = (HMDModel)EditorGUILayout.EnumPopup("Model", myTarget.HmdModel);

        myTarget.FollowMode = (FollowMode)EditorGUILayout.EnumPopup("Follow Mode", myTarget.FollowMode);
        myTarget.RelativePosition = EditorGUILayout.Vector3Field("Relative Position", myTarget.RelativePosition);


        EditorGUILayout.Separator();
        EditorGUILayout.LabelField("Alignment", EditorStyles.boldLabel);
        myTarget.LookAt = EditorGUILayout.Vector3Field("Align To Position", myTarget.LookAt);
        myTarget.FollowObject = (GameObject)EditorGUILayout.ObjectField("Follow Object", myTarget.FollowObject, typeof(GameObject), true);
        if (GUILayout.Button("Realign HMD"))
        {
            myTarget.Realign(myTarget.FollowObject.transform.position,Vector3.zero);
        }
        EditorGUILayout.Separator();
        EditorGUILayout.LabelField("Object Detection", EditorStyles.boldLabel);
        myTarget.SearchForObject = EditorGUILayout.Toggle("Enabled", myTarget.SearchForObject);
        if (myTarget.SearchForObject)
        {
            myTarget.hoverDuration = EditorGUILayout.FloatField("Hover Duration", myTarget.hoverDuration);
            myTarget.HighlightColor = EditorGUILayout.ColorField("Highlight Color", myTarget.HighlightColor);
            myTarget.DetectionMode = (DetectionMode)EditorGUILayout.EnumPopup("Method", myTarget.DetectionMode);
            if (myTarget.DetectionMode == DetectionMode.SPHERECAST)
            {
                myTarget.SphereRadius = EditorGUILayout.FloatField("Sphere Radius", myTarget.SphereRadius);
            }

            myTarget.DetectionLayerMask = LayerMaskField("Layers", myTarget.DetectionLayerMask);

            myTarget.FocusObject = (GameObject)EditorGUILayout.ObjectField("Looking At Object", myTarget.FocusObject, typeof(GameObject), true);
            EditorGUILayout.BeginVertical("Box");
            EditorGUILayout.IntField("Size", myTarget.DetectionList.Count);
            for (int i = 0; i < myTarget.DetectionList.Count; i++)
            {
                EditorGUILayout.ObjectField("GameObject " + i, myTarget.DetectionList[i], typeof(GameObject), true);
            }
            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.Space();

        ShowReticles = EditorGUILayout.Foldout(ShowReticles, "Reticle", myFoldoutStyle);
        if (ShowReticles)
        {
            myTarget.ShowReticle = EditorGUILayout.Toggle("Show Reticle", myTarget.ShowReticle);
            if (myTarget.ShowReticle)
            {
                myTarget.Reticle = (GameObject)EditorGUILayout.ObjectField("Reticle 3D", myTarget.Reticle, typeof(GameObject), true);
            }
            myTarget.ShowReticle2D = EditorGUILayout.Toggle("Show Reticle 2D", myTarget.ShowReticle2D);
            if (myTarget.ShowReticle2D)
            {
                myTarget.Reticle2D = (GameObject)EditorGUILayout.ObjectField("Reticle 2D", myTarget.Reticle2D, typeof(GameObject), true);
            }
        }

        EditorGUILayout.Separator();
        ShowGizmos = EditorGUILayout.Foldout(ShowGizmos, "Gizmos", myFoldoutStyle);
        if (ShowGizmos)
        {
            myTarget.ShowHmdReferences = EditorGUILayout.Toggle("Show HMD References", myTarget.ShowHmdReferences);
            myTarget.GizmosForwardColor = EditorGUILayout.ColorField("Forward Vector Color", myTarget.GizmosForwardColor);
            myTarget.GizmosLookAtColor = EditorGUILayout.ColorField("Look At Vector Color", myTarget.GizmosLookAtColor);
        }

        EditorGUILayout.Space();

        EditorUtility.SetDirty(myTarget);
    }







    public static LayerMask LayerMaskField(string label, LayerMask selected)
    {
        return LayerMaskField(label, selected, true);
    }

    public static LayerMask LayerMaskField(string label, LayerMask selected, bool showSpecial)
    {

        List<string> layers = new List<string>();
        List<int> layerNumbers = new List<int>();

        string selectedLayers = "";

        for (int i = 0; i < 32; i++)
        {

            string layerName = LayerMask.LayerToName(i);

            if (layerName != "")
            {
                if (selected == (selected | (1 << i)))
                {

                    if (selectedLayers == "")
                    {
                        selectedLayers = layerName;
                    }
                    else {
                        selectedLayers = "Mixed";
                    }
                }
            }
        }

        EventType lastEvent = Event.current.type;

        if (Event.current.type != EventType.MouseDown && Event.current.type != EventType.ExecuteCommand)
        {
            if (selected.value == 0)
            {
                layers.Add("Nothing");
            }
            else if (selected.value == -1)
            {
                layers.Add("Everything");
            }
            else {
                layers.Add(selectedLayers);
            }
            layerNumbers.Add(-1);
        }

        if (showSpecial)
        {
            layers.Add((selected.value == 0 ? "[X] " : "     ") + "Nothing");
            layerNumbers.Add(-2);

            layers.Add((selected.value == -1 ? "[X] " : "     ") + "Everything");
            layerNumbers.Add(-3);
        }

        for (int i = 0; i < 32; i++)
        {

            string layerName = LayerMask.LayerToName(i);

            if (layerName != "")
            {
                if (selected == (selected | (1 << i)))
                {
                    layers.Add("[X] " + layerName);
                }
                else {
                    layers.Add("     " + layerName);
                }
                layerNumbers.Add(i);
            }
        }

        bool preChange = GUI.changed;

        GUI.changed = false;

        int newSelected = 0;

        if (Event.current.type == EventType.MouseDown)
        {
            newSelected = -1;
        }

        newSelected = EditorGUILayout.Popup(label, newSelected, layers.ToArray(), EditorStyles.layerMaskField);

        if (GUI.changed && newSelected >= 0)
        {
            //newSelected -= 1;

            Debug.Log(lastEvent + " " + newSelected + " " + layerNumbers[newSelected]);

            if (showSpecial && newSelected == 0)
            {
                selected = 0;
            }
            else if (showSpecial && newSelected == 1)
            {
                selected = -1;
            }
            else {

                if (selected == (selected | (1 << layerNumbers[newSelected])))
                {
                    selected &= ~(1 << layerNumbers[newSelected]);
                    //Debug.Log ("Set Layer "+LayerMask.LayerToName (LayerNumbers[newSelected]) + " To False "+selected.value);
                }
                else {
                    //Debug.Log ("Set Layer "+LayerMask.LayerToName (LayerNumbers[newSelected]) + " To True "+selected.value);
                    selected = selected | (1 << layerNumbers[newSelected]);
                }
            }
        }
        else {
            GUI.changed = preChange;
        }

        return selected;
    }

    void OnSceneGUI()
    {




        if (!myTarget.ShowHmdReferences) return;
        Vector3 flatPoint = Vector3.forward * myTarget.some3d;
        Vector3 leftLimit;
        Vector3 rightLimit;
        Vector3 topLimit;
        Vector3 bottomLimit;


        Handles.color = new Color(0f, 1f, 1f, 0.1f);
        leftLimit = Quaternion.Euler(0, -myTarget.maxHeadTurn / 2f, 0) * flatPoint;
        Handles.DrawSolidArc(myTarget.transform.position,
            myTarget.transform.up,
            leftLimit,
            myTarget.maxHeadTurn,
            myTarget.some3d
            );

        Handles.color = new Color(1f, 1f, 0f, 0.1f);
        leftLimit = Quaternion.Euler(0, -myTarget.comfort / 2f, 0) * flatPoint;
        Handles.DrawSolidArc(myTarget.transform.position,
            myTarget.transform.up,
            leftLimit,
            myTarget.comfort,
            myTarget.some3d
            );

        Handles.color = new Color(1f, 0f, 0f, 0.1f);
        leftLimit = Quaternion.Euler(0, -myTarget.fovH / 2f, 0) * flatPoint;
        Handles.DrawSolidArc(myTarget.transform.position,
            myTarget.transform.up,
            leftLimit,
            myTarget.fovH,
            myTarget.some3d
            );

        Handles.color = new Color(1f, 0f, 1f, 0.1f);
        float contentAngle = myTarget.fovH + myTarget.comfort;
        float peripheralAngle = myTarget.fovH + myTarget.maxHeadTurn;
        leftLimit = Quaternion.Euler(0, -contentAngle / 2f, 0) * flatPoint;
        rightLimit = Quaternion.Euler(0, contentAngle / 2f, 0) * flatPoint;
        Handles.DrawSolidArc(myTarget.transform.position,
           myTarget.transform.up,
           leftLimit,
           (contentAngle-peripheralAngle)/2f,
           myTarget.some3d
           );

        Handles.DrawSolidArc(myTarget.transform.position,
           myTarget.transform.up,
           rightLimit,
           -(contentAngle - peripheralAngle) / 2f,
           myTarget.some3d
           );

        

        //Vertical Zones
        Handles.color = new Color(0f, 1f, 1f, 0.1f);
        topLimit = Quaternion.Euler(-myTarget.upHeadMax, 0f, 0) * flatPoint;
        Handles.DrawSolidArc(myTarget.transform.position,
            myTarget.transform.right,
            topLimit,
            myTarget.upHeadMax + myTarget.downHeadMax,
            myTarget.some3d
            );

        Handles.color = new Color(1f, 0f, 0f, 0.1f);
        topLimit = Quaternion.Euler(-myTarget.fovV / 2f, 0f, 0) * flatPoint;
        Handles.DrawSolidArc(myTarget.transform.position,
            myTarget.transform.right,
            topLimit,
            myTarget.fovV,
            myTarget.some3d
            );

        Handles.color = new Color(1f, 1f, 0f, 0.1f);
        topLimit = Quaternion.Euler(-myTarget.upComfort, 0f, 0) * flatPoint;
        Handles.DrawSolidArc(myTarget.transform.position,
            myTarget.transform.right,
            topLimit,
            myTarget.upComfort + myTarget.downComfort,
            myTarget.some3d
            );

    }
}
