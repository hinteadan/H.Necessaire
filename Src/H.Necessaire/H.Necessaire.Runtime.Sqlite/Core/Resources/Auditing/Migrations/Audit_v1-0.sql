CREATE TABLE [H.Necessaire.Audit] (

	ID TEXT NOT NULL,
	AuditedObjectType TEXT NOT NULL,
	AuditedObjectID TEXT NOT NULL,
	HappenedAt TEXT NOT NULL,
	HappenedAtTicks INTEGER NOT NULL,
	DoneByID TEXT,
	DoneByIDTag TEXT,
	DoneByDisplayName TEXT,
	DoneByJson TEXT,
	ActionType INTEGER NOT NULL,
	ActionTypeLabel TEXT NOT NULL,
	AppVersionJson TEXT,
	AppVersionNumber TEXT,
	AppVersionTimestamp TEXT,
	AppVersionBranch TEXT,
	AppVersionCommit TEXT,
	
	PRIMARY KEY(ID)
);

CREATE INDEX [IX_H.Necessaire.Audit_AuditedObjectType] ON [H.Necessaire.Audit]
(
	AuditedObjectType
);

CREATE INDEX [IX_H.Necessaire.Audit_AuditedObjectID] ON [H.Necessaire.Audit]
(
	AuditedObjectID
);

CREATE INDEX [IX_H.Necessaire.Audit_HappenedAt] ON [H.Necessaire.Audit]
(
	HappenedAt
);

CREATE INDEX [IX_H.Necessaire.Audit_HappenedAtTicks] ON [H.Necessaire.Audit]
(
	HappenedAtTicks
);

CREATE INDEX [IX_H.Necessaire.Audit_DoneByID] ON [H.Necessaire.Audit]
(
	DoneByID
);

CREATE INDEX [IX_H.Necessaire.Audit_DoneByIDTag] ON [H.Necessaire.Audit]
(
	DoneByIDTag
);

CREATE INDEX [IX_H.Necessaire.Audit_DoneByDisplayName] ON [H.Necessaire.Audit]
(
	DoneByDisplayName
);

CREATE INDEX [IX_H.Necessaire.Audit_ActionType] ON [H.Necessaire.Audit]
(
	ActionType
);

CREATE INDEX [IX_H.Necessaire.Audit_AppVersionNumber] ON [H.Necessaire.Audit]
(
	AppVersionNumber
);

CREATE INDEX [IX_H.Necessaire.Audit_AppVersionTimestamp] ON [H.Necessaire.Audit]
(
	AppVersionTimestamp
);

CREATE INDEX [IX_H.Necessaire.Audit_AppVersionBranch] ON [H.Necessaire.Audit]
(
	AppVersionBranch
);

CREATE INDEX [IX_H.Necessaire.Audit_AppVersionCommit] ON [H.Necessaire.Audit]
(
	AppVersionCommit
);
