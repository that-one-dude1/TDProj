using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace TDProj
{
    public enum TowerType
    {
        Basic,      //Balanced
        Sniper,     //Long range, slow fire
        Rapid,      //Short range, fast fire
    }

    public class TowerDefinition
    {
        public TowerType Type { get; }
        public float Range { get; }
        public float FireRate { get; }
        public float Damage { get; }
        public int Cost { get; }
        public Color Color { get; }

        public TowerDefinition(TowerType type, float range, float fireRate, float damage, int cost, Color color)
        {
            Type = type;
            Range = range;
            FireRate = fireRate;
            Damage = damage;
            Cost = cost;
            Color = color;
        }

        public static TowerDefinition GetDefinition(TowerType type)
        {
            return type switch
            {
                TowerType.Sniper => new TowerDefinition(type, range: 350f, fireRate: 0.5f, damage: 120f, cost: 150, Color.Crimson),
                TowerType.Rapid => new TowerDefinition(type, range: 150f, fireRate: 3f, damage: 20f, cost: 100, Color.LimeGreen),
                _ => new TowerDefinition(type, range: 250f, fireRate: 1.0f, damage: 50f, cost: 50, Color.CornflowerBlue)
            };
        }
    }
}
