using System.Data;
using Dapper;
using LoggerService;
using Profiling.Api.Context;
using Profiling.Api.Contracts;
using Profiling.Api.DataTransferObjects;
using Profiling.Api.Entities;

namespace Profiling.Api.Repository;

public class ResourceRepository : IResourceRepository
{
    private readonly DataContext _context;
    private readonly ILoggerManager _loggerManager;

    public ResourceRepository(DataContext context, ILoggerManager loggerManager)
    {
        _context = context;
        _loggerManager = loggerManager;
    }

    public async Task<Resource> Create(CreateResourceDto NewResource)
    {

        var query = @"INSERT INTO [SsoProfiling].[dbo].[Resources]
                        ([Name], [NormalizedName])
                        VALUES(@Name, @NormalizedName); " +
                        "SELECT CAST(SCOPE_IDENTITY() AS INT);";

        var parameters = new DynamicParameters();
        parameters.Add("Name", NewResource.Name, dbType: DbType.String);
        parameters.Add("NormalizedName", NewResource.Name.ToUpper(), dbType: DbType.String);

        using (var connection = _context.CreateConnection())
        {
            var id = await connection.QuerySingleAsync<int>(query, parameters);

            return new Resource { Id = id, Name = NewResource.Name, NormalizedName = NewResource.Name.ToUpper() };
        }
    }

    public async Task<int> Delete(string Name)
    {
        var NormalizedName = Name.ToUpper();
        var query = @"DELETE 
                        FROM [SsoProfiling].[dbo].[Resources]
                        WHERE Name = @NormalizedName;";

        using (var connection = _context.CreateConnection())
        {
            var result = await connection.ExecuteAsync(query, new { NormalizedName });
            return result;
        }
    }

    public async Task<IEnumerable<Resource>> GetAllResources()
    {
        var query = @"SELECT [Id]
                        ,[Name]
                        ,[NormalizedName]
                    FROM [SsoProfiling].[dbo].[Resources]";

        using (var connection = _context.CreateConnection())
        {
            var resources = await connection.QueryAsync<Resource>(query);
            return resources.ToList();
        }
    }

    public async Task<Resource?> GetResourceByName(string Name)
    {
        var NormalizedName = Name.ToUpper();
        var query = @" SELECT [Id]
                            ,[Name]
                            ,[NormalizedName]
                        FROM [SsoProfiling].[dbo].[Resources]
                        WHERE NormalizedName = @NormalizedName";

        using (var connection = _context.CreateConnection())
        {
            var resource = await connection.QueryFirstOrDefaultAsync<Resource>(query, new { NormalizedName });
            return resource;
        }
    }
}
