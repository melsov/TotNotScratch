using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;
using System;

public class CursorInputClient : MonoBehaviour
{
    private List<Action<VectorXY>> _cursorDown;
    private List<Action<VectorXY>> cursorDown { get {
            if(_cursorDown == null) { _cursorDown = new List<Action<VectorXY>>(); } return _cursorDown;
        }
    }

    private List<Action<VectorXY>> _cursorDrag;
    private List<Action<VectorXY>> cursorDrag {
        get {
            if (_cursorDrag == null) { _cursorDrag = new List<Action<VectorXY>>(); } return _cursorDrag;
        }
    }

    private List<Action<VectorXY>> _cursorUp;
    private List<Action<VectorXY>> cursorUp {
        get {
            if(_cursorUp == null) { _cursorUp = new List<Action<VectorXY>>(); } return _cursorUp;
        }
    }

    public void addDownAction(Action<VectorXY> down) { cursorDown.Add(down); }
    public void addDragAction(Action<VectorXY> drag) { cursorDrag.Add(drag); }
    public void addUpAction(Action<VectorXY> up) { cursorUp.Add(up); }

    public virtual void mouseDown(VectorXY worldPoint) {
        foreach(Action<VectorXY> cursDown in cursorDown) { cursDown.Invoke(worldPoint); }
    }

    public virtual void drag(VectorXY worldPoint) {
        foreach(Action<VectorXY> cursDrag in cursorDrag) { cursDrag.Invoke(worldPoint); }
    }

    public virtual void mouseUp(VectorXY worldPoint) {
        foreach(Action<VectorXY> cursUp in cursorUp) { cursUp.Invoke(worldPoint); }
    }

}


