using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class Map : MonoBehaviour
{
    List<Vector3> lsMapPos;
    const float mapOffset = 3f;
    const int maxBrick = 20;
    List<Vector3> lsPathPos;
    // Start is called before the first frame update
    void Start()
    {
        lsMapPos = new List<Vector3>();
        lsPathPos = new List<Vector3>();
        CreatMap();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(GetRandomDirection(1));
    }

    private void CreatMap()
    {
        int dir = 2; //up
        lsMapPos.Clear();
        lsPathPos.Clear();
        lsPathPos.Add(Vector3.zero);
        while (lsPathPos.Count < maxBrick)
        {
            RawPathByDirectionAndCurrentPosition(ref lsPathPos, dir);
            if (lsPathPos.Count < maxBrick)
                dir = GetRandomDirection(dir);
        }
        foreach (Vector3 pos in lsPathPos)
        {
            GameObject prefab = Resources.Load<GameObject>("Prefab/Pivote_Brick"); // 
            if (prefab != null)
            {
                GameObject newObject = Instantiate(prefab, pos, Quaternion.identity);
            }
            else
            {
                Debug.LogError("Prefab not found!");
            }
        }
        DrawWall();

    }

    private int GetRandomDirection(int curDir = 0)
    {
        var dir = UnityEngine.Random.Range(-2, 3);
        while (dir == 0 || dir == curDir)
        {
            dir = UnityEngine.Random.Range(-2, 3);
        }
        return dir;
    }

    private void RawPathByDirectionAndCurrentPosition(ref List<Vector3> pPathPos, int pDir)
    {
        int countContinue = UnityEngine.Random.Range(3, 5);
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

    void DrawWall()
    {
        Vector3 minPos = Vector3.zero;
        Vector3 maxPos = Vector3.zero;
        float minX = lsPathPos[0].x;
        float maxX = lsPathPos[0].x;
        float minZ = lsPathPos[0].z;
        float maxZ = lsPathPos[0].z;
        for (int i = 0; i < lsPathPos.Count - 2; i++) // without last pos
        {
            if (minX > lsPathPos[i].x) minX = lsPathPos[i].x;
            if (minZ > lsPathPos[i].z) minZ = lsPathPos[i].z;
            if (maxX < lsPathPos[i].x) maxX = lsPathPos[i].x;
            if (maxZ < lsPathPos[i].z) maxZ = lsPathPos[i].z;
        }
        minX -= 1; maxX += 1;
        minZ -= 1; maxZ += 1;
        if (minX < minZ)
        {
            minZ = minX;
        }
        else
        {
            minX = minZ;
        }
        if (maxX > maxZ)
        {
            maxZ = maxX;
        }
        else
        {
            maxX = maxZ;
        }
        Debug.Log("posX: " + minX + " | " + maxX);
        Debug.Log("posZ: " + minZ + " | " + maxZ);
        for (int x = (int)minX; x <= maxX; x++)
        {
            for (int z = (int)minZ; z <= maxZ; z++)
            {
                Vector3 pos = new Vector3(x, 0, z);
                if (!lsPathPos.Contains(pos))
                {
                    GameObject prefab = Resources.Load<GameObject>("Prefab/Pivote_Wall"); // 
                    if (prefab != null)
                    {
                        GameObject newObject = Instantiate(prefab, pos, Quaternion.identity);
                    }
                    else
                    {
                        Debug.LogError("Prefab not found!");
                    }
                }
                else
                {
                    Debug.Log("posPath: " + pos);
                }
            }
        }
    }
}
