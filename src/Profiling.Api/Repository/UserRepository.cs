using System.Data;
using System.Text.Json;
using Dapper;
using LoggerService;
using Profiling.Api.Context;
using Profiling.Api.Contracts;
using Profiling.Api.DataTransferObjects;

namespace Profiling.Api.Repository;

public class UserRepository : IUserRepository
{
    private readonly DataContext _context;
    private readonly ILoggerManager _loggerManager;

    public UserRepository(DataContext context, ILoggerManager loggerManager)
    {
        _context = context;
        _loggerManager = loggerManager;
    }

    public async Task<string> AddUserModule(List<AddUserModuleDto> userModules)
    {

        var UserIds = userModules.Select(x => x.UserId.ToUpper()).ToList();
        _loggerManager.LogInfo($"Checking If All Users Exists: {JsonSerializer.Serialize(UserIds)}");
        var NotExistingUserIds = await CheckUsersExists(UserIds);

        if (NotExistingUserIds.Count() > 0)
        {
            _loggerManager.LogInfo($"Users does not exist {JsonSerializer.Serialize(NotExistingUserIds)}");
            return $"The following Users does not exist: {string.Join(" , ", NotExistingUserIds)}";
        }

        var query = @"insert into [SsoProfiling].[dbo].[SSOADM.SSO_MODULE_ACCESS_TBL]
                        (USER_ID, ENTITY_ID, RES_ID, MODULE_ID) 
                        Values (@UserId, '01', @Resource, @Module);";

        int maxRetry = 3; int retryCount = 0;
        List<Exception> ConnectionExceptions = [];
        _loggerManager.LogInfo($"Open Connection....");
        using (var connection = _context.CreateConnection())
        {

            while (retryCount < maxRetry)
            {
                try
                {
                    connection.Open();
                    if (connection.State == ConnectionState.Open)
                        break;
                }
                catch (Exception ex)
                {
                    retryCount++;
                    ConnectionExceptions.Add(ex);

                }
            }

            if (connection.State != ConnectionState.Open)
            {
                _loggerManager.LogInfo($"Connection Open Unsuccessful.....{JsonSerializer.Serialize(ConnectionExceptions)}");
                throw new ArgumentException($"Error Occurred Opening Connection: {JsonSerializer.Serialize(ConnectionExceptions)}");
            }

            using (var transaction = connection.BeginTransaction())
            {
                _loggerManager.LogInfo($"Begin Transaction For: {JsonSerializer.Serialize(userModules)}");
                try
                {
                    foreach (var module in userModules)
                    {
                        DynamicParameters parameters = new DynamicParameters();
                        parameters.Add("Resource", module.ResourceName.ToUpper(), DbType.String);
                        parameters.Add("UserId", module.UserId.ToUpper(), DbType.String);
                        parameters.Add("Module", module.ModuleName.ToUpper(), DbType.String);

                        _loggerManager.LogInfo($"Executing Query To Add Module - Query: {query}, Parameter: {JsonSerializer.Serialize(parameters)}");

                        var result = await connection.ExecuteAsync(query, parameters, transaction: transaction);
                    }

                    _loggerManager.LogInfo($"Commit Transactions");
                    transaction.Commit();

                    return $"Modules Added Successfully.";
                }
                catch (Exception ex)
                {
                    _loggerManager.LogError($"Error Adding User Modules - {JsonSerializer.Serialize(userModules)} - Error: {JsonSerializer.Serialize(ex)}");
                    transaction.Rollback();
                    throw;
                }
            }
        }
    }

