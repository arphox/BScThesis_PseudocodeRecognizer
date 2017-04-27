using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexicalAnalysis.Tokens
{
    public abstract class TerminalToken : Token
    {
        public TerminalToken(int ID, int rowNumber) : base(ID, rowNumber)
        {

        }
    }
}