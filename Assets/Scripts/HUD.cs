using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class HUD
    {
        private Text Lap { get; set; }

        private RawImage ItemImage { get; set; }
        private Dictionary<Item, Texture> ItemTextures { get; set; }

        private RawImage StatusImage1 { get; set; }
        private RawImage StatusImage2 { get; set; }
        private Dictionary<Status, Texture> StatusTextures { get; set; }
        private Animator StatusImage1Controller { get; set; }
        private Animator StatusImage2Controller { get; set; }

        private RawImage InTrackImage { get; set; }
        private RawImage StartImage { get; set; }

        private RawImage PositionImage { get; set; }
        private Dictionary<Position, Texture> PositionTextures { get; set; }

        private RawImage ResultImage { get; set; }
        private Dictionary<Position, Texture> ResultTextures { get; set; }
        
        public HUD()
        {
            Lap = GameObject.Find("Lap").GetComponent<Text>();
            Lap.text = "Lap 0/3";

            ItemImage = GameObject.Find("ItemImage").GetComponent<RawImage>();

            //load item images as textures
            ItemTextures = new Dictionary<Item, Texture>();
            ItemTextures[Item.BOOST] = Resources.Load<Texture>("Items/Shroom");
            ItemTextures[Item.LIGHTNING] = Resources.Load<Texture>("Items/Lightning");
            ItemTextures[Item.REVERSE] = Resources.Load<Texture>("Items/Reverse");

            StatusImage1 = GameObject.Find("StatusImage1").GetComponent<RawImage>();
            StatusImage2 = GameObject.Find("StatusImage2").GetComponent<RawImage>();
            StatusImage1Controller = StatusImage1.GetComponent<Animator>();
            StatusImage2Controller = StatusImage2.GetComponent<Animator>();
            //prevents that the animation is starting, state stays in idle until a new status is received
            StatusImage1Controller.SetBool("StartAnimation", false);
            StatusImage2Controller.SetBool("StartAnimation", false);

            //load status images as textures
            StatusTextures = new Dictionary<Status, Texture>();
            StatusTextures[Status.BOOSTED] = Resources.Load<Texture>("Status/Boosted");
            StatusTextures[Status.SLOWED] = Resources.Load<Texture>("Status/Slowed");
            StatusTextures[Status.REVERSED] = Resources.Load<Texture>("Status/Reverse");

            //Get InTrackImage (WarningTriangle) and disable it
            InTrackImage = GameObject.Find("InTrackImage").GetComponent<RawImage>();

            //Image for countdown when the race starts
            StartImage = GameObject.Find("StartImage").GetComponent<RawImage>();

            //image that shows if the player is 1st or 2nd
            PositionImage = GameObject.Find("PositionImage").GetComponent<RawImage>();
            PositionTextures = new Dictionary<Position, Texture>();
            PositionTextures[Position.FIRST] = Resources.Load<Texture>("Position/1st_black");
            PositionTextures[Position.SECOND] = Resources.Load<Texture>("Position/2nd_black");
            PositionImage.texture = PositionTextures[Position.FIRST];

            //shows Winner/Loser when player finishes
            ResultImage = GameObject.Find("ResultImage").GetComponent<RawImage>();
            ResultTextures = new Dictionary<Position, Texture>();
            ResultTextures[Position.FIRST] = Resources.Load<Texture>("Result/Winner");
            ResultTextures[Position.SECOND] = Resources.Load<Texture>("Result/Loser");
        }


        private bool StartCountdown = true;
        /// <summary>
        /// Update HUD according to argGameState-Properties.
        /// </summary>
        /// <param name="argGameState"></param>
        public void Update(GameState argGameState)
        {
            //set current lap
            Lap.text = "Lap " + argGameState.Lap.ToString() + "/3";

            //start countdown when game starts
            if (argGameState.State == GameStatus.STARTING) 
            {
                //get the Script from the actual ImageObject
                StartScript s = StartImage.GetComponent<StartScript>();
                s.Run = true;
            }

            //show currently held item, if there is one
            if (argGameState.Item == Item.NONE)
            {
                ItemImage.enabled = false;
            }
            else
            {
                ItemImage.texture = ItemTextures[argGameState.Item];
                ItemImage.enabled = true;
            }

            //show current status, if there is one
            if (argGameState.Status1 == Status.NORMAL)
            {
                StatusImage1.enabled = false;
            }
            else
            {
                //only play animation if it's a new status
                if (StatusImage1.texture != StatusTextures[argGameState.Status1] || !StatusImage1.enabled)
                {
                    StatusImage1.texture = StatusTextures[argGameState.Status1];
                    StatusImage1.enabled = true;
                    //start animation always at pulse
                    StatusImage1Controller.Play("Pulse", -1, 0f);
                }
            }

            if (argGameState.Status2 == Status.NORMAL)
            {
                StatusImage2.enabled = false;
            }
            else
            {
                //only play animation if it's a new status
                if (StatusImage2.texture != StatusTextures[argGameState.Status2] || !StatusImage1.enabled)
                {
                    StatusImage2.texture = StatusTextures[argGameState.Status2];
                    StatusImage2.enabled = true;
                    //start animation
                    StatusImage2Controller.Play("Pulse", -1, 0f);
                }
            }


            //enables/disables blinking warning triangle
            InTrackImage.enabled = !argGameState.InTrack;

            //sets the players position 
            PositionImage.texture = PositionTextures[argGameState.Position];

            //shows Winner/Loser when the race is over for the respective player
            if (argGameState.State == GameStatus.FINISHED)
            {
                ResultImage.enabled = true;
                ResultImage.texture = ResultTextures[argGameState.Position];
            } else
            {
                ResultImage.enabled = false;
            }
        }
    }
}
