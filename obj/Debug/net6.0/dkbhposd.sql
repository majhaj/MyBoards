IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [Tags] (
    [Id] int NOT NULL IDENTITY,
    [Value] nvarchar(max) NULL,
    CONSTRAINT [PK_Tags] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Users] (
    [Id] uniqueidentifier NOT NULL,
    [FirstName] nvarchar(max) NULL,
    [LastName] nvarchar(max) NULL,
    [Email] nvarchar(max) NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [workItemStates] (
    [Id] int NOT NULL IDENTITY,
    [State] nvarchar(50) NOT NULL,
    CONSTRAINT [PK_workItemStates] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Addresses] (
    [Id] uniqueidentifier NOT NULL,
    [Country] nvarchar(max) NULL,
    [City] nvarchar(max) NULL,
    [Street] nvarchar(max) NULL,
    [PostalCode] nvarchar(max) NULL,
    [UserId] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_Addresses] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Addresses_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [WorkItems] (
    [Id] int NOT NULL IDENTITY,
    [Area] varchar(200) NULL,
    [Iteration_Path] nvarchar(max) NULL,
    [Priority] int NOT NULL DEFAULT 1,
    [AuthorId] uniqueidentifier NOT NULL,
    [StateId] int NOT NULL,
    [Discriminator] nvarchar(max) NOT NULL,
    [StartDate] datetime2 NULL,
    [EndDate] datetime2(3) NULL,
    [Efford] decimal(5,2) NULL,
    [Activity] nvarchar(200) NULL,
    [RemainingWork] decimal(14,2) NULL,
    CONSTRAINT [PK_WorkItems] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_WorkItems_Users_AuthorId] FOREIGN KEY ([AuthorId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_WorkItems_workItemStates_StateId] FOREIGN KEY ([StateId]) REFERENCES [workItemStates] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Comments] (
    [Id] int NOT NULL IDENTITY,
    [Message] nvarchar(max) NULL,
    [Author] nvarchar(max) NULL,
    [CreatedDate] datetime2 NOT NULL DEFAULT (getutcdate()),
    [UpdatedDate] datetime2 NULL,
    [WorkItemId] int NOT NULL,
    CONSTRAINT [PK_Comments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Comments_WorkItems_WorkItemId] FOREIGN KEY ([WorkItemId]) REFERENCES [WorkItems] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [WorkItemTag] (
    [WorkItemId] int NOT NULL,
    [TagId] int NOT NULL,
    [PublicationDate] datetime2 NOT NULL DEFAULT (getutcdate()),
    CONSTRAINT [PK_WorkItemTag] PRIMARY KEY ([TagId], [WorkItemId]),
    CONSTRAINT [FK_WorkItemTag_Tags_TagId] FOREIGN KEY ([TagId]) REFERENCES [Tags] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_WorkItemTag_WorkItems_WorkItemId] FOREIGN KEY ([WorkItemId]) REFERENCES [WorkItems] ([Id]) ON DELETE CASCADE
);
GO

CREATE UNIQUE INDEX [IX_Addresses_UserId] ON [Addresses] ([UserId]);
GO

CREATE INDEX [IX_Comments_WorkItemId] ON [Comments] ([WorkItemId]);
GO

CREATE INDEX [IX_WorkItems_AuthorId] ON [WorkItems] ([AuthorId]);
GO

CREATE INDEX [IX_WorkItems_StateId] ON [WorkItems] ([StateId]);
GO

CREATE INDEX [IX_WorkItemTag_WorkItemId] ON [WorkItemTag] ([WorkItemId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20221123094821_Init', N'7.0.0');
GO

COMMIT;
GO

