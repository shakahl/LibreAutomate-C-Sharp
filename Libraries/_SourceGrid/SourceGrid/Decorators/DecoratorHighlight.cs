using System;
using System.Collections.Generic;
using System.Text;

namespace SourceGrid.Decorators
{
    public class DecoratorHighlight : DecoratorBase
    {
        private GridRange mRange = GridRange.Empty;
        /// <summary>
        /// Gets or sets the range to draw
        /// </summary>
        public GridRange Range
        {
            get { return mRange; }
            set { mRange = value; }
        }


        public override bool IntersectWith(GridRange range)
        {
            return Range.IntersectsWith(range);
        }

        public override void Draw(RangePaintEventArgs e)
        {
        }
    }
}
