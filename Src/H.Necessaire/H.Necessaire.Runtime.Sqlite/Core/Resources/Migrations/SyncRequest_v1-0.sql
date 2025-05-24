CREATE TABLE [H.Necessaire.SyncRequest] (

	ID TEXT NOT NULL,
	PayloadIdentifier TEXT NOT NULL,
	PayloadType TEXT NOT NULL,
	Payload TEXT,
	SyncStatus INTEGER NOT NULL,
	SyncStatusLabel TEXT NOT NULL,
	HappenedAt TEXT NOT NULL,
	HappenedAtTicks INTEGER NOT NULL,
	OperationContextJson TEXT,
	
	PRIMARY KEY(ID)
);

CREATE INDEX [IX_H.Necessaire.SyncRequest_PayloadIdentifier] ON [H.Necessaire.SyncRequest]
(
	PayloadIdentifier
);

CREATE INDEX [IX_H.Necessaire.SyncRequest_PayloadType] ON [H.Necessaire.SyncRequest]
(
	PayloadType
);

CREATE INDEX [IX_H.Necessaire.SyncRequest_SyncStatus] ON [H.Necessaire.SyncRequest]
(
	SyncStatus
);

CREATE INDEX [IX_H.Necessaire.SyncRequest_HappenedAt] ON [H.Necessaire.SyncRequest]
(
	HappenedAt
);

CREATE INDEX [IX_H.Necessaire.SyncRequest_HappenedAtTicks] ON [H.Necessaire.SyncRequest]
(
	HappenedAtTicks
);
