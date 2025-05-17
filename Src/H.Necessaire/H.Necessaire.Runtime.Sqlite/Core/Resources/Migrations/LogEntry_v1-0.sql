CREATE TABLE [H.Necessaire.Log] (

	ID TEXT NOT NULL,
	[Level] INTEGER NOT NULL,
	LevelLabel TEXT NOT NULL,
	ScopeID TEXT NOT NULL,
	OperationContextJson TEXT,
	HappenedAt TEXT NOT NULL,
	HappenedAtTicks INTEGER NOT NULL,
	[Message] TEXT,
	Method TEXT,
	StackTrace TEXT,
	Component TEXT,
	[Application] TEXT,
	ExceptionJson TEXT,
	PayloadJson TEXT,
	NotesJson TEXT,
	AppVersionJson TEXT,
	AppVersionNumber TEXT,
	AppVersionTimestamp TEXT,
	AppVersionBranch TEXT,
	AppVersionCommit TEXT,
	
	PRIMARY KEY(ID)
);

CREATE INDEX [IX_H.Necessaire.Log_Level] ON [H.Necessaire.Log]
(
	[Level]
);

CREATE INDEX [IX_H.Necessaire.Log_LevelLabel] ON [H.Necessaire.Log]
(
	LevelLabel
);

CREATE INDEX [IX_H.Necessaire.Log_ScopeID] ON [H.Necessaire.Log]
(
	ScopeID
);

CREATE INDEX [IX_H.Necessaire.Log_HappenedAt] ON [H.Necessaire.Log]
(
	HappenedAt
);

CREATE INDEX [IX_H.Necessaire.Log_HappenedAtTicks] ON [H.Necessaire.Log]
(
	HappenedAtTicks
);

CREATE INDEX [IX_H.Necessaire.Log_Method] ON [H.Necessaire.Log]
(
	Method
);

CREATE INDEX [IX_H.Necessaire.Log_Component] ON [H.Necessaire.Log]
(
	Component
);

CREATE INDEX [IX_H.Necessaire.Log_Application] ON [H.Necessaire.Log]
(
	[Application]
);

CREATE INDEX [IX_H.Necessaire.Log_AppVersionNumber] ON [H.Necessaire.Log]
(
	AppVersionNumber
);

CREATE INDEX [IX_H.Necessaire.Log_AppVersionTimestamp] ON [H.Necessaire.Log]
(
	AppVersionTimestamp
);

CREATE INDEX [IX_H.Necessaire.Log_AppVersionBranch] ON [H.Necessaire.Log]
(
	AppVersionBranch
);

CREATE INDEX [IX_H.Necessaire.Log_AppVersionCommit] ON [H.Necessaire.Log]
(
	AppVersionCommit
);