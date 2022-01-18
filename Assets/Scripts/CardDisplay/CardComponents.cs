using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class CardComponents : MonoBehaviour {
    const int maxInputs = 9;
    const int maxOutputs = 9;
    public GameObject[] resourceBoxes;
    public int resourceGrouping = 3;
    public Canvas textCanvas;
    public SpriteRenderer art;
    public GameObject iconPrefab;
    public TMP_Text nameBox,descriptionBox;
    public void DisplayName(string name) {
        nameBox.text = name;
    }
    public void DisplayDescription(string description) {
        descriptionBox.text = description;
    }
	public void SetLayers(GameObject card) {
		int layer = 10+(int)Mathf.Abs(card.GetInstanceID()) % 32000;
		// give each instance its own layer
		card.GetComponent<Renderer>().sortingOrder=layer;
		textCanvas.sortingOrder=layer+1;
		art.sortingOrder=layer+1;
		foreach (GameObject icon in inputIcons) {
			icon.GetComponent<Renderer>().sortingOrder=layer+1;
		}
	}
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
		if (nameBox != null) {
			nameBox.SetText(cardName);
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
	public void DisplayInputsOutputs(Resource[] inputs,Resource[] outputs) {
        if (inputIcons[0] == null) MakeIcons();
		for (int i=0; i<maxInputs; i++) {
            if (i >= inputs.Length) {
                displayedInputs[i] = Resource.None;
                inputIcons[i].SetActive(false);
            } else {
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
    public void MakeIcons() {
        for (int i = 0; i < maxInputs; i++) {
            inputIcons[i] = MakeIcon(i);
        }
        for (int i = 0; i < maxOutputs; i++) {
            outputIcons[i] = MakeIcon(i+maxOutputs);
        }
    }
	GameObject MakeIcon(int iconNum) {
		Vector2 iconSize = GetIconSize();
		Vector2 iconCentre = GetIconCentre(iconNum,iconSize);
		GameObject newIcon = Instantiate(iconPrefab,iconCentre,Quaternion.identity);
		SpriteRenderer spriter = newIcon.GetComponent<SpriteRenderer>();
		Vector2 spriteSize = spriter.bounds.size;
		spriter.transform.localScale = iconSize/spriteSize;
		spriter.transform.SetParent(transform);
        newIcon.SetActive(false);
		return newIcon;
	}
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
