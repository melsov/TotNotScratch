using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using UnityEngine.SceneManagement;

/*
 * TODO:
 * DONE: change background / color
 * DONE: gen particles
 * maps and sectors
 * sprite sheet animator?? (that's not a particle system)
 * joints
seek slowly slow turns * or just lookAt with lerp if not already exists
 *  */

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
    private DirectionKeySet directionKeySet;
    private DirectionKeys directionKeys;
    private bool useDirectionKeys { get { return directionKeySet != DirectionKeySet.NONE; } }
    [SerializeField, Header("Which movement style?")]
    private DirectionKeyMovementType directionKeyMovementType;
    [SerializeField, Header("Platformer jump force (value will be multiplied by rb mass)")]
    protected float platformerJumpForce = 100f;


    private DirectionInput mv;

    [SerializeField, Header("Other keys we want to use")]
    private KeyCode[] otherKeys;

    [SerializeField]
    protected float speed = 4f;

    protected bool isAClone;

    protected bool ignoreKeyInput;

    #region lazy-properties

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
                _rb.isKinematic = directionKeyMovementType != DirectionKeyMovementType.PLATFORMER;
            }
            return _rb;
        }
    }

    private SpriteRenderer _srendrr;
    protected SpriteRenderer srendrr {
        get { if (!_srendrr) { _srendrr = ComponentHelper.AddIfNotPresent<SpriteRenderer>(transform); } return _srendrr; }
    }

    private TextMesh _sayTextMesh;
    private TextMesh sayTextMesh {
        get {
            if (!_sayTextMesh) {
                _sayTextMesh = ComponentHelper.FindInChildrenOrAddChildFromResourcesPrefab<TextMesh>(
                    transform, PrefabHelper.PrefabGameThingHelperFolder + "/SayText", 
                    Vector3.Scale(colldr.bounds.extents, new Vector3(.8f, .9f, 0f)) + Vector3.forward * -1f);
            }
            return _sayTextMesh;
        }
    }

    private GTParticleSet _particleSet;
    private GTParticleSet particlesSet {
        get {
            if(_particleSet == null) {
                _particleSet = new GTParticleSet(transform);
            }
            return _particleSet;
        }
    }

    private GTAnimator _gtAnimator;
    protected GTAnimator gtAnimator {
        get {
            if(_gtAnimator == null) {
                _gtAnimator = GetComponent<GTAnimator>();
            }
            return _gtAnimator;
        }
    }

    #endregion


    [SerializeField, Header("Ignore terrain collisions while moving upwards")]
    protected bool ignoreTerrainWhileMovingUp;
    protected Collider2D feet;

    protected GroundedDetector groundedDetector;

    private bool alreadyTalking;

    [SerializeField]
    private bool wantDBugMessages = true;
    private int lastHorizontalMove;
    [SerializeField]
    private float jumpSpeedScaler = .4f;
    private Vector2 lastGroundNormal;
    private float wallBoost;
    [SerializeField, Header("Inclines slow down platformers. setting this >1 will speed them up")]
    private float inclineScaler;

    private void Awake() {
        if (useDirectionKeys) {
            directionKeys = new DirectionKeys(directionKeySet == DirectionKeySet.WASD);
        }
        groundedDetector = GetComponentInChildren<GroundedDetector>();
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

        StartCoroutine(waitThenCallLateStart());
        if(ignoreTerrainWhileMovingUp) {
            feet = findFeet();
            StartCoroutine(ignoreTerrainWhileUp());

        }
        start();
    }

    public Collider2D findFeet() {
        foreach(Collider2D coll in GetComponentsInChildren<Collider2D>()) {
            if (coll.gameObject == gameObject) { continue; }
            if (coll.name.ToLower().Equals("feet")) {
                return coll;
            }
        }
        return colldr;
    }


    protected virtual void start() { }

    private IEnumerator waitThenCallLateStart() {
        yield return new WaitForSeconds(.4f);
        lateStart();
    }

    protected virtual void lateStart() { }

    #region physics

    protected void physicsWorksOnMe(bool yesItDoes) {
        rb.isKinematic = !yesItDoes;
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision) {
        collisionEnterWithSomethingTagged(collision);
    }

    public struct TaggedCollision
    {
        public Collision2D collision;
        public Collider2D collider { get { return collision.collider; } }
        
        public TaggedCollision(Collision2D collision) {
            this.collision = collision;
        }

        public static implicit operator string(TaggedCollision tc) { return tc.collision.collider.tag; }
        public static implicit operator TaggedCollision(Collision2D coll) { return new TaggedCollision(coll); }
    }

    protected virtual void collisionEnterWithSomethingTagged(TaggedCollision tag) { }

    protected GroundedInfo isGrounded {
        get {
            if(!groundedDetector) { return new GroundedInfo(true, Vector2.up); } //If no grounded detector, allow infinite multi-jumps
            return groundedDetector.isGrounded();
        }
    }
    
    private IEnumerator ignoreTerrainWhileUp() {
        int layer = feet.gameObject.layer;
        
        while(true) {
            if(ignoreTerrainWhileMovingUp) {
                if( (LayerMask.NameToLayer("IgnoreTerrain") == feet.gameObject.layer) != rb.velocity.y > .05f) {
                    feet.gameObject.layer = rb.velocity.y > .05f ? LayerMask.NameToLayer("IgnoreTerrain") : layer;
                }
            }
            yield return new WaitForFixedUpdate();
        }
    }

    #endregion

    #region helpful-data
    
    protected float time { get { return Time.time; } }

    protected float sinWaveTime(float period) { return Mathf.Sin(time * period / (2 * Mathf.PI)); }

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
        if(ignoreKeyInput) { return; }
        if(useDirectionKeys) {
            switch(directionKeyMovementType) {
                case DirectionKeyMovementType.PLATFORMER:
                    checkPlatformerKeys();
                    break;
                case DirectionKeyMovementType.NORTH_SOUTH_EAST_WEST:
                    checkNSEWKeys();
                    break;
                case DirectionKeyMovementType.NOT_USING_DIRECTION_KEYS:
                default:
                    break;
            }
        }

        //Other keys
        foreach(KeyCode kc in otherKeys) {
            if(Input.GetKeyDown(kc)) {
                keyDown(kc);
            }
        }
    }

    private void checkNSEWKeys() {
        Vector3 mv = Vector3.zero;
        if (Input.GetKey(directionKeys.up)) {
            mv.y = 1;
        } else if (Input.GetKey(directionKeys.down)) {
            mv.y = -1;
        }
        if (Input.GetKey(directionKeys.right)) {
            mv += Vector3.right;
        } else if (Input.GetKey(directionKeys.left)) {
            mv += Vector3.left;
        }

        moveInDirection(mv.normalized);
    }

    private void checkPlatformerKeys() {
        mv = new DirectionInput();
        if (Input.GetKeyDown(directionKeys.up)) {
            mv.vertical = 1;
        } else if (Input.GetKey(directionKeys.down)) {
            mv.vertical = -1;
        }
        if (Input.GetKey(directionKeys.right)) {
            mv.horiztonal = -1;
        } else if (Input.GetKey(directionKeys.left)) {
            mv.horiztonal = 1;
        }
        movePlatformer(mv);
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
        cl.transform.position = global;
        cl.isAClone = true;
        cl.onJustGotCloned();
    }

    protected virtual void onJustGotCloned() {     }

    #endregion

    #region particles

    protected void addParticles(string particlePrefabName, Vector3 localOffset = default(Vector3)) {
        particlesSet.getOrAddParticles(particlePrefabName, localOffset);
    }

    protected void playParticles(string particleName) { particlesSet.play(particleName); }

    protected void stopParticles(string particleName) { particlesSet.stop(particleName); }

    protected void destroyParticles(string particleName) { particlesSet.destroyParticles(particleName); }

    #endregion

    #region wait-repeated-action

    protected void waitThen(float seconds, Action _action) {
        StartCoroutine(_waitThen(seconds, _action));
    }

    protected void waitRealtimeThen(float seconds, Action _action) {
        StartCoroutine(_waitThen(seconds, _action, true));
    }

    private IEnumerator _waitThen(float seconds, Action _action, bool realtime = false) {
        if (realtime) {
            yield return new WaitForSecondsRealtime(seconds);
        } else {
            yield return new WaitForSeconds(seconds);
        }
        if (_action != null) {
            _action.Invoke();
        }
    }

    //TODO: slow down time for...

    protected void pauseGameFor(float seconds, Action _action = null) {
        Time.timeScale = 0f;
        waitRealtimeThen(Mathf.Max(0f, seconds), () => {
            Time.timeScale = 1f;
            if(_action != null) {
                _action.Invoke();
            }
        });
    }

    protected void repeatAction(float interval, int repeats, Action _action) {
        repeatAction(interval, repeats, _action, false);
    }

    protected void repeatActionForever(float interval, Action _action) {
        repeatAction(interval, -1, _action, false);
    }

    protected void repeatActionWaitInRealTime(float interval, int repeats, Action _action) {
        repeatAction(interval, repeats, _action, true);
    }

    private void repeatAction(float interval, int repeats, Action _action, bool inRealTime) {
        IntervalCallback ic = IntervalCallback.Attach(transform);
        ic.setup(interval, _action, repeats, inRealTime);
        ic.commence();
    }

    #endregion

    #region visible-enable-disable

    /// <summary>
    /// Set false to hide this GameThing while still allowing it
    /// to be active otherwise. True reveals.
    /// </summary>
    protected bool visible {
        get {
            return srendrr.enabled;
        }
        set {
            srendrr.enabled = value;
        }
    }

    /// <summary>
    /// Set false to turn off this GameThing's game object. 
    /// When inactive, it's as though the game thing has been deleted;
    /// the only difference being that this can be undone
    /// by setting active true again; whereas destroyed game objects can't be un-destroyed.
    /// </summary>
    public bool active {
        get {
            return gameObject.activeSelf;
        }
        set {
            gameObject.SetActive(value);
        }
    }

    /// <summary>
    /// Set false to turn off this GameThing component. Unlike with active,
    /// All other components and children, etc., will continue doing whatever they were doing.
    /// </summary>
    public bool isGameThingEnabled {
        get {
            return enabled;
        }
        set {
            enabled = value;
        }
    }

    /// <summary>
    /// Destroy this GameThing's gameObject. Also destroys the gameThing component and all of its children, needless to say.
    /// </summary>
    public void getDestroyed() {
        Destroy(gameObject);
    }

    private void OnDestroy() {
        onWillBeDestroyed();
    }

    protected virtual void onWillBeDestroyed() {     }

    #endregion

    #region levels-scenes

    protected Scene activeScene {
        get {
            return SceneManager.GetActiveScene();
        }
    }

    protected bool isFinalScene {
        get {
            return SceneManager.sceneCountInBuildSettings - 1 == activeScene.buildIndex;
        }
    }

    protected void loadNextScene() {
        if(!isFinalScene) {
            SceneManager.LoadScene(activeScene.buildIndex + 1);
        }
    }

    protected void loadFirstScene() {
        SceneManager.LoadScene(0);
    }

    #endregion

    protected void setBackground(string backgroundInBackgroundsFolderName) { BackgroundManager.Instance.setBackground(backgroundInBackgroundsFolderName); } 

    protected void setBackgroundColor(Color c) { BackgroundManager.Instance.setColor(c); }

    protected void moveForward() { moveInDirection(rb.transform.right); }

    protected virtual void moveInDirection(Vector3 dir) {
        rb.MovePosition(transform.position + dir * speed * Time.deltaTime);
    }

    protected virtual void movePlatformer(DirectionInput mv) {
        GroundedInfo _groundedInfo = isGrounded;
        if (_groundedInfo) {
            lastHorizontalMove = mv.horiztonal;
            lastGroundNormal = _groundedInfo.groundNormal;
            wallBoost = Mathf.Abs(Vector2.Dot(lastGroundNormal, Vector2.right));
            //wallBoost = wallBoost > .7f ? wallBoost : 0f;
        }
        boost((Vector2.up * (_groundedInfo ? 1f : 0f) * mv.vertical * platformerJumpForce + 
            Vector2.right * (_groundedInfo ? 1f + (inclineScaler * Mathf.Min( wallBoost, .6f)) : jumpSpeedScaler * (1 + wallBoost)) * lastHorizontalMove * -1f * speed )
            * rb.mass);
    }

    private void boost(object p) {
        throw new NotImplementedException();
    }

    protected virtual void moveTo(Vector3 global) {
        rb.MovePosition(global);
    }

    protected virtual void lerpTo(Vector3 global, float lerpFactor = 1.4f) {
        rb.MovePosition(Vector3.Lerp(transform.position, global, Mathf.Clamp01(lerpFactor * Time.deltaTime)));
    }

    protected virtual void boost(Vector2 force, ForceMode2D mode = ForceMode2D.Force) {
        rb.AddForce(force, mode);
    }

    protected void rotate(float byDegrees) {
        rb.MoveRotation(rb.rotation + byDegrees);
    }

    protected void lookAt(Vector3 target) { 
        slerpLookAt(target, 1f);
    }

    protected void slerpLookAt(Vector3 target, float slerp) {
        float ang = Angle.angle(Vector3.Slerp(rb.transform.right, (target - transform.position), slerp));
        rb.MoveRotation(ang);
    }



    protected void changeColor(Color c) {
        srendrr.color = c;
    }

    private void Update() {
        checkKeys();
        update();
    }

    protected virtual void update() { }

    private void FixedUpdate() {
        fixedUpdate();
    }

    protected virtual void fixedUpdate() { }

    protected void dbug(string s) {
        if(wantDBugMessages) {
            Debug.Log(s);
        }
    }

    protected struct DirectionInput
    {
        public int vertical, horiztonal;

        public bool isMovingUpwards() {
            return vertical > 0;
        }

        public bool isMovingRight() {
            return horiztonal > 0;
        }

        public bool isMovingLeft() {
            return horiztonal < 0;
        }

        public Vector3 toVector3() { return new Vector3(horiztonal, vertical); }
    }

}

public enum DirectionKeySet { NONE, ARROWS, WASD }

public enum DirectionKeyMovementType { NOT_USING_DIRECTION_KEYS, NORTH_SOUTH_EAST_WEST, PLATFORMER }

public struct DirectionKeys
{
    public KeyCode up, down, right, left;

    public DirectionKeys(bool isWASD) {
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



//public enum GameThingPhysicsType
//{
//    NO_COLLISIONS, BUMPS_INTO_OTHER_GAMETHINGS, NONE_OF_THE_ABOVE
//}
