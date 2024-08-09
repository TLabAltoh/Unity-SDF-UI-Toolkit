using System;
using System.Collections.Generic;

namespace TLab.UI.SDF.Registry
{
	internal struct MaterialKeyword
	{
		public string KeywordID;
		public bool Value;

		public override bool Equals(object obj)
		{
			if (obj is not MaterialKeyword keyword)
				return false;

			if (keyword.KeywordID != KeywordID)
				return false;

			if (keyword.Value != Value)
				return false;

			return true;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(KeywordID, Value);
		}

		internal struct Comparer : IComparer<MaterialKeyword>
		{
			public int Compare(MaterialKeyword x, MaterialKeyword y)
			{
				return x.KeywordID.CompareTo(y.KeywordID);
			}
		}
	}
}
