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
    public class Wave
    {
        public int EnemyCount { get; }
        public float SpawnInterval { get; }

        public Wave(int enemyCount, float spawnInterval)
        {
            EnemyCount = enemyCount;
            SpawnInterval = spawnInterval;
        }

        public static List<Wave> CreateDefaultWaves()
        {
            return new List<Wave>
            {
                new Wave(enemyCount : 5, spawnInterval : 1.5f),
                new Wave(enemyCount : 10, spawnInterval : 1.2f),
                new Wave(enemyCount : 15, spawnInterval : 1.0f),
                new Wave(enemyCount : 20, spawnInterval : 0.8f)

                //KNOWN ISSUE: Game breaks after wave 4, maybe add a wave loop?
            };
        }
    }
}
