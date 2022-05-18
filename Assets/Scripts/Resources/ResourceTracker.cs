using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
// tracks how much of each resource the player has
public class ResourceTracker {
    Dictionary<Resource,int> resourceDictionary = new Dictionary<Resource, int>();
    public ResourceTracker() {
        foreach (Resource resource in Enum.GetValues(typeof(Resource))) {
            resourceDictionary.Add(resource,0);
        }
    }
    public static void Add(Resource resource,int amount) {
        if (resource==Resource.Card) {
            Debug.Log("Draw and discard via resourcetracker is ambiguous");
        } else {
            Game.S.resourceTracker.resourceDictionary[resource] += amount;
            if (!Game.S.ReversibleMode) Game.GameStateChanged();
        }
    }
    public static void Add(Resource resource) {Add(resource,1);}
    public static void Remove(Resource resource,int amount) {Add(resource,-amount);}
    public static void Remove(Resource resource) {Add(resource,-1);}
    public static int Get(Resource resource) {
        switch (resource) {
            case Resource.Card:
                return ZoneTracker.availableDiscards - Game.S.discardRequester.pendingRequests;
            default:
                return Game.S.resourceTracker.resourceDictionary[resource];
        }
    }
    public static void Reset(Resource resource) { Game.S.resourceTracker.resourceDictionary[resource] = 0; }
    public static bool IsLegal() {
        foreach (Resource resource in Enum.GetValues(typeof(Resource))) {
            if (Get(resource) < 0) return false;
        }
        return true;
    }
    public static int scrap {
        get { return Game.S.resourceTracker._scrap; }
        set { 
            Game.S.resourceTracker._scrap = value;
            ResourceDisplay.Update(Resource.Metal);
        }
    }
    public int _scrap;
}
