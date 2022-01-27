using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class Game: MonoBehaviour {

    public GameObject gameScreen, pauseScreen, completeScreen, gameoverScreen;
    public float pauseDelay, completedDelay, gameoverDelay;
    GameObject[] screens;
    [Space]
    public TMP_Text gameoverReason;
    public string gameoverCatsText, gameoverTargetText;
    [Space]
    public CanvasGroup tutorial;
    public float fadeTime;

    public int level { get; private set; }

    public enum State { inGame, paused, completed, gameover };
    State _state;
    public State state {
        get {
            return _state;
        }
        private set {
            _state = value;
            stateTime = Time.time + _state switch {
                State.paused => pauseDelay,
                State.completed => completedDelay,
                State.gameover => gameoverDelay,
                _ => 0
            };
            stateDelayed = true;
        }
    }
    float stateTime;
    bool stateDelayed;

    public static Game instance { get; private set; }

    void Awake() {
        instance = this;
        level = SceneManager.GetActiveScene().buildIndex;
        state = State.inGame;
    }

    void Start() {
        screens = new GameObject[] { gameScreen, pauseScreen, completeScreen, gameoverScreen };
    }

    void Update() {
        if(stateDelayed && Time.time > stateTime) {
            switch(state) {
                case State.paused:
                    Time.timeScale = 0;
                    SetScreen(1);
                    break;
                case State.completed:
                    SetScreen(2);
                    break;
                case State.gameover:
                    SetScreen(3);
                    break;
            }

            stateDelayed = false;
        }

        if(tutorial != null && tutorial.alpha == 1 && (Input.GetMouseButtonDown(0))) {
            StartCoroutine(FadeTutorial());
        }
    }

    public void SetScreen(int screenIndex) {
        for(int i = 0; i < screens.Length; i++) {
            if(screens[i]) {
                screens[i].SetActive(i == screenIndex);
            }
        }
    }
    IEnumerator FadeTutorial() {
        float start = Time.time;
        while(tutorial.alpha > 0) {
            tutorial.alpha = Mathf.Lerp(1, 0, Mathf.InverseLerp(start, start + fadeTime, Time.time));
            yield return null;
        }
    }

    public void Play() {
        SceneManager.LoadScene(level = 1);
    }
    public void Menu() {
        Resume();
        SceneManager.LoadScene(level = 0);
    }
    public void Quit() {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    public void Resume() {
        Time.timeScale = 1;
        state = State.inGame;
        SetScreen(0);
    }
    public void Pause() {
        state = State.paused;
    }
    public void Completed() {
        state = State.completed;
    }

    public void Gameover(bool outOfCats = false) {
        gameoverReason.text = outOfCats ? gameoverCatsText : gameoverTargetText;
        state = State.gameover;
    }

    public void NextLevel() {
        SceneManager.LoadScene(++level);
    }
    public void Restart() {
        Resume();
        SceneManager.LoadScene(level);
    }
}