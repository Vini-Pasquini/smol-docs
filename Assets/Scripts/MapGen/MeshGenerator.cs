// src: https://www.youtube.com/watch?v=yOgIncKp0BE
//      https://www.youtube.com/watch?v=2gIxh8CX3Hk
//      https://www.youtube.com/watch?v=AsR0-wCTJl8

using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
    public struct Triangle
    {
        public int vertexIndexA;
        public int vertexIndexB;
        public int vertexIndexC;

        int[] vertices;

        public Triangle(int vertexIndexA, int vertexIndexB, int vertexIndexC)
        {
            this.vertexIndexA = vertexIndexA;
            this.vertexIndexB = vertexIndexB;
            this.vertexIndexC = vertexIndexC;

            this.vertices = new int[3];
            this.vertices[0] = vertexIndexA;
            this.vertices[1] = vertexIndexB;
            this.vertices[2] = vertexIndexC;
        }

        public int this[int index] // indexer
        {
            get { return this.vertices[index]; }
        }

        public bool Contains(int vertexIndex)
        {
            return vertexIndex == this.vertexIndexA || vertexIndex == this.vertexIndexB || vertexIndex == this.vertexIndexC;
        }
    }

    public class Node
    {
        public Vector3 position;
        public int vertexIndex;

        public Node(Vector3 position)
        {
            this.position = position;
            this.vertexIndex = -1;
        }
    }

    public class ControlNode : Node
    {
        public bool isActive;
        public Node up, right;

        public ControlNode(Vector3 position, bool isActive, float squareSize) : base(position)
        {
            this.isActive = isActive;
            this.up = new Node(position + Vector3.up * squareSize/2f);
            this.right = new Node(position + Vector3.right * squareSize/2f);
        }
    }

    public class Square
    {
        public ControlNode topLeft, topRight, bottomRight, bottomLeft;
        public Node centerTop, centerRight, centerBottom, centerLeft;

        public int configurationCode;

        public Square(ControlNode topLeft, ControlNode topRight, ControlNode bottomRight, ControlNode bottomLeft)
        {
            this.topLeft = topLeft;
            this.topRight = topRight;
            this.bottomRight = bottomRight;
            this.bottomLeft = bottomLeft;

            this.centerTop = topLeft.right;
            this.centerRight = bottomRight.up;
            this.centerBottom = bottomLeft.right;
            this.centerLeft = bottomLeft.up;

            this.configurationCode = 0;
            if (this.topLeft.isActive) this.configurationCode += 8;
            if (this.topRight.isActive) this.configurationCode += 4;
            if (this.bottomRight.isActive) this.configurationCode += 2;
            if (this.bottomLeft.isActive) this.configurationCode += 1;
        }
    }

    public class SquareGrid
    {
        public Square[,] squares;

        public SquareGrid(int[,] mapWallData, float squareSize)
        {
            int nodeCountX = mapWallData.GetLength(0);
            int nodeCountY = mapWallData.GetLength(1);

            float mapWidth = nodeCountX * squareSize;
            float mapHeight = nodeCountY * squareSize;

            ControlNode[,] controlNodes = new ControlNode[nodeCountX, nodeCountY];

            for (int xIndex = 0; xIndex < nodeCountX; xIndex++)
            {
                for (int yIndex = 0; yIndex < nodeCountY; yIndex++)
                {
                    Vector3 position = new Vector3((xIndex * squareSize) + (squareSize/2f) - mapWidth/2f, (yIndex * squareSize) + (squareSize / 2f) - mapHeight / 2f, 0);
                    controlNodes[xIndex, yIndex] = new ControlNode(position, mapWallData[xIndex, yIndex] == 1, squareSize);
                }
            }

            this.squares = new Square[nodeCountX - 1, nodeCountY - 1];

            for (int xIndex = 0; xIndex < nodeCountX - 1; xIndex++)
            {
                for (int yIndex = 0; yIndex < nodeCountY - 1; yIndex++)
                {
                    this.squares[xIndex, yIndex] = new Square(controlNodes[xIndex, yIndex+1], controlNodes[xIndex+1, yIndex+1], controlNodes[xIndex+1, yIndex], controlNodes[xIndex, yIndex]);
                }
            }
        }
    }

    public SquareGrid squareGrid;

    private List<Vector3> vertices;
    private List<int> triangles;

    private Dictionary<int, List<Triangle>> triangleDict = new Dictionary<int, List<Triangle>>();
    private List<List<int>> outlines = new List<List<int>>();
    private HashSet<int> visited = new HashSet<int>();
    
    public MeshFilter walls;

    public void GenerateMesh(int[,] mapData, float squareSize)
    {
        this.triangleDict.Clear();
        this.outlines.Clear();
        this.visited.Clear();

        this.squareGrid = new SquareGrid(mapData, squareSize);

        this.vertices = new List<Vector3>();
        this.triangles = new List<int>();

        for (int xIndex = 0; xIndex < this.squareGrid.squares.GetLength(0); xIndex++)
        {
            for (int yIndex = 0; yIndex < this.squareGrid.squares.GetLength(1); yIndex++)
            {
                this.TriangulateSquare(this.squareGrid.squares[xIndex, yIndex]);
            }
        }

        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        mesh.vertices = this.vertices.ToArray();
        mesh.triangles = this.triangles.ToArray();
        mesh.RecalculateNormals();

        // CreateWallMesh
        this.CalculateMeshOutlines();

        List<Vector3> wallVertices = new List<Vector3>();
        List<int> wallTriangles = new List<int>();
        Mesh wallMesh = new Mesh();
        float wallHeight = 10f;

        foreach (List<int> outline in this.outlines)
        {
            for (int index = 0; index < outline.Count - 1; index++)
            {
                int startIndex = wallVertices.Count;
                wallVertices.Add(this.vertices[outline[index]]); // left
                wallVertices.Add(this.vertices[outline[index+1]]); // right
                wallVertices.Add(this.vertices[outline[index]] + Vector3.forward * wallHeight); // bottom left
                wallVertices.Add(this.vertices[outline[index+1]] + Vector3.forward * wallHeight); // bottom right

                wallTriangles.Add(startIndex + 0);
                wallTriangles.Add(startIndex + 2);
                wallTriangles.Add(startIndex + 3);

                wallTriangles.Add(startIndex + 3);
                wallTriangles.Add(startIndex + 1);
                wallTriangles.Add(startIndex + 0);
            }
        }
        wallMesh.vertices = wallVertices.ToArray();
        wallMesh.triangles = wallTriangles.ToArray();

        this.walls.mesh = wallMesh;
        this.walls.GetComponent<MeshCollider>().sharedMesh = wallMesh;

    }

    private void TriangulateSquare(Square square)
    {
        switch(square.configurationCode)
        {
            case 0: break;
            // 1 points
            case 1: this.MeshFromPoints(square.centerLeft, square.centerBottom, square.bottomLeft); break;
            case 2: this.MeshFromPoints(square.bottomRight, square.centerBottom, square.centerRight); break;
            case 4: this.MeshFromPoints(square.topRight, square.centerRight, square.centerTop); break;
            case 8: this.MeshFromPoints(square.topLeft, square.centerTop, square.centerLeft); break;
            // 2 points
            case 3: this.MeshFromPoints(square.centerRight, square.bottomRight, square.bottomLeft, square.centerLeft); break;
            case 6: this.MeshFromPoints(square.centerTop, square.topRight, square.bottomRight, square.centerBottom); break;
            case 9: this.MeshFromPoints(square.topLeft, square.centerTop, square.centerBottom, square.bottomLeft); break;
            case 12: this.MeshFromPoints(square.topLeft, square.topRight, square.centerRight, square.centerLeft); break;
            case 5: this.MeshFromPoints(square.centerTop, square.topRight, square.centerRight, square.centerBottom, square.bottomLeft, square.centerLeft); break;
            case 10: this.MeshFromPoints(square.topLeft, square.centerTop, square.centerRight, square.bottomRight, square.centerBottom, square.centerLeft); break;
            // 3 point
            case 7: this.MeshFromPoints(square.centerTop, square.topRight, square.bottomRight, square.bottomLeft, square.centerLeft); break;
            case 11: this.MeshFromPoints(square.topLeft, square.centerTop, square.centerRight, square.bottomRight, square.bottomLeft); break;
            case 13: this.MeshFromPoints(square.topLeft, square.topRight, square.centerRight, square.centerBottom, square.bottomLeft); break;
            case 14: this.MeshFromPoints(square.topLeft, square.topRight, square.bottomRight, square.centerBottom, square.centerLeft); break;
            // 4 point
            case 15:
                this.MeshFromPoints(square.topLeft, square.topRight, square.bottomRight, square.bottomLeft);
                this.visited.Add(square.topLeft.vertexIndex);
                this.visited.Add(square.topRight.vertexIndex);
                this.visited.Add(square.bottomRight.vertexIndex);
                this.visited.Add(square.bottomLeft.vertexIndex);
                break;
        }
    }

    private void MeshFromPoints(params Node[] points)
    {
        // AssignVertices
        for (int index = 0; index < points.Length; index++)
        {
            if (points[index].vertexIndex != -1) continue;

            points[index].vertexIndex = this.vertices.Count;
            this.vertices.Add(points[index].position);
        }

        // Create Triangles
        if (points.Length >= 3) { CreateTriangle(points[0], points[1], points[2]); }
        if (points.Length >= 4) { CreateTriangle(points[0], points[2], points[3]); }
        if (points.Length >= 5) { CreateTriangle(points[0], points[3], points[4]); }
        if (points.Length >= 6) { CreateTriangle(points[0], points[4], points[5]); }
    }

    private void CreateTriangle(Node nodeA, Node nodeB, Node nodeC)
    {
        this.triangles.Add(nodeA.vertexIndex);
        this.triangles.Add(nodeB.vertexIndex);
        this.triangles.Add(nodeC.vertexIndex);

        Triangle triangle = new Triangle(nodeA.vertexIndex, nodeB.vertexIndex, nodeC.vertexIndex);

        this.AddTriangleToDict(triangle.vertexIndexA, triangle);
        this.AddTriangleToDict(triangle.vertexIndexB, triangle);
        this.AddTriangleToDict(triangle.vertexIndexC, triangle);
    }

    private void AddTriangleToDict(int vertexIndexKey, Triangle triangle)
    {
        if (this.triangleDict.ContainsKey(vertexIndexKey))
        {
            this.triangleDict[vertexIndexKey].Add(triangle);
            return;
        }

        List<Triangle> triangleList = new List<Triangle>();
        triangleList.Add(triangle);
        this.triangleDict.Add(vertexIndexKey, triangleList);
    }

    private bool IsOutlineEdge(int vertexA, int vertexB)
    {
        int sharedTriangleCount = 0;
        
        List<Triangle> trianglesContainingVertexA = this.triangleDict[vertexA];
        for (int index = 0; index < trianglesContainingVertexA.Count; index++)
        {
            if (trianglesContainingVertexA[index].Contains(vertexB))
            {
                sharedTriangleCount++;
                if (sharedTriangleCount > 1) break;
            }
        }
        return sharedTriangleCount == 1; // aaa
    }

    private int GetConnectedOutlineVertex(int vertexIndex)
    {
        List<Triangle> trianglesContainingVertex = this.triangleDict[vertexIndex];
        for (int i = 0; i < trianglesContainingVertex.Count; i++)
        {
            Triangle triangle = trianglesContainingVertex[i];
            for (int j = 0; j < 3; j++)
            {
                int vertexB = triangle[j];
                if (vertexB == vertexIndex || visited.Contains(vertexB)) continue;
                if (this.IsOutlineEdge(vertexIndex, vertexB)) { return vertexB; }
            }
        }

        return -1;
    }

    private void CalculateMeshOutlines()
    {
        for (int vertexIndex = 0; vertexIndex < this.vertices.Count; vertexIndex++)
        {
            if (this.visited.Contains(vertexIndex)) continue;

            int newOutlineVertex = GetConnectedOutlineVertex(vertexIndex);
            if (newOutlineVertex == -1) continue;

            this.visited.Add(vertexIndex); // aaa
            List<int> newOutline = new List<int>();
            newOutline.Add(vertexIndex);
            this.outlines.Add(newOutline);
            this.FollowOutline(newOutlineVertex, this.outlines.Count - 1);
            this.outlines[this.outlines.Count - 1].Add(vertexIndex);
        }
    }

    private void FollowOutline(int vertexIndex, int outlineIndex)
    {
        this.outlines[outlineIndex].Add(vertexIndex);
        this.visited.Add(vertexIndex);
        int nextVertexIndex = this.GetConnectedOutlineVertex(vertexIndex);

        if (nextVertexIndex == -1) return;
        this.FollowOutline(nextVertexIndex, outlineIndex);
    }
}
