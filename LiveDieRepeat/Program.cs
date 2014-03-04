using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveDieRepeat
{
	class Program
	{
		[STAThread]
		static void Main(string[] args)
		{
			MainGame mainGame = new MainGame();
			mainGame.Run();
		}
	}
}
