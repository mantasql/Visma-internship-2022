using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Visma_internship_2022;
namespace MeetingsUnitTests
{
    internal class FakeUserInput : IUserInput
    {
        public List<string> LinesToRead = new List<string>();
        public string GetInput()
        {
            string result = LinesToRead[0];
            LinesToRead.RemoveAt(0);
            return result;
        }
    }
}
