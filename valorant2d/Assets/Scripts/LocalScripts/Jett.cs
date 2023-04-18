using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;


public class Jett : MonoBehaviourPunCallbacks
{
    private Rigidbody2D rb;
    Vector2 movement;
    Vector2 mousePos;
    private bool canDash = true;
    private float dashingPower = 24f;
    private float dashingTime = 0.2f;
    public bool isDashing;
    private PhotonView pv;
    private Vector3 _startPosition;
    private float _progress;
    public Transform _gunPoint;

    
    private bool canSmoke = true;

    public TrailRenderer tr;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        pv = GetComponent<PhotonView>();
    }
    void Update(){
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        if(Input.GetKeyDown(KeyCode.LeftShift) && canDash && pv.IsMine)
        {
            StartCoroutine(Dash());
        } 
        if (Input.GetKeyDown(KeyCode.LeftControl) && canSmoke && pv.IsMine){
            StartCoroutine(Smoke());
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Smoke"), _gunPoint.position, _gunPoint.rotation);
        }
    }

    private IEnumerator Smoke(){
        canSmoke = false;
        yield return new WaitForSeconds(2f);
        canSmoke = true;
    }


    private IEnumerator Dash(){
        canDash = false;
        isDashing = true;
        rb.velocity = new Vector2(movement.x * dashingPower, movement.y * dashingPower);
        tr.emitting = true;
        yield return new WaitForSeconds(dashingTime);
        tr.emitting = false;
        isDashing = false;
        yield return new WaitForSeconds(1f);
        canDash = true;
    }

}
