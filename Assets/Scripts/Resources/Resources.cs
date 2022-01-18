using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public static class Resources
{
    public static Dictionary<Resource,int> resourceDictionary = new Dictionary<Resource, int>();
    public static void Reset() {
        resourceDictionary.Clear();
        BuildDictionary();
    }
    public static void BuildDictionary() {
        foreach (Resource resource in Enum.GetValues(typeof(Resource)))
        {
            resourceDictionary.Add(resource,0);
        }
    }
    public static void Add(Resource resource,int amount) {
        if (resourceDictionary.Count==0) BuildDictionary();
        switch (resource)
        {
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
    public static int Get(Resource resource) {
        if (resourceDictionary.Count == 0) BuildDictionary();
        switch (resource)
        {
            case Resource.Card:
                return Game.S.zoneTracker.availableDiscards;
            default:
                return resourceDictionary[resource];
        }
    }
    public static void DrawDiscard(int amount) {
        while (amount > 0)
        {
            Game.S.DrawCard();
            amount--;
        }
        while (amount < 0)
        {
            Game.S.DiscardCard();
            amount++;
        }
    }
}
