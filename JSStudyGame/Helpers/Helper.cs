using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSStudyGame.Helpers
{
    public static class Helper
    {
        public static string GetPathToSolution()
        {
            string workingDirectory = Environment.CurrentDirectory;
            return System.IO.Directory.GetParent(workingDirectory).Parent.FullName;
        }
    }
}
