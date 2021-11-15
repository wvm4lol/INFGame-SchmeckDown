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
        public int health = 1000; //player health
        public Matrix matrix; //matrix for rendering player
        public Matrix hitbTL; //player hitbox location rendering matrix (top left) (temp)
        public Matrix hitbBR; //(bottom right)
        public Matrix atthitbTL;  //player attack hitbox location rendering matrix (temp)
        public Matrix atthitbBR;
        public bool onFloor; //if player is on the floor/in the air
        public float speedY = 0; //player speed horizontal
        public float speedX = 0; //player speed vertical
        public Model model; //player model
        public GamePadState gamePadState; //player current controller state
        public GamePadState oldGamePadState; //player previous controller state (for checking if button was already pressed or newly pressed)
        public float gamePadX; //controller right joystick x axis
        public float gamePadY; //controller right joystick y axis
        public Vector3 hitboxTL; //player hitbox location (top left)
        public Vector3 hitboxBR; //(bottom right)
        public float hitboxWidth = 1.5f; //player hitbox dimensions
        public float hitboxheight = 2.5f;
        public Attack currAttack = new Attack(); //for loading stats of attack player is currently performing
        public Attack[] attacks; //list of attacks for easy storing/referencing
        public bool hit; //if atack hits (temp)
        public int facing; //direction player is facing (+1 = right, -1 = left)

        //set player hitbox relative to playerpos
        public void SetPlayerHitBox()
        {
            hitboxTL = new Vector3(position.X - hitboxWidth / 2, position.Y + hitboxheight, 0);
            hitboxBR = new Vector3(position.X + hitboxWidth / 2, position.Y, 0);
            hitbTL = Matrix.CreateScale(new Vector3(0.1f, 0.1f, 0.1f)) * Matrix.CreateTranslation(hitboxTL);
            hitbBR = Matrix.CreateScale(new Vector3(0.1f, 0.1f, 0.1f)) * Matrix.CreateTranslation(hitboxBR);
            atthitbTL = Matrix.CreateScale(new Vector3(0.1f, 0.1f, 0.1f)) * Matrix.CreateTranslation(currAttack.hitboxTL);
            atthitbBR = Matrix.CreateScale(new Vector3(0.1f, 0.1f, 0.1f)) * Matrix.CreateTranslation(currAttack.hitboxBR);
        }

        //checks if player is attacking, if it hits, and damages enemy
        public void Attack(Player defender)
        {
            
            //checking which attack was used
            if (gamePadState.Buttons.X != oldGamePadState.Buttons.X && gamePadState.Buttons.X == ButtonState.Pressed)
            {
                currAttack = attacks[0];
            } else if (gamePadState.Buttons.Y != oldGamePadState.Buttons.Y && gamePadState.Buttons.Y == ButtonState.Pressed)
            {
                currAttack = attacks[1];
            } else
            {
                return;
            }

            //checking for hit
            currAttack.hitboxTL = new Vector3(position.X + currAttack.hitboxOffset.X,position.Y + currAttack.hitboxOffset.Y + currAttack.hitboxHeight, 0);
            currAttack.hitboxBR = new Vector3(position.X + currAttack.hitboxWidth + currAttack.hitboxOffset.X,position.Y + currAttack.hitboxOffset.Y, 0);

            if (currAttack.hitboxTL.X >= defender.hitboxBR.X || currAttack.hitboxBR.X <= defender.hitboxTL.X)
            {
                hit = false;
            } else if (currAttack.hitboxBR.Y >= defender.hitboxTL.Y || currAttack.hitboxTL.Y <= defender.hitboxBR.Y)
            {
                hit = false;
            } else
            {
                hit = true;
                defender.speedX = facing * currAttack.knockbackX;
                defender.speedY = facing * currAttack.knockbackY;

            }
            speedX = facing * currAttack.attackermoveX;
            speedY = facing * currAttack.attackermoveY;

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
