using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildOn : MonoBehaviour {
    public GameObject buildblock;
    public Vector2 tstart;
    private float angle = 0;
    public float height = 7;
    public float distance = 7;
    private float tstartt;
    private State state;
    const float MIN_MULTI_TIME = 1;
    const float MIN_DIST = 50;
    const float MENU_DIST = 10;
    const float ANG_MULT = 0.00005f;
    private bool menuing = false;
    public GameObject BlockSelect;
    public GameObject Target;
    public bool ignore_once;
    // Use this for initialization
    void Start () {
        state = GameObject.FindGameObjectWithTag("Engine").GetComponent<State>();
        BlockSelect.SetActive(false);
	}
	public void BuildOne (Vector2 pos, Quaternion rot) {
        RaycastHit hit;
        var cam = GetComponent<Camera>();
        Ray ray = cam.ScreenPointToRay(pos);

        if (Physics.Raycast(ray, out hit))
        {
            var bpoint = hit.point + 0.5f * hit.normal;
            state.Spawn(buildblock, new IVector3(bpoint), rot);
        }
    }
    public void Erase(Vector2 pos)
    {
        RaycastHit hit;
        var cam = GetComponent<Camera>();
        Ray ray = cam.ScreenPointToRay(pos);

        if (Physics.Raycast(ray, out hit))
        {
            var dbs = hit.collider.gameObject.GetComponent<BlockScript>();
            if (dbs!=null){
                state.Dest(dbs);
            }
        }
    }
    public void Update()
    {
        re_loc(Target);
        if (Input.touchCount > 0)
        {
            if (Input.touchCount == 1 && !state.running)
            {
                var ctouch = Input.GetTouch(0);
                switch (ctouch.phase)
                {
                    case TouchPhase.Began:
                        tstart = ctouch.position;
                        tstartt = Time.time;
                        menuing = tstart.x < MENU_DIST || tstart.x > Screen.width - MENU_DIST;
                        break;
                    case TouchPhase.Ended:
                        if (menuing)
                        {
                            BlockSelect.SetActive(true);
                        }
                        if (ignore_once)
                        {
                            ignore_once = false;
                            return;
                        }
                        if (!BlockSelect.activeInHierarchy)
                        {
                            var dir = ctouch.position - tstart;
                            if (dir.magnitude > MIN_DIST)
                            {
                                var rot = Quaternion.FromToRotation(Vector2.right, dir);
                                var eu = rot.eulerAngles;
                                eu.x = 0;
                                eu.y = -Mathf.Round((eu.z - Mathf.Rad2Deg*angle+180) / 90) * 90;
                                eu.z = 0;
                                rot.eulerAngles = eu;
                                BuildOne(tstart, rot);
                            }
                            else
                            {
                                Erase(tstart);
                            }
                        }
                        break;
                }
            }
            else if (Input.touchCount == 2)
            {
                var ctouch = Input.GetTouch(1);
                switch (ctouch.phase)
                {
                    case TouchPhase.Moved:
                        angle += ctouch.deltaPosition.x * ANG_MULT * Screen.dpi;
                        break;
                    case TouchPhase.Ended:
                        ignore_once = true;
                        break;
                }
            }
        }
    }
    public void re_loc(GameObject floor)
    {
        transform.position = floor.transform.position + new Vector3(distance*Mathf.Sin(angle), height, distance * Mathf.Cos(angle));
        transform.LookAt(floor.transform);
    }
}
