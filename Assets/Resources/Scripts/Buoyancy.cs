using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Buoyancy : MonoBehaviour {

    MeshFilter meshFilter;
    Rigidbody rb;

    // Density of water
    float rhoWater = 1000f;

	void Start () {
        
        rb = GetComponent<Rigidbody>();
        meshFilter = GetComponent<MeshFilter>();

        rb.maxAngularVelocity = 0.5f;

        Debug.Log(meshFilter.mesh.triangles.Length);
        Debug.Log(meshFilter.mesh.vertices.Length);

	}
	
	void FixedUpdate () {
        CalculateForces();
	}

    void CalculateForces () {

        MeshCollider water = GameObject.Find("Water").GetComponent<MeshCollider>();

        int[] triangles = meshFilter.mesh.triangles;
        Vector3[] verticies = meshFilter.mesh.vertices;

        List<Vector3[]> retUnderwaterTrianglePoints = new List<Vector3[]>();
        List<Vector3> retUnderwaterTriangleCenters = new List<Vector3>();
        List<float> retUnderwaterNumVert = new List<float>();
        List<float> retUnderwaterHeightAt = new List<float>();

        for ( int t = 0; t < triangles.Length; t +=3 ) {

            // Get verticies @ worldSpace to correctly calculate water height.
            Vector3 ptA = meshFilter.transform.TransformPoint(verticies[triangles[t]]);
            Vector3 ptB = meshFilter.transform.TransformPoint(verticies[triangles[t+1]]);
            Vector3 ptC = meshFilter.transform.TransformPoint(verticies[triangles[t+2]]);

            int numUnder = 0;
            if (DistanceToWater(ptA) < 0) ++numUnder;
            if (DistanceToWater(ptB) < 0) ++numUnder;
            if (DistanceToWater(ptC) < 0) ++numUnder;

            Vector3 triangleCenter = (ptA + ptB + ptC) / 3;
            Ray ray = new Ray( triangleCenter, Vector3.up); RaycastHit hit;

            // Is the triangle underwater?
            // if ( triangleCenter.y - water.transform.position.y < 0 ) {

            if ( water.Raycast(ray, out hit, 100f) ) { 
                Vector3 hitAt = hit.point;
                retUnderwaterTrianglePoints.Add(new Vector3[] { ptA, ptB, ptC });
                retUnderwaterTriangleCenters.Add(triangleCenter);
                retUnderwaterHeightAt.Add(triangleCenter.y - hitAt.y); // TODO - update with water height not at 0;
                retUnderwaterNumVert.Add(numUnder);
            }

        }

        // Add forces to triangles marked as underwater.
        for ( int t = 0; t < retUnderwaterTriangleCenters.Count; ++t ) {

            // Debug.Log("Adding force!");

            Vector3 triCent = retUnderwaterTriangleCenters[t];
            Vector3[] triPoints = retUnderwaterTrianglePoints[t];
            float waterHgt = retUnderwaterHeightAt[t];

            Vector3 normalToTri = Vector3.Cross(triPoints[1] - triPoints[0], triPoints[2] - triPoints[0]).normalized;

            Debug.DrawRay(triCent, normalToTri * 3f);

            float a = Vector3.Distance(triPoints[0], triPoints[1]);
            float c = Vector3.Distance(triPoints[2], triPoints[0]);
            float area = (a * c * Mathf.Sin(Vector3.Angle(triPoints[1] - triPoints[0], triPoints[2] - triPoints[0]) * Mathf.Deg2Rad)) / 2f;

            // Add forces!

            // Up n' Down
            Vector3 F = rhoWater * Physics.gravity.y * waterHgt * area * normalToTri;
            F = new Vector3(0f, -F.y * retUnderwaterNumVert[t] / 3f, 0f);
            rb.AddForceAtPosition(F, triCent);

            // Drifting 
            
            //F = 0.5f * rhoWater * Physics.gravity.y * area * area * normalToTri;
            //F = new Vector3(F.x, 0f, F.z);
            //rb.AddForceAtPosition(F, triCent); 
            
        }

    }

    float DistanceToWater ( Vector3 pt ) {
        return pt.y;
    }

}
