using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umaru.Core.Services;

namespace Umaru.Core.Store
{
	public class SuperSqlite
	{
		private readonly SQLiteAsyncConnection _database;

		public SuperSqlite(string dbPath)
		{
			_database = new SQLiteAsyncConnection(dbPath);
			_database.CreateTableAsync<SqliteModel>().Wait();
		}

		public List<SqliteModel> GetItems()
		{
			return _database.Table<SqliteModel>().ToListAsync().Result;
		}

		public SqliteModel GetItem(int id)
		{
			return _database.Table<SqliteModel>().FirstAsync((t) => t.Id == id).Result;
		}

		public SqliteModel GetItem(string key)
		{
			return _database.Table<SqliteModel>().FirstAsync((t)=>t.Key == key).Result;
		}

		public int SaveItem(SqliteModel item)
		{
			if (item.Id != 0)
			{
				return _database.UpdateAsync(item).Result;
			}
			else
			{
				return _database.InsertAsync(item).Result;
			}
		}


		public int DeleteItem(SqliteModel item)
		{
			return _database.DeleteAsync(item).Result;
		}

		public static SuperSqlite Instance = new SuperSqlite(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SuperSqlite.db"));
	}

	public class SqliteModel
	{
		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }

		public string? Key { get; set; }

		public string? Value { get; set; }
	}

}
