using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class MeshGenerator : MonoBehaviour
{
    Mesh mesh;
    List<Vector3> vertices;
    List<Vector3> targetVertices;
    List<Vector3> currentVertices;
    List<int> triangles;
    List<Vector2> uvs;

    public int xSize;
    public int ySize;

    public float scale;
    public float sphereSize;

    public bool doDrawGizmos;

    public Material mat;

    public float time = 0;

    void InterpolateVertices(float t)
    {
        currentVertices.Clear();

        for (int i = 0; i < vertices.Count; i++)
        {
            currentVertices.Add(Vector3.Lerp(vertices[i], targetVertices[i], t));
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        vertices = new List<Vector3>();
        targetVertices = new List<Vector3>();
        currentVertices = new List<Vector3>();
        triangles = new List<int>();
        uvs = new List<Vector2>();

        mesh = new Mesh();
        //GetComponent<MeshFilter>().mesh = mesh;
        mesh = GetComponent<MeshFilter>().mesh;

        GenerateMesh();
        //GenerateSphereMesh();
        generateCylinder();
        currentVertices = targetVertices;
        UpdateMesh();
        currentVertices = new List<Vector3>(vertices);
    }

    void GenerateMesh()
    {
        float xOffset = -1 * (xSize + 1) / 2.0f;
        float yOffset = -1 * (ySize + 1) / 2.0f;

        Vector3 point = new Vector3(0, 0, 0);

        for (int x = 0; x < xSize + 1; x++)
        {
            point.y = (-1 * (x + xOffset));
            for (int y = 0; y < ySize + 1; y++)
            {
                // Add vertex
                point.x = (y + yOffset);
                
                vertices.Add(point * scale);

                uvs.Add(new Vector2(-(point.y - yOffset) / (ySize + 1),(point.x - xOffset) / (xSize + 1)));

                // Add square
                if (x < xSize && y < ySize)
                {
                    AddSquare(x, y);
                }
            }
        }
    }

    void generateCylinder()
    {
        targetVertices.Clear();
        Vector3 point = new Vector3(0, 0, 0);

        // X is colatitude
        // y is Longitude
        // https://socratic.org/questions/what-is-the-parametric-equation-of-a-sphere

        for (int x = 0; x < xSize + 1; x++)
        {
            for (int y = 0; y < ySize + 1; y++)
            {
                point.x = sphereSize * Mathf.Cos((x / (float)ySize + 1) * 2 * Mathf.PI) * Mathf.Sin((y / (float)xSize) * Mathf.PI);
                point.y = sphereSize * Mathf.Sin((x / (float)ySize + 1) * 2 * Mathf.PI) * Mathf.Sin((y / (float)xSize) * Mathf.PI);
                point.z = 1.075f * sphereSize * Mathf.Cos((y / (float)xSize) * Mathf.PI);
                targetVertices.Add(new Vector3(point.x, point.y, point.z));

                Debug.Log("Point" + point.ToString());


            }
        }


    }

    void GenerateSphereMesh()
    {
        float xOffset = -1 * (xSize + 1) / 2.0f;

        Vector3 point = new Vector3(0, 0, 0);

        for (int x = 0; x < xSize + 1; x++)
        {
            point.y = (-1 * (x + xOffset)) * 100;
            for (int y = 0; y < ySize + 1; y++)
            {
                // Add vertex
                point.x = 1.1f * Mathf.Sin(-y / (float) (ySize) * Mathf.PI * 2) * Mathf.Sin(x / (float)(xSize) * Mathf.PI) * 10;
                point.z = Mathf.Cos(-y / (float) (ySize) * Mathf.PI * 2) * Mathf.Sin(x / (float)(xSize) * Mathf.PI) * 10;

                point = Vector3.Normalize(point);
                
                targetVertices.Add(point * sphereSize);

            }
        }
    }

    void AddSquare(int x, int y)
    {
        // Add top triangle
        triangles.Add((x * (ySize + 1)) + y);
        triangles.Add((x * (ySize + 1)) + (y + 1));
        triangles.Add(((x + 1) * (ySize + 1)) + (y + 1));

        // Add bottom triangle
        triangles.Add((x * (ySize + 1)) + y);
        triangles.Add(((x + 1) * (ySize + 1)) + (y + 1));
        triangles.Add(((x + 1) * (ySize + 1)) + y);

    }

    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = currentVertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();

        GetComponent<MeshFilter>().mesh = mesh;
    }

    private void OnDrawGizmos()
    {

        Vector3 zOffset = new Vector3(0, 0, 0);
        if (vertices != null && doDrawGizmos)
        {
            for (int i = 0; i < vertices.Count; i++)
            {
                zOffset.z += scale / 100;
                Gizmos.DrawCube(vertices[i] + zOffset, new Vector3(scale / 2, scale / 2, scale / 2));
            }
        }
    }

    private void Update()
    {
        time -= Time.deltaTime;

        transform.rotation = Quaternion.Euler(0,time * 5, -time * 30);
    }
}
