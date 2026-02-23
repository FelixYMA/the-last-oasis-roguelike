using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using GamePlay;
using Random = UnityEngine.Random;

public class RoomGenerator : MonoBehaviour
{
    public enum Direction { up, down, left, right };
    public Direction direction;

    [Header("Room Info")]
    public GameObject[] roomPrefabs;
    public int roomNumber;
    public Color startColor, endColor;
    private Room m_EndRoom;

    [Header("Position control")]
    public Transform generatorPoint;
    public float xOffset;
    public float yOffset;
    public LayerMask roomLayer;

    [Header("Prop Generation")]
    public GameObject coin, food, heart, chest;
    [Header("Env Generation")]
    public GameObject[] hEnvs;
    public GameObject[] vEnvs;
    [Header("Enemy Generation")]
    public GameObject[] enemyPrefabs;
    public int minEnemies = 1;
    public int maxEnemies = 3;

    [Header("Boss")]
    public GameObject bossPrefab;

    public int maxStep;
    public WallType wallType;

    public List<Room> rooms = new List<Room>();
    List<GameObject> farRooms = new List<GameObject>();
    List<GameObject> oneWayRooms = new List<GameObject>();
    List<GameObject> lessFarRooms = new List<GameObject>();

    private int basicRoomCount = 0;
    private const int maxBasicRoomCount = 2;
    private GameObject basicRoom3;

    public GameObject npcPrefab;
    public GameObject player;

    void Start()
    {
        // Level1 = 2， Level2 = 3, Level3 = 4, Level4 = 5, Level5 = 6
        roomNumber = 2 + (int)GameManager.Ist.curScene; 
        for (var i = 0; i < roomNumber; i++)
        {
            GameObject roomPrefab = null;
            var valid = false;
            while (!valid)
            {
                if (i == 0)
                {
                    roomPrefab = roomPrefabs[0];
                    if (basicRoomCount < maxBasicRoomCount)
                    {
                        basicRoomCount++;
                        valid = true;
                    }
                }
                else
                {
                    // var randIndex = Random.Range(1, roomPrefabs.Length - 1);
                    roomPrefab = roomPrefabs[i];
                    valid = true;
                }
            }
            var newRoom = Instantiate(roomPrefab, generatorPoint.position, Quaternion.identity).GetComponent<Room>();
            newRoom.gameObject.name = i == 0 ? "StartRoom" : $"Room{i}";
            newRoom.index = i;
            rooms.Add(newRoom);
            ChangePointPos();
        }
        GameManager.Ist.curRoom = rooms[0];
        rooms[0].GetComponent<SpriteRenderer>().color = startColor;
        // FindEndRoom();
        m_EndRoom = rooms.Last();
        // ReplaceRoomWith(endRoom, basicRoom3);
        m_EndRoom.GetComponent<SpriteRenderer>().color = endColor;
        foreach (var room in rooms)
        {
            SetupRoom(room, room.transform.position);
        }
        SpawnBoss(m_EndRoom);
        // Instantiate(npcPrefab, rooms[0].transform.position + new Vector3(1.5f, 0f, 0f), Quaternion.identity);
        if (player != null)
        {
            player.transform.position = rooms[0].transform.position;
        }
        CameraController.instance.ChangeTarget(rooms[0].transform);
    }

    private void SpawnBoss(Room r)
    {
        if (bossPrefab != null) { Instantiate(bossPrefab, Vector3.zero, Quaternion.identity, r.transform); }
    }

    // public void ReplaceRoomWith(GameObject targetRoom, GameObject replacementPrefab)
    // {
    //     if (targetRoom == null || replacementPrefab == null) return;
    //
    //     Vector3 pos = targetRoom.transform.position;
    //     Destroy(targetRoom);
    //
    //     GameObject newRoomObj = Instantiate(replacementPrefab, pos, Quaternion.identity);
    //     Room newRoom = newRoomObj.GetComponent<Room>();
    //
    //     SetupRoom(newRoom, pos);
    //
    //     for (int i = 0; i < rooms.Count; i++)
    //     {
    //         if (rooms[i].transform.position == pos)
    //         {
    //             rooms[i] = newRoom;
    //             break;
    //         }
    //     }
    //
    //     endRoom = newRoomObj;
    // }

