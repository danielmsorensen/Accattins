using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Target: Block {

    [Header("Target")]
    public bool protect;
    public bool optional;

    static List<Target> targets;

    void Start() {
        if(targets == null) {
            targets = new List<Target>(FindObjectsOfType<Target>());
        }
    }

    void SceneLoaded(Scene scene, LoadSceneMode mode) {
        targets = null;
    }

    protected override void Destruct() {
        if(protect) {
            Game.instance.Gameover();
        }
        else {
            targets.Remove(this);

            bool completed = true;

            foreach(Target target in targets) {
                if(!target.optional && !target.protect) {
                    completed = false;
                }
            }

            if(completed) {
                Game.instance.Completed();
            }
        }

        base.Destruct();
    }

    void OnEnable() {
        SceneManager.sceneLoaded += SceneLoaded;
    }
    void OnDisable() {
        SceneManager.sceneLoaded -= SceneLoaded;
    }
}