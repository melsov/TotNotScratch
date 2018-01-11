using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Assertions;

public struct ProbeEventInfo
{
    public Collider2D colldr;
    public bool isEnterEvent;
    public Probe probe;
}

public class Probe : MonoBehaviour
{
    protected Collider2D triggerColldr;
    
    private Collider2D[] contactPoints;
    private ContactFilter2D filter;


    private List<Action<ProbeEventInfo>> triggerEvents = new List<Action<ProbeEventInfo>>();

    private void Awake() {
        contactPoints = new Collider2D[6];
        triggerColldr = GetComponent<Collider2D>();
        triggerColldr.isTrigger = true;
        filter.layerMask = LayerMask.GetMask("GameThingTerrain");
    }

    public void addTriggerEventListener(Action<ProbeEventInfo> a) {
        triggerEvents.Add(a);
    }

    public void removeTriggerEventListener(Action<ProbeEventInfo> a) {
        triggerEvents.Remove(a);
    }

    public IEnumerable<Collider2D> getOverlappingColliders() {
        int count = triggerColldr.OverlapCollider(filter, contactPoints);
        for(int i=0; i< count; ++i) {
            yield return contactPoints[i];
        }
    }

    public bool isOverlappingTerrain() {
        return triggerColldr.IsTouchingLayers(filter.layerMask);
    }

    private void OnTriggerEnter2D(Collider2D collidr) {
        foreach (var listener in triggerEvents) {
            listener(new ProbeEventInfo() { colldr = collidr, isEnterEvent = true, probe = this });
        }
    }

    private void OnTriggerExit2D(Collider2D collidr) {
        foreach(var listener in triggerEvents) {
            listener(new ProbeEventInfo() { colldr = collidr, isEnterEvent = false, probe = this });
        }
    }



}
