using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    void Update()
    {
        if(Input.GetButtonDown("Jump"))
        {
            ClearConsole();
            Debug.Log("@Reset scene...");
            SceneManager.LoadScene(0);
        }
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