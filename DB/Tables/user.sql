CREATE TABLE [dbo].[user]
(
	[UserId] INT NOT NULL PRIMARY KEY,
	[Email] NVARCHAR(320) NOT NULL,
	[Password]                NVARCHAR (320) NOT NULL,
    [RunnerId]                INT NOT NULL,
    [Role]                      NVARCHAR (320) NOT NULL,
    [AccessToken]               NVARCHAR (1000) NULL,
    [MaxRefreshTokenExpiration] DATETIME2 (7)  NULL,
    [RefreshToken]              NVARCHAR (44) NULL,
    [RefreshTokenExpiration]    DATETIME2 (7)  NULL
)
