using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour
{
    public int width;
    public int height;
    public float generationInterval;
    public GameObject cellPrefab;
    public Color aliveColor;
    public Color deadColor;

    private GameObject[,] _cells;
    private float _cellSize;
    private bool _paused;
    private Vector3 _lastClickPosition;

    private void Start()
    {
        _cellSize = cellPrefab.GetComponent<SpriteRenderer>().bounds.size.x;
        _cells = new GameObject[height, width];
        for (var i = 0; i < height; i++)
        for (var j = 0; j < width; j++)
            _cells[i, j] = Instantiate(cellPrefab,
                new Vector3(_cellSize * -width / 2f + _cellSize / 2 + _cellSize * j,
                    _cellSize * height / 2f - _cellSize / 2 - _cellSize * i, 0),
                Quaternion.identity);

        StartCoroutine(UpdatePopulation());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) GenerateRandomCells();
        if (Input.GetKeyDown(KeyCode.C)) ClearCells();
        if (Input.GetKeyDown(KeyCode.Space)) _paused = !_paused;

        if (Input.GetMouseButtonDown(0))
        {
            if (!_paused) _paused = true;

            var mouseDownPosition = PointToGrid(Camera.main.ScreenToWorldPoint(Input.mousePosition), _cellSize);
            _lastClickPosition = mouseDownPosition;

            ClickOnCell((int) mouseDownPosition.x, (int) mouseDownPosition.y);
        }

        if (Input.GetMouseButton(0))
        {
            var currentMousePosition = PointToGrid(Camera.main.ScreenToWorldPoint(Input.mousePosition), _cellSize);

            if (currentMousePosition != _lastClickPosition)
            {
                _lastClickPosition = currentMousePosition;
                ClickOnCell((int) currentMousePosition.x, (int) currentMousePosition.y);
            }
        }
    }

    private IEnumerator UpdatePopulation()
    {
        while (true)
        {
            yield return new WaitForSeconds(generationInterval);

            if (_paused) continue;

            var cellsToDie = new List<GameObject>();
            var cellsToBorn = new List<GameObject>();

            for (var i = 0; i < height; i++)
            for (var j = 0; j < width; j++)
            {
                var count = 0;
                var cell = _cells[i, j];
                var cellAlive = cell.GetComponent<Cell>().Alive;

                for (var k = -1; k <= 1; k++)
                for (var l = -1; l <= 1; l++)
                    if (i + k < height && i + k >= 0 && j + l < width && j + l >= 0)
                        count += _cells[i + k, j + l].GetComponent<Cell>().Alive ? 1 : 0;

                count -= cellAlive ? 1 : 0;

                switch (count)
                {
                    case 2:
                        break;
                    case 3:
                        if (!cellAlive) cellsToBorn.Add(cell);
                        break;
                    default:
                        if (cellAlive) cellsToDie.Add(cell);
                        break;
                }
            }

            foreach (var cell in cellsToDie) KillCell(cell);

            foreach (var cell in cellsToBorn) BornCell(cell);
        }
    }

    public void GenerateRandomCells()
    {
        for (var i = 0; i < height; i++)
        for (var j = 0; j < width; j++)
        {
            var cell = _cells[i, j];
            if (Random.Range(0, 100) > 75)
                BornCell(cell);
            else
                KillCell(cell);
        }
    }

    public void ClearCells()
    {
        for (var i = 0; i < height; i++)
        for (var j = 0; j < width; j++)
            KillCell(_cells[i, j]);
    }

    private void ClickOnCell(int x, int y)
    {
        var clickX = x;
        var clickY = y;
        if (clickX <= width - 1 && clickX >= 0 && clickY <= height - 1 && clickY >= 0)
        {
            var clickedCell = _cells[clickY, clickX];
            if (clickedCell.GetComponent<Cell>().Alive) KillCell(clickedCell);
            else BornCell(clickedCell);
        }
    }
    
    private void KillCell(GameObject cell)
    {
        cell.GetComponent<Cell>().Alive = false;
        cell.GetComponent<SpriteRenderer>().color = deadColor;
    }

    private void BornCell(GameObject cell)
    {
        cell.GetComponent<Cell>().Alive = true;
        cell.GetComponent<SpriteRenderer>().color = aliveColor;
    }

    private Vector2 PointToGrid(Vector2 point, float gridSize)
    {
        var position = point;
        position /= gridSize;
        position = math.floor(position);
        position.y += height / 2f;
        position.x += width / 2f;
        position.y = height - 1 - position.y;

        return position;
    }

    private Vector3 PointToGrid(Vector3 point, float gridSize)
    {
        var position = point;
        position /= gridSize;
        position = math.floor(position);
        position.y += height / 2f;
        position.x += width / 2f;
        position.y = height - 1 - position.y;

        return position;
    }
}