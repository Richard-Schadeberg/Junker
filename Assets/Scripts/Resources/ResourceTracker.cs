using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class ResourceTracker
{
    Dictionary<Resource,int> resourceDictionary = new Dictionary<Resource, int>();
    public ResourceTracker() {
        foreach (Resource resource in Enum.GetValues(typeof(Resource))) {
            resourceDictionary.Add(resource,0);
        }
    }
    public static void Add(Resource resource,int amount) {Game.S.resourceTracker._Add(resource,amount);}
    public void _Add(Resource resource,int amount) {
        if (resource==Resource.Card) {
            Debug.Log("Draw and discard via resourcetracker is ambiguous");
        } else {
            resourceDictionary[resource] += amount;
            Counter counter = Define.Counter(resource);
            if (counter != null) counter.Set(Get(resource));
        }
    }
    public static void Add(Resource resource) {Add(resource,1);}
    public static void Remove(Resource resource,int amount) {Add(resource,-amount);}
    public static void Remove(Resource resource) {Add(resource,-1);}

    public static int Get(Resource resource) {return Game.S.resourceTracker._Get(resource);}
    public int _Get(Resource resource) {
        switch (resource) {
            case Resource.Card:
                return ZoneTracker.availableDiscards - Game.S.discardRequester.pendingRequests;
            default:
                return resourceDictionary[resource];
        }
    }
    public static bool IsValid() {
        foreach (Resource resource in Enum.GetValues(typeof(Resource))) {
            if (Get(resource) < 0) return false;
        }
        return true;
    }
}
