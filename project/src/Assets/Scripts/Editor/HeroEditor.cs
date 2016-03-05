using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Hero))]
public class HeroEditor : Editor
{
    public static bool ShowStates = false;
    public static bool ShowGameObjects = true;
    public static bool ShowGizmos = true;

    public override void OnInspectorGUI()
    {
        GUIStyle myFoldoutStyle = new GUIStyle(EditorStyles.foldout);
        myFoldoutStyle.fontStyle = FontStyle.Bold;
        Hero myTarget = (Hero)target;

        EditorGUILayout.LabelField("Status", EditorStyles.boldLabel);
        myTarget.Health = EditorGUILayout.FloatField("HP", myTarget.Health);
        myTarget.facing = EditorGUILayout.Vector3Field("Facing Direction", myTarget.facing);
        myTarget.platform = (GameObject)EditorGUILayout.ObjectField("Platform", myTarget.platform, typeof(GameObject), true);
        myTarget.wall = (GameObject)EditorGUILayout.ObjectField("Wall", myTarget.wall, typeof(GameObject), true);
        myTarget.lockedObject = (GameObject)EditorGUILayout.ObjectField("Locked Object", myTarget.lockedObject, typeof(GameObject), true);
        
        myTarget.velocity = EditorGUILayout.Vector3Field("Velocity", myTarget.velocity);
        myTarget.terminalVelocity = EditorGUILayout.FloatField("Terminal Velocity", myTarget.terminalVelocity);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Inventory", EditorStyles.boldLabel);
        myTarget.weapon = (GameObject)EditorGUILayout.ObjectField("Weapon", myTarget.weapon, typeof(GameObject), true);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Forces", EditorStyles.boldLabel);
        myTarget.gravity = EditorGUILayout.FloatField("Gravity", myTarget.gravity);
        myTarget.jumpForce = EditorGUILayout.FloatField("Jump", myTarget.jumpForce);
        myTarget.dashForce = EditorGUILayout.FloatField("Dash", myTarget.dashForce);

        EditorGUILayout.Space();

        ShowGameObjects = EditorGUILayout.Foldout(ShowGameObjects, "Reference GameObjects", myFoldoutStyle);
        if (ShowGameObjects)
        {

            myTarget.Root = (GameObject)EditorGUILayout.ObjectField("Root", myTarget.Root, typeof(GameObject), true);
            myTarget.hmdRig = (GameObject)EditorGUILayout.ObjectField("HMD", myTarget.hmdRig, typeof(GameObject), true);
            myTarget.Focal = (GameObject)EditorGUILayout.ObjectField("Focal", myTarget.Focal, typeof(GameObject), true);
            myTarget.Crosshair = (GameObject)EditorGUILayout.ObjectField("Crosshair", myTarget.Crosshair, typeof(GameObject), true);
            myTarget.LiftingReference = (GameObject)EditorGUILayout.ObjectField("Lifting Reference", myTarget.LiftingReference, typeof(GameObject), true);
            myTarget.TouchingGameObject = (GameObject)EditorGUILayout.ObjectField("Touching", myTarget.TouchingGameObject, typeof(GameObject), true);
            myTarget.LiftingGameObject = (GameObject)EditorGUILayout.ObjectField("Lifting", myTarget.LiftingGameObject, typeof(GameObject), true);
            myTarget.GrabbingGameObject = (GameObject)EditorGUILayout.ObjectField("Grabbing", myTarget.GrabbingGameObject, typeof(GameObject), true);
        }


        EditorGUILayout.Space();
       

        ShowStates = EditorGUILayout.Foldout(ShowStates, "States", myFoldoutStyle);
        if (ShowStates)
        {
            EditorGUILayout.LabelField("Jumping", myTarget.IsJumping.ToString());
            EditorGUILayout.LabelField("Dashing", myTarget.IsDashing.ToString());
            EditorGUILayout.LabelField("Charging", myTarget.IsCharging.ToString());
            EditorGUILayout.LabelField("Grabbing", myTarget.IsGrabbing.ToString());
            EditorGUILayout.LabelField("Lifting", myTarget.IsLifting.ToString());
            EditorGUILayout.LabelField("Shooting", myTarget.IsShooting.ToString());
            EditorGUILayout.LabelField("Standing", myTarget.IsStanding.ToString());
            EditorGUILayout.LabelField("Scaling", myTarget.IsScaling.ToString());

            EditorGUILayout.Space();
        }

        ShowGizmos = EditorGUILayout.Foldout(ShowGizmos, "Gizmos", myFoldoutStyle);
        if (ShowGizmos)
        {
            myTarget.GizmosFacingColor = EditorGUILayout.ColorField("Facing Color", myTarget.GizmosFacingColor);
            myTarget.GizmosFacingLength = EditorGUILayout.FloatField("Facing Length", myTarget.GizmosFacingLength);
        }
        /* 
        //Moved audio stuff to HeroAudio
        showAudioClips = EditorGUILayout.Foldout(showAudioClips, "Audio Clips", myFoldoutStyle);
        if (showAudioClips)
        {
            myTarget.JumpAudioClip = (AudioClip)EditorGUILayout.ObjectField("Jump Audio", myTarget.JumpAudioClip, typeof(AudioClip));
            myTarget.ShotAudioClip = (AudioClip)EditorGUILayout.ObjectField("Shot Audio", myTarget.ShotAudioClip, typeof(AudioClip));
            myTarget.WalkAudioClip = (AudioClip)EditorGUILayout.ObjectField("Walk Audio", myTarget.WalkAudioClip, typeof(AudioClip));
            myTarget.DashAudioClip = (AudioClip)EditorGUILayout.ObjectField("Dash Audio", myTarget.DashAudioClip, typeof(AudioClip));
            myTarget.DeathAudioClip = (AudioClip)EditorGUILayout.ObjectField("Death Audio", myTarget.DeathAudioClip, typeof(AudioClip));
            myTarget.LockAudioClip = (AudioClip)EditorGUILayout.ObjectField("Lock Audio", myTarget.LockAudioClip, typeof(AudioClip));
            EditorGUILayout.Space();
        }
        */
    }

    [DrawGizmo(GizmoType.Selected | GizmoType.NonSelected)]
    static void DrawFacingDirection(Hero h, GizmoType gizmoType)
    {
        Debug.DrawLine(h.gameObject.transform.position, h.gameObject.transform.position + h.facing*h.GizmosFacingLength, h.GizmosFacingColor);
    }
}