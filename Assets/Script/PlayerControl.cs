using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static Direction;
using static UnityEditor.PlayerSettings;

public class Player : MonoBehaviour
{
    [SerializeField] float speed = 5f;
    private Map map;
    public GameObject playerPrefab;
    private Dir direction;
    private Vector2 startMousePos, endMousePos;
    private bool isMove;
    private Vector3 endPoint;
    private List<GameObject> bricks;
    private List<Vector3> lsCurrPath;
    private List<Vector3> lsCurrLine;
    private GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        ResetState();
    }

    public void ResetState()
    {
        endPoint = Vector3.zero;
        Vector3 startPoint = new Vector3(0, offsetPlayerY, 0);
        GameObject newObject = Instantiate(playerPrefab, startPoint, Quaternion.identity);
        newObject.transform.SetParent(transform);
        newObject.tag = "Player";
        player = newObject;
        isMove = false;
        map = GameObject.FindObjectOfType<Map>();
        bricks = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isMove)
        {
            //Debug.Log("+");
            GetInput();
        }
        else
        {
            //Debug.Log("-");
            MoveCharacter(endPoint);
        }
        //
    }
    private void GetInput()
    {
        endPoint = Vector3.zero;
        if (Input.GetMouseButtonDown(0))
        {
            startMousePos = Input.mousePosition;
        }
        if (Input.GetMouseButtonUp(0))
        {
            endMousePos = Input.mousePosition;
            Vector2 dir = endMousePos - startMousePos;
            isMove = true;
            if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
            {
                if (dir.x < 0)
                {
                    direction = Direction.Dir.Left;
                    Debug.Log("left");
                }
                else
                {
                    direction = Direction.Dir.Right;
                    Debug.Log("right");
                }
            }
            else
            {
                if (dir.y < 0)
                {
                    direction = Direction.Dir.Down;
                    Debug.Log("down");
                }
                else
                {
                    direction = Direction.Dir.Up;
                    Debug.Log("up");
                }
            }
            endPoint = GetVectorPos(direction);
        }
    }
    private Vector3 GetVectorPos(Direction.Dir dir)
    {
        lsCurrPath = new List<Vector3>();
        lsCurrLine = new List<Vector3>();
        Vector3 pos = Vector3.zero;
        Vector3 currentPos = new Vector3(transform.position.x, offsetWallY, transform.position.z);
        lsCurrPath.Add(currentPos);
        switch (dir)
        {
            //case Direction.Dir.Left: pos = Vector3.left; break;
            //case Direction.Dir.Right: pos = Vector3.right; break;
            //case Direction.Dir.Up: pos = Vector3.forward; break;
            //case Direction.Dir.Down: pos = Vector3.back; break;
            case Direction.Dir.Left: pos = Vector3.forward; break;
            case Direction.Dir.Right: pos = Vector3.back; break;
            case Direction.Dir.Up: pos = Vector3.right; break;
            case Direction.Dir.Down: pos = Vector3.left; break;
        }
        while (map.LsPathPos.Contains(currentPos) || map.LsPathPos.Contains(currentPos + pos))
        {
            Debug.Log("GetVectorPos: " + currentPos);
            if (map.LsPathPos.Contains(currentPos + pos))
            {
                currentPos += pos;
                lsCurrPath.Add(currentPos);
            }
            else break;
        };
        while (map.LsLinePos.Contains(new Vector3(currentPos.x, 2.64f, currentPos.z) + pos) || map.LsPathPos.Contains(currentPos + pos))
        {
            Debug.Log("GetVectorPos: " + currentPos);
            if (map.LsLinePos.Contains(new Vector3(currentPos.x, offsetLineY, currentPos.z) + pos))
            {
                currentPos += pos;
                lsCurrLine.Add(currentPos);
            }
            else if (map.LsPathPos.Contains(currentPos + pos))
            {
                currentPos += pos;
                lsCurrPath.Add(currentPos);
            }
            else
                break;
        };
        //return new Vector3(currentPos.x, offsetPlayerY, currentPos.z);
        return currentPos;
    }
    private void MoveCharacter(Vector3 targetPosition)
    {

        if (transform.position == targetPosition)
        {
            isMove = false;
            return;
        }
        //targetPosition.y = transform.position.y;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        AddBrick(transform.position);
        RemoveBrick(transform.position);
        if (transform.position == targetPosition)
        {
            isMove = false;
        }
    }
    private void AddBrick(Vector3 currPos)
    {
        foreach (Vector3 pos in lsCurrPath)
        {
            if (Mathf.RoundToInt(currPos.x) == (int)pos.x && Mathf.RoundToInt(currPos.z) == (int)pos.z)
            {
                GameObject brick = map.GetBrickFromPath(pos);
                if (brick != null)
                {
                    
                    brick.transform.SetParent(transform, false);
                    brick.transform.localPosition = new Vector3(0, offsetBrickY + bricks.Count * 0.2f, 0);
                    player.transform.position = new Vector3(player.transform.position.x, offsetPlayerY + bricks.Count * 0.2f, player.transform.position.z);
                    bricks.Add(brick);
                    //lsCurrPath.Remove(pos);
                }
            }
        }
    }
    private void RemoveBrick(Vector3 currPos)
    {
        if (lsCurrLine.Count == 0) return;
        if (bricks.Count == 0)
        {
            isMove = false;
            return;
        }
        //
        foreach (Vector3 pos in lsCurrLine)
        {
            if (Mathf.RoundToInt(currPos.x) == (int)pos.x && Mathf.RoundToInt(currPos.z) == (int)pos.z)
            {
                GameObject brick = bricks[bricks.Count - 1];
                if (brick != null)
                {
                    brick.transform.SetParent(null);

                    if (map.SetBrickToLine(brick, pos))
                    {
                        bricks.Remove(brick);
                        lsCurrLine.Remove(pos);
                        player.transform.position = new Vector3(player.transform.position.x, offsetPlayerY + bricks.Count * 0.2f, player.transform.position.z);
                    }
                    else
                    {
                        brick.transform.SetParent(transform);
                    }
                    break;
                }
            }
        }
    }
}
