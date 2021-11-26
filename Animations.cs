using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace INFGame
{
    public class Animations
    {
        public Model[] idle = new Model[2]; //animations as lists of models
        public Model[] walk = new Model[8];
        public Model[] punch;
        public Model[] blockingWalk;
        public Model[] blocking;
        public Model[] stunned;
        public Model[] currAnim; //current animation
        public Model[] prevAnim; //previous animation for checking if a new one was loaded
        public int currFrame; //frame of current animation
        public int currUpdate; //on which gameupdate the game is (60Hz)
        public float animFrameRate; //how many updates are necessary for one frame


        public Model SetCurrAnim(int type)
        {
            prevAnim = currAnim;
            switch (type)
            {
                case 0:
                    currAnim = idle;
                    animFrameRate = 15;
                    break;
                case 1:
                    currAnim = walk;
                    animFrameRate = 15;
                    break;
                case 2:
                    currAnim = punch;
                    break;
                case 3:
                    currAnim = blockingWalk;
                    break;
                case 4:
                    currAnim = blocking;
                    break;
                case 5:
                    currAnim = stunned;
                    break;
            }
            currUpdate++;
            if (currUpdate < animFrameRate)
            {
                currFrame = 0;
            }
            else
            {
                currFrame = (int)Math.Ceiling(currUpdate / animFrameRate);
            }
            if (currAnim != prevAnim || currFrame >= currAnim.Length)
            {
                currFrame = 0;
                currUpdate = 0;
            }
            return currAnim[currFrame];
        }
    }
}
