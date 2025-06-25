using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace Profiling.Api.Context;

public class DataContext
{
    private readonly IConfiguration _configuration;
    private readonly string _connectionString;

    public DataContext(IConfiguration configuration)
    {
        _configuration = configuration;
        _connectionString = _configuration.GetConnectionString("SqlConnectionString")!;
    }

    public IDbConnection CreateConnection()
    {
        return new SqlConnection(_connectionString);
    }
}
