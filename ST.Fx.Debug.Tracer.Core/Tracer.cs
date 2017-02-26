using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ST.Fx.Debug.Tracer
{
    public static class Tracer
    {
        public enum Levels
        {
            Trace = 0,
            Debug = 1,
            Info = 2,
        }

        public static List<Action<string>> _listeners = new List<Action<string>>();
        public static Levels Level { get; set; } = Levels.Trace;


        public static void addListener(Action<string> listener)
        {
            _listeners.Add(listener);
        }
        public static void removeListener(Action<string> listener)
        {
            if (_listeners.Contains(listener))
                _listeners.Remove(listener);
        }

        public static void writeLine(string msg, Levels level = Levels.Debug)
        {
            if (level < Level) return;

            foreach (var l in _listeners)
            {
                try
                {
                    l(msg);
                }
                catch (Exception)
                {

                }
            }
        }
    }
}
