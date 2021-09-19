using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace UnitTest.Infrastructure.Repository
{
	public class RepositoryTestBase : IClassFixture<DatabaseFixture>
	{
		protected DatabaseFixture _db;
		public RepositoryTestBase(DatabaseFixture fixture)
		{
			_db = fixture;
		}
	}
}
