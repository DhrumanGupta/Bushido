using System.Collections.Generic;
using Game.Combat;
using Game.Control;
using UnityEngine;

namespace Game.Networking
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        public static Dictionary<int, CharacterManager> players = new Dictionary<int, CharacterManager>();
        public static Dictionary<int, CharacterManager> enemies = new Dictionary<int, CharacterManager>();

        private GameObject localPlayerPrefab;
        private GameObject playerPrefab;
        
        private GameObject nunchuckPrefab;
        private GameObject kunaiPrefab;
        private GameObject shurikenPrefab;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Debug.Log("Instance already exists, destroying object!");
                Destroy(this);
            }

            localPlayerPrefab = Resources.Load<GameObject>("LocalPlayer");
            playerPrefab = Resources.Load<GameObject>("Player");
            
            nunchuckPrefab = Resources.Load<GameObject>("Enemy_Kunai");
            kunaiPrefab = Resources.Load<GameObject>("Enemy__Nunchuck");
            shurikenPrefab = Resources.Load<GameObject>("Enemy_Shuriken");
        }

        public void SpawnPlayer(int _id, string _username, Vector3 _position, Quaternion _rotation)
        {
            GameObject _player;
            if (_id == Client.Instance.myId)
            {
                _player = Instantiate(localPlayerPrefab, _position, _rotation);
            }
            else
            {
                _player = Instantiate(playerPrefab, _position, _rotation);
            }

            CharacterManager characterManager = _player.GetComponent<CharacterManager>();
            characterManager.id = _id;
            characterManager.username = _username;
            _player.name = _username;
            players.Add(_id, characterManager);
            CameraController.Instance.targets.Add(_player.transform);
        }

        public void SpawnEnemy(int id, Vector3 position, Quaternion rotation, int type, float health)
        {
            GameObject enemy;
            switch (type)
            {
                case 1:
                    // Spawn Nunchuck
                    enemy = Instantiate(nunchuckPrefab, position, rotation);
                    break;
                case 2:
                    // Spawn Kunai
                    enemy = Instantiate(kunaiPrefab, position, rotation);
                    break;
                case 3:
                    // Spawn Shuriken
                    enemy = Instantiate(shurikenPrefab, position, rotation);
                    break;
                case 4:
                    // Spawn Glider
                    enemy = Instantiate(nunchuckPrefab, position, rotation);
                    break;
                case 5:
                    // Spawn Elder
                    enemy = Instantiate(nunchuckPrefab, position, rotation);
                    break;
                default:
                    enemy = Instantiate(nunchuckPrefab, position, rotation);
                    break;
            }

            enemy.GetComponent<Health>().SetHealth(health);
            
            CharacterManager characterManager = enemy.GetComponent<CharacterManager>();
            
            characterManager.id = id;
            
            enemies[id] = characterManager;
        }

        public static void PlayerLeft(int id)
        {
            CameraController.Instance.targets.Remove(players[id].transform);
            Destroy(players[id].gameObject);
            players.Remove(id);
            print($"Player {id} has left the game.");
            Resources.UnloadUnusedAssets();
        }
    }
}
