using Microsoft.EntityFrameworkCore;

namespace Shevastream.Extensions
{
	public static class DbSetExtensions
	{
		/// <summary>
		/// Removes all records in DbSet.
		/// Does not SaveChanges.
		/// </summary>
		/// <param name="dbSet">DbSet to clear</param>
		public static void Clear<T>(this DbSet<T> dbSet) where T : class
		{
			dbSet.RemoveRange(dbSet);
		}
	}
}
