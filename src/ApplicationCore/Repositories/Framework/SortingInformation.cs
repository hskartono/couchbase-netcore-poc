using System;
using System.Linq.Expressions;
using System.Text.Json.Serialization;

namespace AppCoreApi.ApplicationCore.Repositories
{
	public class SortingInformation<T> where T : class
	{
		private Expression<Func<T, dynamic>> _predicate;
		private SortingType _sortType;
		private string _keyName;
		public SortingInformation()
		{

		}
		public SortingInformation(Expression<Func<T, dynamic>> predicate, SortingType sortType)
		{
			_predicate = predicate;
			_sortType = sortType;
		}

		[JsonConstructor]
		public SortingInformation(string keyName, SortingType sortType)
		{
			_keyName = keyName;
			_sortType = sortType;
		}

		public SortingInformation(Expression<Func<T, dynamic>> predicate, string keyName, SortingType sortType)
		{
			_predicate = predicate;
			_keyName = keyName;
			_sortType = sortType;
		}

		public Expression<Func<T, dynamic>> Predicate {
			get 
			{
				return _predicate;
			}

			set 
			{
				_predicate = value;
			}
		}

		public SortingType SortType
		{
			get
			{
				return _sortType;
			}

			set
			{
				_sortType = value;
			}
		}

		public string KeyName
		{
			get
			{
				return _keyName;
			}
			set
			{
				_keyName = value;
			}
		}
	}

	public enum SortingType
	{
		Ascending,
		Descending
	}
}
