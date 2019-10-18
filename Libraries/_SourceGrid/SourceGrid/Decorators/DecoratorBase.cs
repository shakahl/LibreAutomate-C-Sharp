using System;
using System.Collections.Generic;
using System.Text;

namespace SourceGrid.Decorators
{
    public abstract class DecoratorBase
    {
        public abstract bool IntersectWith(GridRange range);

        public abstract void Draw(RangePaintEventArgs e);
    }
}
