using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class CardComponents : MonoBehaviour {
    public GameObject[] resourceBoxes;
    public int resourceGrouping = 3;
    public Canvas textCanvas;
    public SpriteRenderer art;
    public GameObject iconPrefab;
    public TMP_Text nameBox,descriptionBox;
    void Start() {
        for (int i = 0; i < maxInputs + maxOutputs; i++) {
            MakeIcon(Resource.None,i);
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
    const int maxInputs = 9;
    const int maxOutputs = 9;
    GameObject[] inputIcons = new GameObject[maxInputs];
    GameObject[] outputIcons = new GameObject[maxOutputs];
    Resource[] displayedInputs = new Resource[maxInputs];
    Resource[] displayedOutputs = new Resource[maxOutputs];
	public void DisplayIcons(Resource[] inputs,Resource[] outputs) {
		for (int i=0; i<maxInputs; i++) {
            if (i >= inputs.Length) {
                displayedInputs[i] = Resource.None;
            }
        }
	}
	private GameObject MakeIcon(Resource resource,int iconNum) {
		Vector2 iconSize = GetIconSize();
		Vector2 iconCentre = GetIconCentre(iconNum,iconSize);
		GameObject newIcon = Instantiate(iconPrefab,iconCentre,Quaternion.identity);
		SpriteRenderer spriter = newIcon.GetComponent<SpriteRenderer>();
		spriter.sprite = Define.Sprite(resource);
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
