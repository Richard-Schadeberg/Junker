using UnityEngine;
using System;
[Serializable]
public class CardProperties {
	// design aspects
	public Resource[] inputs;
	public Resource[] outputs;
	// tools can't be discarded and are always in your starting hand
	// singleUse parts discard themselves after use
	// scaleable cards create a tool copy of themselves whenever they are played
	// they can be played any number of times as long as you can pay their inputs
	public bool isTool, winsGame, singleUse, scaleable;
	// some parts restrict the number of parts you can play this turn (0 for no restriction)
	public int partLimit = 0;
	// some parts can only be installed as the nth part of the turn (0 for no restriction)
	public int requiredPart = 0;
	public string getJSON() { return JsonUtility.ToJson(this); }
	public static CardProperties readJSON(string json) { return JsonUtility.FromJson<CardProperties>(json); }
}