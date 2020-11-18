using System.Collections.Generic;
using UnityEngine;

namespace Game.Networking
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance = null;

        public static Dictionary<int, PlayerManager> players = new Dictionary<int, PlayerManager>();

        [SerializeField] private GameObject localPlayerPrefab;
        [SerializeField] private GameObject playerPrefab;
        
        // Start is called before the first frame update
        void Awake()
        {
            if (Instance != null) Destroy(gameObject);
            Instance = this;
        }

        public void SpawnPlayer(int id, string username, Vector3 position, Quaternion rotation)
        {
            GameObject player;
            
            if (id == Client.instance.myId) player = Instantiate(localPlayerPrefab, position, rotation);
            else player = Instantiate(playerPrefab, position, rotation);

            PlayerManager playerManager = player.GetComponent<PlayerManager>();
            playerManager.id = id;
            playerManager.username = username;
            players.Add(id, playerManager);
        }
    }
}
