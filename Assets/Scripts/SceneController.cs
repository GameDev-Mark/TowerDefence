using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    #region Unity
    private void Start()
    {
        EventSystemManager.Instance.onTriggerSceneGameReset += ResetScene;    
    }
    private void OnDestroy()
    {
        EventSystemManager.Instance.onTriggerSceneGameReset -= ResetScene;
    }
    #endregion

    // called from InputKeyBindManager.cs 
    public void ResetScene()
    {
        ClearConsole();
        Debug.Log("@Reset scene...");
        SceneManager.LoadScene(0);
    }

    private void ClearConsole()
    {
        // Clear the console log in the Unity editor
        Assembly assembly = Assembly.GetAssembly(typeof(SceneView));
        Type logEntries = assembly.GetType("UnityEditor.LogEntries");
        MethodInfo clearMethod = logEntries.GetMethod("Clear");
        clearMethod.Invoke(new object(), null);
    }
}