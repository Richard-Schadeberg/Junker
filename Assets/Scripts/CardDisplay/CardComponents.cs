using UnityEngine;
using System.Collections.Generic;
using TMPro;
// helps with card generation boilerplate,
// so that Card objects don't need to assign a bunch of pointers
// in the editor when changing script
public class CardComponents : MonoBehaviour {
    const int maxInputs = 9;
    const int maxOutputs = 9;
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
		int layer = 10+(int)Mathf.Abs(card.GetInstanceID()) % 32000;
		// give each instance its own layer
		card.GetComponent<Renderer>().sortingOrder=layer;
		textCanvas.sortingOrder=layer+1;
		art.sortingOrder=layer+1;
		foreach (GameObject icon in inputIcons) {
			icon.GetComponent<Renderer>().sortingOrder=layer+1;
		}
		foreach (GameObject icon in outputIcons) {
			icon.GetComponent<Renderer>().sortingOrder=layer+1;
		}
	}
	// previewing resources in editor
	public void DrawGizmos(Resource[] inputs,Resource[] outputs,string cardName) {
		int iconNum = 0;
		foreach (Resource input in inputs) {
			MakeResourceGizmo(input,iconNum);
			iconNum++;
		}
		iconNum = resourceGrouping*resourceBoxes.Length/2;
		foreach (Resource output in outputs) {
			MakeResourceGizmo(output,iconNum);
			iconNum++;
		}
	}
	void MakeResourceGizmo(Resource resource,int iconNum) {
		Vector2 size = GetIconSize();
		Vector2 centre = GetIconCentre(iconNum,size);
		size.y = -size.y;
		centre -= size/2;
        Gizmos.DrawGUITexture(new Rect(centre,size), Define.Sprite(resource).texture);
	}
    GameObject[] inputIcons = new GameObject[maxInputs];
    GameObject[] outputIcons = new GameObject[maxOutputs];
	Resource[] displayedInputs = new Resource[maxInputs];
    Resource[] displayedOutputs = new Resource[maxOutputs];
	// update the resource icons to reflect new inputs/outputs
	// inputs array can be longer than maxInputs, they just won't be displayed
	public void DisplayInputsOutputs(Resource[] inputs,Resource[] outputs) {
		// if thereare no icons, generate them
        if (inputIcons[0] == null) MakeIcons();
		for (int i=0; i<maxInputs; i++) {
			// disable icons for unused resource slots
            if (i >= inputs.Length) {
                displayedInputs[i] = Resource.None;
                inputIcons[i].SetActive(false);
			// set sprites
            } else {
				// skip sprite setting if sprite already matches
                if (displayedInputs[i] != inputs[i]) {
                    displayedInputs[i] = inputs[i];
                    inputIcons[i].SetActive(true);
                    inputIcons[i].GetComponent<SpriteRenderer>().sprite = Define.Sprite(inputs[i]);
                }
            }
        }
		for (int i=0; i<maxOutputs; i++) {
            if (i >= outputs.Length) {
                displayedOutputs[i] = Resource.None;
                outputIcons[i].SetActive(false);
            } else {
                if (displayedOutputs[i] != outputs[i]) {
                    displayedOutputs[i] = outputs[i];
                    outputIcons[i].SetActive(true);
                    outputIcons[i].GetComponent<SpriteRenderer>().sprite = Define.Sprite(outputs[i]);
                }
            }
        }
	}
	public void UpdateInOutDarkness(Resource[] inputs,bool isPlayable,Zone zone) {
		// if thereare no icons, generate them
		if (inputIcons[0] == null) MakeIcons();
		// if the part is in play, it should be completely lit up
		if (zone==Zone.Play) {
			SetWhite(inputIcons);
			SetWhite(outputIcons);
			return;
        }
		Dictionary<Resource, int> requiredResources = new Dictionary<Resource, int>();
		for (int i = 0; i < inputs.Length; i++) {
			Resource resource = inputs[i];
			if (!requiredResources.ContainsKey(resource)) requiredResources[resource] = 0;
			requiredResources[resource]++;
			if (requiredResources[resource] > ResourceTracker.Get(resource)) {
				inputIcons[i].GetComponent<SpriteRenderer>().color = Color.gray;
			} else {
				inputIcons[i].GetComponent<SpriteRenderer>().color = Color.white;
			}
        }
		foreach (GameObject outputIcon in outputIcons) {
			outputIcon.GetComponent<SpriteRenderer>().color = isPlayable ? Color.white : Color.gray;
        }
    }
	private void SetWhite(GameObject[] objects) {
		foreach (GameObject obj in objects) {
			obj.GetComponent<SpriteRenderer>().color = Color.white;
        }
    }
    public void MakeIcons() {
		// scaleable copies will already have resource icons
		if (transform.childCount!=0) {
			foreach (Transform child in transform) {
				Destroy(child.gameObject);
			}
		}
        for (int i = 0; i < maxInputs; i++) {
            inputIcons[i] = MakeIcon(i);
			displayedInputs[i] = Resource.None;
        }
        for (int i = 0; i < maxOutputs; i++) {
            outputIcons[i] = MakeIcon(i+maxInputs);
			displayedOutputs[i] = Resource.None;
        }
    }
	GameObject MakeIcon(int iconNum) {
		Vector2 iconSize = GetIconSize();
		Vector2 iconCentre = GetIconCentre(iconNum,iconSize);
		GameObject newIcon = Instantiate(iconPrefab,iconCentre,Quaternion.identity);
		Vector2 spriteSize = newIcon.GetComponent<SpriteRenderer>().bounds.size;
		newIcon.gameObject.transform.localScale = iconSize/spriteSize;
		newIcon.gameObject.transform.SetParent(transform);
        newIcon.SetActive(false);
		return newIcon;
	}
	// check all resourceBoxes and find the largest box that can fit 3 in all of them
	Vector2 GetIconSize() {
		Vector2 size = Vector2.zero;
		foreach (GameObject box in resourceBoxes) {
			Vector2 boxSize = box.GetComponent<SpriteRenderer>().bounds.size;
			if (size == Vector2.zero) {
				size = boxSize;
			}
			size.x = Mathf.Min(size.x,boxSize.x);
			size.y = Mathf.Min(size.y,boxSize.y/resourceGrouping);
		}
		size.x = Mathf.Min(size.x,size.y);
		size.y = size.x;
		return size;
	}
	Vector2 GetIconCentre(int iconNum,Vector2 iconSize) {
		int boxNum = 0;
		while (iconNum >= resourceGrouping) {
			iconNum -= resourceGrouping;
			boxNum++;
		}
		if (boxNum >= resourceBoxes.Length) return Vector2.zero;
		Vector2 boxCentre = resourceBoxes[boxNum].GetComponent<SpriteRenderer>().bounds.center;
		float upshift = iconSize.y * (resourceGrouping-1)/2f;
		float downshift = iconSize.y * iconNum;
		boxCentre.y += upshift - downshift;
		return boxCentre;
	}
}
