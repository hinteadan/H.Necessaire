﻿CREATE TABLE [dbo].[H.Necessaire.KeyValueStore] 
(
	[ID] [uniqueidentifier] NOT NULL,
	[StoreName] [nvarchar](450) NOT NULL,
	[Key] [nvarchar](450) NOT NULL,
	[Value] [ntext] NULL,
	[ExpiresAtTicks] [bigint] NULL,

	CONSTRAINT [PK_H.Necessaire.KeyValueStore] PRIMARY KEY CLUSTERED 
	(
		[StoreName] ASC,
		[Key] ASC
	)
	WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) 
ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]


CREATE NONCLUSTERED INDEX [IX_H.Necessaire.KeyValueStore_ExpiresAtTicks] ON [dbo].[H.Necessaire.KeyValueStore]
(
	[ExpiresAtTicks] ASC
)
WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]


CREATE NONCLUSTERED INDEX [IX_H.Necessaire.KeyValueStore_ID] ON [dbo].[H.Necessaire.KeyValueStore]
(
	[ID] ASC
)
WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]


CREATE NONCLUSTERED INDEX [IX_H.Necessaire.KeyValueStore_Key] ON [dbo].[H.Necessaire.KeyValueStore]
(
	[Key] ASC
)
WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]


CREATE NONCLUSTERED INDEX [IX_H.Necessaire.KeyValueStore_StoreName] ON [dbo].[H.Necessaire.KeyValueStore]
(
	[StoreName] ASC
)
WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
