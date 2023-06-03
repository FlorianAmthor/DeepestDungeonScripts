using UnityEngine.UI;
using UnityEngine;

namespace WatStudios.DeepestDungeon.UI.Gameplay
{
    public class Compass : MonoBehaviour
    {
        public RawImage CompassImage;
        internal Transform Player;
        public Text CompassDirectionText;

        public void Update()
        {
            //Get a handle on the Image's uvRect
            CompassImage.uvRect = new Rect(Player.localEulerAngles.y / 360, 0, 1, 1);

            // Get a copy of your forward vector
            Vector3 forward = Player.transform.forward;

            // Zero out the y component of your forward vector to only get the direction in the X,Z plane
            forward.y = 0;

            //Clamp our angles to only 5 degree increments
            float headingAngle = Quaternion.LookRotation(forward).eulerAngles.y;
            headingAngle = 5 * (Mathf.RoundToInt(headingAngle / 5.0f));

            //Convert float to int for switch
            //int displayangle;
            //displayangle = Mathf.RoundToInt(headingAngle);

            if (CompassDirectionText != null)
            {
                //Set the text of Compass Degree Text to the clamped value, but change it to the letter if it is a True direction
                switch ((int)headingAngle)
                {
                    case 0:
                        CompassDirectionText.text = "N";
                        break;
                    case 360:                    
                        CompassDirectionText.text = "N";
                        break;
                    case 45:                       
                        CompassDirectionText.text = "NE";
                        break;
                    case 90:                        
                        CompassDirectionText.text = "E";
                        break;
                    case 130:                        
                        CompassDirectionText.text = "SE";
                        break;
                    case 180:                       
                        CompassDirectionText.text = "S";
                        break;
                    case 225:                        
                        CompassDirectionText.text = "SW";
                        break;
                    case 270:                       
                        CompassDirectionText.text = "W";
                        break;
                    default:
                        CompassDirectionText.text = ((int)headingAngle).ToString();
                        break;
                }
            }
        }
    }
}