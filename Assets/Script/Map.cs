using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using static Direction;
using static UnityEditor.PlayerSettings;

public class Map : MonoBehaviour
{
    [SerializeField] GameObject wall;
    [SerializeField] GameObject path;
    [SerializeField] GameObject line;
    public GameObject wallPrefab;
    public GameObject pathPrefab;
    public GameObject linePrefab;
    public GameObject winPoint;
    List<Vector3> lsMapPos;
    List<Vector3> lsPathPos;
    List<Vector3> lsLinePos;
    //const float mapOffset = 3f;
    const int maxBrick = 20;
    const float offsetLineY = 2.64f;
    const float offsetWallY = 0f;
    int currDirection = 2;//up
    // Start is called before the first frame update
    void Start()
    {
        lsMapPos = new List<Vector3>();
        lsPathPos = new List<Vector3>();
        lsLinePos = new List<Vector3>();
        CreatMap();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(GetRandomDirection(1));
    }

    private void CreatMap()
    {
        lsPathPos.Clear();
        lsPathPos.Add(Vector3.zero);
        currDirection = 2; // up
        while (lsPathPos.Count < maxBrick)
        {
            DrawPathByDirectionAndCurrentPosition(ref lsPathPos, currDirection);
            if (lsPathPos.Count < maxBrick)
                currDirection = GetRandomDirection(currDirection);
        }
        foreach (Vector3 pos in lsPathPos)
        {
            GameObject newObject = Instantiate(pathPrefab, pos, Quaternion.identity);
            newObject.transform.SetParent(path.transform);
            //newObject.tag = "Path";
        }
        DrawWall();
        DrawLine();
        DrawWinPoint();
    }

    private int GetRandomDirection(int curDir = 0)
    {
        var dir = UnityEngine.Random.Range(-2, 3);
        while (dir == 0 || dir == curDir * -1)
        {
            dir = UnityEngine.Random.Range(-2, 3);
        }
        return dir;
    }

    private void DrawPathByDirectionAndCurrentPosition(ref List<Vector3> pPathPos, int pDir)
    {
        int countContinue = UnityEngine.Random.Range(2, 4);
        Vector3 current = pPathPos[pPathPos.Count - 1];
        for (int i = 1; i <= countContinue; i++)
        {
            switch (pDir)
            {
                case (int)Direction.Dir.Up:
                    pPathPos.Add(new Vector3(current.x + i, current.y, current.z));
                    break;
                case (int)Direction.Dir.Down:
                    pPathPos.Add(new Vector3(current.x - i, current.y, current.z));
                    break;
                case (int)Direction.Dir.Left:
                    pPathPos.Add(new Vector3(current.x, current.y, current.z - i));
                    break;
                case (int)Direction.Dir.Right:
                    pPathPos.Add(new Vector3(current.x, current.y, current.z + i));
                    break;
            }
        }
    }

