using UnityEngine;
using UnityEngine.UI;

public class StartScript : MonoBehaviour {

    public bool Run { get; set; }

    private RawImage StartImage { get; set; }

    private Texture[] StartTextures { get; set; }

    private float? StartAnimationTime { get; set; }


    // Use this for initialization
    void Start () {
        StartImage = GameObject.Find("StartImage").GetComponent<RawImage>();

        StartTextures = new Texture[4];
        StartTextures[0] = Resources.Load<Texture>("Start/3");
        StartTextures[1] = Resources.Load<Texture>("Start/2");
        StartTextures[2] = Resources.Load<Texture>("Start/1");
        StartTextures[3] = Resources.Load<Texture>("Start/Go");
    }
	
	// Update is called once per frame
	void Update () {
        if (Run)
        {
            if (StartAnimationTime == null)
            {
                StartImage.texture = StartTextures[0];
                StartImage.enabled = true;
                StartAnimationTime = Time.time;
            }

            if (Time.time >= StartAnimationTime + 1) StartImage.texture = StartTextures[1];
            if (Time.time >= StartAnimationTime + 2) StartImage.texture = StartTextures[2];
            if (Time.time >= StartAnimationTime + 3) StartImage.texture = StartTextures[3];
            if (Time.time >= StartAnimationTime + 5)
            {
                StartImage.enabled = false;
                Run = false;
                StartAnimationTime = null;
            }
        }
    }
}
