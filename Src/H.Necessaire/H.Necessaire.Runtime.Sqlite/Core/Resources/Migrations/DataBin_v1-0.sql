CREATE TABLE [H.Necessaire.DataBin] (

	ID TEXT NOT NULL,
	[Name] TEXT,
	[Description] TEXT,
	AsOf TEXT NOT NULL,
	AsOfTicks INTEGER NOT NULL,
	FormatJson TEXT,
	FormatID TEXT,
	FormatExtension TEXT,
	FormatMimeType TEXT,
	FormatEncoding TEXT,
	NotesJson TEXT,
	NotesString TEXT,
	
	PRIMARY KEY(ID)
);

CREATE INDEX [IX_H.Necessaire.DataBin_Name] ON [H.Necessaire.DataBin]
(
	[Name]
);

CREATE INDEX [IX_H.Necessaire.DataBin_AsOf] ON [H.Necessaire.DataBin]
(
	AsOf
);

CREATE INDEX [IX_H.Necessaire.DataBin_AsOfTicks] ON [H.Necessaire.DataBin]
(
	AsOfTicks
);

CREATE INDEX [IX_H.Necessaire.DataBin_FormatID] ON [H.Necessaire.DataBin]
(
	FormatID
);

CREATE INDEX [IX_H.Necessaire.DataBin_FormatExtension] ON [H.Necessaire.DataBin]
(
	FormatExtension
);

CREATE INDEX [IX_H.Necessaire.DataBin_FormatMimeType] ON [H.Necessaire.DataBin]
(
	FormatMimeType
);

CREATE INDEX [IX_H.Necessaire.DataBin_FormatEncoding] ON [H.Necessaire.DataBin]
(
	FormatEncoding
);

CREATE INDEX [IX_H.Necessaire.DataBin_NotesString] ON [H.Necessaire.DataBin]
(
	NotesString
);
