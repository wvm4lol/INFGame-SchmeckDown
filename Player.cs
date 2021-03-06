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
        public int health = 100; //player health
        public Matrix matrix; //matrix for rendering player
        public bool onFloor; //if player is on the floor/in the air
        public float speedY = 0; //player speed horizontal
        public float speedX = 0; //player speed vertical
        public Model model; //player model
        public int animFrameRate; //var to pass trough for setting animation speed, i just want the pain of animations to stop
        public Animations animations = new Animations();//object containing model and animation

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

        public float hitboxWidth = 3f; //player hitbox dimensions
        public float hitboxheight = 6.5f;

        public Attack currAttack = new Attack(); //for loading stats of attack player is currently performing
        public Attack[] attacks; //list of attacks for easy storing/referencing
        public float currAttackFrameRate; //frame rate of current attack, i really just want this to sttop so im just making new variables for everything
        public bool hit; //if attack hits (temp)
        public int facing; //direction player is facing (+1 = right, -1 = left)
        public bool blocking; //if player is blocking (no damage taken and less knockback, but can be broken to stun you)
        public int inAction; //if player is still doing something else, used to block animation canceling
        public int stunned; //updates player is frozen/stunned for, 0 = not stunned
        public int attackTimer; //timer starts at button input, 0 when attack should be done




        //set player hitbox relative to playerpos
        public void SetPlayerHitBox()
        {
            hitboxTL = new Vector3(position.X + facing - hitboxWidth / 2, position.Y + hitboxheight, 0);
            hitboxBR = new Vector3(position.X + facing + hitboxWidth / 2, position.Y, 0);
        }

        //checks if player is attacking (or blocking), if it hits, and damages enemy
        public void Attack(Player defender)
        {
            float inAirAttackOffset;
            if (!onFloor)
            {
                inAirAttackOffset = 3;
            }
            else
            {
                inAirAttackOffset = 0;
            }
            if (stunned > 0)
            {
                return;
            }
            if (inAction > 0)
            {
                inAction--;
                attackTimer--;
                if (attackTimer == 0)
                {
                    //setting the location of the hitboxes of the attack, one for each direction
                    if (facing == 1)
                    {
                        currAttack.hitboxTL = new Vector3(position.X + currAttack.hitboxOffset.X, position.Y + currAttack.hitboxOffset.Y + currAttack.hitboxHeight - inAirAttackOffset, 0);
                        currAttack.hitboxBR = new Vector3(position.X + currAttack.hitboxOffset.X + currAttack.hitboxWidth, position.Y + currAttack.hitboxOffset.Y - inAirAttackOffset, 0);
                    }
                    else
                    {
                        currAttack.hitboxTL = new Vector3(position.X - currAttack.hitboxOffset.X - currAttack.hitboxWidth, position.Y + currAttack.hitboxOffset.Y + currAttack.hitboxHeight - inAirAttackOffset, 0);
                        currAttack.hitboxBR = new Vector3(position.X - currAttack.hitboxOffset.X, position.Y + currAttack.hitboxOffset.Y - inAirAttackOffset, 0);
                    }


                    //checking if the attack hits the enemy player
                    if (currAttack.hitboxTL.X >= defender.hitboxBR.X || currAttack.hitboxBR.X <= defender.hitboxTL.X)
                    {
                        hit = false;
                    }
                    else if (currAttack.hitboxBR.Y >= defender.hitboxTL.Y || currAttack.hitboxTL.Y <= defender.hitboxBR.Y)
                    {
                        hit = false;
                    }
                    else
                    {
                        hit = true;

                        if (!defender.blocking)
                        {
                            //if it hits apply knockback and damage, unblocked version
                            defender.speedX = facing * currAttack.knockbackX;
                            defender.speedY = currAttack.knockbackY;
                            defender.health = defender.health - currAttack.damage;
                        }
                        else
                        {
                            if (currAttack.guardBreak)
                            {
                                defender.stunned = 60;
                                defender.health = defender.health - currAttack.damage;
                                defender.speedX = facing * currAttack.knockbackX;
                                defender.speedY = currAttack.knockbackY;
                            }
                            else
                            {
                                //if it hits apply knockback and damage, blocked version
                                defender.speedX = 0.6f * facing * currAttack.knockbackX;
                                defender.speedY = 0.6f * currAttack.knockbackY;
                            }
                        }
                    }
                }
                model = animations.SetCurrAnim(2, currAttackFrameRate); //punch animation
                return;
            }
            //checking what control scheme to use and then checking 
            if (contrlScheme == 0)
            {
                if (gamePadState.Triggers.Right <= 0.7f)
                {
                    blocking = false;
                    if (gamePadState.Buttons.X != oldGamePadState.Buttons.X && gamePadState.Buttons.X == ButtonState.Pressed) //only allowing an attack if the correct button is pressed and it was not pressed the previous loop
                    {
                        currAttack = attacks[0];
                        currAttackFrameRate = 2;
                    }
                    else if (gamePadState.Buttons.Y != oldGamePadState.Buttons.Y && gamePadState.Buttons.Y == ButtonState.Pressed)
                    {
                        currAttack = attacks[1];
                        currAttackFrameRate = 5;

                    }
                    else if (gamePadState.Buttons.B != oldGamePadState.Buttons.B && gamePadState.Buttons.B == ButtonState.Pressed)
                    {
                        currAttack = attacks[2];
                        currAttackFrameRate = 10;
                    }
                    else
                    {
                        return;
                    }
                } else
                {
                    Block();
                    return;
                }

            } else if (contrlScheme == 1)
            {
                if (keybState.IsKeyUp(Keys.Space))
                {
                    blocking = false;
                    if (keybState.IsKeyDown(Keys.G) != oldKeybState.IsKeyDown(Keys.G) && keybState.IsKeyDown(Keys.G)) //only allowing an attack if the correct button is pressed and it was not pressed the previous loop
                    {
                        currAttack = attacks[0];
                        currAttackFrameRate = 2;

                    }
                    else if (keybState.IsKeyDown(Keys.H) != oldKeybState.IsKeyDown(Keys.H) && keybState.IsKeyDown(Keys.H))
                    {
                        currAttack = attacks[1];
                        currAttackFrameRate = 5;

                    }
                    else if (keybState.IsKeyDown(Keys.J) != oldKeybState.IsKeyDown(Keys.J) && keybState.IsKeyDown(Keys.J))
                    {
                        currAttack = attacks[2];
                        currAttackFrameRate = 10;

                    }
                    else
                    {
                        return;
                    }
                } else
                {
                    Block();
                    return;
                }

            } else if (contrlScheme == 2)
            {
                if (keybState.IsKeyUp(Keys.NumPad0))
                {
                    blocking = false;
                    if (keybState.IsKeyDown(Keys.NumPad1) != oldKeybState.IsKeyDown(Keys.NumPad1) && keybState.IsKeyDown(Keys.NumPad1))
                    {
                        currAttack = attacks[0];
                        currAttackFrameRate = 2;

                    }
                    else if (keybState.IsKeyDown(Keys.NumPad2) != oldKeybState.IsKeyDown(Keys.NumPad2) && keybState.IsKeyDown(Keys.NumPad2))
                    {
                        currAttack = attacks[1];
                        currAttackFrameRate = 5;
                    }
                    else if (keybState.IsKeyDown(Keys.NumPad3) != oldKeybState.IsKeyDown(Keys.NumPad3) && keybState.IsKeyDown(Keys.NumPad3))
                    {
                        currAttack = attacks[2];
                        currAttackFrameRate = 10;

                    }
                    else
                    {
                        return;
                    }
                } else
                {
                    Block();
                    return;
                }
            }
            inAction = 6 * (int) currAttackFrameRate;
            attackTimer = 5 * (int) currAttackFrameRate;
            model = animations.SetCurrAnim(2, currAttackFrameRate); //punch animation



            speedX = facing * currAttack.attackermoveX;
            speedY = currAttack.attackermoveY;

            void Block()
            {
                blocking = true;
            }
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
            if (stunned > 0)
            {
                stunned--;
                model = animations.SetCurrAnim(5, 20); //stunned animation
                gamePadX = 0;
                gamePadY = 0;
            }

        }

        //update player speedX
        public void PlayerMoveX()
        {
            //player max speed, player acceleration speed and player deacceleration speed
            float maxSpeed = 0.2f; //speed limit you can change 
            float speedLim; //speed limit used in calculations
            float accelMultiplier = 0.1f;
            float decelMultiplierFloor = 0.05f;
            float decelMultiplierAir = speedX * 0.03f;
            float blockSlow;
            if (blocking)
            {
                blockSlow = 0.4f;
                speedLim = maxSpeed * blockSlow;
            } else if (inAction > 0)
            {
                blockSlow = 0.2f;
                speedLim = maxSpeed * 0.2f;
            } else
            {
                blockSlow = 1;
                speedLim = maxSpeed;
            }
            //if there is input, update speed to match it
            if (gamePadX != 0)
            {
                if (blocking)
                {
                    model = animations.SetCurrAnim(4, 15); //blocking animation

                    //animations.SetCurrAnim(3. 10); //blocking walk animation
                }
                else
                {
                    if (inAction <= 0)
                    {
                        model = animations.SetCurrAnim(1, 6); //walk animation
                    }
                }
                speedX = speedX + accelMultiplier * blockSlow * gamePadX;
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
                    else if (speedX == 0)
                    {
                        if (blocking)
                        {
                            model = animations.SetCurrAnim(4, 15); //blocking animation
                        }
                        else
                        {
                            model = animations.SetCurrAnim(0, 15); //idle animation
                        }
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
            float fallSpeed = 0.03f;
            float jumpSpeed = 0.7f;
            onFloor = position.Y <= 0;
            if (contrlScheme == 0)
            {
                //checking if player is in the air or on the floor, if he is on he floor, set the vertical speed to jump speed (for multiple control schemes)
                if (onFloor)
                {
                    if (gamePadState.Buttons.A == ButtonState.Pressed && stunned <= 0 && inAction <= 0)
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
                    if (keybState.IsKeyDown(Keys.W) && stunned <= 0 && inAction <= 0)
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
                    if (keybState.IsKeyDown(Keys.Up) && stunned <= 0 && inAction <= 0)
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

        public void PlayerAnimation()
        {

        }
    }
}
