using UnityEngine;

public class InputKeyBindManager : MonoBehaviour
{
    public void Update() 
    {
        CustomInputKey();
    }

    private void CustomInputKey()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            EventSystemManager.Instance.TriggerSceneGameReset();
        }
    }
}