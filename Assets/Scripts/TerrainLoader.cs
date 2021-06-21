using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 3D mesh
[RequireComponent(typeof(MeshFilter))]
public class TerrainLoader : MonoBehaviour
{   
    
    // Int Vector3s to compute and store vertex offset.
    static readonly Vector3Int[] CubeVertices = new Vector3Int[] 
        { new Vector3Int(), //0
          new Vector3Int(0, 0, 1), //1
          new Vector3Int(1, 0, 0), //2
          new Vector3Int(1, 0, 1), //3
          new Vector3Int(0, 1, 0), //4
          new Vector3Int(0, 1, 1), //5
          new Vector3Int(1, 1, 0), //6
          new Vector3Int(1, 1, 1) //7
        };
    static readonly int[] CubeMesh = new int[] 
        { 2, 1, 0, //Bottom 1
          3, 1, 2, //Bottom 2
          0, 4, 2, //-Z 1
          2, 4, 6, //-Z 2
          2, 6, 3, //+X 1
          3, 6, 7, //+X 2
          3, 7, 1, //+Z 1
          1, 7, 5, //+Z 2
          1, 5, 0, //-X 1
          0, 5, 4, //-X 2
          4, 5, 6, //Top 1
          6, 5, 7  //Top 2
        };
    static readonly int CubeMeshSize = CubeMesh.Length;

    // Powers of 2 for bit shifting with coordinates.
    public int totalCellsX = 32, totalCellsY = 48, totalCellsZ = 32;
    public float cellSizeX = 0.5f, cellSizeY = 0.5f, cellSizeZ = 0.5f;
    public Material defaultMaterial;
    Mesh mesh;
    Vector3[] vertices;
    int[] triangles;
    public Vector3 offset = new Vector3();
    public bool showGizmos = true;

    // Start is called before the first frame update
    void Start()
    {
        CreateMesh();
        UpdateMesh();
    }

    // v = v - o
    // iX = floor(v.x / cellSizeX)
    // iY = floor(v.y / cellSizeY)
    // iZ = floor(v.z / cellSizeZ)
    // ^^^ Position based.
    // verticesPerY = (totalCellsX + 1) * (totalCellsZ + 1)
    // f(iX, iY, iZ) = (iY * verticesPerY) + (iZ * (totalCellsX + 1))+ iX
    private int ComputeIndexOfVertexByIndex(int x, int y, int z) {
        return (y * ((totalCellsX + 1) * (totalCellsZ + 1))) + (z * (totalCellsX + 1))+ x;
    } 

    public void UpdateVertices() {
        this.vertices = new Vector3[(totalCellsX + 1) * (totalCellsY + 1) * (totalCellsZ + 1)];
        for (int y = 0, i = 0; y <= totalCellsY; y++) {
            for (int x = 0; x <= totalCellsX; x++) {
                for (int z = 0; z <= totalCellsZ; z++) {
                    vertices[i] = new Vector3(offset.x + (x * cellSizeX), offset.y + (y * cellSizeY), offset.z + (z * cellSizeZ));
                    i++;
                }
            }            
        }
    }

    public void UpdateTriangles() {
        this.triangles = new int[CubeMeshSize * totalCellsY * totalCellsX * totalCellsZ];
        for (int y = 0, i = 0; y < totalCellsY; y++) {
            for (int x = 0; x < totalCellsX; x++) {
                for (int z = 0; z < totalCellsZ; z++) {
                    //Computed Vertex Index Array.
                    int[] vertexIndices = new int[] {
                        ComputeIndexOfVertexByIndex(x + CubeVertices[0].x, y + CubeVertices[0].y, z + CubeVertices[0].z), 
                        ComputeIndexOfVertexByIndex(x + CubeVertices[1].x, y + CubeVertices[1].y, z + CubeVertices[1].z),
                        ComputeIndexOfVertexByIndex(x + CubeVertices[2].x, y + CubeVertices[2].y, z + CubeVertices[2].z),
                        ComputeIndexOfVertexByIndex(x + CubeVertices[3].x, y + CubeVertices[3].y, z + CubeVertices[3].z),
                        ComputeIndexOfVertexByIndex(x + CubeVertices[4].x, y + CubeVertices[4].y, z + CubeVertices[4].z),
                        ComputeIndexOfVertexByIndex(x + CubeVertices[5].x, y + CubeVertices[5].y, z + CubeVertices[5].z),
                        ComputeIndexOfVertexByIndex(x + CubeVertices[6].x, y + CubeVertices[6].y, z + CubeVertices[6].z),
                        ComputeIndexOfVertexByIndex(x + CubeVertices[7].x, y + CubeVertices[7].y, z + CubeVertices[7].z)
                    };
                    foreach(int index in CubeMesh) {
                        this.triangles[i] = vertexIndices[index];
                        i++;
                    }
                }
            }
        }
    }

    public void CreateMesh() {
        CreateMesh(new Mesh());
    }

    public void CreateMesh(Mesh mesh) {
        this.mesh = mesh;
        GetComponent<MeshFilter>().mesh = mesh;
    }

    public void UpdateMesh() {
        mesh.Clear();
        UpdateVertices();
        UpdateTriangles();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos() {
        // if (showGizmos && vertices != null) {
        //     foreach(Vector3 vertex in vertices) {
        //         Gizmos.DrawSphere(vertex, 0.2f);
        //     }
        // }
    }
}
