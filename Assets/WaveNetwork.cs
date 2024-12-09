using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;

public class WaveNetwork : NetworkBehaviour
{
    public int Dimensions ;
    public Octave[] Octaves;
    public float UVScale;

    protected MeshFilter MeshFilter;
    protected Mesh Mesh;

    // Start is called before the first frame update
    void Start()
    {
        Mesh = new Mesh();
        Mesh.name = gameObject.name;

        Mesh.vertices = GenerateVerts();
        Mesh.triangles = GenerateTries();
        Mesh.uv = GenerateUVs();
        Mesh.RecalculateBounds();
        Mesh.RecalculateNormals();

        MeshFilter = gameObject.AddComponent<MeshFilter>();
        MeshFilter.mesh = Mesh;

        //NetworkObject.Spawn(gameObject);
    }

    private Vector2[] GenerateUVs()
    {
        var uvs = new Vector2[Mesh.vertices.Length];

        for (int x = 0; x <= Dimensions; x++)
        {
            for (int z = 0; z <= Dimensions; z++)
            {
                var vec = new Vector2((x / UVScale) % 2, (z / UVScale) % 2);
                uvs[index(x, z)] = new Vector2(vec.x <= 1 ? vec.x : 2 - vec.x, vec.y <= 1 ? vec.y : 2 - vec.y);
            }
        }

        return uvs;
    }

    private int[] GenerateTries()
    {
        var tries = new int[Mesh.vertices.Length * 6];

        for (int x = 0; x < Dimensions; x++)
        {
            for (int z = 0; z < Dimensions; z++)
            {
                tries[index(x, z) * 6 + 0] = index(x, z);
                tries[index(x, z) * 6 + 1] = index(x + 1, z + 1);
                tries[index(x, z) * 6 + 2] = index(x + 1, z);
                tries[index(x, z) * 6 + 3] = index(x, z);
                tries[index(x, z) * 6 + 4] = index(x, z + 1);
                tries[index(x, z) * 6 + 5] = index(x + 1, z + 1);
            }
        }

        return tries;
    }


    public float GetHeight(Vector3 position)
    {

        var scale = new Vector3(1 / transform.lossyScale.x, 0, 1 / transform.lossyScale.z);
        var localPos = Vector3.Scale((position - transform.position), scale);

        var p1 = new Vector3(Mathf.Floor(localPos.x), 0, Mathf.Floor(localPos.z));
        var p2 = new Vector3(Mathf.Floor(localPos.x), 0, Mathf.Ceil(localPos.z));
        var p3 = new Vector3(Mathf.Ceil(localPos.x), 0, Mathf.Floor(localPos.z));
        var p4 = new Vector3(Mathf.Ceil(localPos.x), 0, Mathf.Ceil(localPos.z));

        p1.x = Mathf.Clamp(p1.x, 0, Dimensions);
        p1.z = Mathf.Clamp(p1.z, 0, Dimensions);
        p2.x = Mathf.Clamp(p2.x, 0, Dimensions);
        p2.z = Mathf.Clamp(p2.z, 0, Dimensions);
        p3.x = Mathf.Clamp(p3.x, 0, Dimensions);
        p3.z = Mathf.Clamp(p3.z, 0, Dimensions);
        p4.x = Mathf.Clamp(p4.x, 0, Dimensions);
        p4.z = Mathf.Clamp(p4.z, 0, Dimensions);


        var max = Mathf.Max(Vector3.Distance(p1, localPos), Vector3.Distance(p2, localPos), Vector3.Distance(p3, localPos), Vector3.Distance(p4, localPos) + Mathf.Epsilon);
        var dist = (max - Vector3.Distance(p1, localPos))
                 + (max - Vector3.Distance(p2, localPos))
                 + (max - Vector3.Distance(p3, localPos))
                 + (max - Vector3.Distance(p4, localPos) + Mathf.Epsilon);

        var height = Mesh.vertices[index(p1.x, p1.z)].y * (max - Vector3.Distance(p1, localPos))
                   + Mesh.vertices[index(p2.x, p2.z)].y * (max - Vector3.Distance(p2, localPos))
                   + Mesh.vertices[index(p3.x, p3.z)].y * (max - Vector3.Distance(p3, localPos))
                   + Mesh.vertices[index(p4.x, p4.z)].y * (max - Vector3.Distance(p4, localPos));

        return height * transform.lossyScale.y / dist;
    }


    private Vector3[] GenerateVerts()
    {
        var verts = new Vector3[(Dimensions + 1) * (Dimensions + 1)];

        for (int x = 0; x <= Dimensions; x++)
        {
            for (int z = 0; z <= Dimensions; z++)
            {
                verts[index(x, z)] = new Vector3(x, 0, z);
            }
        }

        return verts;

    }

    private int index(float x, float z)
    {
        return (int)((x * (Dimensions + 1)) + z);
    }

    // Update is called once per frame

    [BurstCompile]
    void Update()
    {
        var verts = Mesh.vertices;
        for (int x = 0; x <= Dimensions; x++)
        {
            for (int z = 0; z <= Dimensions; z++)
            {
                var y = 0f;
                for (int i = 0; i < Octaves.Length; i++)
                {
                    if (Octaves[i].alternate)
                    {
                        var perl = Mathf.PerlinNoise((x * Octaves[i].scale.x) / Dimensions, (z * Octaves[i].scale.y) / Dimensions) * Mathf.PI * 2f;
                        y += Mathf.Cos(perl + Octaves[i].speed.magnitude * Time.time) * Octaves[i].height;
                    }
                    else
                    {
                        var perls = Mathf.PerlinNoise((x * Octaves[i].scale.x + Time.time * Octaves[i].speed.x) / Dimensions, (z * Octaves[i].scale.y + Time.time * Octaves[i].speed.y) / Dimensions) - 0.5f;
                        y += perls * Octaves[i].height;
                    }
                }

                verts[index(x, z)] = new Vector3(x, y, z);
            }
        }
        Mesh.vertices = verts;
        Mesh.RecalculateNormals();



        //ServerWavesServerRpc();
        
    }

    [Serializable]
    public struct Octave
    {
        public Vector2 speed;
        public Vector2 scale;
        public float height;
        public bool alternate;
    }


    

    /*[ServerRpc]
    public void ServerWavesServerRpc()
    {
        ClientWavesClientRpc();
    }

    [ClientRpc]
    public void ClientWavesClientRpc()
    {
        var verts = Mesh.vertices;
        for (int x = 0; x <= Dimensions; x++)
        {
            for (int z = 0; z <= Dimensions; z++)
            {
                var y = 0f;
                for (int i = 0; i < Octaves.Length; i++)
                {
                    if (Octaves[i].alternate)
                    {
                        var perl = Mathf.PerlinNoise((x * Octaves[i].scale.x) / Dimensions, (z * Octaves[i].scale.y) / Dimensions) * Mathf.PI * 2f;
                        y += Mathf.Cos(perl + Octaves[i].speed.magnitude * Time.time) * Octaves[i].height;
                    }
                    else
                    {
                        var perls = Mathf.PerlinNoise((x * Octaves[i].scale.x + Time.time * Octaves[i].speed.x) / Dimensions, (z * Octaves[i].scale.y + Time.time * Octaves[i].speed.y) / Dimensions) - 0.5f;
                        y += perls * Octaves[i].height;
                    }
                }

                verts[index(x, z)] = new Vector3(x, y, z);
            }
        }
        Mesh.vertices = verts;
        Mesh.RecalculateNormals();
    }*/
}
