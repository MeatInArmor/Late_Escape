using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockCursor : MonoBehaviour
{

	[Header("Mouse Cursor Settings")]
	//public bool cursorLocked = true;
	public bool cursorInputForLook = true;

	// Start is called before the first frame update
	void Start()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    void Update()
    {
		if (Time.timeScale == 1f)
		{
			cursorInputForLook = true;
			if (Input.GetKey(KeyCode.Space))
			{
				Cursor.lockState = CursorLockMode.None;

			}
			else
			{
				Cursor.lockState = CursorLockMode.Locked;
			}
		}
		else
		{
			Cursor.lockState = CursorLockMode.None;
			cursorInputForLook = false;
		}
	}
}