using System.Data;
using System.Text.Json;
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
    private readonly IHttpContextAccessor _httpContext;

    public ResourceRepository(DataContext context, ILoggerManager loggerManager, IHttpContextAccessor httpContext)
    {
        _context = context;
        _loggerManager = loggerManager;
        _httpContext = httpContext;
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

        _loggerManager.LogInfo($"Open Connection....");
        using (var connection = _context.CreateConnection())
        {
            _loggerManager.LogInfo($"Executing Ceate Resource - Query: {query} Parameter: {JsonSerializer.Serialize(parameters)}");
            var id = await connection.QuerySingleAsync<int>(query, parameters);
            _loggerManager.LogInfo($"Resource Create Query Return : {id}");

            return new Resource { Id = id, Name = NewResource.Name, NormalizedName = NewResource.Name.ToUpper() };
        }
    }

    public async Task<int> Delete(string Name)
    {
        var NormalizedName = Name.ToUpper();
        var query = @"DELETE 
                        FROM [SsoProfiling].[dbo].[Resources]
                        WHERE Name = @NormalizedName;";
        _loggerManager.LogInfo($"Open Connection....");
        using (var connection = _context.CreateConnection())
        {
            _loggerManager.LogInfo($"Executing Delete Module - Query: {query} Parameters: {NormalizedName}");
            var result = await connection.ExecuteAsync(query, new { NormalizedName });
            _loggerManager.LogInfo($"Resource Delete Query Return : {result}");
            return result;
        }
    }

    public async Task<IEnumerable<Resource>> GetAllResources()
    {
        var query = @"SELECT [Id]
                        ,[Name]
                        ,[NormalizedName]
                    FROM [SsoProfiling].[dbo].[Resources]";
        var userName = _httpContext.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "Username").Value;
        _loggerManager.LogInfo($"Open Connection.... by: {userName}");
        using (var connection = _context.CreateConnection())
        {
            _loggerManager.LogInfo($"Executing Get Resource - Query: {query}");
            var resources = await connection.QueryAsync<Resource>(query);
            _loggerManager.LogInfo($"Get Resource Query Return : {JsonSerializer.Serialize(resources)}");
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
        _loggerManager.LogInfo($"Open Connection....");
        using (var connection = _context.CreateConnection())
        {
            _loggerManager.LogInfo($"Executing Get Resource - Query: {query} Parameter: {NormalizedName}");
            var resource = await connection.QueryFirstOrDefaultAsync<Resource>(query, new { NormalizedName });
            _loggerManager.LogInfo($"Get Resource By Name Query Return : {JsonSerializer.Serialize(resource)}");
            return resource;
        }
    }
}
