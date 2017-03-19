using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildOn : MonoBehaviour {
    public GameObject buildblock;
    public Vector2 tstart;
    private Vector2 rlast = Vector2.zero;
    private float angle = 0;
    private float yangle = 0.5f;
    public float distance = 7;
    private float tstartt;
    private State state;
    const float LONG_PRESS_TIME = 0.5f;
    const float MIN_DIST = 50;
    const float ANG_MULT = 0.00005f;
    public IVector3 Target = IVector3.zero;
    public bool ignore_once;
    private bool editor;
    // Use this for initialization
    void Start () {
        state = GameObject.FindGameObjectWithTag("Engine").GetComponent<State>();
        editor = Application.platform == RuntimePlatform.WindowsEditor;
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
                state.Dest(dbs.block);
            }
        }
    }
    public void Change_Cam(Vector2 pos)
    {
        RaycastHit hit;
        var cam = GetComponent<Camera>();
        Ray ray = cam.ScreenPointToRay(pos);

        if (Physics.Raycast(ray, out hit))
        {
            var dbs = hit.collider.gameObject.GetComponent<BlockScript>();
            if (dbs != null)
            {
                Target = new IVector3(dbs.transform.position);
            }
        }
    }
    public void Update()
    {
        re_loc(Target);
        if (!editor)
        {
            if (Input.touchCount > 0)
            {
                if (Input.touchCount == 1)
                {
                    var ctouch = Input.GetTouch(0);
                    switch (ctouch.phase)
                    {
                        case TouchPhase.Began:
                            tstart = ctouch.position;
                            tstartt = Time.time;
                            break;
                        case TouchPhase.Ended:
                            if (ignore_once)
                            {
                                ignore_once = false;
                                return;
                            }
                            on_single_touch(ctouch.position);
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
                            yangle = Mathf.Clamp(yangle- ctouch.deltaPosition.y * ANG_MULT * Screen.dpi, -1, 1);
                            break;
                        case TouchPhase.Ended:
                            ignore_once = true;
                            break;
                    }
                }
            }
        }else
        {
            if (Input.GetMouseButtonDown(0))
            {
                tstart = Input.mousePosition;
                tstartt = Time.time;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (ignore_once)
                {
                    ignore_once = false;
                    return;
                }
                on_single_touch(Input.mousePosition);
            }
            if (Input.GetMouseButtonDown(1))
            {
                rlast = Input.mousePosition;
            }else if (Input.GetMouseButton(1))
            {
                angle += ((Vector2)Input.mousePosition - rlast).x * ANG_MULT * Screen.dpi;
                angle %= Mathf.PI * 2;
                yangle -= ((Vector2)Input.mousePosition - rlast).y * ANG_MULT * Screen.dpi;
                yangle = Mathf.Clamp(yangle, -1, 1);
                rlast = Input.mousePosition;
            }
        }
    }
    public void re_loc(IVector3 pos)
    {
        var zxd = distance * Mathf.Cos(yangle);
        transform.position = pos + new Vector3(zxd*Mathf.Sin(angle), distance*Mathf.Sin(yangle), zxd * Mathf.Cos(angle));
        transform.LookAt(pos);
    }
    public void on_single_touch(Vector2 end_pos)
    {
        var dir = end_pos - tstart;
        if (dir.magnitude > MIN_DIST)
        {
            if (!state.running)
            {
                var rot = Quaternion.FromToRotation(Vector2.right, dir);
                var eu = rot.eulerAngles;
                eu.x = 0;
                eu.y = -Mathf.Round((eu.z - Mathf.Rad2Deg * angle + 180) / 90) * 90;
                eu.z = 0;
                rot.eulerAngles = eu;
                BuildOne(tstart, rot);
            }
        }
        else
        {
            if (Time.time - tstartt > LONG_PRESS_TIME/2)
            {
                Change_Cam(end_pos);
            }
            else if (!state.running)
            {
                Erase(end_pos);
            }
        }
    }
}
