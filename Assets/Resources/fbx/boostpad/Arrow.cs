using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour {
   
    private Renderer Renderer;
    private Material ActiveArrow;
    private Material NonActiveArrow;
    private string ParentName;
    private static float? StartTime;

    // Use this for initialization
    void Start()
    {
        Renderer = GetComponent<Renderer>();
        ActiveArrow = Resources.Load<Material>("fbx/boostpad/Materials/ActiveArrow");
        NonActiveArrow = Resources.Load<Material>("fbx/boostpad/Materials/NonActiveArrow");
        if (StartTime == null) StartTime = Time.time;
        ParentName = this.transform.parent.parent.name;
    }

    // Update is called once per frame
    void Update () {
        float currentTime = Time.time;

        if (currentTime >= StartTime && currentTime <= StartTime + 0.32)
        {
            if (ParentName.Equals("Arrow_1"))
            {
                Renderer.material = ActiveArrow;
            }
            else
            {
                Renderer.material = NonActiveArrow;
            }
        }

        if (currentTime >= StartTime + 0.33 && currentTime <= StartTime + 0.65)
        {
            if (ParentName.Equals("Arrow_2"))
            {
                Renderer.material = ActiveArrow;
            }
            else
            {
                Renderer.material = NonActiveArrow;
            }
        }

        if (currentTime >= StartTime + 0.66 && currentTime <= StartTime + 0.99)
        {
            if (ParentName.Equals("Arrow_3"))
            {
                Renderer.material = ActiveArrow;
            }
            else
            {
                Renderer.material = NonActiveArrow;
            }
        } 


        if (currentTime >= StartTime + 1) {
            StartTime = Time.time;
        }
    }
}
