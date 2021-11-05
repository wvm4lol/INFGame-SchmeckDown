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
        public float hitBoxWidth = 1.5f;
        public float hitBoxheight = 2.5f;
        public Attack currentAttack;

        public void SetPlayerHitBox()
        {
            hitBoxTL = new Vector2(position.X - hitBoxWidth / 2, position.Y + hitBoxheight);
            hitBoxBR = new Vector2(position.X + hitBoxWidth / 2, position.Y);
        }

        public void CalcAttack()
        {

        }

        //update player speedX
        public float PlayerSpeedX()
        {
            //player max speed, player acceleration speed and player deacceleration speed
            float speedLim = 0.5f;
            float accelMultiplier = 0.2f;
            float decelMultiplier = 0.05f;
            //if there is input, update speed to match it
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

        //changing player speed value if player presses A while on floor (used in player collision method)
        public void OnFloor()
        {
            onFloor = position.Y <= 0;

            if (onFloor)
            {
                if (gamePadState.Buttons.A == ButtonState.Pressed)
                {
                    speedY = 1;

                }
                else
                {
                    speedY = 0;
                    position.Y = 0;
                }
            }
            else
            {
                speedY = speedY - 0.05f;
            }
        }

        //method that checks player collision and updates position
        public void PlayerCollision(Vector3 arena)
        {
            position += new Vector3(speedX, 0, 0);
            if (speedX > 0 && position.X > 0)
            {
                if (position.X > arena.X)
                {
                    position += new Vector3(arena.X - position.X, 0, 0);
                }
            }
            else if (speedX < 0 && position.X < 0)
            {
                if (position.X < -arena.X)
                {
                    position += new Vector3(-arena.X - position.X, 0, 0);
                }
            }
            position += new Vector3(0, speedY, 0);
        }
    }
}
