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
    public class SpawnGroup
    {
        public EnemyType EnemyType;
        public int Count;
        public float Interval;      //time between enemy spawns
        public float StartDelay;    //delay before this group starts

        public SpawnGroup(EnemyType enemyType, int count, float interval, float startDelay = 0f)
        {
            EnemyType = enemyType;
            Count = count;
            Interval = interval;
            StartDelay = startDelay;
        }
    }

    public class Wave
    {
        public List<SpawnGroup> SpawnGroups = new();

        public void ParseWave(string script)
        {
            float pendingDelay = 0f;

            var tokens = script.Split('>');

            foreach (var raw in tokens)
            {
                var token = raw.Trim();

                //delay
                if (token.StartsWith("+"))
                {
                    pendingDelay += float.Parse(token[1..]);
                    continue;
                }

                //enemy group
                char typeChar = token[0];
                int atIndex = token.IndexOf('@');

                int count = int.Parse(token[1..atIndex]);
                float interval = float.Parse(token[(atIndex + 1)..]);

                SpawnGroups.Add(
                    new SpawnGroup(
                        ParseEnemyType(typeChar),
                        count,
                        interval,
                        pendingDelay
                    )
                );

                pendingDelay = 0f;
            }
        }

        private static EnemyType ParseEnemyType(char c) =>
            c switch
            {
                'C' => EnemyType.Circle,
                'T' => EnemyType.Triangle,
                'P' => EnemyType.Pentagon,
                'H' => EnemyType.Hexagon,
                _ => throw new Exception("Unknown enemy type")
            };

        public int TotalEnemyCount()
        {
            int total = 0;
            foreach (var group in SpawnGroups)
                total += group.Count;
            return total;
        }
    }
}
