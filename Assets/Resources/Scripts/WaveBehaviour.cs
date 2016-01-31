using UnityEngine;
using System.Collections;

public class WaveBehaviour : MonoBehaviour {

    // For visual effect only.

    public float yMinRg = -.1f;
    public float yMaxRg = .1f;
    public float yVarient = .005f;
    Vector3[] startPoints;

    Mesh mesh = null;

	void Start () {
        mesh = GetComponent<MeshFilter>().mesh;
        startPoints = mesh.vertices;
	}
	
	void FixedUpdate () {
        oscilPoints();
	}

    void oscilPoints () {
        Vector3[] curPoints = mesh.vertices;
        for ( int v = 0; v < startPoints.Length; ++v ) {
            if (Random.Range(0, 2) == 0) {
                float newY = curPoints[v].y + Random.Range(-yVarient, yVarient);
                if (newY > yMaxRg + startPoints[v].y)
                    newY = yMaxRg + startPoints[v].y;
                else if (newY < yMinRg + startPoints[v].y)
                    newY = yMinRg + startPoints[v].y;
                curPoints[v].y = newY;
            }
        }
        mesh.vertices = curPoints;
    }

}
