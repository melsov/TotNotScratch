using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;

[CustomEditor(typeof(GameThing), true)]
public class GameThingDataEditor : Editor
{
    public override void OnInspectorGUI() {
        GameThing gt = (GameThing)target;

        header(gt);

        GUI.backgroundColor = new Color(.8f, 1f, .1f);
        base.OnInspectorGUI();
        EditorGUILayout.EndVertical();
    }

    private void header(GameThing gt) {
        GUI.backgroundColor = Color.white;
        GUIStyle headerStyle = EditorStyles.helpBox;
        string txPath = "Assets/Textures/Tots.png";
        if(gt.GetComponent<SpriteRenderer>()) {
            txPath = AssetDatabase.GetAssetPath(gt.GetComponent<SpriteRenderer>().sprite);
        }
        Texture2D tex = LoadPNG(txPath); 
        if (tex) {
            headerStyle.normal.background = tex;
        } 
        headerStyle.fixedHeight = 44f;
        headerStyle.fixedWidth = headerStyle.fixedHeight;
        EditorGUILayout.BeginVertical(headerStyle);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
        EditorGUILayout.BeginVertical();
    }

    public static Texture2D LoadPNG(string filePath) {

        Texture2D tex = null;
        byte[] fileData;

        if (File.Exists(filePath)) {
            fileData = File.ReadAllBytes(filePath);
            tex = new Texture2D(2, 2, TextureFormat.BGRA32, false);
            tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
        }
        return tex;
    }

}

public class GameThing : MonoBehaviour {

    [SerializeField]
    protected bool dragable;
    private VectorXY mouDownRelativePos;

    [SerializeField, Header("Which keys move this game thing?")]
    private DirectionKeyType directionKeyType;
    private DirectionKeySet directionKeySet;
    private bool useDirectionKeys { get { return directionKeyType != DirectionKeyType.NONE; } }
    [SerializeField, Header("Other keys we want to use")]
    private KeyCode[] otherKeys;

    [SerializeField]
    protected float speed = 4f;

    protected bool isAClone;

    private CursorInputClient _cursorInputClient;
    private CursorInputClient cursorInputClient {
        get {
            if (!_cursorInputClient) {
                _cursorInputClient = ComponentHelper.AddIfNotPresent<CursorInputClient>(transform);
            }
            return _cursorInputClient;
        }
    }

    private Collider2D _colldr;
    protected Collider2D colldr {
        get {
            if (!_colldr) {
                _colldr = GetComponent<Collider2D>();
                if (!_colldr) { _colldr = gameObject.AddComponent<BoxCollider2D>(); }
            }
            return _colldr;
        }
    }

    private Rigidbody2D _rb;
    protected Rigidbody2D rb {
        get {
            if (!_rb) {
                _rb = ComponentHelper.AddIfNotPresent<Rigidbody2D>(transform);
                _rb.isKinematic = true;
            }
            return _rb;
        }
    }

    private SpriteRenderer _srendrr;
    protected SpriteRenderer srendrr {
        get { if (!_srendrr) { _srendrr = ComponentHelper.AddIfNotPresent<SpriteRenderer>(transform); } return _srendrr; }
    }

    private TextMesh _sayTextMesh;
    private bool alreadyTalking;

    [SerializeField]
    private bool wantDBugMessages = true;

    [SerializeField]
    private GameThingPhysicsType _physicsType;

    private TextMesh sayTextMesh {
        get {
            if (!_sayTextMesh) {
                _sayTextMesh = ComponentHelper.FindInChildrenOrAddChildFromResourcesPrefab<TextMesh>(transform, PrefabHelper.PrefabGameThingHelperFolder + "/SayText", colldr.bounds.extents + Vector3.forward * -1f);
            }
            return _sayTextMesh;
        }
    }

    private void Awake() {
        if (useDirectionKeys) {
            directionKeySet = new DirectionKeySet(directionKeyType == DirectionKeyType.WASD);
        }
        awake();
    }

    protected virtual void awake() { }

    private void Start() {
        if (!colldr) {
            _colldr = gameObject.AddComponent<BoxCollider2D>();
        }

        cursorInputClient.addDownAction((VectorXY global) => { _mouseDown(global); });
        cursorInputClient.addDragAction((VectorXY global) => { _mouseDrag(global); });
        cursorInputClient.addUpAction((VectorXY global) => { _mouseUp(global); });

        setLayerWithPhysicsType();

        StartCoroutine(waitThenCallLateStart());
        start();
    }

    protected virtual void start() { }

    private IEnumerator waitThenCallLateStart() {
        yield return new WaitForSeconds(.4f);
        LateStart();
    }

    protected virtual void LateStart() { }

    #region physics

    private void setLayerWithPhysicsType() {
        physicsType = _physicsType;
    }

    protected GameThingPhysicsType physicsType {
        set {
           switch (value) {
                case GameThingPhysicsType.BUMPS_INTO_OTHER_GAMETHINGS:
                    gameObject.layer = LayerMask.NameToLayer("GameThingPhysics");
                    break;
                case GameThingPhysicsType.NO_COLLISIONS:
                    gameObject.layer = LayerMask.NameToLayer("NoCollisions");
                    break;
                case GameThingPhysicsType.NON_OF_THE_ABOVE:
                default:
                    break;
            }
        }
        get {
            if (gameObject.layer == LayerMask.NameToLayer("GameThingPhysics")) {
                return GameThingPhysicsType.BUMPS_INTO_OTHER_GAMETHINGS;
            } else if (gameObject.layer == LayerMask.NameToLayer("NoCollisions")) {
                return GameThingPhysicsType.NO_COLLISIONS;
            }
            return GameThingPhysicsType.NON_OF_THE_ABOVE;
        }
    }

