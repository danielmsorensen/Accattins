using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class Block: MonoBehaviour {

    [HideInInspector]
    public new Rigidbody2D rigidbody;
    [HideInInspector]
    public new Collider2D collider;
    [HideInInspector]
    public new SpriteRenderer renderer;

    [HideInInspector]
    public float health;
    [Header("Block")]
    public float maxHealth;
    public bool unbreakable;
    public State[] states;

    public string damageSound;
    public string destructSound;

    void Awake() {
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
        renderer = GetComponent<SpriteRenderer>();

        health = 1;
    }

    void Start() {
        if(!unbreakable) {
            UpdateState();
        }
        else {
            maxHealth = 1;
        }
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if(Time.time < Time.deltaTime) {
            return;
        }

        float mass = collision.rigidbody ? collision.rigidbody.mass : 1;
        float dmg = collision.relativeVelocity.magnitude * mass / maxHealth;

        if(!unbreakable) {
            health -= dmg;

            UpdateState();
        }

        if(!string.IsNullOrEmpty(damageSound)) {
            Sound.Play(damageSound, Mathf.Min(dmg * maxHealth, 1));
        }
    }

    void UpdateState() {
        if(health <= 0) {
            Destruct();
        }

        foreach(State state in states) {
            if(health <= state.health) {
                renderer.sprite = state.sprite;
            }
        }
    }

    protected virtual void Destruct() {
        if(!string.IsNullOrEmpty(destructSound)) {
            Sound.Play(destructSound);
        }

        Destroy(gameObject);
    }

    [System.Serializable]
    public struct State {
        [Range(0,1)]
        public float health;
        public Sprite sprite;
    }
}