using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Arrow : NetworkBehaviour {
    [SyncVar]
    public Quaternion Rotation;    
    public bool Collided { get; set; }
    
    [SyncVar]
    private GameObject owner;

    public void SetOwner(GameObject player) {
        owner = player;
    }

	void Start () {
        Collided = false;
        transform.rotation = Rotation;
        Physics2D.IgnoreLayerCollision(gameObject.layer, gameObject.layer, true);
    }

    void OnBecameInvisible() {
        if (owner != null && !Collided) {
            owner.GetComponent<Player>().IncreaseArrowCount();
        }
        Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D col) {
        if (isServer) {
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            Collided = true;
            GetComponent<Rigidbody2D>().drag = 10;

            if (col.collider.gameObject.tag == "WeakSpot") {
                Destroy(col.gameObject);
            }
        }
    }

    public GameObject GetOwner() {
        return owner;
    }
}