    public async Task<string> AddUserResource(List<AddUserResourceDto> userResources)
    {

        var UserIds = userResources.Select(x => x.UserId.ToUpper()).ToList();
        _loggerManager.LogInfo($"Checking If All Users Exists: {JsonSerializer.Serialize(UserIds)}");

        var NotExistingUserIds = await CheckUsersExists(UserIds);

        if (NotExistingUserIds.Count() > 0)
        {
            _loggerManager.LogInfo($"Users does not exist {JsonSerializer.Serialize(NotExistingUserIds)}");
            return $"The following Users does not exist: {string.Join(" , ", NotExistingUserIds)}";
        }


        var query = @"Insert into [SsoProfiling].[dbo].[SSOADM.SSO_RESOURCE_ACCESS_TBL]
                        (USER_ID, ENTITY_ID, RES_ID, IS_DEFAULT) 
                        Values (@UserId, '01', @Resource, 'N');";

        int maxRetry = 3; int retryCount = 0;
        List<Exception> ConnectionExceptions = [];
        _loggerManager.LogInfo($"Open Connection....");
        using (var connection = _context.CreateConnection())
        {

            while (retryCount < maxRetry)
            {
                try
                {
                    connection.Open();
                }
                catch (Exception ex)
                {
                    retryCount++;
                    ConnectionExceptions.Add(ex);
                }
            }

            if (connection.State != ConnectionState.Open)
            {
                _loggerManager.LogInfo($"Connection Open Unsuccessful.....{JsonSerializer.Serialize(ConnectionExceptions)}");
                throw new ArgumentException($"Error Occurred Opening Connection: {JsonSerializer.Serialize(ConnectionExceptions)}");
            }

            _loggerManager.LogInfo($"Begin Transaction For: {JsonSerializer.Serialize(userResources)}");
            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    foreach (var resource in userResources)
                    {

                        DynamicParameters parameters = new DynamicParameters();
                        parameters.Add("Resource", resource.ResourceName.ToUpper(), DbType.String);
                        parameters.Add("UserId", resource.UserId.ToUpper(), DbType.String);
                        
                        _loggerManager.LogInfo($"Executing Query To Add Module - Query: {query}, Parameter: {JsonSerializer.Serialize(parameters)}");

                        await connection.ExecuteAsync(query, parameters, transaction);
                    }

                    _loggerManager.LogInfo($"Commit Transactions");

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    _loggerManager.LogError($"Error Adding User Resources - {JsonSerializer.Serialize(userResources)} - Error: {JsonSerializer.Serialize(ex)}");
                    transaction.Rollback();
                    throw;
                }
            }

