using System;
using ContentPipelineExtensionLibrary;
using LiveDieRepeat.Entities;
using LiveDieRepeat.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;

namespace LiveDieRepeat.Items
{
    public abstract class Weapon
    {
        #region Members

        private int projectileTypeId;           // determines the type of projectile that is generated when this weapon is fired (factory takes an id)
        protected int ammunitionCount;          // max ammo that the gun can carry
        private TimeSpan timeBetweenShots;      // time to delay between each successive fire
        private TimeSpan timeOfLastShot;        // time of last shot
        private Type owner;                     // the type of the owner that is carrying this weapon (enemy, player, etc)

        private int powerUpCountSinceReset = 0;
        private int powerUpCount = 0;
        private const int MAX_POWERUP_COUNT = 10;

        #endregion

        #region Properties

        public float PowerUpPercentThisLevel { get { return ((float)powerUpCountSinceReset / MAX_POWERUP_COUNT) * 100; } }

        #endregion

        #region Events Handlers

        public event EventHandler<WeaponGainedExperienceEventArgs> WeaponGainedExperienceEvent;

        #endregion

        #region Constructors

        public Weapon(ContentManager content, String weaponDataPath, Type owner)
        {
            WeaponData weaponData = content.Load<WeaponData>(weaponDataPath);

            this.projectileTypeId = weaponData.ProjectileType;
            this.ammunitionCount = weaponData.MaxAmmo;
            this.timeBetweenShots = TimeSpan.FromSeconds(weaponData.SecondsBetweenShots);
            this.owner = owner;
        }

        #endregion

        #region Helper Methods

        public virtual List<ProjectileEntity> Fire(Vector2 position, float radians, TimeSpan timeOfShot)
        {
            TimeSpan timeBetweenLastShotAndNow = timeOfShot - timeOfLastShot;

            List<ProjectileEntity> newProjectiles = new List<ProjectileEntity>();

            if (ammunitionCount > 0 && timeBetweenLastShotAndNow > timeBetweenShots || timeOfLastShot == timeOfShot)
            {
                ProjectileEntity newProjectile = Factory<ProjectileEntity>.Create(projectileTypeId);
                newProjectile.Spawn(position, radians, owner);

                timeOfLastShot = timeOfShot;
                newProjectile.PlayActivatedSound();

                newProjectiles.Add(newProjectile);
            }

            return newProjectiles;
        }

        public abstract void Reload(int ammunitionCount);

        public abstract void Upgrade(EntityId.WeaponUpgrade upgrade);

        protected void SetProjectileType(EntityId.Projectile ProjectileTypeId)
        {
            this.projectileTypeId = (int)ProjectileTypeId;
        }

        public void AddExperienceToWeapon()
        {
            powerUpCount++;
            powerUpCountSinceReset++;

            // this is awful, figure out a better way to upgrade weapons
            if (powerUpCountSinceReset >= MAX_POWERUP_COUNT)
            {
                // think of a better way to evaluate these power level ranges
                if (powerUpCount >= 10 && powerUpCount < 20)
                    Upgrade(EntityId.WeaponUpgrade.DoubleShot);
                else if (powerUpCount >= 20 && powerUpCount < 30)
                    Upgrade(EntityId.WeaponUpgrade.TripleShot);

                powerUpCountSinceReset = 0;
            }

            OnWeaponGainedExperienceEvent();
        }

        #endregion

        #region Events

        private void OnWeaponGainedExperienceEvent()
        {
            WeaponGainedExperienceEventArgs e = new WeaponGainedExperienceEventArgs(PowerUpPercentThisLevel);

            if (WeaponGainedExperienceEvent != null)
                WeaponGainedExperienceEvent(this, e);
        }

        #endregion
    }

    public class WeaponGainedExperienceEventArgs : EventArgs
    {
        public float PowerPercentGained { get; private set; }

        public WeaponGainedExperienceEventArgs(float powerPercentGained)
        {
            PowerPercentGained = powerPercentGained;
        }
    }
}
