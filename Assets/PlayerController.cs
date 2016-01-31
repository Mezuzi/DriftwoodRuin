using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    Camera cam;
    UnityStandardAssets.Characters.FirstPerson.MouseLook mouseLook;
    Rigidbody rb;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
        cam = GetComponentInChildren<Camera>();
	    mouseLook = new UnityStandardAssets.Characters.FirstPerson.MouseLook();
        mouseLook.Init(transform, cam.transform);
    }

    // Update is called once per frame
    void Update () {

        rb.AddForce( new Vector3(0f, 0f, Input.GetAxis("Vertical") * 10f ));
        rb.AddForce(new Vector3(0, Input.GetAxis("Horizontal") * 10f));

        RotateView();
    }

    private void RotateView() {
        //avoids the mouse looking if the game is effectively paused
        if (Mathf.Abs(Time.timeScale) < float.Epsilon) return;

        // get the rotation before it's changed
        float oldYRotation = transform.eulerAngles.y;

        mouseLook.LookRotation(transform, cam.transform);

        /*
        if (m_IsGrounded || advancedSettings.airControl) {
            // Rotate the rigidbody velocity to match the new direction that the character is looking
            Quaternion velRotation = Quaternion.AngleAxis(transform.eulerAngles.y - oldYRotation, Vector3.up);
            m_RigidBody.velocity = velRotation * m_RigidBody.velocity;
        }
        */

    }

}
