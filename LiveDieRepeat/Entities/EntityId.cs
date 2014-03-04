using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiveDieRepeat.Entities
{
    public static class EntityId
    {
        #region Entity

        public enum DeathAnimation
        {
            ExplosionMedium = 1,
            ExplosionLarge = 2
        }

        public enum Object
        {
            ArrowUp = 2,
            ArrowDown = 3,
            ArrowLeft = 4,
            ArrowRight = 5
        }

        #endregion

        public enum Enemy
        {
            Simple = 1,
            Simple2 = 2,
            WallHugger = 3,
            Eyeball = 4,
            Brain = 5,
            BossBrain = 6
        }

        public enum Projectile
        {
            MachineGunSingle = 1,
            MachineGunDouble = 2,
            MachineGunTriple = 3,
            EnemySimple = 4,
            Rocket = 5,
            Grenade = 6,
            LaserBeam = 7,
            Fireball = 8,
            Mine = 9,
            Spread = 10
        }

        public enum Item
        {
            ShieldPickup = 1,
            WeaponPowerUpPickup = 2,
            MoneyRedPickup = 3,
            SpreadGunPickup = 4,
            FlameThrowerPickup = 5,
            MinePickup = 6,
            RocketLauncherPickup = 7,
            GrenadeLauncherPickup = 8,
            MachineGunPickup = 9
        }

        public enum WeaponUpgrade
        {
            DoubleShot = 1,
            TripleShot = 2,
            Fire = 3,
            Ice = 4,
            Poison = 5,
            Explosive = 6,
            Spread = 7
        }

        public enum Weapon
        {
            MachineGun = 1,
            RocketLauncher = 2,
            FlameThrower = 3,
            MineLayer = 4,
            GrenadeLauncher = 5,
            SpreadGun = 6
        }
    }
}
