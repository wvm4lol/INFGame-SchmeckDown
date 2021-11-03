using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace INFGame
{
    class Attack
    {
        public int damage; //attack damage
        public float knockbackX; //attack's knockback
        public float knockbackY;
        public float atackermoveX; //movement done to attacker to allow for mid air combos
        public float atackermoveY;
        public float hitBoxWidth;
        public float hitBoxHeight;
        public Vector2 hitBoxTL; //top left corner of hitbox
        public Vector2 hitBoxBR; //bottom right corner of hitbox

        public void CalcAttack(Player attacker, Player defender)
        {
            //WIP
        }
    }
}
