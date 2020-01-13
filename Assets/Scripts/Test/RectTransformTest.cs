using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Sprites;
using UnityEngine.UI;

[ExecuteInEditMode]
public class RectTransformTest : MonoBehaviour
{
    [MenuItem("Test/test")]
    private static void Test()
    {
        var go = Selection.activeGameObject;
        if (go != null)
        {
            var img = go.GetComponent<Image>();
            Debug.Log(DataUtility.GetInnerUV(img.sprite));
            Debug.Log(DataUtility.GetOuterUV(img.sprite));
        }
    }


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }
}