    protected void physicsWorksOnMe(bool yesItDoes) {
        rb.isKinematic = !yesItDoes;
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision) {
        collisionEnterWithSomethingTagged(collision.collider.tag);
    }

    protected virtual void collisionEnterWithSomethingTagged(string tag) { }

    #endregion

    #region helpful-data
    
    protected float time { get { return Time.time; } }

    protected float sinWaveTime(float period) {
        return Mathf.Sin(time * period / (2 * Mathf.PI));
    }

    protected Vector3 position { get { return rb.transform.position; } }
    protected float rotationDegrees { get { return rb.rotation; } }

    #endregion

    #region cursor

    protected Vector3 mousePosition {
        get {
            return CursorHelper.cursorGlobalXY.toVector2;
        }
    }

    private void _mouseDown(VectorXY global) {
        if (dragable) {
            mouDownRelativePos = new VectorXY(transform.position - global.vector3());
        }
        mouseDown(global);
    }

    private void _mouseDrag(VectorXY global) {
        if(dragable) {
            transform.position = (global + mouDownRelativePos).vector3(transform.position.z);
        }
        mouseDrag(global);
    }

    private void _mouseUp(VectorXY global) {
        mouseUp(global);
    }

    protected virtual void mouseDown(VectorXY global) { }

    protected virtual void mouseDrag(VectorXY global) { }

    protected virtual void mouseUp(VectorXY global) { }

    #endregion

    #region keyboard

    private void checkKeys() {
        if(useDirectionKeys) {
            Vector3 mv = Vector3.zero;
            if(Input.GetKey(directionKeySet.up)) {
                mv += Vector3.up;
            } else if (Input.GetKey(directionKeySet.down)) {
                mv += Vector3.down;
            }
            if (Input.GetKey(directionKeySet.right)) {
                mv += Vector3.right;
            } else if (Input.GetKey(directionKeySet.left)) {
                mv += Vector3.left;
            }
            moveInDirection(mv.normalized);
        }
        foreach(KeyCode kc in otherKeys) {
            if(Input.GetKeyDown(kc)) {
                keyDown(kc);
            }
        }
    }

    protected virtual void keyDown(KeyCode kc) {
        if (this is GameThing) {
            dbug("Got key " + kc.ToString() + ". To use this key press you have to make a sub class of GameThing (or use one that you already wrote).");
        } else {
            dbug("You're listening for key: " + kc.ToString() + " but not doing anything with it. override 'keyDown' in " + name + "'s " + GetType() + " by adding: 'protected override void keyDown(KeyCode kc) ...' ");
        }
    }

    protected void sayHi() {
        say("Hi there", 1f);
    }

    protected void say(string words, float forSeconds = 2f, Action whenDone = null) {
        StartCoroutine(sayForSeconds(words, forSeconds, whenDone));
    }

    private IEnumerator sayForSeconds(string words, float forSeconds, Action whenDone) {
        while(alreadyTalking) {
            yield return new WaitForSecondsRealtime(.1f);
        }
        alreadyTalking = true;
        sayTextMesh.gameObject.SetActive(true);
        sayTextMesh.text = words;
        yield return new WaitForSecondsRealtime(forSeconds);
        sayTextMesh.gameObject.SetActive(false);
        alreadyTalking = false;
        if (whenDone != null) {
            whenDone.Invoke();
        }
    }

    protected void announce(string words, float seconds = 2f, Action callback = null) {
        Announcer.Instance.announce(words, seconds, callback);
    }

    /*
     * play an audio file inside of Assets/Resources/Audio/Clip 
     */
    protected void play(string clipPathResourcesAudioClipRelative, Action callback = null) {
        AudioManager.Instance.play(clipPathResourcesAudioClipRelative, callback);
    }

    #endregion

    #region clone

    public void clone() {
        clone(transform.position);
    }

    public void clone(Vector3 global) {
        GameThing cl = Instantiate<GameThing>(this);
        cl.isAClone = true;
    }

    #endregion

    protected virtual void moveInDirection(Vector3 dir) {
        rb.MovePosition(transform.position + dir * speed * Time.deltaTime);
    }

    protected virtual void moveTo(Vector3 global) {
        rb.MovePosition(global);
    }

    protected virtual void boost(Vector2 force, ForceMode2D mode = ForceMode2D.Force) {
        rb.AddForce(force, mode);
    }

    protected void rotate(float byDegrees) {
        rb.MoveRotation(rb.rotation + byDegrees);
    }

    protected void lookAt(Vector3 target) {
        rb.MoveRotation(Angle.angle(target - transform.position));
    }


    protected void changeColor(Color c) {
        srendrr.color = c;
    }

    private void Update() {
        checkKeys();
        update();
    }

    protected virtual void update() { }

    protected void dbug(string s) {
        if(wantDBugMessages) {
            Debug.Log(s);
        }
    }

}

public enum DirectionKeyType { NONE, ARROWS, WASD }

public struct DirectionKeySet
{
    public KeyCode up, down, right, left;

    public DirectionKeySet(bool isWASD) {
        if(isWASD) {
            up = KeyCode.W;
            down = KeyCode.S;
            right = KeyCode.D;
            left = KeyCode.A;
        } else {
            up = KeyCode.UpArrow;
            down = KeyCode.DownArrow;
            right = KeyCode.RightArrow;
            left = KeyCode.LeftArrow;
        }
    }
}

public enum GameThingPhysicsType
{
    NO_COLLISIONS, BUMPS_INTO_OTHER_GAMETHINGS, NON_OF_THE_ABOVE
}
