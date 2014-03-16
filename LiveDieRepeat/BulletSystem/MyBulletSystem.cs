using BulletMLLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveDieRepeat.BulletSystem
{
	public class MyBulletSystem : IBulletMLManager
    {
        private Random random;
        private Player player;

        public MyBulletSystem(Player player)
        {
			this.player = player;
            random = new Random();
        }

        public float GetRandom() { return (float)random.NextDouble(); }

        public float GetRank() { return 0; }

        public float GetShipPosX() { return player.Position.X; } //自機の座標を返す

        public float GetShipPosY() { return player.Position.Y; } //自機の座標を返す
    }

}
