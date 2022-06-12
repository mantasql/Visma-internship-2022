using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Visma_internship_2022
{
    internal class Attendee
    {
        public string Name { get; private set; }
        public DateTime Time { get; private set; }

        public Attendee(string name, DateTime time)
        {
            Name = name;
            Time = time;
        }
    }
}
