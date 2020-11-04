using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    public List<Room> startingRooms;
    public List<Room> randomRoomPool;
    public float roomSize = 20f;
    Queue<Room> queue;
    int randomRoomIndex = -1;
    Vector2 currCoord = Vector2.zero;

    //Keep reference to main(?) camera
    Camera mainCam;

    // Start is called before the first frame update
    void Start()
    {
        queue = new Queue<Room>();
        mainCam = Camera.main;

        //Create starting rooms
        for(int i = 0; i < startingRooms.Count; i++)
        {
            Room room = Instantiate(startingRooms[i], currCoord, Quaternion.identity);
            currCoord += Vector2.right * roomSize;
            queue.Enqueue(room);
        }

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        //Get top room
        Room room = queue.Peek();

        //Calculate top room's position against camera
        Vector2 displacement = room.transform.position - mainCam.transform.position;
        if (displacement.sqrMagnitude >= 400f) //Good enough to get rid off from queue
        {
            Room roomToDestroy = queue.Dequeue();
            Destroy(roomToDestroy.gameObject);
            CreateRoom();
        }
    }

    void CreateRoom()
    {
        randomRoomIndex = Random.Range(0, randomRoomPool.Count);
        Room roomToInstantiate = randomRoomPool[randomRoomIndex];
        Room room = Instantiate(roomToInstantiate, currCoord, Quaternion.identity);
        queue.Enqueue(room);
        currCoord += Vector2.right * roomSize;
    }
}
