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
    public Collider2D colldr { get; protected set; }
    
    private Collider2D[] contactPoints;
    private ContactFilter2D filter;

    private List<Action<ProbeEventInfo>> triggerEvents = new List<Action<ProbeEventInfo>>();

    private void Awake() {
        contactPoints = new Collider2D[6];
        colldr = GetComponent<Collider2D>();
        colldr.isTrigger = true;
        filter.layerMask = LayerMask.GetMask("GameThingTerrain");
    }

    public void addTriggerEventListener(Action<ProbeEventInfo> a) {
        triggerEvents.Add(a);
    }

    public void removeTriggerEventListener(Action<ProbeEventInfo> a) {
        triggerEvents.Remove(a);
    }

    public IEnumerable<Collider2D> getOverlappingColliders() {
        int count = colldr.OverlapCollider(filter, contactPoints);
        for(int i=0; i< count; ++i) {
            yield return contactPoints[i];
        }
    }
  

    public bool isOverlappingTerrain() {
        return colldr.IsTouchingLayers(filter.layerMask);
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
