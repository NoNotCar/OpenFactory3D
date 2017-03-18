using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartGame : MonoBehaviour {
    public Sprite stop;
    private Sprite play;
    public void Start()
    {
        play = GetComponent<Image>().sprite;
    }
    public void Click()
    {
        var state = GameObject.FindGameObjectWithTag("Engine").GetComponent<State>();
        state.StopStart();
        GetComponent<Image>().sprite = state.running ? stop : play;
    }
}
