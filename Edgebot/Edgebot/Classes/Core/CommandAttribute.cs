using System;

namespace EdgeBot.Classes.Core
{
    public class CommandAttribute : Attribute
    {
        public string Listener { get; set; }

        public CommandAttribute(string listener)
        {
            Listener = listener;
        }
    }
}
