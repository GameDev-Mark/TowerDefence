using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    void Update()
    {
        if(Input.GetButtonDown("Jump"))
        {
            Debug.Log("@Reset scene...");
            SceneManager.LoadScene(0);
        }
    }
}