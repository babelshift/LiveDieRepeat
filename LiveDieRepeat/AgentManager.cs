using BulletMLLib;
using LiveDieRepeat.BulletSystem;
using LiveDieRepeat.Content;
using LiveDieRepeat.UserInterface;
using SharpDL;
using SharpDL.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveDieRepeat
{
	public class AgentManager : IDisposable
	{
		private Random random = new Random();
		private TimeSpan timeSinceLastPlayerBullet = TimeSpan.MaxValue;
		private TimeSpan timeBetweenPlayerBullets = TimeSpan.FromSeconds(0.1);

		private Player player;
		private List<Enemy> enemies = new List<Enemy>();
		private List<Bullet> playerBullets = new List<Bullet>();

		private ContentManager contentManager;

		public AgentManager(ContentManager contentManager)
		{
			this.contentManager = contentManager;

			Icon iconCenterRing = ControlFactory.CreateIcon(contentManager, "CenterRing");
			iconCenterRing.Position = new Vector(MainGame.SCREEN_WIDTH_LOGICAL / 2 - iconCenterRing.Width / 2, MainGame.SCREEN_HEIGHT_LOGICAL - iconCenterRing.Height / 2);

			Icon iconPlayer = ControlFactory.CreateIcon(contentManager, "Player");
			player = new Player(iconPlayer, iconCenterRing);
			player.TeleportTo(new Vector(MainGame.SCREEN_WIDTH_LOGICAL / 2, MainGame.SCREEN_HEIGHT_LOGICAL));

			BulletMLManager.Init(new MyBulletSystem(player));
		}

		public void Update(GameTime gameTime, bool isArrowLeftDown, bool isArrowRightDown, bool isLeftMouseButtonDown, bool isSpacebarDown)
		{
			UpdatePlayer(isArrowLeftDown, isArrowRightDown);

			UpdateEnemies(gameTime);

			UpdatePlayerBullets(gameTime);

			ShootPlayerBullet(gameTime, isLeftMouseButtonDown, isSpacebarDown);

			CreateRandomEnemy();

			RemoveDeadAgents();
		}

		public void Draw(GameTime gameTime, Renderer renderer)
		{
			foreach (var enemy in enemies)
				enemy.Draw(gameTime, renderer);

			player.Draw(gameTime, renderer);

			foreach (var playerBullet in playerBullets)
				playerBullet.Draw(gameTime, renderer);
		}

		private void MovePlayer(bool isArrowLeftDown, bool isArrowRightDown)
		{
			if (isArrowLeftDown)
			{
				if (player.Position.X >= 0)
				{
					player.AdjustPosition(new Vector(-10, 0));
				}
			}
			else if (isArrowRightDown)
			{
				if (player.Position.X <= MainGame.SCREEN_WIDTH_LOGICAL)
				{
					player.AdjustPosition(new Vector(10, 0));
				}
			}
		}

		private void ShootPlayerBullet(GameTime gameTime, bool isLeftMouseButtonDown, bool isSpacebarDown)
		{
			if (timeSinceLastPlayerBullet < TimeSpan.MaxValue)
				timeSinceLastPlayerBullet += gameTime.ElapsedGameTime;

			if (timeSinceLastPlayerBullet >= timeBetweenPlayerBullets)
			{
				if (isLeftMouseButtonDown || isSpacebarDown)
				{
					Bullet bullet = player.FireWeapon(contentManager);
					playerBullets.Add(bullet);

					timeSinceLastPlayerBullet = TimeSpan.Zero;
				}
			}
		}

		private void CreateRandomEnemy()
		{
			if (enemies.Count < 500)
			{
				int randomX = random.Next(0, 1200);
				int randomY = random.Next(100, 700);
				Vector randomPosition = new Vector(randomX, randomY);
				Icon iconEnemy = ControlFactory.CreateIcon(contentManager, "Enemy");
				Enemy newEnemy = new Enemy(new Vector(400, 400), iconEnemy);
				newEnemy.TeleportTo(randomPosition);
				enemies.Add(newEnemy);
			}
		}

		private void UpdatePlayer(bool isArrowLeftDown, bool isArrowRightDown)
		{
			MovePlayer(isArrowLeftDown, isArrowRightDown);

			float angle = (float)Math.Atan2((double)((player.Position.Y - 10) - player.Position.Y), (double)(player.Position.X - player.Position.X));
			angle = MathExtensions.ToDegrees(angle);
			angle -= 45;
			player.RotateTo(angle);
		}

		private void UpdateEnemies(GameTime gameTime)
		{
			foreach (var enemy in enemies)
				enemy.Update(gameTime);
		}

		private void UpdatePlayerBullets(GameTime gameTime)
		{
			foreach (var playerBullet in playerBullets)
			{
				playerBullet.Update(gameTime);

				if (playerBullet.Position.Y <= 59)
				{
					playerBullet.Die();
				}
			}
		}

		private void RemoveDeadAgents()
		{
			int deadEnemyCount = enemies.Count(e => e.IsDead);
			UpdateScore(deadEnemyCount);

			var deadPlayerBullets = playerBullets.Where(pb => pb.IsDead);
			foreach (var deadPlayerBullet in deadPlayerBullets)
				deadPlayerBullet.Dispose();
			playerBullets.RemoveAll(pb => pb.IsDead);

			var deadEnemies = enemies.Where(e => e.IsDead);
			foreach (var deadEnemy in deadEnemies)
				deadEnemy.Dispose();
			enemies.RemoveAll(e => e.IsDead);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			player.Dispose();

			foreach (var enemy in enemies)
				enemy.Dispose();

			foreach (var playerBullet in playerBullets)
				playerBullet.Dispose();
		}
	}
}