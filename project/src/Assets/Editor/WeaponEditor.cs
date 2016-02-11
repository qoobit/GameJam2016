using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Weapon))]
public class WeaponEditor : Editor
{
    public override void OnInspectorGUI()
    {
        
        Weapon weapon = (Weapon)target;
        weapon.damage = EditorGUILayout.Slider("Damage", weapon.damage, 0, 100f);
        //EditorGUILayout.LabelField("") 
    }
}