using System;

namespace EdgeBot.Classes.Core
{
    public class Command : Attribute
    {
        public string Listener { get; set; }

        public Command(string listener)
        {
            Listener = listener;
        }
    }
}
