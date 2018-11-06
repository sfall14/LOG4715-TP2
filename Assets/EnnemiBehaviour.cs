using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnnemiBehaviour : MonoBehaviour {

    private float wanderSpeed = 0.5f;
    Rigidbody _RbEnn { get; set; }
    Animator _Anim { get; set; }
    bool _Grounded { get; set; }

    public string stunnedLayer = "StunnedEnemy";
    public string playerLayer = "Player";
    public GameObject stunCheck;
    public bool isStunned = false;

    int _stunnedLayer;
    int _enemyLayer;

    [SerializeField]
    LayerMask WhatIsGround;

    // Use this for initialization
    void Start () {
        _enemyLayer = this.gameObject.layer;
        _stunnedLayer = LayerMask.NameToLayer("StunnedLayer");
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("PlayerLayer"), _stunnedLayer, true);
        _Grounded = false;
        _Anim = GetComponent<Animator>();
        _RbEnn = GetComponent<Rigidbody>();
    }

    void HorizontalMoveEnn()
    {
        _RbEnn.velocity = new Vector3(_RbEnn.velocity.x, _RbEnn.velocity.y, wanderSpeed);
        _Anim.SetFloat("MoveSpeed", Mathf.Abs(wanderSpeed));
    }
    // Update is called once per frame
    void Update () {
        if (!isStunned)
        {
            HorizontalMoveEnn();
        }
        
    }

    // Collision avec le sol
    void OnCollisionEnter(Collision coll)
    {
        // On s'assure de bien être en contact avec le sol
        if ((WhatIsGround & (1 << coll.gameObject.layer)) == 0)
            return;

        // Évite une collision avec le plafond
        if (coll.relativeVelocity.y > 0)
        {
            _Grounded = true;
            _Anim.SetBool("Grounded", _Grounded);
        }
    }

    public void Stunned()
    {
        if (!isStunned)
        {
            isStunned = true;
            _Anim.SetFloat("MoveSpeed", 0);
            _RbEnn.velocity = new Vector3(0, 0, 0);
            this.gameObject.layer = _stunnedLayer;
            stunCheck.layer = _stunnedLayer;
             
            StartCoroutine ( EnemyStunned() );
        }
    }

    public IEnumerator EnemyStunned()
    {
        
        yield return new WaitForSeconds(5);
        isStunned = false;

        this.gameObject.layer = _enemyLayer;
        stunCheck.layer = _enemyLayer;
    }
}
