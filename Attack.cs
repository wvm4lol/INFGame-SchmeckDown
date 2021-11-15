using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace INFGame
{
    public class Attack
    {
        public int damage; //attack damage
        public float knockbackX; //attack's knockback
        public float knockbackY;
        public float attackermoveX; //movement done to attacker to allow for mid air combos
        public float attackermoveY;
        public float hitboxWidth;
        public float hitboxHeight;
        public Vector3 hitboxOffset; //location of top left corner of hitbox compared to playerpos
        public Vector3 hitboxTL; //top left corner of hitbox
        public Vector3 hitboxBR; //bottom right corner of hitbox

        public void LightAttack()
        {
            damage = 50;
            knockbackX = 1.0f;
            knockbackY = 0.5f;
            attackermoveX = 0.9f;
            attackermoveY = 0.4f;
            hitboxWidth = 1;
            hitboxHeight = 1;
            hitboxOffset = new Vector3(0.5f, 1, 0);
        }
        public void HeavyAttack()
        {
            damage = 150;
            knockbackX = 2.0f;
            knockbackY = 1.0f;
            attackermoveX = 1.9f;
            attackermoveY = 0.9f;
            hitboxWidth = 1;
            hitboxHeight = 1;
            hitboxOffset = new Vector3(0.5f, 1, 0);
        }
    }
}
