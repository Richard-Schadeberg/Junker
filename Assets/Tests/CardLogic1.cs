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
        Assert.AreNotEqual(Game.S,null);
        ResourceCounter clock = GameObject.Find("Clock").GetComponent<ResourceCounter>();
        Assert.AreNotEqual(clock,null);
        Card pocketResearcher = GameObject.Find("Pocket Researcher").GetComponent<Card>();
        Card solarPanel = GameObject.Find("Solar Panel").GetComponent<Card>();
        Card batteryMaker = GameObject.Find("Battery Maker").GetComponent<Card>();
        Card brushMotor = GameObject.Find("Brush Motor").GetComponent<Card>();
        Card extension = GameObject.Find("Extension").GetComponent<Card>();
        Card drill = GameObject.Find("Drill").GetComponent<Card>();
        Card crucible = GameObject.Find("Crucible").GetComponent<Card>();
        Card wideSolar = GameObject.Find("Wide Solar Panel").GetComponent<Card>();
        Card groundSonar = GameObject.Find("Ground Sonar").GetComponent<Card>();
        //Card = GameObject.Find("").GetComponent<Card>();

        Assert.AreEqual(pocketResearcher.zone,Zone.Hand);
        Assert.AreEqual(solarPanel.zone,Zone.Hand);
        Assert.AreEqual(batteryMaker.zone,Zone.Hand);
        Assert.AreEqual(brushMotor.zone, Zone.Hand);
        Assert.AreEqual(extension.zone, Zone.Hand);
        Assert.AreEqual(drill.zone, Zone.Deck);
        Assert.AreEqual(crucible.zone, Zone.Deck);
        Assert.AreEqual(pocketResearcher.Playability,Playability.Playable);
        Assert.AreEqual(solarPanel.Playability,Playability.Playable);
        Assert.AreEqual(batteryMaker.Playability,Playability.Playable);
        Assert.AreEqual(brushMotor.Playability,Playability.Almost);

        Assert.True(solarPanel.cardComponents.outputIcons[0].brightened);
        Assert.True(solarPanel.cardComponents.outputIcons[1].brightened);
        Assert.False(brushMotor.cardComponents.inputIcons[0].brightened);
        Assert.False(brushMotor.cardComponents.outputIcons[0].brightened);
        Assert.True(pocketResearcher.cardComponents.inputIcons[0].brightened);
        Assert.True(pocketResearcher.cardComponents.inputIcons[1].brightened);
        Assert.True(pocketResearcher.cardComponents.outputIcons[0].brightened);
        Assert.True(pocketResearcher.cardComponents.outputIcons[1].brightened);

        Assert.AreEqual(extension.inputs[0], Resource.Metal);
        Assert.AreEqual(extension.inputs.Length, 1);
        Assert.AreEqual(extension.outputs.Length, 0);
        Assert.False(extension.cardComponents.inputIcons[0].brightened);

        Assert.AreEqual(ZoneTracker.GetCards(Zone.Hand).Length,5);
        Assert.AreEqual(ZoneTracker.GetCards(Zone.Play).Length,0);
        Assert.AreEqual(ZoneTracker.GetCards(Zone.Deck).Length,4);
        foreach (Resource resource in Enum.GetValues(typeof(Resource))) {
            if (resource == Resource.Card) {
                Assert.AreEqual(ResourceTracker.Get(resource),4);
            } else {
                Assert.AreEqual(ResourceTracker.Get(resource),0);
            }
        }

        brushMotor.OnMouseUp();
        Assert.AreEqual(brushMotor.zone, Zone.Hand);
        Assert.False(brushMotor.cardComponents.inputIcons[0].brightened);
        Assert.False(brushMotor.cardComponents.outputIcons[0].brightened);

        solarPanel.OnMouseUp();
        Assert.True(brushMotor.cardComponents.inputIcons[0].brightened);
        Assert.True(brushMotor.cardComponents.outputIcons[0].brightened);
        Assert.True(brushMotor.cardComponents.outputIcons[1].brightened);
        Assert.AreEqual(extension.inputs[0], Resource.Metal);
        Assert.AreEqual(extension.outputs[0], Resource.Electric);
        Assert.AreEqual(extension.inputs.Length, 1);
        Assert.AreEqual(extension.outputs.Length, 1);
        Assert.False(extension.cardComponents.outputIcons[0].brightened);
        batteryMaker.OnMouseUp();
        Assert.AreEqual(extension.inputs[0], Resource.Metal);
        Assert.AreEqual(extension.outputs[0], Resource.Battery);
        Assert.AreEqual(extension.inputs.Length, 1);
        Assert.AreEqual(extension.outputs.Length, 1);
        Assert.AreEqual(ResourceTracker.Get(Resource.Electric),1);
        Assert.AreEqual(ResourceTracker.Get(Resource.Battery),1);
        Assert.AreEqual(solarPanel.zone,Zone.Play);
        Assert.AreEqual(batteryMaker.zone,Zone.Play);
        Assert.AreEqual(ZoneTracker.GetCards(Zone.Hand).Length,3);
        Assert.AreEqual(ZoneTracker.GetCards(Zone.Play).Length,2);
        Assert.AreEqual(ZoneTracker.GetCards(Zone.Deck).Length,4);
        Assert.True(solarPanel.cardComponents.outputIcons[0].brightened);
        Assert.True(batteryMaker.cardComponents.outputIcons[0].brightened);

        brushMotor.OnMouseUp();
        Assert.AreEqual(ResourceTracker.Get(Resource.Electric),0);
        Assert.AreEqual(ResourceTracker.Get(Resource.Battery),1);
        Assert.AreEqual(ResourceTracker.Get(Resource.Distance),2);
        Assert.AreEqual(brushMotor.zone,Zone.Play);
        Assert.AreEqual(ZoneTracker.GetCards(Zone.Hand).Length,3);
        Assert.AreEqual(ZoneTracker.GetCards(Zone.Play).Length,3);
        Assert.AreEqual(ZoneTracker.GetCards(Zone.Deck).Length,4);
        Assert.False(solarPanel.cardComponents.outputIcons[0].brightened);
        Assert.True(batteryMaker.cardComponents.outputIcons[0].brightened);
        Assert.False(brushMotor.cardComponents.inputIcons[0].brightened);
        Assert.True(brushMotor.cardComponents.outputIcons[0].brightened);
        Assert.True(brushMotor.cardComponents.outputIcons[1].brightened);
        Assert.True(pocketResearcher.cardComponents.inputIcons[0].brightened);
        Assert.False(pocketResearcher.cardComponents.inputIcons[1].brightened);
        Assert.False(pocketResearcher.cardComponents.outputIcons[0].brightened);
        Assert.False(pocketResearcher.cardComponents.outputIcons[1].brightened);
        Assert.AreEqual(extension.inputs[1], Resource.Electric);
        Assert.AreEqual(extension.outputs[1], Resource.Distance);
        Assert.AreEqual(extension.inputs.Length, 2);
        Assert.AreEqual(extension.outputs.Length, 2);

        Assert.AreEqual(pocketResearcher.Playability,Playability.Unplayable);
        Assert.AreEqual(brushMotor.tempCopy.Playability,Playability.Playable);

        solarPanel.OnMouseUp();
        Assert.AreEqual(ResourceTracker.Get(Resource.Electric),0);
        Assert.AreEqual(ResourceTracker.Get(Resource.Battery),0);
        Assert.AreEqual(solarPanel.zone,Zone.Hand);
        Assert.AreEqual(batteryMaker.zone,Zone.Hand);
        Assert.AreEqual(ZoneTracker.GetCards(Zone.Hand).Length,5);
        Assert.AreEqual(ZoneTracker.GetCards(Zone.Play).Length,0);
        Assert.AreEqual(ZoneTracker.GetCards(Zone.Deck).Length,4);

        batteryMaker.OnMouseUp();
        solarPanel.OnMouseUp();
        clock.OnMouseDown();
        Assert.AreEqual(ResourceTracker.Get(Resource.Electric),0);
        Assert.AreEqual(ResourceTracker.Get(Resource.Battery),1);
        Assert.AreEqual(solarPanel.zone,Zone.Hand);
        Assert.AreEqual(batteryMaker.zone,Zone.Hand);
        Assert.AreEqual(ZoneTracker.GetCards(Zone.Hand).Length,5);
        Assert.AreEqual(ZoneTracker.GetCards(Zone.Play).Length,0);
        Assert.AreEqual(ZoneTracker.GetCards(Zone.Deck).Length,4);
        Assert.AreEqual(Game.S.clock.turnsRemaining,19);

        Assert.AreEqual(pocketResearcher.Playability,Playability.Playable);
        Assert.AreEqual(solarPanel.Playability,Playability.Playable);
        Assert.AreEqual(batteryMaker.Playability,Playability.Playable);
        Assert.AreEqual(brushMotor.Playability,Playability.Playable);

        solarPanel.OnMouseUp();
        brushMotor.OnMouseUp();
        Card brushCopy1 = brushMotor.tempCopy;
        brushCopy1.OnMouseUp();
        Assert.AreEqual(ResourceTracker.Get(Resource.Battery), 0);
        batteryMaker.OnMouseUp();
        pocketResearcher.OnMouseUp();
        Assert.AreEqual(ResourceTracker.Get(Resource.Electric),0);
        Assert.AreEqual(ResourceTracker.Get(Resource.Battery),1);
        Assert.AreEqual(ResourceTracker.Get(Resource.Distance),4);
        Assert.AreEqual(ResourceTracker.Get(Resource.Card),1);
        Assert.AreEqual(solarPanel.zone,Zone.Play);
        Assert.AreEqual(brushMotor.zone,Zone.Play);
        Assert.AreEqual(brushCopy1.zone,Zone.Play);
        Assert.AreEqual(brushCopy1.tempCopy.zone,Zone.Hand);
        Assert.AreEqual(pocketResearcher.zone,Zone.Hand);
        Assert.AreEqual(ZoneTracker.GetCards(Zone.Hand).Length,3);
        Assert.AreEqual(ZoneTracker.GetCards(Zone.Play).Length,4);
        Assert.AreEqual(ZoneTracker.GetCards(Zone.Deck).Length,4);

        clock.OnMouseDown();
        Assert.AreEqual(ResourceTracker.Get(Resource.Electric),0);
        Assert.AreEqual(ResourceTracker.Get(Resource.Battery),1);
        Assert.AreEqual(solarPanel.zone,Zone.Hand);
        Assert.AreEqual(batteryMaker.zone,Zone.Hand);
        Assert.AreEqual(ZoneTracker.GetCards(Zone.Hand).Length,5);
        Assert.AreEqual(ZoneTracker.GetCards(Zone.Play).Length,0);
        Assert.AreEqual(ZoneTracker.GetCards(Zone.Deck).Length,4);
        Assert.AreEqual(Game.S.clock.turnsRemaining,18);

        batteryMaker.OnMouseUp();
        clock.OnMouseDown();
        Assert.AreEqual(Game.S.clock.turnsRemaining, 17);

        brushMotor.OnMouseUp();
        Assert.AreEqual(ResourceTracker.Get(Resource.Card), 3);
        pocketResearcher.OnMouseUp();
        Assert.AreEqual(ResourceTracker.Get(Resource.Card), 1);
        Debug.Assert(batteryMaker.selectable);
        Debug.Assert(solarPanel.selectable);
        Debug.Assert(!brushMotor.selectable);
        Debug.Assert(!brushMotor.tempCopy.selectable);
        Debug.Assert(!batteryMaker.selected);
        solarPanel.OnMouseUp();
        Debug.Assert(solarPanel.selected);

        brushMotor.OnMouseUp();
        Assert.True(!batteryMaker.selectable);
        Assert.True(!solarPanel.selectable);
        Assert.True(!solarPanel.selected);

        Assert.AreEqual(ResourceTracker.Get(Resource.Card), 4);
        pocketResearcher.OnMouseUp();
        Assert.AreEqual(ResourceTracker.Get(Resource.Card), 2);
        batteryMaker.OnMouseUp();
        solarPanel.OnMouseUp();
        Assert.AreEqual(ResourceTracker.Get(Resource.Electric),0);
        Assert.AreEqual(ResourceTracker.Get(Resource.Battery),2);
        Assert.AreEqual(pocketResearcher.zone,Zone.Play);
        Assert.AreEqual(solarPanel.zone,Zone.Junk);
        Assert.AreEqual(batteryMaker.zone,Zone.Junk);
        Assert.AreEqual(brushMotor.zone,Zone.Hand);
        Assert.AreEqual(ZoneTracker.GetCards(Zone.Hand).Length,4);
        Assert.AreEqual(ZoneTracker.GetCards(Zone.Play).Length,1);
        Assert.AreEqual(ZoneTracker.GetCards(Zone.Deck).Length,2);
        Assert.AreEqual(ZoneTracker.GetCards(Zone.Junk).Length,2);

        pocketResearcher.OnMouseUp();
        Assert.AreEqual(pocketResearcher.zone, Zone.Hand);
        Assert.AreEqual(solarPanel.zone, Zone.Hand);
        Assert.AreEqual(batteryMaker.zone, Zone.Hand);
        Assert.AreEqual(brushMotor.zone, Zone.Hand);
        Assert.AreEqual(ZoneTracker.GetCards(Zone.Hand).Length, 5);
        Assert.AreEqual(ZoneTracker.GetCards(Zone.Play).Length, 0);
        Assert.AreEqual(ZoneTracker.GetCards(Zone.Deck).Length, 4);
        Assert.AreEqual(ZoneTracker.GetCards(Zone.Junk).Length, 0);
        Assert.AreEqual(ResourceTracker.Get(Resource.Card), 4);
        Assert.AreEqual(ResourceTracker.Get(Resource.Battery), 2);

        solarPanel.OnMouseUp();
        batteryMaker.OnMouseUp();
        pocketResearcher.OnMouseUp();
        extension.OnMouseUp();
        batteryMaker.OnMouseUp();
        extension.OnMouseUp();
        drill.OnMouseUp();
        Assert.AreEqual(drill.zone, Zone.Deck);
        solarPanel.OnMouseUp();
        drill.OnMouseUp();
        crucible.OnMouseUp();
        extension.OnMouseUp();
        brushMotor.OnMouseUp();
        brushMotor.tempCopy.OnMouseUp();
        brushMotor.tempCopy.tempCopy.OnMouseUp();
        pocketResearcher.OnMouseUp();
        brushMotor.tempCopy.tempCopy.OnMouseUp();
        brushMotor.OnMouseUp();
        batteryMaker.OnMouseUp();
        pocketResearcher.OnMouseUp();
        brushMotor.OnMouseUp();
        extension.OnMouseUp();
        batteryMaker.OnMouseUp();
        clock.OnMouseDown();

        Assert.AreEqual(Game.S.clock.turnsRemaining, 17);
        Assert.AreEqual(pocketResearcher.zone, Zone.Hand);
        Assert.AreEqual(solarPanel.zone, Zone.Hand);
        Assert.AreEqual(batteryMaker.zone, Zone.Hand);
        Assert.AreEqual(brushMotor.zone, Zone.Hand);
        Assert.AreEqual(extension.zone, Zone.Hand);
        Assert.AreEqual(drill.zone, Zone.Deck);
        Assert.AreEqual(crucible.zone, Zone.Deck);
        Assert.AreEqual(pocketResearcher.Playability, Playability.Playable);
        Assert.AreEqual(solarPanel.Playability, Playability.Playable);
        Assert.AreEqual(batteryMaker.Playability, Playability.Playable);
        Assert.AreEqual(brushMotor.Playability, Playability.Playable);
        Assert.True(solarPanel.cardComponents.outputIcons[0].brightened);
        Assert.True(solarPanel.cardComponents.outputIcons[1].brightened);
        Assert.True(brushMotor.cardComponents.inputIcons[0].brightened);
        Assert.True(brushMotor.cardComponents.outputIcons[0].brightened);
        Assert.True(pocketResearcher.cardComponents.inputIcons[0].brightened);
        Assert.True(pocketResearcher.cardComponents.inputIcons[1].brightened);
        Assert.True(pocketResearcher.cardComponents.outputIcons[0].brightened);
        Assert.True(pocketResearcher.cardComponents.outputIcons[1].brightened);
        Assert.AreEqual(extension.inputs[0], Resource.Metal);
        Assert.AreEqual(extension.inputs.Length, 1);
        Assert.AreEqual(extension.outputs.Length, 0);
        Assert.False(extension.cardComponents.inputIcons[0].brightened);
        Assert.AreEqual(ZoneTracker.GetCards(Zone.Hand).Length, 5);
        Assert.AreEqual(ZoneTracker.GetCards(Zone.Play).Length, 0);
        Assert.AreEqual(ZoneTracker.GetCards(Zone.Deck).Length, 4);

        batteryMaker.OnMouseUp();
        clock.OnMouseDown();

        pocketResearcher.OnMouseUp();
        batteryMaker.OnMouseUp();
        solarPanel.OnMouseUp();
        Assert.AreEqual(ResourceTracker.Get(Resource.Electric), 0);
        Assert.AreEqual(ResourceTracker.Get(Resource.Card), 4);
        Assert.AreEqual(ResourceTracker.Get(Resource.Battery), 3);
        Assert.AreEqual(pocketResearcher.zone, Zone.Play);
        Assert.AreEqual(solarPanel.zone, Zone.Junk);
        Assert.AreEqual(batteryMaker.zone, Zone.Junk);
        Assert.AreEqual(ZoneTracker.GetCards(Zone.Hand).Length, 4);
        Assert.AreEqual(ZoneTracker.GetCards(Zone.Play).Length, 1);
        Assert.AreEqual(ZoneTracker.GetCards(Zone.Deck).Length, 2);
        Assert.AreEqual(ZoneTracker.GetCards(Zone.Junk).Length, 2);

        Assert.AreEqual(brushMotor.zone, Zone.Hand);
        Assert.AreEqual(extension.zone, Zone.Hand);
        Assert.AreEqual(drill.zone, Zone.Hand);
        Assert.AreEqual(crucible.zone, Zone.Hand);

        brushMotor.OnMouseUp();
        drill.OnMouseUp();
        Assert.AreEqual(ResourceTracker.Get(Resource.Fuel), 1);
        crucible.OnMouseUp();
        Assert.AreEqual(ResourceTracker.Get(Resource.Fuel), 0);
        Assert.AreEqual(ResourceTracker.Get(Resource.Battery), 2);
        Assert.AreEqual(ResourceTracker.Get(Resource.Metal), 1);
        Assert.AreEqual(extension.Playability, Playability.Almost);

        crucible.OnMouseUp();
        clock.OnMouseDown();
        Assert.AreEqual(ResourceTracker.Get(Resource.Fuel), 1);
        Assert.AreEqual(ResourceTracker.Get(Resource.Battery), 2);
        Assert.AreEqual(ResourceTracker.Get(Resource.Metal), 0);
        Assert.AreEqual(Game.S.clock.turnsRemaining, 15);

        Assert.AreEqual(brushMotor.zone, Zone.Hand);
        Assert.AreEqual(extension.zone, Zone.Hand);
        Assert.AreEqual(drill.zone, Zone.Hand);
        Assert.AreEqual(crucible.zone, Zone.Hand);
        Assert.AreEqual(pocketResearcher.zone, Zone.Hand);
        Assert.AreEqual(ZoneTracker.GetCards(Zone.Hand).Length, 5);

        brushMotor.OnMouseUp();
        drill.OnMouseUp();
        crucible.OnMouseUp();
        Assert.AreEqual(ResourceTracker.Get(Resource.Fuel), 1);
        Assert.AreEqual(ResourceTracker.Get(Resource.Battery), 1);
        Assert.AreEqual(ResourceTracker.Get(Resource.Metal), 1);
        clock.OnMouseDown();
        Assert.AreEqual(ResourceTracker.Get(Resource.Fuel), 1);
        Assert.AreEqual(ResourceTracker.Get(Resource.Battery), 1);
        Assert.AreEqual(ResourceTracker.Get(Resource.Metal), 1);
        Assert.AreEqual(Game.S.clock.turnsRemaining, 14);
        brushMotor.OnMouseUp();
        drill.OnMouseUp();
        clock.OnMouseDown();
        Assert.AreEqual(ResourceTracker.Get(Resource.Fuel), 2);
        Assert.AreEqual(ResourceTracker.Get(Resource.Battery), 0);
        Assert.AreEqual(ResourceTracker.Get(Resource.Metal), 1);
        Assert.AreEqual(Game.S.clock.turnsRemaining, 13);

        pocketResearcher.OnMouseUp();
        drill.OnMouseUp();
        brushMotor.OnMouseUp();
        Assert.AreEqual(brushMotor.zone, Zone.Junk);
        Assert.AreEqual(drill.zone, Zone.Junk);
        Assert.AreEqual(groundSonar.Playability, Playability.Unplayable);
        Assert.AreEqual(wideSolar.Playability, Playability.Playable);
        crucible.OnMouseUp();
        Assert.AreEqual(extension.Playability, Playability.Playable);
        Assert.AreEqual(ZoneTracker.GetCards(Zone.Play).Length, 2);
        Assert.AreEqual(groundSonar.Playability, Playability.Almost);
        extension.OnMouseUp();
        Assert.AreEqual(groundSonar.Playability, Playability.Playable);
        Assert.AreEqual(ResourceTracker.Get(Resource.Recon), 0);
        groundSonar.OnMouseUp();
        Assert.AreEqual(ResourceTracker.Get(Resource.Recon), 1);
        crucible.OnMouseUp();
        clock.OnMouseDown();
        Assert.AreEqual(brushMotor.zone, Zone.Deck);
        Assert.AreEqual(drill.zone, Zone.Deck);
        Assert.AreEqual(extension.zone, Zone.Hand);
        Assert.AreEqual(crucible.zone, Zone.Hand);
        Assert.AreEqual(pocketResearcher.zone, Zone.Hand);
        Assert.AreEqual(wideSolar.zone, Zone.Hand);
        Assert.AreEqual(groundSonar.zone, Zone.Hand);
        Assert.AreEqual(ZoneTracker.GetCards(Zone.Hand).Length, 5);
        Assert.AreEqual(Game.S.clock.turnsRemaining, 12);

        Assert.AreEqual(extension.Playability, Playability.Playable);
        Assert.AreEqual(crucible.Playability, Playability.Playable);
        Assert.AreEqual(pocketResearcher.Playability, Playability.Playable);
        Assert.AreEqual(wideSolar.Playability, Playability.Playable);
        Assert.AreEqual(groundSonar.Playability, Playability.Unplayable);
        crucible.OnMouseUp();
        extension.OnMouseUp();
        Assert.AreEqual(ZoneTracker.GetCards(Zone.Play).Length, 2);
        Assert.AreEqual(groundSonar.Playability, Playability.Unplayable);
        Assert.AreEqual(wideSolar.Playability, Playability.Playable);
        Assert.AreEqual(pocketResearcher.Playability, Playability.Playable);
        Assert.AreEqual(ZoneTracker.GetCards(Zone.Hand).Length, 3);
        crucible.OnMouseUp();

        pocketResearcher.OnMouseUp();
        wideSolar.OnMouseUp();
        crucible.OnMouseUp();
        Assert.AreEqual(solarPanel.zone, Zone.Hand);
        Assert.AreEqual(batteryMaker.zone, Zone.Hand);
        Assert.AreEqual(groundSonar.Playability, Playability.Unplayable);
        solarPanel.OnMouseUp();
        Assert.AreEqual(groundSonar.Playability, Playability.Almost);
        Assert.AreEqual(extension.outputs[0], Resource.Electric);
        batteryMaker.OnMouseUp();
        Assert.AreEqual(groundSonar.Playability, Playability.Playable);
        groundSonar.OnMouseUp();
        Assert.AreEqual(extension.outputs[0], Resource.Recon);
        Assert.AreEqual(extension.Playability, Playability.Playable);
        extension.OnMouseUp();
        Assert.AreEqual(ResourceTracker.Get(Resource.Fuel), 2);
        Assert.AreEqual(ResourceTracker.Get(Resource.Battery), 1);
        Assert.AreEqual(ResourceTracker.Get(Resource.Metal), 0);
        Assert.AreEqual(ResourceTracker.Get(Resource.Recon), 2);
        Assert.AreEqual(ZoneTracker.GetCards(Zone.Play).Length, 5);
        Assert.AreEqual(ZoneTracker.GetCards(Zone.Hand).Length, 0);

        pocketResearcher.OnMouseUp();
        pocketResearcher.OnMouseUp();
        crucible.OnMouseUp();
        groundSonar.OnMouseUp();
        Assert.AreEqual(solarPanel.zone, Zone.Hand);
        Assert.AreEqual(batteryMaker.zone, Zone.Hand);
        Assert.AreEqual(wideSolar.Playability, Playability.Playable);
        solarPanel.OnMouseUp();
        batteryMaker.OnMouseUp();
        Assert.AreEqual(wideSolar.Playability, Playability.Unplayable);
        batteryMaker.OnMouseUp();
        Assert.AreEqual(wideSolar.Playability, Playability.Playable);
        Assert.AreEqual(extension.Playability, Playability.Playable);
        wideSolar.OnMouseUp();
        Assert.AreEqual(ResourceTracker.Get(Resource.Fuel), 2);
        Assert.AreEqual(ResourceTracker.Get(Resource.Battery), 0);
        Assert.AreEqual(ResourceTracker.Get(Resource.Metal), 1);
        Assert.AreEqual(ResourceTracker.Get(Resource.Electric), 3);
        Assert.AreEqual(extension.Playability, Playability.Unplayable);
        Assert.AreEqual(extension.outputs[0], Resource.Electric);
        Assert.AreEqual(extension.outputs[1], Resource.Electric);


        yield return null;
    }
}
