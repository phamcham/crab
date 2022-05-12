using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameCursor : MonoBehaviour {
    private static GameCursor instance;
    [SerializeField] Image cursorImage;
    [SerializeField] Sprite defaultCursor;
    [Header("Sprites")]
    [SerializeField] List<CursorSprites> cursorSprites;
    Dictionary<CursorState, Sprite> sprites = new Dictionary<CursorState, Sprite>();
    CursorState currentState = CursorState.Default;
    private void Awake() {
        DontDestroyOnLoad(cursorImage.canvas.gameObject);
        instance = this;

        foreach (CursorSprites cursorSprite in cursorSprites) {
            if (sprites.ContainsKey(cursorSprite.state)) {
                Debug.LogErrorFormat("Cursor sprite {0} already exist", cursorSprite.state);
                continue;
            }
            sprites.Add(cursorSprite.state, cursorSprite.sprite);
        }

        Cursor.lockState = CursorLockMode.Confined;
    }
    private void Start() {
        cursorImage.sprite = defaultCursor;
    }
    private void Update() {
        Cursor.visible = false;
        transform.position = Input.mousePosition;
    }
    private void LateUpdate() {
        cursorImage.sprite = sprites[currentState];
        currentState = CursorState.Default;
    }

    // public static void SetCursorSprite(CursorState state) {
        // if (!instance.sprites.ContainsKey(state)) {
        //     Debug.LogErrorFormat("Cursor state {0} doesn't exist", state);
        //     instance.cursorImage.sprite = instance.defaultCursor;
        //     return;
        // }
        // Sprite sprite = instance.sprites[state];
        // if (sprite == null) { 
        //     instance.cursorImage.sprite = instance.defaultCursor;
        // }
        // else {
        //     instance.cursorImage.sprite = sprite;
        // }
    // }

    // called on update, if not called, mouse with set default
    public static void SetCursorSpriteOnFrame(CursorState state) {
        instance.currentState = state;
    }
    public enum CursorState {
        Default,
        Up, UpRight, Right, DownRight, Down, DownLeft, Left, UpLeft,
        Move
    }

    [System.Serializable]
    public struct CursorSprites {
        public CursorState state;
        public Sprite sprite;
    }
}
