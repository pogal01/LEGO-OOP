using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MichaelScript : MonoBehaviour
{
    private bool CursorToggle;
    // Start is called before the first frame update
    void Start()
    {
        ToggleCursor();
    }

    bool ToggleCursor()
    {
        CursorToggle = !CursorToggle;

        return CursorToggle;
    }

    void IdentiyCrisis()
    {
        gameObject.GetComponent<Animator>().SetTrigger("IdentityCrisis");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            IdentiyCrisis();
        }
        if (Input.GetKeyUp(KeyCode.Tab))
        {
           Debug.Log(ToggleCursor());
        }
        if (CursorToggle == true)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

    }
}
