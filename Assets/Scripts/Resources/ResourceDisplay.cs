using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ResourceDisplay {
    public static void Update(Resource resource) {
        switch (resource) {
            case (Resource.Metal):
                string displayString = ResourceTracker.Get(Resource.Metal).ToString() + "/" + ResourceTracker.scrap.ToString();
                Define.S.metalCounter.setText(displayString);
                break;
            default:
                return;
        }
    }
}
