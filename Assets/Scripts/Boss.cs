using UnityEngine;
using System;
using System.Collections;

public class Boss : MonoBehaviour {
    void Update () {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        GameObject[] alivePlayers = Array.FindAll(players, player => player.GetComponent<Player>().isAlive());

        if (alivePlayers.Length > 0) {
            Vector2 myPosition = new Vector2(transform.position.x, transform.position.y);
            Transform closestPlayer = alivePlayers[0].transform;
            float closestDistance = distance(myPosition, new Vector2(alivePlayers[0].transform.position.x, alivePlayers[0].transform.position.y));

            for (int i = 1; i < alivePlayers.Length; i++) {
                Vector2 playerPos = new Vector2(alivePlayers[i].transform.position.x, alivePlayers[i].transform.position.y);
                float currDistance = distance(myPosition, playerPos);
                if (currDistance < closestDistance) {
                    closestDistance = currDistance;
                    closestPlayer = alivePlayers[i].transform;
                }
            }

            Vector3 dir = closestPlayer.position - transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            GetComponent<Rigidbody2D>().velocity = transform.up * 2f;

        }
        else {
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
    }

    private float distance(Vector2 p1, Vector2 p2) {
        return Mathf.Sqrt((p1.x - p2.x) * (p1.x - p2.x) + (p1.y - p2.y) * (p1.y - p2.y));
    }
}
