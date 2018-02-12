using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour {
    public SquareGrid SqrGrid;

    public void GenerateMesh(int[,] Map, float SquareSize)
    {
        SqrGrid = new SquareGrid(Map, SquareSize);
    }

    void OnDrawGizmos()
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
    }

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