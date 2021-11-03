using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace INFGame
{
    //player class (2 instances created a start), holds player specific info
    public class Player
    {
        public Vector3 position;
        public float health;
        public Matrix matrix;
        public bool onFloor;
        public float speedY = 0;
        public float speedX = 0;
        public Model model;
        public GamePadState gamePadState;
        public float gamePadX;
        public float gamePadY;
        public Vector2 hitBoxTL;
        public Vector2 hitBoxBR;
        public float hitBoxWidth;
        public float hitBoxheight;

        public void SetPlayerHitBox()
        {
            hitBoxTL = new Vector2(position.X - hitBoxWidth / 2, position.Y + hitBoxheight);
            hitBoxBR = new Vector2(position.X + hitBoxWidth / 2, position.Y);
        }
        public float PlayerSpeedX()
        {
            //playere max speed, player acceleration speed and player deacceleration speed
            float speedLim = 0.5f;
            float accelMultiplier = 0.2f;
            float decelMultiplier = 0.05f;
            //if there is input, update speed tto match it
            if (gamePadX != 0)
            {
                speedX = speedX + accelMultiplier * gamePadX;
            }
            else
            {
                //if not, slow down player using decel var
                if (speedX > 0)
                {
                    //if it slows down too far, set speed to 0
                    if (speedX - decelMultiplier < 0)
                    {
                        speedX = 0;
                    }
                    speedX = speedX - decelMultiplier;
                }
                else if (speedX < 0)
                {
                    if (speedX + decelMultiplier > 0)
                    {
                        speedX = 0;
                    }
                    speedX = speedX + decelMultiplier;
                }
            }

            //if the speed is over the limit set the speed to the speed limit
            if (speedX > speedLim)
            {
                speedX = speedLim;
            }
            else if (speedX < -speedLim)
            {
                speedX = -speedLim;
            }
            return speedX;
        }
    }
}
