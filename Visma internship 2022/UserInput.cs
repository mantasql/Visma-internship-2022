using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Visma_internship_2022
{
    internal class UserInput : IUserInput
    {

        public UserInput() { }
        public string GetInput()
        {
            return Console.ReadLine();
        }
    }
}
