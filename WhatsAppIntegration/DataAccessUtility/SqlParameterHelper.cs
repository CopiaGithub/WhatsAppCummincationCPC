using System.Data.SqlClient;

namespace Utility.DataAccessUtility 
{  
	/// <summary> 
	/// Summary description for SqlParameters.  
	/// </summary> 
	public class SQLParameterHelper  
	{
		/// <summary> 
		/// Return the parametes for a given sproc from the cache. If the params are not in the cache 
		/// the params are loaded from the dbase into the cache 
		/// </summary> 
		/// <remarks> 
		/// e.g.
		///  SqlParameters[] parameters = GetSqlParameters(connString, "mySproc", false);  
		/// </remarks> 
		/// <param name="connectionString">A valid connection string for a SqlConnection</param> 
		/// <param name="spName">The name of the stored procedure</param> 
		/// <param name="includeReturnValue">Whether the sproc will have return value at [0]</param> 
		/// <returns>Returns a SqlParameter[]</returns> 
		public static SqlParameter[] GetSqlParameters(string connectionString, string spName, bool includeReturnValue)  
		{ 
			SqlParameter[] parameters = SqlHelperParameterCache.GetCachedParameterSet(connectionString, spName); 
  
			if(parameters==null) 
			{
				parameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName, includeReturnValue); 
			} 
			return parameters;
		}
	}
}