            return $"Resources Added Successfully";
        }
    }

    public async Task<string> Create(CreateUserDto NewUser)
    {


        var AddToProfileTblQuery = @"insert into [SsoProfiling].[dbo].[SSOADM.USER_PROFILE_TBL]
                                    (USER_ID, USER_NAME, DEL_FLG, HOME_ENTITY, DEFAULT_TZ, DEFAULT_CALENDAR, USER_MAX_INACTIVE_TIME, REQ_TWO_FACTOR_AUTH, IS_GLOBAL_ADMIN, TS_CNT)
                                    Values (@UserId, @UserName, 'N', '01', 'WAT', 'G', @UserMaxInactiveTime, @RequireTwoFactorAuth, @IsGlobalAdmin, 0);
                                ";
        var AddToCredTblQuery = @"insert into [SsoProfiling].[dbo].[SSOADM.USER_CREDS_TBL]
                            (
                                [USER_ID], [USER_PW], [NUM_PWD_HISTORY],
                                [PWD_HISTORY], [PWD_LAST_MOD_TIME], [NUM_PWD_ATTEMPTS], 
                                [NEW_USER_FLG], [LOGIN_TIME_LOW], [LOGIN_TIME_HIGH], 
                                [DISABLED_FROM_DATE], [DISABLED_UPTO_DATE], [PW_EXPY_DATE], 
                                [ACCT_EXPY_DATE], [ACCT_INACTIVE_DAYS], [LAST_ACCESS_TIME], [TS_CNT]
                            ) 
                            Values (
                                @UserId, '.', 0, 
                                '.', 1747874290113, 0, 
                                'N', CAST(@LoginTimeLow AS datetime2), CAST(@LoginTimeHigh AS datetime2),
                                CAST(@DisabledFromDate AS datetime2), CAST(@DisabledUpToDate AS datetime2), CAST(@PasswordExpiryDate AS datetime2), 
                                CAST(@AcctExpiryDate AS datetime2), @AcctInactiveDays, 1748519077222, 0
                            );";
        
        var AddReqourceQuery = @"Insert into [SsoProfiling].[dbo].[SSOADM.SSO_RESOURCE_ACCESS_TBL]
                                    (USER_ID, ENTITY_ID, RES_ID, IS_DEFAULT) 
                                    Values (@UserId, '01', @Resource, 'N');";

        var AddModuleQuery = @"insert into [SsoProfiling].[dbo].[SSOADM.SSO_MODULE_ACCESS_TBL]
                                (USER_ID, ENTITY_ID, RES_ID, MODULE_ID) 
                                Values (@UserId, '01', @Resource, @Module);";

        _loggerManager.LogInfo($"Create Parameters......");

        var UserCredTblParameters = new DynamicParameters();
        var UserProfileTblParameters = new DynamicParameters();

        UserCredTblParameters.Add("UserId", NewUser.UserId.ToUpper(), DbType.String);
        UserCredTblParameters.Add("LoginTimeLow", NewUser.LoginTimeLow.ToDateTime(TimeOnly.MinValue), DbType.DateTime);
        UserCredTblParameters.Add("LoginTimeHigh", NewUser.LoginTimeHigh.ToDateTime(TimeOnly.MinValue), DbType.DateTime);
        UserCredTblParameters.Add("DisabledFromDate", NewUser.DisabledFromDate.ToDateTime(TimeOnly.MinValue), DbType.DateTime);
        UserCredTblParameters.Add("DisabledUpToDate", NewUser.DisabledUpToDate.ToDateTime(TimeOnly.MinValue), DbType.DateTime);
        UserCredTblParameters.Add("PasswordExpiryDate", NewUser.PasswordExpiryDate.ToDateTime(TimeOnly.MinValue), DbType.DateTime);
        UserCredTblParameters.Add("AcctExpiryDate", NewUser.AcctExpiryDate.ToDateTime(TimeOnly.MinValue), DbType.DateTime);
        UserCredTblParameters.Add("AcctInactiveDays", NewUser.AcctInactiveDays, DbType.Int16);


        UserProfileTblParameters.Add("UserId", NewUser.UserId.ToUpper(), DbType.String);
        UserProfileTblParameters.Add("UserName", NewUser.UserName.ToUpper(), DbType.String);
        UserProfileTblParameters.Add("UserMaxInactiveTime", NewUser.UserMaxInactiveTime, DbType.Int16);
        UserProfileTblParameters.Add("RequireTwoFactorAuth", NewUser.Require2FA, DbType.String);
        UserProfileTblParameters.Add("IsGlobalAdmin", NewUser.IsGlobalAdmin, DbType.String);

        _loggerManager.LogInfo($"Open Connection....");
        using (var connection = _context.CreateConnection())
        {
            begin:
            connection.Open();
            if (connection.State != ConnectionState.Open)
                goto begin;

            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    _loggerManager.LogInfo($"Executing Query To Add User To [SSOADM.USER_CREDS_TBL] - Query: {AddToCredTblQuery}, Parameter: {JsonSerializer.Serialize(UserCredTblParameters)}");
                    var result = await connection.ExecuteAsync(AddToCredTblQuery, UserCredTblParameters, transaction: transaction);
                    _loggerManager.LogInfo($"Executing Query To Add User To [SSOADM.USER_PROFILE_TBL] - Query: {AddToProfileTblQuery}, Parameter: {JsonSerializer.Serialize(UserProfileTblParameters)}");
                    var result2 = await connection.ExecuteAsync(AddToProfileTblQuery, UserProfileTblParameters, transaction: transaction);

                    _loggerManager.LogInfo($"Query Returns - [SSOADM.USER_CREDS_TBL]:{result}, [SSOADM.USER_PROFILE_TBL]:{result2}");

                    foreach (var resource in NewUser.Resources)
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("Resource", resource.ResourceName.ToUpper(), DbType.String);
                        parameters.Add("UserId", resource.UserId.ToUpper(), DbType.String);

                        _loggerManager.LogInfo($"Executing Query To Add Module To Created User - Query: {AddReqourceQuery}, Parameter: {JsonSerializer.Serialize(parameters)}");

                        await connection.ExecuteAsync(AddReqourceQuery, parameters, transaction: transaction);
                    }

                    foreach (var Module in NewUser.Modules)
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("UserId", Module.UserId.ToUpper(), DbType.String);
                        parameters.Add("Resource", Module.ResourceName.ToUpper(), DbType.String);
                        parameters.Add("Module", Module.ModuleName, DbType.String);
                        
                        _loggerManager.LogInfo($"Executing Query To Add Module To Created User - Query: {AddModuleQuery}, Parameter: {JsonSerializer.Serialize(parameters)}");

                        await connection.ExecuteAsync(AddModuleQuery, parameters, transaction: transaction);
                    }

                    _loggerManager.LogInfo($"Commit Transactions");

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    _loggerManager.LogError($"Error Regster User - {JsonSerializer.Serialize(NewUser)} - Error: {JsonSerializer.Serialize(ex)}");
                    transaction.Rollback();
                    throw;
                }
            }

            return NewUser.UserId.ToUpper();

        }
    }

    public async Task<string?> GetUser(string UserId)
    {
        var query = @"SELECT USER_ID FROM [SSOADM.USER_CREDS_TBL] WHERE USER_ID = @UserId;";

        _loggerManager.LogInfo($"Open Connection....");
        using (var connection = _context.CreateConnection())
        {
            _loggerManager.LogInfo($"Get User - query: {query}, parameter; {UserId}");
            var userId = await connection.QueryFirstOrDefaultAsync<string>(query, new { UserId });
            _loggerManager.LogInfo($"Get User Returns: {userId}");
            return userId;
        }
    }

    public async Task<List<ModuleDto>> GetUserModules(string UserId)
    {
        var query = @"SELECT RES_ID As ResourceName, MODULE_ID As ModuleName
                        FROM [SsoProfiling].[dbo].[SSOADM.SSO_MODULE_ACCESS_TBL]
                        WHERE USER_ID = UPPER(@UserId)";

        _loggerManager.LogInfo($"Open Connection....");
        using (var connection = _context.CreateConnection())
        {
            _loggerManager.LogInfo($"Get User Modules - query: {query}, parameter; {UserId}");
            var UserModules = await connection.QueryAsync<ModuleDto>(query, new { UserId });
            _loggerManager.LogInfo($"Get User Modules Returns: {JsonSerializer.Serialize(UserModules)}");
            return UserModules.ToList();
        }
    }

    public async Task<List<ResourceDto>> GetUserResources(string UserId)
    {
        var query = @"SELECT RES_ID AS ResourceName
                        FROM [SsoProfiling].[dbo].[SSOADM.SSO_RESOURCE_ACCESS_TBL]
                        WHERE USER_ID = UPPER(@UserId)";

        _loggerManager.LogInfo($"Open Connection....");
        using (var connection = _context.CreateConnection())
        {
            _loggerManager.LogInfo($"Get User Resources - query: {query}, parameter; {UserId}");
            var UserResources = await connection.QueryAsync<ResourceDto>(query, new { UserId });
            _loggerManager.LogInfo($"Get User Resources Returns: {JsonSerializer.Serialize(UserResources)}");
            return UserResources.ToList();
        }

    }

    public async Task<string> RemoveUserModuleAccess(List<AddUserModuleDto> userModulesToDelete)
    {
        var UserIds = userModulesToDelete.Select(x => x.UserId.ToUpper());
        var UsersNotExist = await CheckUsersExists(UserIds.ToList());

        if (UsersNotExist.Count() > 0)
        {
            _loggerManager.LogInfo($"Users does not exist {JsonSerializer.Serialize(UsersNotExist)}");
            return $"The following Users does not exist: {string.Join(" , ", UsersNotExist)}";
        }

        var query = @"DELETE
                        FROM [SsoProfiling].[dbo].[SSOADM.SSO_MODULE_ACCESS_TBL]
                        WHERE USER_ID = @UserId AND RES_ID = @Resource AND MODULE_ID = @Module;";

        int maxRetry = 3; int retryCount = 0;
        List<Exception> ConnectionExceptions = [];

        _loggerManager.LogInfo($"Open Connection....");
        using (var connection = _context.CreateConnection())
        {
            while (retryCount < maxRetry)
            {
                try
                {
                    connection.Open();
                    if (connection.State == ConnectionState.Open)
                        break;
                }
                catch (Exception ex)
                {
                    retryCount++;
                    ConnectionExceptions.Add(ex);
                }
            }

            if (connection.State != ConnectionState.Open)
            {
                _loggerManager.LogInfo($"Connection Open Unsuccessful.....{JsonSerializer.Serialize(ConnectionExceptions)}");
                throw new ArgumentException($"Error Occurred Opening Connection: {JsonSerializer.Serialize(ConnectionExceptions)}");
            }

            _loggerManager.LogInfo($"Begin Transaction For: {JsonSerializer.Serialize(userModulesToDelete)}");
            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    foreach (var module in userModulesToDelete)
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("UserId", module.UserId.ToUpper(), DbType.String);
                        parameters.Add("Resource", module.ResourceName.ToUpper(), DbType.String);
                        parameters.Add("Module", module.ModuleName.ToUpper(), DbType.String);

                        _loggerManager.LogInfo($"Executing Query To Add Module - Query: {query}, Parameter: {JsonSerializer.Serialize(parameters)}");

                        await connection.ExecuteAsync(query, parameters, transaction: transaction);

                    }
                    
                    _loggerManager.LogInfo($"Commit Transactions");
                    transaction.Commit();

                }
                catch (Exception ex)
                {
                    _loggerManager.LogError($"Error Removing User Modules - {JsonSerializer.Serialize(userModulesToDelete)} - Error: {JsonSerializer.Serialize(ex)}");
                    transaction.Rollback();
                    throw;
                }
            }

            return $"Module(s) Removed Successfully";
        }

    }


    public async Task<string> RemoveUserResourceAccess(List<AddUserResourceDto> userResourcesToDelete)
    {
        var UserIds = userResourcesToDelete.Select(x => x.UserId.ToUpper()).ToList();

        _loggerManager.LogInfo($"Checking If All Users Exists: {JsonSerializer.Serialize(UserIds)}");

        var NotExistingUserIds = await CheckUsersExists(UserIds);

        if (NotExistingUserIds.Count() > 0)
        {
            _loggerManager.LogInfo($"Users does not exist {JsonSerializer.Serialize(NotExistingUserIds)}");
            return $"The following Users does not exist: {string.Join(" , ", NotExistingUserIds)}";
        }

        var query = @"DELETE 
                        FROM [SsoProfiling].[dbo].[SSOADM.SSO_RESOURCE_ACCESS_TBL]
                        WHERE USER_ID = @UserId AND RES_ID = @Resource;";

        int maxRetry = 3; int retryCount = 0;
        List<Exception> ConnectionExceptions = [];

        _loggerManager.LogInfo($"Open Connection....");
        using (var connection = _context.CreateConnection())
        {
            while (retryCount < maxRetry)
            {
                try
                {
                    connection.Open();
                    if (connection.State == ConnectionState.Open)
                        break;
                }
                catch (Exception ex)
                {
                    retryCount++;
                    ConnectionExceptions.Add(ex);

                }
            }

            if (connection.State != ConnectionState.Open)
            {
                _loggerManager.LogInfo($"Connection Open Unsuccessful.....{JsonSerializer.Serialize(ConnectionExceptions)}");
                throw new ArgumentException($"Error Occurred Opening Connection: {ConnectionExceptions.ToString()}");
            }

            _loggerManager.LogInfo($"Begin Transaction For: {JsonSerializer.Serialize(userResourcesToDelete)}");

            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    foreach (var resource in userResourcesToDelete)
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("UserId", resource.UserId.ToUpper(), DbType.String);
                        parameters.Add("Resource", resource.ResourceName.ToUpper(), DbType.String);

                        _loggerManager.LogInfo($"Executing Query To Add Module - Query: {query}, Parameter: {JsonSerializer.Serialize(parameters)}");

                        await connection.QueryAsync(query, parameters, transaction: transaction);
                    }

                    _loggerManager.LogInfo($"Commit Transactions");

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    _loggerManager.LogError($"Error Adding User Resources - {JsonSerializer.Serialize(userResourcesToDelete)} - Error: {JsonSerializer.Serialize(ex)}");
                    transaction.Rollback();
                    throw;
                }
            }

            return $"Resource(s) Removed Successfully";

        }
    }

















    public async Task<string> DeleteUser(string UserId)
    {
        var query = @"UPDATE [SsoProfiling].[dbo].[SSOADM.USER_PROFILE_TBL]
                        SET DEL_FLG = 'Y'
                        WHERE USER_ID = @UserId;";
        _loggerManager.LogInfo($"Open Connection....");
        using (var connection = _context.CreateConnection())
        {
            _loggerManager.LogInfo($"Executing Delete User - Query: {query} : User Id: {UserId}");
            var numberOfRowsAffected = await connection.ExecuteAsync(query, new { UserId });
             _loggerManager.LogInfo($"Delete User Returnes - User Id: {UserId}, Result: {numberOfRowsAffected}");
            return numberOfRowsAffected > 0 ? $"User Deletion Successful" : $"User with Id: {UserId} does not exist";
        }
    }

    public async Task<string> UnDeleteUser(string UserId)
    {
        var query = @"UPDATE [SsoProfiling].[dbo].[SSOADM.USER_PROFILE_TBL]
                        SET DEL_FLG = 'N'
                        WHERE USER_ID = @UserId;";
        _loggerManager.LogInfo($"Open Connection....");
        using (var connection = _context.CreateConnection())
        {
            _loggerManager.LogInfo($"Executing UnDelete User - Query: {query} : User Id: {UserId}");

            var numberOfRowsAffected = await connection.ExecuteAsync(query, new { UserId });

            _loggerManager.LogInfo($"UnDelete User Returnes - User Id: {UserId}, Result: {numberOfRowsAffected}");

            return numberOfRowsAffected > 0 ? $"User UnDeletion Successful" : $"User with Id: {UserId} does not exist";
        }
    }
    
    public async Task<List<string>> CheckUsersExists(List<string> InputUserIds)
    {
        var query = @"SELECT [value]
                        FROM string_split(@UserIds, ',') S
                        LEFT JOIN [SsoProfiling].[dbo].[SSOADM.USER_CREDS_TBL] U
                        ON U.user_id = S.[value]
                        WHERE u.user_id is NULL";
        var UserIds = string.Join(",", InputUserIds);
        _loggerManager.LogInfo($"Open Connection....");
        using (var connection = _context.CreateConnection())
        {
            _loggerManager.LogInfo($"Execute User Validation Query - Query: {query}, Parameters: {JsonSerializer.Serialize(InputUserIds)}");
            var NotExists = await connection.QueryAsync<string>(query, new { UserIds });
            return NotExists.ToList();
        }
    }
}
