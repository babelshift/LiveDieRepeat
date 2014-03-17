using SharpDL.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveDieRepeat
{
	/// <summary>
	/// A grid cell is a single cell in the grid used for collision detection. Multiple entities can exist in multiple grid cells.
	/// </summary>
	public class GridCell
	{
		// list of collidable entities in this cell
		private List<ICollidable> collidables;
		public List<ICollidable> Collidables { get { return collidables; } }

		public GridCell()
		{
			collidables = new List<ICollidable>();
		}

		// adds a collidable entity to this cell only if it doesn't already know about this entity
		public void AddCollidable(ICollidable collidable)
		{
			if (!collidables.Contains(collidable))
				collidables.Add(collidable);
		}

		public void Clear()
		{
			collidables.Clear();
		}
	}

	/// <summary>
	/// The grid manager is responsible for maintaining a grid of cells spanning a camera view for collision detection. A single cell size is defined by "cellSize" which represents both
	/// the width and the height of the cell. Each cell has an index determined by the index in the "cells" list. When adding an entity to the grid manager, we first look up any 
	/// cells that the entity belongs to followed by adding that entity to the list of entities in all the resulting cells. When resolving collision, we loop through the collidable entities
	/// and find its nearby neighbors in its cells.
	/// </summary>
	public class GridManager
	{
		private int columns;
		private int rows;
		private const int cellSize = 16;
		private List<GridCell> cells;

		/// <summary>
		/// On construction, determine columns and rows based on the viewport width/height and cell size. Additionally, initialize the grid
		/// to the amount of cells that can exist based on the columns and rows. Finally, clear the cells of this instance and add empty cells.
		/// </summary>
		public GridManager()
		{
			columns = (MainGame.SCREEN_WIDTH_LOGICAL / cellSize);
			rows = (int)Math.Ceiling((double)MainGame.SCREEN_HEIGHT_LOGICAL / (double)cellSize);
			cells = new List<GridCell>(columns * rows);
			ClearCells();
		}

		/// <summary>
		/// When we clear cells of the manager, we clear the list of cells followed by adding empty cells to the list.
		/// </summary>
		public void ClearCells()
		{
			cells.Clear();

			for (int i = 0; i < cells.Capacity; i++)
			{
				GridCell emptyCell = new GridCell();
				cells.Add(emptyCell);
			}
		}

		/// <summary>
		/// Adds a collidable entity to any cells that the entity overlaps.
		/// </summary>
		/// <param name="collidable"></param>
		public void AddCollidable(ICollidable collidable)
		{
			List<int> gridIndicesOfCollidable = GetCollidableGridIndices(collidable);

			foreach (var gridIndex in gridIndicesOfCollidable)
				AddCollidableToCell(collidable, gridIndex);
		}

		/// <summary>
		/// Determines which cell indices the collidable entity overlaps by checking each corner of the entity's collision box. An entity can potentially
		/// span four different cells (assuming the entity is smaller than a single cell).
		/// </summary>
		/// <param name="collidable"></param>
		/// <returns></returns>
		private List<int> GetCollidableGridIndices(ICollidable collidable)
		{
			List<GridCell> cellsCollidableIsIn = new List<GridCell>();

			Vector minCorner = new Vector(collidable.CollisionBox.X, collidable.CollisionBox.Y);
			Vector maxCorner = new Vector(collidable.CollisionBox.X + collidable.CollisionBox.Width, collidable.CollisionBox.Y + collidable.CollisionBox.Height);

			int gridIndexMinCorner = GetGridIndexForPosition(minCorner);
			int gridIndexMaxCorner = GetGridIndexForPosition(maxCorner);
			int gridIndexMinMaxCorner = GetGridIndexForPosition(new Vector(minCorner.X, maxCorner.Y));
			int gridIndexMaxMinCorner = GetGridIndexForPosition(new Vector(maxCorner.X, minCorner.Y));

			// don't allow duplicates if same index
			List<int> gridIndices = new List<int>();

			if (!gridIndices.Contains(gridIndexMinCorner))
				gridIndices.Add(gridIndexMinCorner);

			if (!gridIndices.Contains(gridIndexMaxCorner))
				gridIndices.Add(gridIndexMaxCorner);

			if (!gridIndices.Contains(gridIndexMinMaxCorner))
				gridIndices.Add(gridIndexMinMaxCorner);

			if (!gridIndices.Contains(gridIndexMaxMinCorner))
				gridIndices.Add(gridIndexMaxMinCorner);

			return gridIndices;
		}

		/// <summary>
		/// Determines which cell index a position is in
		/// </summary>
		/// <param name="position"></param>
		/// <returns></returns>
		private int GetGridIndexForPosition(Vector position)
		{
			// grid has an X and Y offset because the camera can scroll outside of the viewport's bounds
			// when the camera shifts on that event, the positions are larger or smaller than the default grid, so an offset must be found
			// example:
			//      assume resolution of 1280 x 720
			//      further assume position is x = 1920, y = 0
			//      index offset is (1920 / 1280) = 1 + 1 = 2
			int gridOffsetX = (int)Math.Floor(Math.Abs(position.X) / MainGame.SCREEN_WIDTH_LOGICAL) + 1;
			int gridOffsetY = (int)Math.Floor(Math.Abs(position.Y) / MainGame.SCREEN_HEIGHT_LOGICAL) + 1;

			// using the grid offset, we determine which actual index the position is in by dividing the position by the cell size and then by the offset
			// example:
			//      assume position and offset from example above
			//      gridX = floor(1920 / 64) = 40 / 2 = 20
			//      gridY = floor(0 / 64) = 0 / 2 = 0
			// if we did not apply the offset, the grid index would be incorrect because the camera is actually outside of the normal grid bounds
			int gridX = (int)Math.Floor(Math.Abs(position.X) / cellSize) / gridOffsetX;
			int gridY = (int)Math.Floor(Math.Abs(position.Y) / cellSize) / gridOffsetY;
			int gridIndex = gridX + (gridY * columns);

			return gridIndex;
		}

		/// <summary>
		/// Adds a collidable entity to the cell at "gridIndex".
		/// </summary>
		/// <param name="collidable"></param>
		/// <param name="gridIndex"></param>
		private void AddCollidableToCell(ICollidable collidable, int gridIndex)
		{
			cells[gridIndex].AddCollidable(collidable);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="collidable"></param>
		/// <returns></returns>
		public IList<ICollidable> GetNearbyCollidables(ICollidable collidable)
		{
			List<ICollidable> nearbyCollidables = new List<ICollidable>();
			List<int> gridIndicesOfCollidable = GetCollidableGridIndices(collidable);

			foreach (int gridIndex in gridIndicesOfCollidable)
			{
				// don't add duplicates to the nearby collection
				foreach (ICollidable nearbyCollidable in cells[gridIndex].Collidables)
					if (!nearbyCollidables.Contains(nearbyCollidable))
						nearbyCollidables.Add(nearbyCollidable);

				// remove any instances of myself from the nearby list because i don't want to check colliding with myself
				nearbyCollidables.RemoveAll(c => c.Equals(collidable));
			}
			return nearbyCollidables;
		}
	}
}
