using UnityEngine;

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

// Based off of the algorithm presented at:
// http://www.gamasutra.com/view/news/237528/Water_interaction_model_for_boats_in_video_games.php


public class SurfaceBuoyancy : MonoBehaviour {

    public bool sideForces = true;
    public float centGravX = 0.0f;
    public float centGravY = 0.0f;
    public float centGravZ = 0.0f;
    public float rhoWater = 1000f;
    public Rigidbody rb = null;

    MeshFilter meshFilter = null;

    Vector3[] meshVerts = null;
    int[] meshTris = null;

    GameObject waterObj;
    MeshCollider water;

    void Start() {

        if (rb == null)
            rb = GetComponent<Rigidbody>();

        rb.maxAngularVelocity = 0.5f;
        rb.centerOfMass = new Vector3(centGravX, centGravY, centGravZ);

        meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null)
            Debug.Log("[SurfaceBuoyancy] " + gameObject.name + " does not have a MeshFilter / Mesh!");
        else {
            meshTris = meshFilter.mesh.triangles;
            meshVerts = meshFilter.mesh.vertices;
        }

        waterObj = GameObject.Find("Water");
        water = waterObj.GetComponent<MeshCollider>();

        // CalcUnderwaterTriangles();
    }

    void FixedUpdate() {
        CalcUnderwaterTriangles();
    }

    void CalcUnderwaterTriangles () {

        List<Vector3> trisUnderwater = new List<Vector3>();

        for ( int t = 0; t < meshTris.Length; t += 3 ) {

            // ------------------------
            // 1) Pull data, count and sort by distance from water
            int totAboveWater = 0;
            int[] submitPoints = new int[] { 0, 1, 2 };
            Vector3[] triPoints = new Vector3[3] {
                meshFilter.transform.TransformPoint( meshVerts[meshTris[t]] ),
                meshFilter.transform.TransformPoint( meshVerts[meshTris[t+1]] ),
                meshFilter.transform.TransformPoint( meshVerts[meshTris[t+2]] ),
            };

            // Easier on the eyes. May go back for performance since alternative is messy with only .NET 2.0
            submitPoints = submitPoints.OrderByDescending( x => distToWater(triPoints[x]) ).ToArray();
            totAboveWater = countPointsAboveWater(triPoints);

            // How does this not work?
            // totAboveWater = submitPoints.Sum( x => ( ( distToWater(triPoints[x]) < 0) ? totAboveWater += 0 : totAboveWater += 1 ) );

            // Debug.Log("Case : " +totAboveWater);

            switch ( totAboveWater ) {
                case 0: { // All points underwater - just add it to the list.
                        trisUnderwater.AddRange(triPoints);
                        break;
                    }
                case 1: { // One point above water. Split into two triangles.
                        RaycastHit hit;
                        Physics.Linecast(triPoints[submitPoints[0]], triPoints[submitPoints[1]], out hit);
                        Vector3 newPointM = hit.point;
                        Physics.Linecast(triPoints[submitPoints[0]], triPoints[submitPoints[2]], out hit);
                        Vector3 newPointL = hit.point;
                        if (submitPoints[1] == 1) { // Normals facing the way they came in.
                            trisUnderwater.AddRange(new Vector3[] { newPointM, triPoints[submitPoints[2]], triPoints[submitPoints[1]] });
                            trisUnderwater.AddRange(new Vector3[] { newPointM, newPointL, triPoints[submitPoints[2]] });
                        }
                        else {
                            trisUnderwater.AddRange(new Vector3[] { newPointM, triPoints[submitPoints[2]], triPoints[submitPoints[1]] }.Reverse());
                            trisUnderwater.AddRange(new Vector3[] { newPointM, newPointL, triPoints[submitPoints[2]] }.Reverse());
                        }

                        break;
                    }
                case 2: { // Two points above water. Cut the triangle into a single smaller one.
                        RaycastHit hit;
                        Physics.Linecast(triPoints[submitPoints[0]], triPoints[submitPoints[2]], out hit);
                        Vector3 newPointH = hit.point;
                        Physics.Linecast(triPoints[submitPoints[1]], triPoints[submitPoints[2]], out hit);
                        Vector3 newPointM = hit.point;
                        if ( submitPoints[1] == 1) // Normals facing the way they came in.
                            trisUnderwater.AddRange(new Vector3[] { newPointH, newPointM, triPoints[submitPoints[2]] });
                        else
                            trisUnderwater.AddRange(new Vector3[] { newPointH, newPointM, triPoints[submitPoints[2]] }.Reverse());
                        break;
                    }
                default:
                    break; // 3+ Points above water. We are having a suntan!
            }

        }

        // Debug.Log(trisUnderwater.Count / 3);

        for ( int t = 0; t < trisUnderwater.Count; t += 3 ) {

            Vector3 ptA = trisUnderwater[t];
            Vector3 ptB = trisUnderwater[t + 1];
            Vector3 ptC = trisUnderwater[t + 2];
            
            Vector3 triCenter = (ptA + ptB + ptC) / 3f;
            Vector3 triNormal = Vector3.Cross(ptB - ptA, ptC - ptA).normalized; 
            float wtrHeight = distToWater (triCenter);

            // Debug.DrawRay(triCenter, triNormal * 10f);

            float a = Vector3.Distance(ptA, ptB);
            float c = Vector3.Distance(ptC, ptA);
            float area = (a * c * Mathf.Sin(Vector3.Angle(ptB - ptA, ptC - ptA) * Mathf.Deg2Rad)) / 2f;

            Vector3 F = -rhoWater * Physics.gravity.y * wtrHeight * area * triNormal;
            F = new Vector3(0f, F.y, 0f);
            rb.AddForceAtPosition(F, triCenter);

            if (!sideForces) continue;

            F = .5f * rhoWater * Physics.gravity.y * area * area * triNormal * wtrHeight;
            F = new Vector3(F.x, 0f, F.z);
            // F = new Vector3(50f, 0f, 0f);
            rb.AddForceAtPosition(F, triCenter);

        }

    }

    int countPointsAboveWater ( Vector3[] points ) {
        int ret = 0;
        foreach ( Vector3 v in points ) {
            if (distToWater(v) > 0)
                ++ret;
        }
        return ret;
    }

    float distToWater ( Vector3 pt ) {

        RaycastHit hit;
        if (water.Raycast(new Ray(pt, Vector3.down), out hit, 10000f))
           return pt.y - hit.point.y;
        water.Raycast(new Ray( new Vector3 (pt.x, pt.y+10000f, pt.z), Vector3.down), out hit, 20000f );
            return pt.y - hit.point.y;
        
        // Estimate the height of water to be 0 for now.
        // Debug.Log("ERROR");
        // return pt.y;

    }

}
