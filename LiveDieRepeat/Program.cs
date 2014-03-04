#region Using Statements
using LiveDieRepeat;
using System;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace LiveDieRepeat
{
#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (var game = new MainGame())
                game.Run();
        }
    }
#endif
}
