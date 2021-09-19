using Microsoft.AspNetCore.TestHost;
using AppCoreApi.Infrastructure;
using System;
using System.Dynamic;
using System.Net.Http;
using Xunit;

namespace UnitTest.Api
{
	public class ControllerTestBase : IClassFixture<WebApiTesterFixture>
	{
		protected enum RequestType { GET, POST, PUT, DELETE };
		protected TestServer _testServer;
		protected WebApiTesterFixture _factory;
		protected HttpClient _client;
		protected AppDbContext _context;
		protected dynamic token;

		public ControllerTestBase(WebApiTesterFixture factory)
		{
			_factory = factory;
			_testServer = factory.TestServer;
			_client = _factory.Client;
			_context = _factory.Context;

			token = new ExpandoObject();
			token.sub = Guid.NewGuid();
			token.role = new[] { "sub_role", "admin" };
		}
	}
}
