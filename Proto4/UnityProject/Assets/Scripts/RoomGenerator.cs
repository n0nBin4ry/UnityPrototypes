using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator : MonoBehaviour {
	[Range(0f, 1f)]
	public float DefaultEasyProbability = 0.3f;
	public float SecsToMaxDifficulty = 20f;
	private float m_difficultyTimer = 0f;
	private int m_introCounter = 0;

	[Range(0f, 1f)]
	public float MaxSlowDownProbability = 0.9f;
	public float SecsToMaxSlowDownProb = 10f;
	private float m_slowdownTimer = 0f;

    [SerializeField] private List<Room> m_StartingRooms;
    [SerializeField] private List<Room> m_EasyRoomPool;
    [SerializeField] private List<Room> m_HardRoomPool;
    
	public const float ROOM_CELL_WIDTH = 20f;
    
    private Queue<Room> m_RoomQueue;
    private Vector2 m_RoomSpawnPosition = Vector2.zero;

    // Keep reference to main(?) camera
    Camera mainCam;

	// variables to keep rooms from repeating consecutiveley
	private int m_lastEasyIndex = -1;
	private int m_lastHardIndex = -1;

    // Start is called before the first frame update
    void Start() {
        m_RoomQueue = new Queue<Room>();
        mainCam = Camera.main;

        // Create starting rooms
        for (int i = 0; i < m_StartingRooms.Count; i++) {
			Vector2 tempSpawnPosition = m_RoomSpawnPosition + (Vector2.right * (ROOM_CELL_WIDTH * ((m_StartingRooms[i].NumUnits / 2f) - 0.5f)));
			Room room = Instantiate(m_StartingRooms[i], tempSpawnPosition, Quaternion.identity);
            m_RoomQueue.Enqueue(room);
			m_RoomSpawnPosition += Vector2.right * ROOM_CELL_WIDTH * room.NumUnits;
		}
    }

	private void Update() {
		// increase difficulty timer if we passed all intro rooms
		if (m_introCounter >= m_StartingRooms.Count && m_difficultyTimer < SecsToMaxDifficulty)
			m_difficultyTimer = Mathf.Min(m_difficultyTimer + Time.deltaTime, SecsToMaxDifficulty);

		// increase slowdowntimer if we passed all intro rooms
		if (m_introCounter >= m_StartingRooms.Count && m_slowdownTimer < SecsToMaxSlowDownProb)
			m_slowdownTimer = Mathf.Min(m_difficultyTimer + Time.deltaTime, SecsToMaxSlowDownProb);

		// if there are less than 3 rooms then make sure that we spawn more
		while (m_RoomQueue.Count < 3)
			CreateRoom();

		// Get room closest to being destroyed
		Room room = m_RoomQueue.Peek();

		// Calculate distance from camera to see if it is time to destroy it; destroy if we are a room to the left of the camera
		if (room.transform.position.x < mainCam.transform.position.x) {
			float displacementSqr = (room.transform.position - mainCam.transform.position).sqrMagnitude;
			// basically adjusting for rooms with variable widths
			float deleteThresholdSqr = ((ROOM_CELL_WIDTH * room.NumUnits) / 2) + ROOM_CELL_WIDTH / 2;
			deleteThresholdSqr *= deleteThresholdSqr; // sqr the value
			if (displacementSqr > deleteThresholdSqr) {
				Room roomToDestroy = m_RoomQueue.Dequeue();
				Destroy(roomToDestroy.gameObject);
				CreateRoom();
				// increase the intro counter to count how many intro rooms we passed
				if (m_introCounter < m_StartingRooms.Count)
					m_introCounter++;
			}
		}
	}

	/*private void FixedUpdate() {
		// if there are less than 3 rooms then make sure that we spawn more
		while (m_RoomQueue.Count < 3)
			CreateRoom();

        // Get room closest to being destroyed
        Room room = m_RoomQueue.Peek();

        // Calculate distance from camera to see if it is time to destroy it; destroy if we are a room to the left of the camera
		if (room.transform.position.x < mainCam.transform.position.x) {
			float displacementSqr = (room.transform.position - mainCam.transform.position).sqrMagnitude;
			// basically adjusting for rooms with variable widths
			float deleteThresholdSqr = ((ROOM_CELL_WIDTH * room.NumUnits) / 2) + ROOM_CELL_WIDTH / 2;
			deleteThresholdSqr *= deleteThresholdSqr; // sqr the value
			if (displacementSqr > deleteThresholdSqr) {
				Room roomToDestroy = m_RoomQueue.Dequeue();
				Destroy(roomToDestroy.gameObject);
				CreateRoom();
				// increase the intro counter to count how many intro rooms we passed
				if (m_introCounter < m_StartingRooms.Count)
					m_introCounter++;
			}
		}
    }*/

    void CreateRoom() {
		// see what room pool we are choosing from
		bool isDifficult = false;
		float difficultProb = Mathf.Min(m_difficultyTimer / SecsToMaxDifficulty, 1f - DefaultEasyProbability);
		if (difficultProb > 0f && UnityEngine.Random.Range(0f, 1f) < difficultProb) {
			isDifficult = true;
		}
		Room room;
		if (isDifficult) {
			int randomIndex = UnityEngine.Random.Range(0, m_HardRoomPool.Count);
			// wrap around if the value is room is the same as before
			if (randomIndex == m_lastHardIndex) {
				randomIndex++;
				if (randomIndex == m_HardRoomPool.Count)
					randomIndex = 0;
			}
			m_lastHardIndex = randomIndex;
			Vector2 tempSpawnPosition = m_RoomSpawnPosition + (Vector2.right * (ROOM_CELL_WIDTH * ((m_HardRoomPool[randomIndex].NumUnits / 2f) - 0.5f)));
			room = Instantiate(m_HardRoomPool[randomIndex], tempSpawnPosition, Quaternion.identity);
		}
		else {
			int randomIndex = UnityEngine.Random.Range(0, m_EasyRoomPool.Count);
			if (randomIndex == m_lastEasyIndex) {
				randomIndex++;
				if (randomIndex == m_EasyRoomPool.Count)
					randomIndex = 0;
			}
			m_lastEasyIndex = randomIndex;
			Vector2 tempSpawnPosition = m_RoomSpawnPosition + (Vector2.right * (ROOM_CELL_WIDTH * ((m_EasyRoomPool[randomIndex].NumUnits / 2f) - 0.5f)));
			room = Instantiate(m_EasyRoomPool[randomIndex], tempSpawnPosition, Quaternion.identity);
		}

		// check if we spawn slowdown collectibles
		float slowdownProb = Mathf.Min(MaxSlowDownProbability, m_slowdownTimer / SecsToMaxSlowDownProb);
		bool spawnSlowdown = slowdownProb > 0f && UnityEngine.Random.Range(0f, 1f) < slowdownProb;
		if (!spawnSlowdown) {
			foreach (var item in room.LavaCollectables)
				if (item)
					item.gameObject.SetActive(false);
		}
		// lower probability of next spawn if the we spawn
		else {
			m_slowdownTimer = Mathf.Max(0, m_slowdownTimer - (SecsToMaxSlowDownProb * 0.7f));
		}


		m_RoomQueue.Enqueue(room);
		m_RoomSpawnPosition += Vector2.right * ROOM_CELL_WIDTH * room.NumUnits;
	}

	public void MarkSlowdown() {
		// reset timer for slowdown
		m_slowdownTimer = 0f;
		foreach (var room in m_RoomQueue)
			// clear all other slowdowns
			if (room)
				foreach (var item in room.LavaCollectables)
					if (item)
						item.MarkDestroy();
	}
}
