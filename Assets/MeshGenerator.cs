using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour {
    public SquareGrid SqrGrid;
    List<Vector3> Vertices;
    List<int> Triangles;

    public void GenerateMesh(int[,] Map, float SquareSize)
    {
        SqrGrid = new SquareGrid(Map, SquareSize);
        Vertices = new List<Vector3>();
        Triangles = new List<int>();

        for (int x = 0; x < SqrGrid.Squares.GetLength(0); x++)
        {
            for (int y = 0; y < SqrGrid.Squares.GetLength(1); y++)
            {
                TriangulateSquare(SqrGrid.Squares[x, y]);
            }
        }

        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        mesh.vertices = Vertices.ToArray();
        mesh.triangles = Triangles.ToArray();
        mesh.RecalculateNormals();
    }

    void TriangulateSquare(Square Square)
    {
        switch (Square.Configuration)
        {
            case 0:
                break;

            // 1 points:
            case 1:
                MeshFromPoints(Square.CentreBottom, Square.BottomLeft, Square.CentreLeft);
                break;
            case 2:
                MeshFromPoints(Square.CentreRight, Square.BottomRight, Square.CentreBottom);
                break;
            case 4:
                MeshFromPoints(Square.CentreTop, Square.TopRight, Square.CentreRight);
                break;
            case 8:
                MeshFromPoints(Square.TopLeft, Square.CentreTop, Square.CentreLeft);
                break;

            // 2 points:
            case 3:
                MeshFromPoints(Square.CentreRight, Square.BottomRight, Square.BottomLeft, Square.CentreLeft);
                break;
            case 6:
                MeshFromPoints(Square.CentreTop, Square.TopRight, Square.BottomRight, Square.CentreBottom);
                break;
            case 9:
                MeshFromPoints(Square.TopLeft, Square.CentreTop, Square.CentreBottom, Square.BottomLeft);
                break;
            case 12:
                MeshFromPoints(Square.TopLeft, Square.TopRight, Square.CentreRight, Square.CentreLeft);
                break;
            case 5:
                MeshFromPoints(Square.CentreTop, Square.TopRight, Square.CentreRight, Square.CentreBottom, Square.BottomLeft, Square.CentreLeft);
                break;
            case 10:
                MeshFromPoints(Square.TopLeft, Square.CentreTop, Square.CentreRight, Square.BottomRight, Square.CentreBottom, Square.CentreLeft);
                break;

            // 3 point:
            case 7:
                MeshFromPoints(Square.CentreTop, Square.TopRight, Square.BottomRight, Square.BottomLeft, Square.CentreLeft);
                break;
            case 11:
                MeshFromPoints(Square.TopLeft, Square.CentreTop, Square.CentreRight, Square.BottomRight, Square.BottomLeft);
                break;
            case 13:
                MeshFromPoints(Square.TopLeft, Square.TopRight, Square.CentreRight, Square.CentreBottom, Square.BottomLeft);
                break;
            case 14:
                MeshFromPoints(Square.TopLeft, Square.TopRight, Square.BottomRight, Square.CentreBottom, Square.CentreLeft);
                break;

            // 4 point:
            case 15:
                MeshFromPoints(Square.TopLeft, Square.TopRight, Square.BottomRight, Square.BottomLeft);
                break;
        }
    }

    void MeshFromPoints(params Node[] Points)
    {
        AssignVertices(Points);

        if (Points.Length >= 3)
            CreateTriangle(Points[0], Points[1], Points[2]);
        if (Points.Length >= 4)
            CreateTriangle(Points[0], Points[2], Points[3]);
        if (Points.Length >= 5)
            CreateTriangle(Points[0], Points[3], Points[4]);
        if (Points.Length >= 6)
            CreateTriangle(Points[0], Points[4], Points[5]);
    }

    void AssignVertices(Node[] points)
    {
        for (int i = 0; i < points.Length; i++)
        {
            if (points[i].VertexIndex == -1)
            {
                points[i].VertexIndex = Vertices.Count;
                Vertices.Add(points[i].Position);
            }
        }
    }

    void CreateTriangle(Node a, Node b, Node c)
    {
        Triangles.Add(a.VertexIndex);
        Triangles.Add(b.VertexIndex);
        Triangles.Add(c.VertexIndex);
    }



    /*void OnDrawGizmos()
    {
        if (SqrGrid != null)
        {
            for (int x = 0; x < SqrGrid.Squares.GetLength(0); x++)
            {
                for (int y = 0; y < SqrGrid.Squares.GetLength(1); y++)
                {

                    Gizmos.color = (SqrGrid.Squares[x, y].TopLeft.Active) ? Color.black : Color.white;
                    Gizmos.DrawCube(SqrGrid.Squares[x, y].TopLeft.Position, Vector3.one * .4f);

                    Gizmos.color = (SqrGrid.Squares[x, y].TopRight.Active) ? Color.black : Color.white;
                    Gizmos.DrawCube(SqrGrid.Squares[x, y].TopRight.Position, Vector3.one * .4f);

                    Gizmos.color = (SqrGrid.Squares[x, y].BottomRight.Active) ? Color.black : Color.white;
                    Gizmos.DrawCube(SqrGrid.Squares[x, y].BottomRight.Position, Vector3.one * .4f);

                    Gizmos.color = (SqrGrid.Squares[x, y].BottomLeft.Active) ? Color.black : Color.white;
                    Gizmos.DrawCube(SqrGrid.Squares[x, y].BottomLeft.Position, Vector3.one * .4f);


                    Gizmos.color = Color.grey;
                    Gizmos.DrawCube(SqrGrid.Squares[x, y].CentreTop.Position, Vector3.one * .15f);
                    Gizmos.DrawCube(SqrGrid.Squares[x, y].CentreRight.Position, Vector3.one * .15f);
                    Gizmos.DrawCube(SqrGrid.Squares[x, y].CentreBottom.Position, Vector3.one * .15f);
                    Gizmos.DrawCube(SqrGrid.Squares[x, y].CentreLeft.Position, Vector3.one * .15f);

                }
            }
        }
    }*/

    public class SquareGrid
    {
        public Square[,] Squares;

        public SquareGrid(int[,] Map, float SquareSize)
        {
            int nodeCountX = Map.GetLength(0);
            int nodeCountY = Map.GetLength(1);
            float mapWidth = nodeCountX * SquareSize;
            float mapHeight = nodeCountY * SquareSize;

            ControlNode[,] controlNodes = new ControlNode[nodeCountX, nodeCountY];

            for (int x = 0; x < nodeCountX; x++)
            {
                for (int y = 0; y < nodeCountY; y++)
                {
                    Vector3 pos = new Vector3(-mapWidth / 2 + x * SquareSize + SquareSize / 2, 0, -mapHeight / 2 + y * SquareSize + SquareSize / 2);
                    controlNodes[x, y] = new ControlNode(pos, Map[x, y] == 1, SquareSize);
                }
            }

            Squares = new Square[nodeCountX - 1, nodeCountY - 1];
            for (int x = 0; x < nodeCountX - 1; x++)
            {
                for (int y = 0; y < nodeCountY - 1; y++)
                {
                    Squares[x, y] = new Square(controlNodes[x, y + 1], controlNodes[x + 1, y + 1], controlNodes[x + 1, y], controlNodes[x, y]);
                }
            }

        }
    }

    public class Square
    {

        public ControlNode TopLeft, TopRight, BottomRight, BottomLeft;
        public Node CentreTop, CentreRight, CentreBottom, CentreLeft;
        public int Configuration;

        public Square(ControlNode _topLeft, ControlNode _topRight, ControlNode _bottomRight, ControlNode _bottomLeft)
        {
            TopLeft = _topLeft;
            TopRight = _topRight;
            BottomRight = _bottomRight;
            BottomLeft = _bottomLeft;

            CentreTop = TopLeft.Right;
            CentreRight = BottomRight.Above;
            CentreBottom = BottomLeft.Right;
            CentreLeft = BottomLeft.Above;

            if (TopLeft.Active)
                Configuration += 8;
            if (TopRight.Active)
                Configuration += 4;
            if (BottomRight.Active)
                Configuration += 2;
            if (BottomLeft.Active)
                Configuration += 1;

        }
    }

    public class Node
    {
        public Vector3 Position;
        public int VertexIndex = -1;

        public Node(Vector3 _pos)
        {
            Position = _pos;
        }
    }

    public class ControlNode : Node
    {

        public bool Active;
        public Node Above, Right;

        public ControlNode(Vector3 _pos, bool _active, float SquareSize) : base(_pos)
        {
            Active = _active;
            Above = new Node(Position + Vector3.forward * SquareSize / 2f);
            Right = new Node(Position + Vector3.right * SquareSize / 2f);
        }

    }
}