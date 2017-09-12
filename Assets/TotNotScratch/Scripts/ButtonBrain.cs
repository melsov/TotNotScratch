using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

public class ButtonBrain : GameThing
{

    enum Correctness { NOT_COMPLETE, CORRECT, INCORRECT }

    [SerializeField]
    private Transform buttonParent;

    private GTButton[] buttons {
        get {
            return buttonParent.GetComponentsInChildren<GTButton>();
        }
    }

    public void handleButton(GTButton button) {
        Debug.Log(button.name);
        int index = indexOfButton(button);
        input.Add(index);
        check();
    }

    private void check() {
        Correctness correctness = Correctness.CORRECT;
        for(int i = 0; i < code.Length; ++i) {
            if(i >= input.Count) {
                correctness = Correctness.NOT_COMPLETE;
                break;
            }
            if(input[i] == code[i]) {
                continue;
            } else {
                correctness = Correctness.INCORRECT;
                break;
            }
        }
        decideWhatToDo(correctness);
    }

    private void decideWhatToDo(Correctness correctness) {
        if(correctness == Correctness.INCORRECT) {
            loseAndReset();
        } else if(correctness == Correctness.CORRECT) {
            winAndResest();
        }
        // else do nothing. let the player keep going
    }

    private void winAndResest() {
        announce("Correct!", 2f, reset);
    }

    private void loseAndReset() {
        announce("Wrong", 2f, reset);
    }

    private void reset() {
        input.Clear();
        play("funky-disco-beat");
        announce("Try to remember the code", 2f, () => {
            StartCoroutine(showCode());
        });
    }

    private int indexOfButton(GTButton button) {
        for(int i=0; i<buttons.Length; ++i) {
            if(button == buttons[i]) { return i; }
        }
        return -1;
    }

    private List<int> input = new List<int>();

    private int[] code = new int[] { 0, 1, 1, 1, 0 };

    protected override void LateStart() {
        for(int i=0; i<buttons.Length; ++i) {
            buttons[i].note = i;
        }
        reset();
    }

    private IEnumerator showCode() {
        yield return new WaitForSecondsRealtime(2f);
        foreach(int i in code) {
            buttons[i].highlight(.4f);
            yield return new WaitForSecondsRealtime(.7f);
        }
    }
}
