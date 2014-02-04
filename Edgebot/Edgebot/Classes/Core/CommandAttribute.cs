using System;

namespace EdgeBot.Classes.Core
{
    public class CommandAttribute : Attribute
    {
        public string Listener { get; set; }
        public string Help { get; set; }

        public CommandAttribute(string listener, string help)
        {
            Listener = listener;
            Help = help;
        }
    }
}
