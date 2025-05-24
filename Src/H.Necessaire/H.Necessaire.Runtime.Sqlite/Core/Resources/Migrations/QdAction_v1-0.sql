CREATE TABLE [H.Necessaire.QdAction] (

	ID TEXT NOT NULL,
	QdAt TEXT NOT NULL,
	QdAtTicks INTEGER NOT NULL,
	[Type] TEXT NOT NULL,
	Payload TEXT,
	[Status] INTEGER NOT NULL,
	StatusLabel TEXT NOT NULL,
	RunCount INTEGER NOT NULL,
	
	PRIMARY KEY(ID)
);

CREATE INDEX [IX_H.Necessaire.QdAction_QdAt] ON [H.Necessaire.QdAction]
(
	QdAt
);

CREATE INDEX [IX_H.Necessaire.QdAction_QdAtTicks] ON [H.Necessaire.QdAction]
(
	QdAtTicks
);

CREATE INDEX [IX_H.Necessaire.QdAction_Type] ON [H.Necessaire.QdAction]
(
	[Type]
);

CREATE INDEX [IX_H.Necessaire.QdAction_Status] ON [H.Necessaire.QdAction]
(
	[Status]
);

CREATE INDEX [IX_H.Necessaire.QdAction_StatusLabel] ON [H.Necessaire.QdAction]
(
	StatusLabel
);

CREATE INDEX [IX_H.Necessaire.QdAction_RunCount] ON [H.Necessaire.QdAction]
(
	RunCount
);
