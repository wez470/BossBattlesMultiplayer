using UnityEngine;
using System;
using XboxCtrlrInput;
using System.Collections.Generic;

public class PlayerManager : MonoBehaviour {
    public GameObject Player;

    private int numPlayers = 0;
    private List<GameObject> players = new List<GameObject>();
    
    void Awake() {
		numPlayers = XCI.GetNumPluggedCtrlrs();
        SetupPlayers();
    }
    
    private void SetupPlayers() {
        for (int i = 1; i <= numPlayers; i++) {
            CreatePlayer(i);
        }
    }
    
    private void CreatePlayer(int playerNum) {
        GameObject newPlayer = Instantiate(Player) as GameObject;
		newPlayer.transform.position = new Vector3(0, 0, 0);
        Player player = newPlayer.gameObject.GetComponent<Player>();
        player.SetPlayerNum(playerNum);
        players.Add(newPlayer);
    }

    public int GetNumPlayers() {
        return numPlayers;
    }
    
    public List<GameObject> GetPlayers() {
        return players;
    }
}
