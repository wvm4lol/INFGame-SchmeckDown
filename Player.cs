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
        public PlayerIndex contrIndex; //which controller this player is using
        public int contrlScheme = 0; //if this player is using a controller
        public KeyboardState keybState; //current state of keyboard
        public KeyboardState oldKeybState; //previous keyboard state
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
            //checking what control scheme to use and then checking 
            if (contrlScheme == 0)
            {
                if (gamePadState.Buttons.X != oldGamePadState.Buttons.X && gamePadState.Buttons.X == ButtonState.Pressed) //only allowing an attack if the correct button is pressed and it was not pressed the previous loop
                {
                    currAttack = attacks[0];
                } else if (gamePadState.Buttons.Y != oldGamePadState.Buttons.Y && gamePadState.Buttons.Y == ButtonState.Pressed)
                {
                    currAttack = attacks[1];
                } else
                {
                    return;
                }
            } else if (contrlScheme == 1)
            {
                if (keybState.IsKeyDown(Keys.G) != oldKeybState.IsKeyDown(Keys.G) && keybState.IsKeyDown(Keys.G)) //only allowing an attack if the correct button is pressed and it was not pressed the previous loop
                {
                    currAttack = attacks[0];
                } else if (keybState.IsKeyDown(Keys.H) != oldKeybState.IsKeyDown(Keys.H) && keybState.IsKeyDown(Keys.H))
                {
                    currAttack = attacks[1];
                } else
                {
                    return;
                }
            } else if (contrlScheme == 2)
            {
                if (keybState.IsKeyDown(Keys.NumPad1) != oldKeybState.IsKeyDown(Keys.NumPad1) && keybState.IsKeyDown(Keys.NumPad1))
                {
                    currAttack = attacks[0];
                } else if (keybState.IsKeyDown(Keys.NumPad2) != oldKeybState.IsKeyDown(Keys.NumPad2) && keybState.IsKeyDown(Keys.NumPad2))
                {
                    currAttack = attacks[1];
                } else
                {
                    return;
                }
            }


            //setting the location of the hitboxes of the attack, one for each direction
            if (facing > 0)
            {
                currAttack.hitboxTL = new Vector3(position.X + currAttack.hitboxOffset.X, position.Y + currAttack.hitboxOffset.Y + currAttack.hitboxHeight, 0);
                currAttack.hitboxBR = new Vector3(position.X + currAttack.hitboxOffset.X + currAttack.hitboxWidth, position.Y + currAttack.hitboxOffset.Y, 0);
            } else
            {
                currAttack.hitboxTL = new Vector3(position.X - currAttack.hitboxOffset.X - currAttack.hitboxWidth, position.Y + currAttack.hitboxOffset.Y + currAttack.hitboxHeight, 0);
                currAttack.hitboxBR = new Vector3(position.X - currAttack.hitboxOffset.X, position.Y - currAttack.hitboxOffset.Y, 0);
            }


            //checking if the attack hits the enemy player
            if (currAttack.hitboxTL.X >= defender.hitboxBR.X || currAttack.hitboxBR.X <= defender.hitboxTL.X)
            {
                hit = false;
            } else if (currAttack.hitboxBR.Y >= defender.hitboxTL.Y || currAttack.hitboxTL.Y <= defender.hitboxBR.Y)
            {
                hit = false;
            } else
            {
                //if it hits apply knockback and damage
                hit = true;
                defender.speedX = facing * currAttack.knockbackX;
                defender.speedY = currAttack.knockbackY;

            }
            speedX = facing * currAttack.attackermoveX;
            speedY = currAttack.attackermoveY;

        }

        //get the current state of the players controller
        public void GetControllerState()
        {
            if (contrlScheme == 0)
            {
                //updating controller state, previous controller state and assigning left thumbstick variables for ease of use
                oldGamePadState = gamePadState;
                gamePadState = GamePad.GetState(contrIndex);
                gamePadX = gamePadState.ThumbSticks.Left.X;
                gamePadY = gamePadState.ThumbSticks.Left.Y;
            } else if (contrlScheme == 1)
            {
                //same as other controlscheme but with keyboard
                oldKeybState = keybState;
                keybState = Keyboard.GetState();
                if (keybState.IsKeyDown(Keys.A) && keybState.IsKeyDown(Keys.D)) 
                {
                    gamePadX = 0;
                } else if (keybState.IsKeyDown(Keys.A))
                {
                    gamePadX = -1;
                } else if (keybState.IsKeyDown(Keys.D))
                {
                    gamePadX = 1;
                } else
                {
                    gamePadX = 0;
                }
            } else if (contrlScheme == 2)
            {
                oldKeybState = keybState;
                keybState = Keyboard.GetState();
                if (keybState.IsKeyDown(Keys.Left) && keybState.IsKeyDown(Keys.Right))
                {
                    gamePadX = 0;
                }
                else if (keybState.IsKeyDown(Keys.Left))
                {
                    gamePadX = -1;
                }
                else if (keybState.IsKeyDown(Keys.Right))
                {
                    gamePadX = 1;
                }
                else
                {
                    gamePadX = 0;
                }
            }


        }

        //update player speedX
        public void PlayerMoveX()
        {
            //player max speed, player acceleration speed and player deacceleration speed
            float speedLim = 0.3f;
            float accelMultiplier = 0.1f;
            float decelMultiplierFloor = 0.05f;
            float decelMultiplierAir = speedX * 0.03f;
            //if there is input, update speed to match it
            if (gamePadX != 0)
            {
                speedX = speedX + accelMultiplier * gamePadX;
            }
            else
            {
                //harsher slowdown if on floor
                if (onFloor)
                {
                    //if not, slow down player using decel var
                    if (speedX > 0)
                    {
                        //if it slows down too far, set speed to 0
                        if (speedX - decelMultiplierFloor < 0)
                        {
                            speedX = 0;
                        }
                        speedX = speedX - decelMultiplierFloor;
                    }
                    else if (speedX < 0)
                    {
                        if (speedX + decelMultiplierFloor > 0)
                        {
                            speedX = 0;
                        }
                        speedX = speedX + decelMultiplierFloor;
                    }
                } else
                {
                    //less slow down when in air
                    //if not, slow down player using decel var
                    if (speedX > 0)
                    {
                        //if it slows down too far, set speed to 0
                        if (speedX - decelMultiplierAir < 0)
                        {
                            speedX = 0;
                        }
                        speedX = speedX - decelMultiplierAir;
                    }
                    else if (speedX < 0)
                    {
                        if (speedX + decelMultiplierAir > 0)
                        {
                            speedX = 0;
                        }
                        speedX = speedX + decelMultiplierAir;
                    }
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
        }

        //changing player speed value if player presses A while on floor (used in player collision method)
        public void PlayerMoveY()
        {
            float fallSpeed = 0.04f;
            float jumpSpeed = 0.7f;
            onFloor = position.Y <= 0;
            if (contrlScheme == 0)
            {
                //checking if player is in the air or on the floor, if he is on he floor, set the vertical speed to jump speed (for multiple control schemes)
                if (onFloor)
                {
                    if (gamePadState.Buttons.A == ButtonState.Pressed)
                    {
                        speedY = jumpSpeed;

                    }
                    else
                    {
                        speedY = 0;
                        position.Y = 0;
                    }
                }
                else
                {
                    speedY = speedY - fallSpeed;
                }
            } else if (contrlScheme == 1)
            {
                if (onFloor)
                {
                    if (keybState.IsKeyDown(Keys.W))
                    {
                        speedY = jumpSpeed;

                    }
                    else
                    {
                        speedY = 0;
                        position.Y = 0;
                    }
                }
                else
                {
                    speedY = speedY - fallSpeed;
                }
            } else if (contrlScheme == 2)
            {
                if (onFloor)
                {
                    if (keybState.IsKeyDown(Keys.Up))
                    {
                        speedY = jumpSpeed;

                    }
                    else
                    {
                        speedY = 0;
                        position.Y = 0;
                    }
                }
                else
                {
                    speedY = speedY - fallSpeed;
                }
            }

        }

        //method that checks player collision and updates position 
        public void PlayerCollision(Vector3 arena)
        {
            //adding the horizontal speed to the player position in the X axis
            position += new Vector3(speedX, 0, 0);
            //if the resulting position is outside of tthe arena, put the player on the edge of the arena
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
