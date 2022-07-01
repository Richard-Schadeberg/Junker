using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using System;
using System.Linq;
public class NewTestScript {
    [OneTimeSetUp] public void OneTimeSetup() => EditorSceneManager.LoadSceneInPlayMode("Assets/Tests/TestScene1.unity", new LoadSceneParameters(LoadSceneMode.Single));

    [UnityTest]
    public IEnumerator Test1() {
        Debug.Assert(Game.S != null);
        ResourceCounter clock = GameObject.Find("Clock").GetComponent<ResourceCounter>();
        Debug.Assert(clock != null);
        Card pocketResearcher = GameObject.Find("Pocket Researcher").GetComponent<Card>();
        Card solarPanel = GameObject.Find("Solar Panel").GetComponent<Card>();
        Card batteryMaker = GameObject.Find("Battery Maker").GetComponent<Card>();
        Card brushMotor = GameObject.Find("Brush Motor").GetComponent<Card>();
        //Card = GameObject.Find("").GetComponent<Card>();

        Debug.Assert(pocketResearcher.zone == Zone.Hand);
        Debug.Assert(solarPanel.zone == Zone.Hand);
        Debug.Assert(batteryMaker.zone == Zone.Hand);
        Debug.Assert(brushMotor.zone == Zone.Hand);
        Debug.Assert(pocketResearcher.Playability == Playability.Playable);
        Debug.Assert(solarPanel.Playability == Playability.Playable);
        Debug.Assert(batteryMaker.Playability == Playability.Playable);
        Debug.Assert(brushMotor.Playability == Playability.Almost);

        Debug.Assert(ZoneTracker.GetCards(Zone.Hand).Length == 4);
        Debug.Assert(ZoneTracker.GetCards(Zone.Play).Length == 0);
        Debug.Assert(ZoneTracker.GetCards(Zone.Deck).Length == 5);
        foreach (Resource resource in Enum.GetValues(typeof(Resource))) {
            if (resource == (Resource.Card)) {
                Debug.Assert(ResourceTracker.Get(resource) == 3);
            } else {
                Debug.Assert(ResourceTracker.Get(resource) == 0);
            }
        }

        brushMotor.OnMouseUp();
        Debug.Assert(brushMotor.zone == Zone.Hand);

        solarPanel.OnMouseUp();
        batteryMaker.OnMouseUp();
        Debug.Assert(ResourceTracker.Get(Resource.Electric) == 1);
        Debug.Assert(ResourceTracker.Get(Resource.Battery) == 1);
        Debug.Assert(solarPanel.zone == Zone.Play);
        Debug.Assert(batteryMaker.zone == Zone.Play);
        Debug.Assert(ZoneTracker.GetCards(Zone.Hand).Length == 2);
        Debug.Assert(ZoneTracker.GetCards(Zone.Play).Length == 2);
        Debug.Assert(ZoneTracker.GetCards(Zone.Deck).Length == 5);

        brushMotor.OnMouseUp();
        Debug.Assert(ResourceTracker.Get(Resource.Electric) == 0);
        Debug.Assert(ResourceTracker.Get(Resource.Battery) == 1);
        Debug.Assert(ResourceTracker.Get(Resource.Distance) == 2);
        Debug.Assert(brushMotor.zone == Zone.Play);
        Debug.Assert(ZoneTracker.GetCards(Zone.Hand).Length == 2);
        Debug.Assert(ZoneTracker.GetCards(Zone.Play).Length == 3);
        Debug.Assert(ZoneTracker.GetCards(Zone.Deck).Length == 5);

        Debug.Assert(pocketResearcher.Playability == Playability.Unplayable);
        Debug.Assert(brushMotor.tempCopy.Playability == Playability.Playable);

        solarPanel.OnMouseUp();
        Debug.Assert(ResourceTracker.Get(Resource.Electric) == 0);
        Debug.Assert(ResourceTracker.Get(Resource.Battery) == 0);
        Debug.Assert(solarPanel.zone == Zone.Hand);
        Debug.Assert(batteryMaker.zone == Zone.Hand);
        Debug.Assert(ZoneTracker.GetCards(Zone.Hand).Length == 4);
        Debug.Assert(ZoneTracker.GetCards(Zone.Play).Length == 0);
        Debug.Assert(ZoneTracker.GetCards(Zone.Deck).Length == 5);

        batteryMaker.OnMouseUp();
        solarPanel.OnMouseUp();
        clock.OnMouseDown();
        Debug.Assert(ResourceTracker.Get(Resource.Electric) == 0);
        Debug.Assert(ResourceTracker.Get(Resource.Battery) == 1);
        Debug.Assert(solarPanel.zone == Zone.Hand);
        Debug.Assert(batteryMaker.zone == Zone.Hand);
        Debug.Assert(ZoneTracker.GetCards(Zone.Hand).Length == 4);
        Debug.Assert(ZoneTracker.GetCards(Zone.Play).Length == 0);
        Debug.Assert(ZoneTracker.GetCards(Zone.Deck).Length == 5);
        Debug.Assert(Game.S.clock.turnsRemaining == 19);

        Debug.Assert(pocketResearcher.Playability == Playability.Playable);
        Debug.Assert(solarPanel.Playability == Playability.Playable);
        Debug.Assert(batteryMaker.Playability == Playability.Playable);
        Debug.Assert(brushMotor.Playability == Playability.Playable);

        solarPanel.OnMouseUp();
        brushMotor.OnMouseUp();
        Card brushCopy1 = brushMotor.tempCopy;
        brushCopy1.OnMouseUp();
        pocketResearcher.OnMouseUp();
        Debug.Assert(ResourceTracker.Get(Resource.Electric) == 0);
        Debug.Assert(ResourceTracker.Get(Resource.Battery) == 0);
        Debug.Assert(ResourceTracker.Get(Resource.Distance) == 4);
        Debug.Assert(ResourceTracker.Get(Resource.Card) == 1);
        Debug.Assert(solarPanel.zone == Zone.Play);
        Debug.Assert(brushMotor.zone == Zone.Play);
        Debug.Assert(brushCopy1.zone == Zone.Play);
        Debug.Assert(brushCopy1.tempCopy.zone == Zone.Hand);
        Debug.Assert(pocketResearcher.zone == Zone.Hand);
        Debug.Assert(ZoneTracker.GetCards(Zone.Hand).Length == 3);
        Debug.Assert(ZoneTracker.GetCards(Zone.Play).Length == 3);
        Debug.Assert(ZoneTracker.GetCards(Zone.Deck).Length == 5);

        batteryMaker.OnMouseUp();
        clock.OnMouseDown();
        Debug.Assert(ResourceTracker.Get(Resource.Electric) == 0);
        Debug.Assert(ResourceTracker.Get(Resource.Battery) == 1);
        Debug.Assert(solarPanel.zone == Zone.Hand);
        Debug.Assert(batteryMaker.zone == Zone.Hand);
        Debug.Assert(ZoneTracker.GetCards(Zone.Hand).Length == 4);
        Debug.Assert(ZoneTracker.GetCards(Zone.Play).Length == 0);
        Debug.Assert(ZoneTracker.GetCards(Zone.Deck).Length == 5);
        Debug.Assert(Game.S.clock.turnsRemaining == 18);


        pocketResearcher.OnMouseUp();

        yield return null;
    }
}
