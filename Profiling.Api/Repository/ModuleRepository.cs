using System;
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

        using (var connection = _context.CreateConnection())
        {
            var id = await connection.QuerySingleAsync<int>(query, parameters);

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
        using (var connection = _context.CreateConnection())
        {
            var result = await connection.ExecuteAsync(query, new { NormalizedName, ResourceNameParameter });
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

        using (var connection = _context.CreateConnection())
        {
            var module = await connection.QueryFirstOrDefaultAsync<Module>(query, new { NormalizedName });
            Console.WriteLine(module?.ToString());
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

        using (var connection = _context.CreateConnection())
        {
            var resourceModules = await connection.QueryAsync<Module>(query, new { FilterResource });
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

        using (var connection = _context.CreateConnection())
        {
            var modules = await connection.QueryAsync<Module>(query);
            return modules.ToList();
        }
    }
}
