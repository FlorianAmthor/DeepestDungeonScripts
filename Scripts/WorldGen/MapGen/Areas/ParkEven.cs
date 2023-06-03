﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
using WatStudios.DeepestDungeon.WorldGen.DCEL;

namespace WatStudios.DeepestDungeon.WorldGen
{
    /// <summary>
    /// Class for Generating a Park at the Level of one of its Edges
    /// </summary>
    public class ParkEven : Area
    {
        protected override string SetAreaName()
        {
            return "ParkEven";
        }
        protected override int SetAreaLayer()
        {
            return LayerMask.NameToLayer("Floor");
        }

        public override GameObject BuildAreaObject()
        {
            HalfEdge currentEdge;

            List<Vector3> corners = new List<Vector3>();
            currentEdge = facet.IncidentHalfEdge;

            do
            {


                if (currentEdge.Predecessor == currentEdge.TwinHalfEdge)
                {
                    float vectorLength = _mGDS.BridgeSpacing;

                    for (int i = 0; i < 7; i++)
                    {
                        Vector2 vec1 = (currentEdge.Predecessor.StartCorner.position - currentEdge.StartCorner.position).normalized;
                        Vector2 vec2 = Quaternion.AngleAxis(-(90 + 30 * i), Vector3.forward) * vec1;

                        Vector2 bisectingOffset = vec2 * vectorLength;
                        Vector2 corner = currentEdge.StartCorner.position + bisectingOffset;
                        corners.Add(new Vector3(corner.x, 0, corner.y));
                    }
                }
                else
                {

                    Vector2 vec1 = (currentEdge.Successor.StartCorner.position - currentEdge.StartCorner.position).normalized;
                    Vector2 vec2 = (currentEdge.Predecessor.StartCorner.position - currentEdge.StartCorner.position).normalized;


                    //Get length of Bisection
                    float a = _mGDS.BridgeWidth / 2f;
                    float alphaDeg = Vector2.Angle(vec1, vec2) / 2f;
                    float alphaRad = alphaDeg * Mathf.PI / 180;
                    float b = a / Mathf.Tan(alphaRad);
                    float c = Mathf.Sqrt(Mathf.Pow(a, 2) + Mathf.Pow(b, 2));
                    float vectorLength;


                    if (Vector2.SignedAngle(vec1, vec2) < 0)
                    {
                        vectorLength = -c;
                    }
                    else
                    {
                        vectorLength = c;
                    }

                    Vector2 bisectingOffset = (vec1 + vec2).normalized * vectorLength;

                    Vector2 corner = currentEdge.StartCorner.position + bisectingOffset;

                    corners.Add(new Vector3(corner.x, 0, corner.y));
                }

                currentEdge = currentEdge.Successor;

            } while (currentEdge != facet.IncidentHalfEdge);


            GameObject building = new GameObject();
            building.AddComponent<MeshFilter>();
            ProBuilderMesh mesh = building.AddComponent<ProBuilderMesh>();

            List<Corner> incidentCorners = facet.GetAllIncidentCorners();
            Corner randomCorner = incidentCorners[Random.Range(0, incidentCorners.Count - 1)];

            float houseHeight = HeightManipulator.TransformToVec3Noise(randomCorner.position).y;

            try
            {
                mesh.CreateShapeFromPolygon(corners, -houseHeight, false);
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("DD_Team: ProBuilder doesn't load the assets correctly: " + e);
            }

            building.GetComponent<MeshRenderer>().materials = new Material[] { Resources.Load<Material>("WorldGen/Materials/Walls") };

            building.AddComponent<MeshCollider>(); //Add Collider at last so it is automatically modified

            return building;

        }
    }
}