    public void ChangePointPos()
    {
        int maxAttempts = 10;
        int attempts = 0;
        bool found = false;

        while (attempts < maxAttempts)
        {
            direction = (Direction)Random.Range(0, 4);

            Vector3 newPos = generatorPoint.position;

            switch (direction)
            {
                case Direction.up: newPos += new Vector3(0, yOffset, 0); break;
                case Direction.down: newPos += new Vector3(0, -yOffset, 0); break;
                case Direction.left: newPos += new Vector3(-xOffset, 0, 0); break;
                case Direction.right: newPos += new Vector3(xOffset, 0, 0); break;
            }

            if (!Physics2D.OverlapCircle(newPos, 0.2f, roomLayer))
            {
                generatorPoint.position = newPos;
                found = true;
                break;
            }

            attempts++;
        }

        if (!found)
            Debug.LogWarning("RoomGenerator: Could not find a valid position after several attempts.");
    }

    public void SetupRoom(Room newRoom, Vector3 roomPosition)
    {
        newRoom.roomUp = Physics2D.OverlapCircle(roomPosition + new Vector3(0, yOffset, 0), 0.2f, roomLayer);
        newRoom.roomDown = Physics2D.OverlapCircle(roomPosition + new Vector3(0, -yOffset, 0), 0.2f, roomLayer);
        newRoom.roomLeft = Physics2D.OverlapCircle(roomPosition + new Vector3(-xOffset, 0, 0), 0.2f, roomLayer);
        newRoom.roomRight = Physics2D.OverlapCircle(roomPosition + new Vector3(xOffset, 0, 0), 0.2f, roomLayer);

        newRoom.UpdateRoom(xOffset, yOffset);
        SpawnWalls(newRoom, roomPosition);
        if (rooms.FindIndex(r => r.transform.position == roomPosition) % 2 == 1) { SpawnEnvH(newRoom); }else { SpawnEnvV(newRoom); }
        SpawnProps(newRoom);
        SpawnEnemies(newRoom);
    }
    private void SpawnProps(Room r)
    {
        if (r == rooms[0]) return;
        var count = Random.Range(5, 21);
        Debug.Log($"room: {r.name}, props-count: {count}");
        for (var i = 0; i < count; i++)
        {
            var prefab = i is 10 or 20 ? heart : i % 5 == 0 ? food : coin;
            var ist = Instantiate(prefab, Vector3.zero, Quaternion.identity, r.transform);
            ist.transform.localPosition = new Vector3(Random.Range(-7, 8), Random.Range(-3, 5), 0);
        }
        if (r.index == 2)
        {
            var ist = Instantiate(chest, Vector3.zero, Quaternion.identity, r.transform);
            ist.transform.localPosition = new Vector3(Random.Range(-7, 8), Random.Range(-3, 5), 0);
        }
    }
    private void SpawnEnvV(Room r)
    {
        if (r == rooms[0]) return;
        var count = Random.Range(3, 8);
        var range = Random.Range(0, vEnvs.Length - 1);
        var istPos1 = new Vector2(Random.Range(1.5f, 8f), 2);
        for (var i = 0; i < count; i++)
        {
            var ist = Instantiate(vEnvs[range], Vector3.zero, Quaternion.identity, r.transform);
            var ist2 = Instantiate(vEnvs[range], Vector3.zero, Quaternion.identity, r.transform);
            ist.transform.localPosition = new Vector3(istPos1.x, istPos1.y + i * 0.5f, 0);
            ist.GetComponent<SpriteRenderer>().sortingOrder -= i+1;
            ist2.transform.localPosition = new Vector3(Math.Clamp(-istPos1.x, -7, 7), istPos1.y + i * 0.5f, 0);
            ist2.GetComponent<SpriteRenderer>().sortingOrder -= i+1;
            var ist3 = Instantiate(vEnvs[range], Vector3.zero, Quaternion.identity, r.transform);
            var ist4 = Instantiate(vEnvs[range], Vector3.zero, Quaternion.identity, r.transform);
            ist3.transform.localPosition = new Vector3(istPos1.x, -istPos1.y - i * 0.5f, 0);
            ist3.GetComponent<SpriteRenderer>().sortingOrder += i+1;
            ist4.transform.localPosition = new Vector3(Math.Clamp(-istPos1.x, -7, 7), -istPos1.y - i * 0.5f, 0);
            ist4.GetComponent<SpriteRenderer>().sortingOrder += i+1;
        }
    }
    private void SpawnEnvH(Room r)
    {
        if (r == rooms[0]) return;
        var count = Random.Range(5, 12);
        var range = Random.Range(0, hEnvs.Length - 1);
        var istPos1 = new Vector2(2, Random.Range(1, 5));
        for (var i = 0; i < count; i++)
        {
            var ist = Instantiate(hEnvs[range], Vector3.zero, Quaternion.identity, r.transform);
            var ist2 = Instantiate(hEnvs[range], Vector3.zero, Quaternion.identity, r.transform);
            ist.transform.localPosition = new Vector3(istPos1.x + i * 0.5f, istPos1.y, 0);
            ist2.transform.localPosition = new Vector3(istPos1.x + i * 0.5f, Math.Clamp(-istPos1.y, -3, 4), 0);
            var ist3 = Instantiate(hEnvs[range], Vector3.zero, Quaternion.identity, r.transform);
            var ist4 = Instantiate(hEnvs[range], Vector3.zero, Quaternion.identity, r.transform);
            ist3.transform.localPosition = new Vector3(-istPos1.x - i * 0.5f, istPos1.y, 0);
            ist4.transform.localPosition = new Vector3(-istPos1.x - i * 0.5f, Math.Clamp(-istPos1.y, -3, 4), 0);
        }
    }

