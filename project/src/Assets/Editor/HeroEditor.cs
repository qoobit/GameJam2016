using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Hero))]
public class HeroEditor : Editor
{
    protected static bool showAudioClips = false;
    protected static bool showStates = false;
    protected static bool showGameObjects = true;

    public override void OnInspectorGUI()
    {
        GUIStyle myFoldoutStyle = new GUIStyle(EditorStyles.foldout);
        myFoldoutStyle.fontStyle = FontStyle.Bold;
        Hero myTarget = (Hero)target;

        /*
        myTarget.experience = EditorGUILayout.IntField("Experience", myTarget.experience);
        EditorGUILayout.LabelField("Level", myTarget.Level.ToString());
        */
        EditorGUILayout.LabelField("Status", EditorStyles.boldLabel);
        myTarget.health.value = EditorGUILayout.FloatField("HP", myTarget.health.value);
        myTarget.facing = EditorGUILayout.Vector3Field("Facing Direction", myTarget.facing);
        myTarget.platform = (GameObject)EditorGUILayout.ObjectField("Platform", myTarget.platform, typeof(GameObject));
        myTarget.wall = (GameObject)EditorGUILayout.ObjectField("Wall", myTarget.wall, typeof(GameObject));
        myTarget.velocity = EditorGUILayout.Vector3Field("Velocity", myTarget.velocity);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Inventory", EditorStyles.boldLabel);
        myTarget.Weapon = (GameObject)EditorGUILayout.ObjectField("Weapon", myTarget.Weapon, typeof(GameObject));

        EditorGUILayout.Space();

        showGameObjects = EditorGUILayout.Foldout(showGameObjects, "Reference GameObjects", myFoldoutStyle);
        if (showGameObjects)
        {

            myTarget.Root = (GameObject)EditorGUILayout.ObjectField("Root", myTarget.Root, typeof(GameObject));
            myTarget.hmdRig = (GameObject)EditorGUILayout.ObjectField("HMD", myTarget.hmdRig, typeof(GameObject));
            myTarget.Focal = (GameObject)EditorGUILayout.ObjectField("Focal", myTarget.Focal, typeof(GameObject));
            myTarget.Crosshair = (GameObject)EditorGUILayout.ObjectField("Crosshair", myTarget.Crosshair, typeof(GameObject));
            myTarget.LiftingReference = (GameObject)EditorGUILayout.ObjectField("Lifting Reference", myTarget.LiftingReference, typeof(GameObject));
            myTarget.TouchingGameObject = (GameObject)EditorGUILayout.ObjectField("Touching", myTarget.TouchingGameObject, typeof(GameObject));
            myTarget.LiftingGameObject = (GameObject)EditorGUILayout.ObjectField("Lifting", myTarget.LiftingGameObject, typeof(GameObject));
            myTarget.GrabbingGameObject = (GameObject)EditorGUILayout.ObjectField("Grabbing", myTarget.GrabbingGameObject, typeof(GameObject));
        }

        


        

        showStates = EditorGUILayout.Foldout(showStates, "States", myFoldoutStyle);
        if (showStates) { 
            EditorGUILayout.LabelField("Jumping", myTarget.IsJumping().ToString());
            EditorGUILayout.LabelField("Dashing", myTarget.IsDashing().ToString());
            EditorGUILayout.LabelField("Charging", myTarget.IsCharging().ToString());
            EditorGUILayout.LabelField("Grabbing", myTarget.IsGrabbing().ToString());
            EditorGUILayout.LabelField("Lifting", myTarget.IsLifting().ToString());
            EditorGUILayout.LabelField("Shooting", myTarget.IsShooting().ToString());
            EditorGUILayout.LabelField("Standing", myTarget.IsStanding().ToString());
            EditorGUILayout.LabelField("Scaling", myTarget.IsScaling().ToString());

            EditorGUILayout.Space();
        }

        
        
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
        
    }

}