    private void DrawWall()
    {
        lsMapPos.Clear();
        float minX = lsPathPos[0].x;
        float maxX = lsPathPos[0].x;
        float minZ = lsPathPos[0].z;
        float maxZ = lsPathPos[0].z;
        for (int i = 0; i < lsPathPos.Count - 1; i++)
        {
            if (minX > lsPathPos[i].x) minX = lsPathPos[i].x;
            if (minZ > lsPathPos[i].z) minZ = lsPathPos[i].z;
            if (maxX < lsPathPos[i].x) maxX = lsPathPos[i].x;
            if (maxZ < lsPathPos[i].z) maxZ = lsPathPos[i].z;
        }
        minX -= 1; maxX += 1;
        minZ -= 1; maxZ += 1;
        //if (minX < minZ)
        //{
        //    minZ = minX;
        //}
        //else
        //{
        //    minX = minZ;
        //}
        //if (maxX > maxZ)
        //{
        //    maxZ = maxX;
        //}
        //else
        //{
        //    maxX = maxZ;
        //}
        Debug.Log("posX: " + minX + " | " + maxX);
        Debug.Log("posZ: " + minZ + " | " + maxZ);
        for (int x = (int)minX; x <= maxX; x++)
        {
            for (int z = (int)minZ; z <= maxZ; z++)
            {
                Vector3 pos = new Vector3(x, offsetWallY, z);
                if (!lsPathPos.Contains(pos) && !checkEndPath(pos))
                {
                    GameObject newObject = Instantiate(wallPrefab, pos, Quaternion.identity);
                    newObject.transform.SetParent(wall.transform);
                    //newObject.tag = "Wall";
                    lsMapPos.Add(pos);
                }
                else
                {
                    Debug.Log("posPath: " + pos);
                }
            }
        }
    }
    private bool checkEndPath(Vector3 pos)
    {
        //return false;
        Vector3 endPoint = lsPathPos[lsPathPos.Count - 1];
        switch (currDirection)
        {
            case (int)Direction.Dir.Up:
                if (pos == new Vector3(endPoint.x + 1, offsetWallY, endPoint.z)) return true;
                break;
            case (int)Direction.Dir.Down:
                if (pos == new Vector3(endPoint.x - 1, offsetWallY, endPoint.z)) return true;
                break;
            case (int)Direction.Dir.Left:
                if (pos == new Vector3(endPoint.x, offsetWallY, endPoint.z - 1)) return true;
                break;
            case (int)Direction.Dir.Right:
                if (pos == new Vector3(endPoint.x, offsetWallY, endPoint.z + 1)) return true;
                break;
        }
        return false;
    }
    private void DrawLine()
    {
        lsLinePos.Clear();
        Vector3 endPoint = lsPathPos[lsPathPos.Count - 1];
        Vector3 pos;
        Vector3 cPos;
        Vector3 ro;
        for (int i = 1; i <= maxBrick; i++)
        {
            switch (currDirection)
            {
                case (int)Direction.Dir.Up:
                    pos = new Vector3(endPoint.x + i, offsetLineY, endPoint.z);
                    cPos = new Vector3(endPoint.x + i, offsetWallY, endPoint.z);
                    ro = new Vector3(-90, 0, 90);
                    if (lsPathPos.Contains(cPos) || lsMapPos.Contains(cPos)) // check conflict
                    {
                        CreatMap();
                        return;
                    }
                    break;
                case (int)Direction.Dir.Down:
                    pos = new Vector3(endPoint.x - i, offsetLineY, endPoint.z);
                    cPos = new Vector3(endPoint.x - i, offsetWallY, endPoint.z);
                    ro = new Vector3(-90, 0, 90);
                    if (lsPathPos.Contains(cPos) || lsMapPos.Contains(cPos)) // check conflict
                    {
                        CreatMap();
                        return;
                    }
                    break;
                case (int)Direction.Dir.Left:
                    pos = new Vector3(endPoint.x, offsetLineY, endPoint.z - i);
                    cPos = new Vector3(endPoint.x, offsetWallY, endPoint.z - i);
                    ro = new Vector3(-90, 0, 0);
                    if (lsPathPos.Contains(cPos) || lsMapPos.Contains(cPos)) // check conflict
                    {
                        CreatMap();
                        return;
                    }
                    break;
                case (int)Direction.Dir.Right:
                    pos = new Vector3(endPoint.x, offsetLineY, endPoint.z + i);
                    cPos = new Vector3(endPoint.x, offsetWallY, endPoint.z + i);
                    ro = new Vector3(-90, 0, 0);
                    if (lsPathPos.Contains(cPos) || lsMapPos.Contains(cPos)) // check conflict
                    {
                        CreatMap();
                        return;
                    }
                    break;
                default: // never run
                    pos = Vector3.zero;
                    ro = Vector3.zero;
                    break;
            }
            //Debug.Log("direction : " + currDirection);
            GameObject newObject = Instantiate(linePrefab, pos, Quaternion.identity);
            newObject.transform.Rotate(ro);
            newObject.transform.SetParent(line.transform);
            //newObject.tag = "Line";
            lsLinePos.Add(pos);
        }
    }
    private void DrawWinPoint()
    {
        //return;
        Vector3 endPoint = lsLinePos[lsLinePos.Count - 1];
        Vector3 ro = Vector3.zero;
        Vector3 pos = Vector3.zero;
        Debug.Log("Dir: " + currDirection);
        switch (currDirection)
        {
            case (int)Direction.Dir.Up:
                pos = new Vector3(endPoint.x + 1, offsetWallY, endPoint.z);
                ro = new Vector3(0, 90, 0);
                break;
            case (int)Direction.Dir.Down:
                pos = new Vector3(endPoint.x - 1, offsetWallY, endPoint.z);
                ro = new Vector3(0, -90, 0);
                break;
            case (int)Direction.Dir.Left:
                pos = new Vector3(endPoint.x, offsetWallY, endPoint.z - 1);
                ro = new Vector3(0, 180, 0);
                break;
            case (int)Direction.Dir.Right:
                pos = new Vector3(endPoint.x, offsetWallY, endPoint.z + 1);
                ro = new Vector3(0, 0, 0);
                break;
        }
        GameObject newObject = Instantiate(winPoint, pos, Quaternion.identity);
        newObject.transform.Rotate(ro);
        //newObject.transform.SetParent(line.transform);
    }
}
