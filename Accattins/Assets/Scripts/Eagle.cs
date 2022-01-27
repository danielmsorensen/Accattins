using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Eagle: MonoBehaviour {

    public new SpriteRenderer renderer;
    public float spriteDuration;
    public Sprite[] sprites;
    int i;
    [Space]
    public string flapSound;
    public int flapIndex;
    [Space]
    public GameObject catPrefab;
    [Space]
    public bool lockX;
    public bool lockY;
    [Space]
    public int catsAllowed;
    public float sleepThreshold;
    public float checkTime;
    [Space]
    public Image[] catIndicators;
    public Sprite usedCat;

    Cat cat;
    int cats;

    Vector2 p, v;

    List<Rigidbody2D> simulation;
    Coroutine checking;

    void Awake() {
        if(catsAllowed == 0) {
            catsAllowed = int.MaxValue;
        }
    }

    void Start() {
        simulation = new List<Rigidbody2D>(FindObjectsOfType<Rigidbody2D>());
    }

    void Update() {
        if(Game.instance.state != Game.State.inGame) {
            return;
        }

        #region Mouse Position
        Vector2 p1 = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        v = (p1 - p) / Time.deltaTime;
        p = p1;

        if(lockX) {
            p.x = transform.position.x;
            v.x = 0;
        }
        if(lockY) {
            p.y = transform.position.y;
            v.y = 0;
        }
        #endregion
        
        #region Transform
        transform.position = p;

        if(cat) {
            cat.transform.position = p;
        }
        #endregion

        #region Mouse Buttons
        if(!EventSystem.current.IsPointerOverGameObject()) {
            if(Input.GetMouseButtonDown(0) && cats < catsAllowed) {
                cat = Instantiate(catPrefab, p, Quaternion.identity).GetComponent<Cat>();
                cat.rigidbody.gravityScale = 0;
                simulation.Add(cat.rigidbody);

                if(catIndicators != null && catIndicators.Length > 0) {
                    catIndicators[cats++].sprite = usedCat;
                }
            }
            else if(Input.GetMouseButtonUp(0) && cat) {
                cat.rigidbody.velocity = v;
                cat.rigidbody.gravityScale = 1;
                cat = null;
            }
        }
        #endregion

        #region Sprites
        if(renderer != null && sprites.Length > 0) {
            if(Time.time % spriteDuration <= Time.deltaTime) {
                renderer.sprite = sprites[i];

                if(i == flapIndex) {
                    Sound.Play(flapSound);
                }

                i = (i + 1) % sprites.Length;
            }
        }
        #endregion
    }

    void FixedUpdate() {
        if(Game.instance.state != Game.State.inGame) {
            return;
        }

        if(cats >= catsAllowed && cat == null) {
            bool finished = true;
            foreach(Rigidbody2D rigidbody in simulation) {
                if(rigidbody && rigidbody.velocity.sqrMagnitude > sleepThreshold) {
                    finished = false;
                }
            }
            if(finished) {
                if(checking == null) {
                    checking = StartCoroutine(CheckSleeping());
                }
            }
            else {
                if(checking != null) {
                    StopCoroutine(checking);
                    checking = null;
                } 
            }
        }
    }

    IEnumerator CheckSleeping() {
        yield return new WaitForSeconds(checkTime);
        Game.instance.Gameover(true);
    }
}