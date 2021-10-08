using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid
{
    private int width;
    private int height;
    private GridSquare[,] gridArray;

    public Grid(int dim)
    {
        this.width = dim;
        this.height = dim;
        gridArray = new GridSquare[height, width];
    }

    public Grid(int width, int height)
    {
        this.width = width;
        this.height = height;
    }

    private class GridSquare
    {
        private Vector2 gridPosition;
        private GameObject groundTile;
        private Actor occupant;

        public GridSquare(Vector2 gridPosition)
        {
            this.gridPosition = gridPosition;
        }

        public GridSquare(Vector2 gridPosition, Actor occupant)
        {
            this.gridPosition = gridPosition;
            this.occupant = occupant;
        }
    }
}
