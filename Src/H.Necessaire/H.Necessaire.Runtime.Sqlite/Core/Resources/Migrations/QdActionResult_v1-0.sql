CREATE TABLE [H.Necessaire.QdActionResult] (

	ID TEXT NOT NULL,
	QdActionID TEXT NOT NULL,
	HappenedAt TEXT NOT NULL,
	HappenedAtTicks INTEGER NOT NULL,
	QdActionJson TEXT NOT NULL,
	IsSuccessful INTEGER NOT NULL,
	Reason TEXT,
	CommentsJson TEXT,
	
	PRIMARY KEY(ID)
);

CREATE INDEX [IX_H.Necessaire.QdActionResult_QdActionID] ON [H.Necessaire.QdActionResult]
(
	QdActionID
);

CREATE INDEX [IX_H.Necessaire.QdActionResult_HappenedAt] ON [H.Necessaire.QdActionResult]
(
	HappenedAt
);

CREATE INDEX [IX_H.Necessaire.QdActionResult_HappenedAtTicks] ON [H.Necessaire.QdActionResult]
(
	HappenedAtTicks
);

CREATE INDEX [IX_H.Necessaire.QdActionResult_IsSuccessful] ON [H.Necessaire.QdActionResult]
(
	IsSuccessful
);
