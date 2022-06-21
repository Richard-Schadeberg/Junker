using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
public class NewTestScript {
    [OneTimeSetUp] public void OneTimeSetup() => EditorSceneManager.LoadSceneInPlayMode("Assets/Tests/TestScene1.unity", new LoadSceneParameters(LoadSceneMode.Single));

    [UnityTest]
    public IEnumerator Test1() {
        var a = GameObject.Find("Chris");
        Debug.Assert(a != null);
        yield return null;
    }
}
