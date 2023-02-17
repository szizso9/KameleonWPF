using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kameleon2.Persistence;
using System.Drawing;
namespace Kameleon2.Model
{
    public class ChangeEventArgs
    {
        private Point _p;

        public Point p { get { return _p; } }   

        public ChangeEventArgs(Point p)
        {
            _p = p;
        }
    }
}
