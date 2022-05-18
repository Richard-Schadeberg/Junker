using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class IconTracker{
    public Dictionary<Resource, ResourceSources> installedOutputs = new Dictionary<Resource, ResourceSources>();
    public static void Reset() {Game.S.iconTracker.installedOutputs = new Dictionary<Resource, ResourceSources>();}
    public static void OutputInstalled(Resource resource,ResourceIcon resourceIcon) {
        Dictionary<Resource, ResourceSources> dict = Game.S.iconTracker.installedOutputs;
        if (!dict.ContainsKey(resource)) dict[resource] = new ResourceSources(resource);
        dict[resource].OutputInstalled(resourceIcon);
    }
    public static void OutputUninstalled(Resource resource) {if (Game.S.iconTracker.installedOutputs.ContainsKey(resource)) Game.S.iconTracker.installedOutputs[resource].OutputUninstalled();}
    public static void OutputConsumed(Resource resource) {if (Game.S.iconTracker.installedOutputs.ContainsKey(resource)) Game.S.iconTracker.installedOutputs[resource].OutputConsumed();}
    public static void OutputReturned(Resource resource) {if (Game.S.iconTracker.installedOutputs.ContainsKey(resource)) Game.S.iconTracker.installedOutputs[resource].OutputReturned(); }

    public class ResourceSources {
        private int numConsumed = 0;
        private List<ResourceIcon> resourceIcons = new List<ResourceIcon>();
        private Resource resource;
        public ResourceSources(Resource resource_) { resource = resource_; }
        public void OutputInstalled(ResourceIcon resourceIcon) {
            if (resource == Resource.Card) {
                resourceIcon.Darken();
            } else {
                resourceIcons.Add(resourceIcon);
                resourceIcon.Brighten();
            }
        }
        public void OutputUninstalled() {
            if (resource == Resource.Card) return;
            resourceIcons.RemoveAt(resourceIcons.Count - 1);
        }
        public void OutputConsumed() {
            if (resource == Resource.Card) return;
            if (numConsumed <= resourceIcons.Count) return;
            resourceIcons[numConsumed].Darken();
            numConsumed++;
        }
        public void OutputReturned() {
            if (resource == Resource.Card) return;
            if (numConsumed == 0) return;
            numConsumed--;
            resourceIcons[numConsumed].Brighten();
        }
    }
}