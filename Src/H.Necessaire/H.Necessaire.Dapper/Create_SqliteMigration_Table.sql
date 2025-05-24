CREATE TABLE "H.Necessaire.Migration" (
	"ID"	TEXT NOT NULL,
	"ResourceIdentifier"	TEXT NOT NULL,
	"VersionNumber"	TEXT NOT NULL,
	"HappenedAt"	TEXT NOT NULL,
	"HappenedAtTicks"	INTEGER NOT NULL,
	"SqlCommand"	TEXT NOT NULL,
	PRIMARY KEY("ID")
);



CREATE INDEX "IX_H.Necessaire.Migration_ResourceIdentifier" ON "H.Necessaire.Migration"
(
	"ResourceIdentifier"
);


CREATE INDEX "IX_H.Necessaire.Migration_VersionNumber" ON "H.Necessaire.Migration"
(
	"VersionNumber"
);


CREATE INDEX "IX_H.Necessaire.Migration_HappenedAt" ON "H.Necessaire.Migration"
(
	"HappenedAt"
);


CREATE INDEX "IX_H.Necessaire.Migration_HappenedAtTicks" ON "H.Necessaire.Migration"
(
	"HappenedAtTicks"
);
