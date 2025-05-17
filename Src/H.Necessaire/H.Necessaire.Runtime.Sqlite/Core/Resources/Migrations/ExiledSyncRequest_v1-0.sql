CREATE TABLE [H.Necessaire.ExiledSyncRequest] (

	ID TEXT NOT NULL,
	PayloadIdentifier TEXT NOT NULL,
	PayloadType TEXT NOT NULL,
	HappenedAt TEXT NOT NULL,
	HappenedAtTicks INTEGER NOT NULL,
	SyncRequestJson TEXT,
	SyncRequestProcessingResultJson TEXT,
	
	PRIMARY KEY(ID)
);

CREATE INDEX [IX_H.Necessaire.ExiledSyncRequest_PayloadIdentifier] ON [H.Necessaire.ExiledSyncRequest]
(
	PayloadIdentifier
);

CREATE INDEX [IX_H.Necessaire.ExiledSyncRequest_PayloadType] ON [H.Necessaire.ExiledSyncRequest]
(
	PayloadType
);

CREATE INDEX [IX_H.Necessaire.ExiledSyncRequest_HappenedAt] ON [H.Necessaire.ExiledSyncRequest]
(
	HappenedAt
);

CREATE INDEX [IX_H.Necessaire.ExiledSyncRequest_HappenedAtTicks] ON [H.Necessaire.ExiledSyncRequest]
(
	HappenedAtTicks
);
