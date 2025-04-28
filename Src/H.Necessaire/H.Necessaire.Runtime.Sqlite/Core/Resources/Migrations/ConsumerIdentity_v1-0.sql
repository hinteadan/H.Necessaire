CREATE TABLE [H.Necessaire.ConsumerIdentity] (

	ID TEXT NOT NULL,
	IDTag TEXT,
	DisplayName TEXT,
	NotesJson TEXT,
	AsOf TEXT NOT NULL,
	AsOfTicks INTEGER NOT NULL,
	IpAddress TEXT,
	HostName TEXT,
	Protocol TEXT,
	UserAgent TEXT,
	AiUserID TEXT,
	Origin TEXT,
	Referer TEXT,
	RuntimePlatformJson TEXT,
	
	
	PRIMARY KEY(ID)
);

CREATE INDEX [IX_H.Necessaire.ConsumerIdentity_IDTag] ON [H.Necessaire.ConsumerIdentity]
(
	IDTag
);

CREATE INDEX [IX_H.Necessaire.ConsumerIdentity_DisplayName] ON [H.Necessaire.ConsumerIdentity]
(
	DisplayName
);

CREATE INDEX [IX_H.Necessaire.ConsumerIdentity_AsOf] ON [H.Necessaire.ConsumerIdentity]
(
	AsOf
);

CREATE INDEX [IX_H.Necessaire.ConsumerIdentity_AsOfTicks] ON [H.Necessaire.ConsumerIdentity]
(
	AsOfTicks
);