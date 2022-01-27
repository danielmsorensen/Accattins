using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Cat: MonoBehaviour {

    [HideInInspector]
    public new Rigidbody2D rigidbody;
    [HideInInspector]
    public new Collider2D collider;

    void Awake() {
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
    }

    void OnBecameInvisible() {
        Destroy(gameObject);
    }
}