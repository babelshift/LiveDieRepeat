using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using LiveDieRepeat.Entities;
using Microsoft.Xna.Framework.Content;

namespace LiveDieRepeat.Engine
{
    public static class Factory<T>
    {
        private static Dictionary<int, Func<T>> types = new Dictionary<int, Func<T>>();

        public static void Clear()
        {
            types.Clear();
        }

        public static T Create(int id)
        {
            Func<T> constructor = null;
            if (types.TryGetValue(id, out constructor))
                return constructor();

            throw new ArgumentException(String.Format("No type registered for the passed id: {0}", id));
        }

        public static void RegisterType(int id, Func<T> constructor)
        {
            types.Add(id, constructor);
        }
    }
}
