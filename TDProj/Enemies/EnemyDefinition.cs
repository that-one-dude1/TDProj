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
    public enum EnemyType
    {
        Circle,
        Triangle,
        Pentagon,
        Hexagon
    }

    public class EnemyDefinition
    {
        public EnemyType Type { get; }
        public float Speed { get; }
        public float Health { get; }
        public int Reward { get; }
        public Color Color { get; }

        public EnemyDefinition(EnemyType type, float speed, float health, int reward, Color color)
        {
            Type = type;
            Speed = speed;
            Health = health;
            Reward = reward;
            Color = color;
        }

        //Default enemy definitions
        public static Dictionary<EnemyType, EnemyDefinition> GetDefaultDefinitions()
        {
            return new Dictionary<EnemyType, EnemyDefinition>
            {
                { EnemyType.Circle,  new EnemyDefinition(EnemyType.Circle, 100f, 100f, 10, Color.Yellow) },
                { EnemyType.Triangle, new EnemyDefinition(EnemyType.Triangle, 160f, 60f, 20, Color.OrangeRed) },
                { EnemyType.Pentagon, new EnemyDefinition(EnemyType.Pentagon, 70f, 200f, 25, Color.LightGreen) },
                { EnemyType.Hexagon, new EnemyDefinition(EnemyType.Hexagon, 50f, 2000f, 50, Color.DarkBlue) }
            };
        }
    }
}
