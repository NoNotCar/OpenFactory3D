using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockSelect : MonoBehaviour {
    public GameObject block;
    public void Start()
    {
        transform.GetChild(0).GetComponent<Image>().sprite = block.GetComponent<BlockScript>().icon;
    }
    public void Click()
    {
        var bo = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<BuildOn>();
        bo.buildblock = block;
        bo.ignore_once = true;
        transform.parent.gameObject.SetActive(false);
    }
}
