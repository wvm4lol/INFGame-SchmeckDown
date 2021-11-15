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
        public int health = 1000;
        public Matrix matrix;
        public bool onFloor;
        public float speedY = 0;
        public float speedX = 0;
        public Model model;
        public GamePadState gamePadState;
        public float gamePadX;
        public float gamePadY;
        public Vector3 hitboxTL;
        public Vector3 hitboxBR;
        public float hitboxWidth = 1.5f;
        public float hitboxheight = 2.5f;
        public Attack currAttack = new Attack();
        public Attack[] attacks;
        public bool hit;

        //set player hitbox relative to playerpos
        public void SetPlayerHitBox()
        {
            hitboxTL = new Vector3(position.X - hitboxWidth / 2, position.Y + hitboxheight, 0);
            hitboxBR = new Vector3(position.X + hitboxWidth / 2, position.Y, 0);
        }

        //checks if player is attacking, if it hits, and damages enemy
        public void Attack(Player defender)
        {
            
            //checking which attack was used
            if (gamePadState.Buttons.X == ButtonState.Pressed)
            {
                currAttack = attacks[0];
            } else if (gamePadState.Buttons.Y == ButtonState.Pressed)
            {
                currAttack = attacks[1];
            } else
            {
                return;
            }

            //checking for hit
            currAttack.hitboxTL = new Vector3(position.X + currAttack.hitboxOffset.X, position.Y + currAttack.hitboxOffset.Y, 0);
            currAttack.hitboxBR = new Vector3(currAttack.hitboxTL.X + hitboxWidth, currAttack.hitboxTL.Y + hitboxheight, 0);

            if (currAttack.hitboxTL.X >= defender.hitboxBR.X || currAttack.hitboxBR.X <= defender.hitboxTL.X)
            {
                hit = false;
            }
            if (currAttack.hitboxBR.Y >= defender.hitboxTL.Y || currAttack.hitboxTL.Y <= defender.hitboxBR.Y)
            {
                hit = false;
            }
            hit = true;
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
        public void Jump()
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
