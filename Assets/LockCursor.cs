using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockCursor : MonoBehaviour
{

	[Header("Mouse Cursor Settings")]
	public bool cursorLocked = true;
	public bool cursorInputForLook = true;

	// Start is called before the first frame update
	void Start()
    {
        Screen.lockCursor = false;
    }

    void Update()
    {
		if (Time.timeScale == 1f)
		{
			cursorInputForLook = true;
			if (Input.GetKey(KeyCode.Space))
			{
				Screen.lockCursor = false;

			}
			else
			{
				Screen.lockCursor = true;
			}
		}
		else
		{
			Screen.lockCursor = false;
			cursorInputForLook = false;
		}
	}

}