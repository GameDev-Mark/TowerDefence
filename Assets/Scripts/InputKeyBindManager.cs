using UnityEngine;

public class InputKeyBindManager : MonoBehaviour
{
    SceneController sceneController;

    private void Start()
    {
        sceneController = FindObjectOfType<SceneController>();
    }

    void Update()
    {
        SpaceBar();
    }

    private void SpaceBar()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            sceneController.ResetScene(); // reset scene
        }
    }
}