using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using NUnit.Framework.Internal;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class SceneviewEditor : Editor
{
    [MenuItem("Test/test")]
    private static void Test()
    {
        var assembly = Assembly.Load("UnityEditor");
        var types = assembly.GetTypes();
        foreach (var item in types)
        {
            Debug.Log(item);
        }
    }
}