    void SpawnWalls(Room room, Vector3 pos)
    {
        switch (room.doorNumber)
        {
            case 1:
                if (room.roomUp) Instantiate(wallType.singleUp, pos, Quaternion.identity);
                if (room.roomDown) Instantiate(wallType.singleBottom, pos, Quaternion.identity);
                if (room.roomLeft) Instantiate(wallType.singleLeft, pos, Quaternion.identity);
                if (room.roomRight) Instantiate(wallType.singleRight, pos, Quaternion.identity);
                break;
            case 2:
                if (room.roomLeft && room.roomUp) Instantiate(wallType.doubleLU, pos, Quaternion.identity);
                if (room.roomLeft && room.roomRight) Instantiate(wallType.doubleLR, pos, Quaternion.identity);
                if (room.roomLeft && room.roomDown) Instantiate(wallType.doubleLB, pos, Quaternion.identity);
                if (room.roomUp && room.roomRight) Instantiate(wallType.doubleUR, pos, Quaternion.identity);
                if (room.roomUp && room.roomDown) Instantiate(wallType.doubleUB, pos, Quaternion.identity);
                if (room.roomRight && room.roomDown) Instantiate(wallType.doubleRB, pos, Quaternion.identity);
                break;
            case 3:
                if (room.roomLeft && room.roomUp && room.roomRight) Instantiate(wallType.tripleLUR, pos, Quaternion.identity);
                if (room.roomLeft && room.roomRight && room.roomDown) Instantiate(wallType.tripleLRB, pos, Quaternion.identity);
                if (room.roomDown && room.roomUp && room.roomRight) Instantiate(wallType.tripleURB, pos, Quaternion.identity);
                if (room.roomLeft && room.roomUp && room.roomDown) Instantiate(wallType.tripleLUB, pos, Quaternion.identity);
                break;
            case 4:
                Instantiate(wallType.fourDoors, pos, Quaternion.identity);
                break;
        }
    }

    void SpawnEnemies(Room room)
    {
        if (room == m_EndRoom?.GetComponent<Room>()) return;
        if (room.index == 0 || room.index >= rooms.Count - 1) return;
        var enemyCount = Random.Range(room.index + minEnemies , room.index + maxEnemies);
        for (var i = 0; i < enemyCount; i++)
        {
            var randomEnemyIndex = Random.Range(0, enemyPrefabs.Length);
            var ist = Instantiate(enemyPrefabs[randomEnemyIndex], Vector3.zero, Quaternion.identity, room.transform);
            ist.transform.localPosition = Vector3.zero;
        }
    }
    
    [Serializable]
    public class WallType
    {
        public GameObject singleLeft, singleRight, singleUp, singleBottom,
                          doubleLU, doubleLR, doubleLB, doubleUR, doubleUB, doubleRB,
                          tripleLUR, tripleLUB, tripleURB, tripleLRB, fourDoors;
    }
}
