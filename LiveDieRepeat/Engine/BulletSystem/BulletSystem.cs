using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using LiveDieRepeat.Entities;
using BulletMLLib;

namespace LiveDieRepeat.Engine.BulletSystem
{
    /// <summary>
    /// BulletMLLibから呼ばれる関数群を実装
    /// </summary>
    class MyBulletFunctions : IBulletMLManager
    {
        private Random random;
        private PlayerEntity player;

        public MyBulletFunctions(PlayerEntity player)
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
