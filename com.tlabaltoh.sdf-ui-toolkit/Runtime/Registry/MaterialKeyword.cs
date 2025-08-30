using System;
using System.Collections.Generic;

namespace TLab.UI.SDF.Registry
{
	internal struct MaterialKeyword
	{
		public string Keyword;
		public bool Active;

		public override bool Equals(object obj)
		{
			if (obj is not MaterialKeyword keyword)
				return false;

			if (keyword.Keyword != Keyword)
				return false;

			if (keyword.Active != Active)
				return false;

			return true;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Keyword, Active);
		}

		internal struct Comparer : IComparer<MaterialKeyword>
		{
			public int Compare(MaterialKeyword x, MaterialKeyword y)
			{
				return x.Keyword.CompareTo(y.Keyword);
			}
		}
	}
}
