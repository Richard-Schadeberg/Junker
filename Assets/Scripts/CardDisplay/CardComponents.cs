using UnityEngine;
using System.Collections.Generic;
using TMPro;
// helps with card generation boilerplate,
// so that Card objects don't need to assign a bunch of pointers
// in the editor when changing script
public class CardComponents : MonoBehaviour {
	// how many resources are grouped together per box
    const int resourceGrouping = 3;
    public GameObject[] resourceBoxes;
    public Canvas textCanvas;
    public SpriteRenderer art;
    public GameObject iconPrefab;
    public TMP_Text nameBox,descriptionBox;
    public string cardName {set {nameBox.text = value;}}
    public string description {set {descriptionBox.text = value;}get {return descriptionBox.text;}}
	// each card needs to be on a randomised layer so that a card can be fully
	// drawn on top of another. 
	// Without this, resource icons from both cards would be visible
	public void SetLayers(GameObject card) {
		const int maxLayer = 32000;
		const int minLayer = 10;
		// a random layer that is probably unique to each card
		int layer = minLayer+(int)Mathf.Abs(card.GetInstanceID()) % (maxLayer-minLayer);
		// backdrop gets the lowest layer
		card.GetComponent<Renderer>().sortingOrder=layer;
		// all other objects on the card are drawn above the backdrop
		// elements are not overlapping
		textCanvas.sortingOrder=layer+1;
		art.sortingOrder=layer+1;
		foreach (ResourceIcon icon in inputIcons) {
			icon.gameObject.GetComponent<Renderer>().sortingOrder=layer+1;
		}
		foreach (ResourceIcon icon in outputIcons) {
			icon.gameObject.GetComponent<Renderer>().sortingOrder=layer+1;
		}
	}
	// previewing resources in editor
	// gizmos are sprites that are only drawn inside the editor
	public void DrawGizmos(Resource[] inputs,Resource[] outputs,string cardName) {
		int iconNum = 0;
		foreach (Resource input in inputs) {
			MakeResourceGizmo(input,iconNum);
			iconNum++;
		}
		// iconNum counts 0->maxInputs for inputs, then maxInputs->(maxInputs+maxOutputs) for outputs
		iconNum = resourceGrouping*resourceBoxes.Length/2;
		foreach (Resource output in outputs) {
			MakeResourceGizmo(output,iconNum);
			iconNum++;
		}
	}
	void MakeResourceGizmo(Resource resource,int iconNum) {
		Vector2 size   = GetIconSize();
		Vector2 centre = GetIconCentre(iconNum,size);
		// gizmos draw upside down compared to iconPrefabs
		size.y = -size.y;
		// gizmos have a different origin compared to iconPrefabs
		centre -= size/2;
		// draw the icon gizmo
        Gizmos.DrawGUITexture(new Rect(centre,size), Define.SpriteFromResource(resource).texture);
	}
	// cards can have their inputs and outputs changed many times mid-game, so changing needs to be optimised
	// instead of destroying and recreating icons, pointers are used to update existing ones
	// this means that a bunch of invisible icons exist ready to change into new icons
	public ResourceIcon[] inputIcons  = new ResourceIcon[Define.maxInputs];
	public ResourceIcon[] outputIcons = new ResourceIcon[Define.maxOutputs];
	// update the resource icons to reflect new inputs/outputs
	// arrays can be any length
	public void DisplayInputsOutputs(Resource[] inputs,Resource[] outputs) {
		// if thereare no icons, generate them
        if (inputIcons[0] == null) MakeIcons();
		for (int i=0; i<Define.maxInputs; i++) {
			// disable icons for unused resource slots
            if (i >= inputs.Length) {
				inputIcons[i].Disable();
			// enable and set sprites
            } else {
				inputIcons[i].Enable(inputs[i]);
            }
        }
		for (int i=0; i< Define.maxOutputs; i++) {
            if (i >= outputs.Length) {
				outputIcons[i].Disable();
            } else {
				outputIcons[i].Enable(outputs[i]);
            }
        }
	}
	// icons in the deck or junk are brightened
	// output icons in play are brightened when they are available to spend
	//  input icons in hand are brightened when you can pay for them
	// output icons in hand are brightened when you can play the card
	// in all other circumstances, they are darkened
	public void UpdateInOutDarkness(Resource[] inputs,bool isPlayable,Zone zone) {
		// no need to change darkness for temporary actions
		if (Game.S.ReversibleMode) return;
		// if thereare no icons, generate them (this is sometimes called before DisplayInputOutputs)
		if (inputIcons[0] == null) MakeIcons();
		// icons in the deck or junk are brightened
		if (zone==Zone.Deck || zone==Zone.Junk) {
			Brighten(inputIcons);
			Brighten(outputIcons);
			return;
		}
		// output icons in play are brightened when they are available to spend
		if (zone == Zone.Play) {
			Darken(inputIcons);
			return;
		}
		Dictionary<Resource, int> requiredResources = new Dictionary<Resource, int>();
		requiredResources[Resource.Battery] = 0;
		// determine which input icons can be paid for
		for (int i = 0; i < inputs.Length; i++) {
			Resource resource = inputs[i];
			// make sure key is in dictionary
			if (!requiredResources.ContainsKey(resource)) requiredResources[resource] = 0;
			requiredResources[resource]++;
			// account for battery -> electric conversion
			if (resource == Resource.Electric && requiredResources[resource] > ResourceTracker.Get(resource) && requiredResources[Resource.Battery] < ResourceTracker.Get(Resource.Battery)) {
				requiredResources[Resource.Electric]--;
				requiredResources[Resource.Battery]++;
			}
			//  input icons in hand are brightened when you can pay for them
			if (requiredResources[resource] <= ResourceTracker.Get(resource)) {
				inputIcons[i].Brighten();
			} else {
				inputIcons[i].Darken();
			}
		}
		// output icons in hand are brightened when you can play the card
		foreach (ResourceIcon outputIcon in outputIcons) {
			if (isPlayable) outputIcon.Brighten(); else outputIcon.Darken();
        }
	}
	private void Brighten(ResourceIcon[] icons) {foreach (ResourceIcon icon in icons) icon.Brighten();}
	private void Darken(ResourceIcon[] icons) {foreach (ResourceIcon icon in icons) icon.Darken();}
	// spawn resource icon objects to display inputs/outputs
	public void MakeIcons() {
		// scaleable copies will already have resource icons as they are cloned recursively, so destroy them first
		if (transform.childCount!=0) {
			foreach (Transform child in transform) {
				Destroy(child.gameObject);
			}
		}
        for (int i = 0; i < Define.maxInputs; i++) {
            inputIcons[i] = MakeIcon(i);
        }
		// the nth output icon corresponds to the "n+maxInputs" input icon
        for (int i = 0; i < Define.maxOutputs; i++) {
            outputIcons[i] = MakeIcon(i+ Define.maxInputs);
        }
    }
	ResourceIcon MakeIcon(int iconNum) {
		Vector2 desiredSize = GetIconSize();
		Vector2 iconCentre = GetIconCentre(iconNum,desiredSize);
		GameObject newIcon = Instantiate(iconPrefab,iconCentre,Quaternion.identity);
		Vector2 currentSize = newIcon.GetComponent<SpriteRenderer>().bounds.size;
		newIcon.gameObject.transform.localScale = desiredSize/currentSize;
		// make it move with the card
		newIcon.gameObject.transform.SetParent(transform);
		ResourceIcon resourceIcon = newIcon.GetComponent<ResourceIcon>();
		resourceIcon.Disable();
		return resourceIcon;
	}
	// check all resourceBoxes and find how large the icon can be while still fitting 3 into each of them
	Vector2 GetIconSize() {
		// for each box, find the largest icon size that fits 3 copies into the box
		// the result is the smallest of these icon sizes
		// track smallest size across loops
		Vector2 minSize = Vector2.zero;
		foreach (GameObject box in resourceBoxes) {
			Vector2 boxSize = box.GetComponent<SpriteRenderer>().bounds.size;
			// minSize can only shrink, so start by making it big to guarantee it shrinks
			if (minSize == Vector2.zero) {
				minSize = boxSize;
			}
			// shrink minSize to fit 3 copies vertically in the given box
			minSize.x = Mathf.Min(minSize.x,boxSize.x);
			minSize.y = Mathf.Min(minSize.y,boxSize.y/resourceGrouping);
		}
		// shrink minSize into a square
		minSize.x = Mathf.Min(minSize.x,minSize.y);
		minSize.y = minSize.x;
		return minSize;
	}
	// for the nth icon, find its position so that each box contains 3 icons touching each other in the centre
	Vector2 GetIconCentre(int iconNum,Vector2 iconSize) {
		// find the relevant box
		int boxNum = 0;
		while (iconNum >= resourceGrouping) {
			// iconNum will be truncated to 0-2
			iconNum -= resourceGrouping;
			boxNum++;
		}
		// start with the centre of the box
		Vector2 boxCentre = resourceBoxes[boxNum].GetComponent<SpriteRenderer>().bounds.center;
		// find how much higher the first icon is above the box's centre
		float upshift   = iconSize.y * (resourceGrouping-1)/2f;
		// find how much lower the nth icon in the box is below the first one in the box
		float downshift = iconSize.y * iconNum;
		// go up to first icon and then down to nth icon
		boxCentre.y += upshift - downshift;
		return boxCentre;
	}
}
