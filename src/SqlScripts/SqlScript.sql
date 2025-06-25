SELECT *
FROM SsoProfiling.dbo.[Module]


DELETE 
FROM [SsoProfiling].[dbo].[Resources]
WHERE Name = ;

DELETE 
FROM [SsoProfiling].[dbo].[Module]
WHERE [NormalizedName] =  AND [Resource] = ;

BEGIN TRANSACTION
insert into [SsoProfiling].[dbo].[SSOADM.USER_CREDS_TBL]
(
    [USER_ID], [USER_PW], [NUM_PWD_HISTORY],
    [PWD_HISTORY], [PWD_LAST_MOD_TIME], [NUM_PWD_ATTEMPTS], 
    [NEW_USER_FLG], [LOGIN_TIME_LOW], [LOGIN_TIME_HIGH], 
    [DISABLED_FROM_DATE], [DISABLED_UPTO_DATE], [PW_EXPY_DATE], 
    [ACCT_EXPY_DATE], [ACCT_INACTIVE_DAYS], [LAST_ACCESS_TIME], [TS_CNT]
) 
Values (
    @UserId, ., 0, 
    ., 1747874290113, 0, 
    N, CAST(@LoginTimeLow AS datetime2), CAST(@LoginTimeHigh AS datetime2),
    CAST(@DisabledFromDate AS datetime2), CAST(@DisabledUpToDate AS datetime2), CAST(@PasswordExpiryDate AS datetime2), 
    CAST(@AcctExpiryDate AS datetime2), @AcctInactiveDays, 1748519077222, 0
);
COMMIT;


insert into [SsoProfiling].[dbo].[SSOADM.USER_PROFILE_TBL]
(USER_ID, USER_NAME, DEL_FLG, HOME_ENTITY, DEFAULT_TZ, DEFAULT_CALENDAR, USER_MAX_INACTIVE_TIME, REQ_TWO_FACTOR_AUTH, IS_GLOBAL_ADMIN, TS_CNT)
Values (OLUAKIPBL0200, OLUMIDE AKINOLA, N, 01, WAT, G, 15, B, N, 0);

insert into [SsoProfiling].[dbo].[SSOADM.USER_PROFILE_TBL]
(USER_ID, USER_NAME, DEL_FLG, HOME_ENTITY, DEFAULT_TZ, DEFAULT_CALENDAR, USER_MAX_INACTIVE_TIME, REQ_TWO_FACTOR_AUTH, IS_GLOBAL_ADMIN, TS_CNT)
Values (@UserId, @UserName, 'N', '01', 'WAT', 'G', @UserMaxInactiveTime, @RequireTwoFactorAuth, 'N', 0);
 
COMMIT;

Insert into [SsoProfiling].[dbo].[SSOADM.SSO_RESOURCE_ACCESS_TBL]
(USER_ID, ENTITY_ID, RES_ID, IS_DEFAULT) 
Values (@UserId, '01', @Resource, 'N');


insert into [SsoProfiling].[dbo].[SSOADM.SSO_MODULE_ACCESS_TBL]
(USER_ID, ENTITY_ID, RES_ID, MODULE_ID) 
Values (@UserId, '01', @Resource, @Module);

SELECT USER_ID FROM [SSOADM.USER_CREDS_TBL] WHERE USER_ID = '';

SELECT RES_ID As ResourceName, MODULE_ID As ModuleName
FROM [SsoProfiling].[dbo].[SSOADM.SSO_MODULE_ACCESS_TBL]
WHERE USER_ID = 'OLUAKIPBL0202'


SELECT [value]
FROM string_split(@UserIds, ',') S
LEFT JOIN [SsoProfiling].[dbo].[SSOADM.USER_CREDS_TBL] U
ON U.user_id = s.[value]
WHERE u.user_id is NULL

DELETE
FROM [SsoProfiling].[dbo].[SSOADM.SSO_MODULE_ACCESS_TBL]
WHERE USER_ID = @UserId AND RES_ID = @Resource AND MODULE_ID = @Module;


DELETE 
FROM [SsoProfiling].[dbo].[SSOADM.SSO_RESOURCE_ACCESS_TBL]
WHERE USER_ID = @UserId AND RES_ID = @Resource;


SELECT *
FROM [SsoProfiling].[dbo].[SSOADM.USER_PROFILE_TBL]

UPDATE [SsoProfiling].[dbo].[SSOADM.USER_PROFILE_TBL]
SET DEL_FLG = 'Y'
WHERE USER_ID = '';