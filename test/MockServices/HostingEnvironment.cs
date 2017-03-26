using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;

namespace Shevastream.Tests
{
	/// <summary>
	/// Only EnvironmentName is implemented.
	/// </summary>
	public class MockHostingEnvironmentDevelopment : IHostingEnvironment
	{
		public string ApplicationName
		{
			get
			{
				throw new NotImplementedException();
			}

			set
			{
				throw new NotImplementedException();
			}
		}

		public IFileProvider ContentRootFileProvider
		{
			get
			{
				throw new NotImplementedException();
			}

			set
			{
				throw new NotImplementedException();
			}
		}

		public string ContentRootPath
		{
			get
			{
				throw new NotImplementedException();
			}

			set
			{
				throw new NotImplementedException();
			}
		}

		public string EnvironmentName
		{
			get
			{
				return "Development";
			}

			set
			{
				throw new NotImplementedException();
			}
		}

		public IFileProvider WebRootFileProvider
		{
			get
			{
				throw new NotImplementedException();
			}

			set
			{
				throw new NotImplementedException();
			}
		}

		public string WebRootPath
		{
			get
			{
				throw new NotImplementedException();
			}

			set
			{
				throw new NotImplementedException();
			}
		}
	}

	/// <summary>
	/// Only EnvironmentName is implemented.
	/// </summary>
	public class MockHostingEnvironmentTesting : IHostingEnvironment
	{
		public string ApplicationName
		{
			get
			{
				throw new NotImplementedException();
			}

			set
			{
				throw new NotImplementedException();
			}
		}

		public IFileProvider ContentRootFileProvider
		{
			get
			{
				throw new NotImplementedException();
			}

			set
			{
				throw new NotImplementedException();
			}
		}

		public string ContentRootPath
		{
			get
			{
				throw new NotImplementedException();
			}

			set
			{
				throw new NotImplementedException();
			}
		}

		public string EnvironmentName
		{
			get
			{
				return "Testing";
			}

			set
			{
				throw new NotImplementedException();
			}
		}

		public IFileProvider WebRootFileProvider
		{
			get
			{
				throw new NotImplementedException();
			}

			set
			{
				throw new NotImplementedException();
			}
		}

		public string WebRootPath
		{
			get
			{
				throw new NotImplementedException();
			}

			set
			{
				throw new NotImplementedException();
			}
		}
	}
}
