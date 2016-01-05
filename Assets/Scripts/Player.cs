using UnityEngine;
using UnityEngine.Networking;
using XboxCtrlrInput;
using System.Collections;

public class Player : NetworkBehaviour {
	private const float ROT_DEAD_ZONE = 0.2f;

	public float Speed;
    public GameObject Arrow;
    [SyncVar]
    public int ArrowCount = 3;
    
    private int playerNum = 1;
    [SyncVar]
    private Quaternion rotation;
	
	public void SetPlayerNum(int playerNum) {
		this.playerNum = playerNum;    
	}

    public void IncreaseArrowCount() {
        if(isServer) {
            ArrowCount++;
        } 
    }

	void Update() {
        if(isLocalPlayer) {
            setRotation();
            setMovement();

            if(XCI.GetButtonDown(XboxButton.A) && ArrowCount > 0) {
                CmdShoot(transform.position);
            }
        }
        else {
            transform.rotation = rotation;
        }
	}

	private void setMovement() {
		float speedX = XCI.GetAxis(XboxAxis.LeftStickX, playerNum) * Speed;
		float speedY = XCI.GetAxis(XboxAxis.LeftStickY, playerNum) * Speed;
		GetComponent<Rigidbody2D>().velocity = new Vector2(speedX, speedY);
	}

	private void setRotation() {
		float rotX = XCI.GetAxis(XboxAxis.RightStickX, playerNum);
		float rotY = -XCI.GetAxis(XboxAxis.RightStickY, playerNum);
		float angle = Mathf.Atan2(-rotY, rotX) * Mathf.Rad2Deg - 90f;
        Quaternion prevRot = transform.rotation;
		
		if(Mathf.Abs(rotX) > ROT_DEAD_ZONE || Mathf.Abs(rotY) > ROT_DEAD_ZONE) {
			transform.rotation = Quaternion.Euler(0, 0, angle);
			GetComponent<Rigidbody2D>().angularVelocity = 0;
		}

        if(!prevRot.Equals(transform.rotation)) {
            CmdUpdateRotation(transform.rotation);
        }
	}

    [Command]
    private void CmdUpdateRotation(Quaternion rotation) {
        this.rotation = rotation;
    }

    [Command]
    private void CmdShoot(Vector3 position) {
        GameObject arrow = Instantiate(Arrow, position + transform.up, transform.rotation) as GameObject;
        arrow.GetComponent<Rigidbody2D>().velocity = transform.up * (Speed + 1);
        arrow.GetComponent<Arrow>().SetOwner(gameObject);
        arrow.GetComponent<Arrow>().Rotation = transform.rotation;
        NetworkServer.Spawn(arrow);
        ArrowCount--;
    }

    void OnCollisionEnter2D(Collision2D col) {
        if (col.gameObject.tag == "Arrow" && isServer) {
            Arrow arrow = col.gameObject.GetComponent<Arrow>();
            if (arrow.Collided) {
                IncreaseArrowCount();
                Destroy(col.gameObject);
            }
            else {
                RpcDie();
            }
        }
        else if (col.collider.gameObject.tag == "Boss" && isServer) {
            RpcDie();
        }
    }

    [ClientRpc]
    private void RpcDie() {
        transform.position = Vector3.zero;
    }
}
