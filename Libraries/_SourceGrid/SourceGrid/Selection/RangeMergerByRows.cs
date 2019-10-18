using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace SourceGrid.Selection
{
	/// <summary>
	/// Contains a list of ranges which differ only by row, that is vertically.
	/// This the reason why the class contains a word "row" in its name.
	/// 
	/// "Merger" means that it will merge adjancent ranges into single big range.
	/// So, for example, if you have 100 ranges from first row to second, that
	/// equals to one big range from row 1 to 100. If Row 50 is not selected,
	/// then this class splits one big range into two smaller. One from first row
	/// to row number 49, and anoter range from row 51 to row 100
	/// 
	/// Although some functions work with Range structure, which has also a horizontal 
	/// span, that information is actually not needed, and used as dummy only.
	/// 
	/// 
	/// </summary>
	public class RangeMergerByRows
	{
		private List<GridRange> m_ranges = new List<GridRange>();
		private int m_columnStart = 0;
		private int m_columnEnd = 0;
		
		public RangeMergerByRows()
		{
		}
		
		private void SortList()
		{
			m_ranges.Sort(new RangeComparerByRows());
		}
		
		private void InternalAdd(RangeRegion rangeRegion)
		{
			foreach (GridRange range in rangeRegion)
			{
				m_ranges.Add(range);
			}
		}
		
		
		/// <summary>
		/// Loops via all ranges. Ranges are guaranteed to be ordered
		/// from lowest row to highest.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<GridRange> LoopAllRanges()
		{
			foreach (var row in m_ranges)
			{
				yield return row;
			}
		}
		
		/// <summary>
		/// Returns a list of selected ranges
		/// </summary>
		/// <param name="startColumn">start column, usually 0</param>
		/// <param name="endColumn">end column, usually equals to column count - 1</param>
		/// <returns></returns>
		public List<GridRange> GetSelectedRowRegions(int startColumn, int endColumn)
		{
			if (startColumn > endColumn)
				throw new ArgumentException("end column can not be less than startColumn");
			List<GridRange> ranges = new List<GridRange>();
			foreach (GridRange range in m_ranges)
			{
				ranges.Add(new GridRange(range.Start.Row, startColumn, range.End.Row, endColumn));
			}
			return ranges;
		}
		
		/// <summary>
		/// </summary>
		/// <param name="rangeToMerge"></param>
		/// <returns></returns>
		private void Merge(GridRange rangeToMerge)
		{
			while (true)
			{
				rangeToMerge = MergeRecursive(rangeToMerge);
				if (rangeToMerge == GridRange.Empty)
					return;
			}
		}
		
		/// <summary>
		/// Merges given range with one of the intersecting range in m_ranges
		/// if no intersecting ranges is found, then rangeToMerge is
		/// added to m_ranges
		/// </summary>
		/// <param name="rangeToMerge"></param>
		/// <returns></returns>
		private GridRange MergeRecursive(GridRange rangeToMerge)
		{
			for (int i = 0; i < m_ranges.Count; i++)
			{
				GridRange range = m_ranges[i];
				if (range.IntersectsWith(rangeToMerge))
				{
					m_ranges.Remove(range);
					return MergeByRow(rangeToMerge, range);
				}
			}
			m_ranges.Add(rangeToMerge);
			return GridRange.Empty;
		}
		
		/// <summary>
		/// Returns true if at least two ranges were joined
		/// </summary>
		/// <returns></returns>
		private bool JoinAjdancedRecursive()
		{
			SortList();
			for (int i = 0; i < m_ranges.Count - 1; i++)
			{
				GridRange first = m_ranges[i];
				GridRange second = m_ranges[i+1];
				if (first.End.Row + 1 >= second.Start.Row)
				{
					GridRange newRange = new GridRange(first.Start.Row, m_columnStart,
					                           second.End.Row, m_columnEnd);
					m_ranges.Remove(first);
					m_ranges.Remove(second);
					m_ranges.Add(newRange);
					return true;
				}
			}
			return false;
		}
		
		private void JoinAdjanced()
		{
			while (JoinAjdancedRecursive())
			{
			}
		}
		
		private GridRange NormalizeRange(GridRange rangeToNormalize)
		{
			return new GridRange(rangeToNormalize.Start.Row, m_columnStart,
			                 rangeToNormalize.End.Row, m_columnEnd);
		}
		
		/// <summary>
		/// Add a range to collection. If the range can be added (merged)
		/// to existing range, it will be added so. This will guarantee
		/// that the number of different ranges is kept to minimal.
		/// In theory only if user selects every second row, this would produce
		/// RowCount / 2 number of ranges. In practice, there are rarely 
		/// more than 3 - 5 different selection regions
		/// </summary>
		/// <param name="rangeToAdd">columns values are ignored, so simply 
		/// put 0 as start colum and 1 as end column. Only row values matter</param>
		/// <returns></returns>
		public RangeMergerByRows AddRange(GridRange rangeToAdd)
		{
			rangeToAdd = NormalizeRange(rangeToAdd);
			Merge(rangeToAdd);
			JoinAdjanced();
			return this;
		}
		
		/// <summary>
		/// Returns new range witch is the max of both ranges in row axis
		/// </summary>
		/// <param name="first"></param>
		/// <param name="second"></param>
		/// <returns></returns>
		private GridRange MergeByRow(GridRange first, GridRange second)
		{
			int x = first.Start.Row;
			if (x > second.Start.Row)
				x = second.Start.Row;
			
			int x1 = first.End.Row;
			if (x1 < second.End.Row)
				x1 = second.End.Row;
			return new GridRange(x, m_columnStart, x1, m_columnEnd);
		}
		
		private bool RemoveRangeRecursive(GridRange rangeToRemove)
		{
			rangeToRemove = NormalizeRange(rangeToRemove);
			for (int i = 0; i < m_ranges.Count; i++)
			{
				GridRange range = m_ranges[i];
				if (range.IntersectsWith(rangeToRemove) == false)
					continue;
				GridRange intersection = range.Intersect(rangeToRemove);
				m_ranges.Remove(range);
				RangeRegion excludedRanges = range.Exclude(intersection);
				InternalAdd(excludedRanges);
				return true;
			}
			return false;
		}
		
		/// <summary>
		/// As opposed to Adding a range, this will remove a range.
		/// It might happend that removing a range is dividing one bigger range into two 
		/// smaller ones. So practically the number of ranges might increase.
		/// 
		/// It might be that this method might carry more exact meaning with "Exclude", instead
		/// of "RemoveRange"
		/// </summary>
		/// <param name="rangeToRemove"></param>
		/// <returns></returns>
		public RangeMergerByRows RemoveRange(GridRange rangeToRemove)
		{
			while (RemoveRangeRecursive(rangeToRemove))
			{
				
			}
			return this;
		}
		
		/// <summary>
		/// Returns a list of indexes of rows
		/// </summary>
		/// <returns></returns>
		public IList<int> GetRowsIndex()
		{
			IList<int> rowIndex = new List<int>();
			foreach (GridRange range in m_ranges)
			{
				for (int r = range.Start.Row; r <= range.End.Row; r++)
				{
					rowIndex.Add(r);
				}
			}
			return rowIndex;
		}
		
		public bool IsEmpty()
		{
			return m_ranges.Count == 0;
		}
		
		public bool IsSelectedRow(int rowIndex)
		{
			Position p = new Position(rowIndex, 0);
			foreach (GridRange range in m_ranges)
			{
				if (range.Contains(p) == true)
					return true;
			}
			return false;
		}
		
		public RangeMergerByRows Clear()
		{
			m_ranges.Clear();
			return this;
		}
	}
}

