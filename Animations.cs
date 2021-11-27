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
        public Model[] blocking = new Model[3];
        public Model[] stunned = new Model[3];
        public Model[] currAnim; //current animation
        public Model[] prevAnim; //previous animation for checking if a new one was loaded
        public int currFrame; //frame of current animation
        public int currUpdate; //on which gameupdate the game is (60Hz)


        public Model SetCurrAnim(int type, float animFrameRate)
        {
            prevAnim = currAnim;
            switch (type)
            {
                case 0:
                    currAnim = idle;
                    break;
                case 1:
                    currAnim = walk;
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
            if (type == 5 || type == 4)
            {
                if (currUpdate <= animFrameRate)
                {
                    return currAnim[0];
                } else if (currUpdate > animFrameRate && currUpdate <= animFrameRate * 2)
                {
                    return currAnim[1];
                } else
                {
                    return currAnim[2];
                }
            }
            if (currUpdate <= animFrameRate)
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
