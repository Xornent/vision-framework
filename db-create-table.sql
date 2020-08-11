CREATE TABLE [dbo].[Category] (
    [Id]    INT  IDENTITY (1, 1) NOT NULL,
    [Name]  TEXT COLLATE CHINESE_PRC_CI_AS NOT NULL,
    [Pages] TEXT COLLATE CHINESE_PRC_CI_AS NOT NULL,
    [Alias] TEXT COLLATE CHINESE_PRC_CI_AS NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

CREATE TABLE [dbo].[Page] (
    [Hash]  NCHAR (32)        COLLATE Chinese_PRC_CI_AS NOT NULL,
    [Id]    INT               IDENTITY (1, 1) NOT NULL,
    [Title] NVARCHAR (50)     COLLATE Chinese_PRC_CI_AS NOT NULL,
    [Namespace] NVARCHAR (50) COLLATE Chinese_PRC_CI_AS NOT NULL,
    [Level] INT               NOT NULL,
    CONSTRAINT [PK_Page] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
CREATE NONCLUSTERED INDEX [Column_Hash]
    ON [dbo].[Page]([Hash] ASC);

CREATE TABLE [dbo].[Record] (
    [Id]       INT  IDENTITY (1, 1) NOT NULL,
    [History]  TEXT COLLATE CHINESE_PRC_CI_AS NOT NULL,
    [Category] TEXT COLLATE CHINESE_PRC_CI_AS NOT NULL,
    [Use]      INT  NOT NULL,
    [Body]     TEXT COLLATE CHINESE_PRC_CI_AS NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

CREATE TABLE [dbo].[User] (
    [Id]         INT           IDENTITY (1, 1) NOT NULL,
    [Level]      INT           NOT NULL,
    [Display]    NVARCHAR (50) COLLATE Chinese_PRC_CI_AS NOT NULL,
    [Contact]    TEXT          COLLATE Chinese_PRC_CI_AS NOT NULL,
    [Edit]       TEXT          COLLATE Chinese_PRC_CI_AS NOT NULL,
    [Evaluation] INT           NOT NULL,
    [Banned]     INT           DEFAULT ((0)) NOT NULL,
    [Password]   NVARCHAR (50) COLLATE Chinese_PRC_CI_AS NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);
