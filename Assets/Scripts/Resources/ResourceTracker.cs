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
        switch (resource) {
            case Resource.Card:
                DrawDiscard(amount);
                break;
            default:
                resourceDictionary[resource] += amount;
                break;
        }
    }
    public static void Add(Resource resource) {Add(resource,1);}
    public static void Remove(Resource resource,int amount) {Add(resource,-amount);}
    public static void Remove(Resource resource) {Add(resource,-1);}

    public static int Get(Resource resource) {return Game.S.resourceTracker._Get(resource);}
    public int _Get(Resource resource) {
        switch (resource) {
            case Resource.Card:
                return Game.S.zoneTracker.availableDiscards;
            default:
                return resourceDictionary[resource];
        }
    }
    public static void DrawDiscard(int amount) {
        while (amount > 0) {
            Game.S.DrawCard();
            amount--;
        }
        while (amount < 0) {
            Game.S.DiscardCard();
            amount++;
        }
    }
}
