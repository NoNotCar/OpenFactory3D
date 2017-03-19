using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockMenu : MonoBehaviour {
    public GameObject BlockSelect;
    public void Start()
    {
        BlockSelect.SetActive(false);
    }
    public void Click()
    {
        BlockSelect.SetActive(!BlockSelect.activeInHierarchy);
    }
}
