CREATE TABLE ConsumerIdentity (

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