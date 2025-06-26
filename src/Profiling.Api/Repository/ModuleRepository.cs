using System;
using System.Text.Json;
using Dapper;
using LoggerService;
using Profiling.Api.Context;
using Profiling.Api.Contracts;
using Profiling.Api.DataTransferObjects;
using Profiling.Api.Entities;

namespace Profiling.Api.Repository;

public class ModuleRepository : IModuleRepository
{
    private readonly DataContext _context;
    private readonly ILoggerManager _loggerManager;

    public ModuleRepository(DataContext context, ILoggerManager loggerManager)
    {
        _context = context;
        _loggerManager = loggerManager;
    }

    public async Task<Module> Create(CreateModuleDto NewModule)
    {
        var query = @"INSERT INTO [dbo].[Module]
                    (
                    [Name], [NormalizedName], [Resource]
                    )
                    VALUES
                    (
                    @Name, @NormalizedName, @ResourceName
                    );" + "\n" +
                    "SELECT CAST(SCOPE_IDENTITY() AS int);";
        var parameters = new DynamicParameters();

        parameters.Add("Name", NewModule.Name, System.Data.DbType.String);
        parameters.Add("NormalizedName", NewModule.Name.ToUpper(), System.Data.DbType.String);
        parameters.Add("ResourceName", NewModule.ResourceName.ToUpper(), System.Data.DbType.String);
        _loggerManager.LogInfo($"Open Connection....");

        using (var connection = _context.CreateConnection())
        {
            _loggerManager.LogInfo($"Executing Module Create query - {query}, Parameter: {JsonSerializer.Serialize(parameters)}");
            var id = await connection.QuerySingleAsync<int>(query, parameters);
            _loggerManager.LogInfo($"Module Create Query Return : {id}");

            return new Module { Id = id, Name = NewModule.Name, NormalizedName = NewModule.Name.ToUpper(), Resource = NewModule.ResourceName };
        }

    }

    public async Task<int> Delete(DeleteModuleDto moduleToDelete)
    {
        var NormalizedName = moduleToDelete.Name.ToUpper();
        var ResourceNameParameter = moduleToDelete.ResourceName.ToUpper();

        var query = @"DELETE 
                        FROM [SsoProfiling].[dbo].[Module]
                        WHERE [NormalizedName] = @NormalizedName AND [Resource] = @ResourceNameParameter;";
                        
        _loggerManager.LogInfo($"Open Connection....");
        using (var connection = _context.CreateConnection())
        {
            _loggerManager.LogInfo($"Execiuting Delete Module: {query} - Parameters: {JsonSerializer.Serialize(moduleToDelete)}");
            var result = await connection.ExecuteAsync(query, new { NormalizedName, ResourceNameParameter });
            _loggerManager.LogInfo($"Returns: {result}");
            return result;
        }
    }

    public async Task<Module?> GetByName(string Name)
    {

        var NormalizedName = Name.ToUpper();

        var query = @"SELECT [Id]
                            ,[Resource]
                            ,[Name]
                            ,[NormalizedName]
                        FROM [SsoProfiling].[dbo].[Module]
                        WHERE [NormalizedName] = @NormalizedName";
        _loggerManager.LogInfo($"Open Connection....");
        using (var connection = _context.CreateConnection())
        {
            _loggerManager.LogInfo($"Executing Get Module By Name - Query: {query} Parameters: {Name}");
            var module = await connection.QueryFirstOrDefaultAsync<Module>(query, new { NormalizedName });
             _loggerManager.LogInfo($"Returns: {JsonSerializer.Serialize(module)}");
            // Console.WriteLine(module?.ToString());
            return module;
        }
    }

    public async Task<IEnumerable<Module>> GetModuleByResource(string ResourceName)
    {
        var FilterResource = ResourceName.ToUpper();
        var query = @"SELECT TOP (1000) [Id]
                        ,[Resource]
                        ,[Name]
                        ,[NormalizedName]
                    FROM [SsoProfiling].[dbo].[Module]
                    WHERE Resource = @FilterResource";
        
        _loggerManager.LogInfo($"Open Connection....");

        using (var connection = _context.CreateConnection())
        {
            _loggerManager.LogInfo($"Executing Module By Resource - Query: {query} Parameter: {ResourceName}");
            var resourceModules = await connection.QueryAsync<Module>(query, new { FilterResource });
            _loggerManager.LogInfo($"Returns: {JsonSerializer.Serialize(resourceModules)}");
            return resourceModules;
        }
    }

    public async Task<IEnumerable<Module>> GetModules()
    {
        var query = @"SELECT [Id]
                        ,[Resource]
                        ,[Name]
                        ,[NormalizedName]
                    FROM [SsoProfiling].[dbo].[Module]";
        
        _loggerManager.LogInfo($"Open Connection....");

        using (var connection = _context.CreateConnection())
        {
            _loggerManager.LogInfo($"Executing Get Modules - Query: {query}");
            var modules = await connection.QueryAsync<Module>(query);
            _loggerManager.LogInfo($"Returns: {JsonSerializer.Serialize(modules)}");
            return modules.ToList();
        }
    }
}
