using System.Linq;
using UnityEngine;

public static class Helper
{
    public static GameObject FindInChildren(this GameObject go, string name)
    {
        foreach (Transform child in go.transform)
        {
            if (child.name == name)
                return child.gameObject;
            var result = FindInChildren(child.gameObject,name);
            if (result != null)
                return result;
        }
        return null;
    }
}
