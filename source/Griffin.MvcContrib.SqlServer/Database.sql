CREATE TABLE [dbo].[LocalizedTypes] (
	[Id] int NOT NULL IDENTITY, 
	[LocaleId] int NOT NULL, 
	[Key] nvarchar(250) NOT NULL, 
	[TypeName] nvarchar(255) NOT NULL, 
	[TextName] nvarchar(250) NOT NULL, 
	[UpdatedAt] datetime2(7) NOT NULL, 
	[UpdatedBy] nvarchar(50) NOT NULL, 
	[Value] text NOT NULL
)
GO


CREATE TABLE [dbo].[LocalizedViews] (
	[Id] int NOT NULL IDENTITY, 
	[LocaleId] int NOT NULL, 
	[Key] nvarchar(50) NOT NULL, 
	[ViewPath] nvarchar(255) NOT NULL, 
	[TextName] text NOT NULL, 
	[Value] text NOT NULL, 
	[UpdatedAt] datetime2(7) NOT NULL, 
	[UpdatedBy] nvarchar(50) NOT NULL
)
GO

/** SqlExpress:

CREATE TABLE LocalizedTypes(
	Id int IDENTITY(1,1) NOT NULL,
	LocaleId int NOT NULL,
	[Key] nvarchar(250) NOT NULL,
	TypeName nvarchar(255) NOT NULL,
	TextName nvarchar(250) NOT NULL,
	UpdatedAt datetime NOT NULL,
	UpdatedBy nvarchar(50) NOT NULL,
	Value nvarchar(2000) NOT NULL
);


CREATE TABLE LocalizedViews(
	Id int IDENTITY(1,1) NOT NULL,
	LocaleId int NOT NULL,
	[Key] nvarchar(50) NOT NULL,
	ViewPath nvarchar(255) NOT NULL,
	TextName nvarchar(2000) NOT NULL,
	Value nvarchar(2000) NOT NULL,
	UpdatedAt datetime NOT NULL,
	UpdatedBy nvarchar(50) NOT NULL
);
*/