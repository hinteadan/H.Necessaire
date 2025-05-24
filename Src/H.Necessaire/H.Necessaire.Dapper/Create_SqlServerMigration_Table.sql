CREATE TABLE [dbo].[H.Necessaire.Migration] 
(
	[ID] [nvarchar](450) NOT NULL,
	[ResourceIdentifier] [nvarchar](450) NOT NULL,
	[VersionNumber] [nvarchar](450) NOT NULL,
	[HappenedAt] [datetime2](7) NOT NULL,
	[HappenedAtTicks] [bigint] NOT NULL,
	[SqlCommand] [ntext] NOT NULL,

	CONSTRAINT [PK_H.Necessaire.Migration] PRIMARY KEY
	(
		[ID] ASC
	) 
	WITH 
	(
		STATISTICS_NORECOMPUTE = OFF,
		IGNORE_DUP_KEY = OFF,
		OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF
	)
	ON [PRIMARY]

)

ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]


CREATE NONCLUSTERED INDEX [IX_H.Necessaire.Migration_ResourceIdentifier] ON [dbo].[H.Necessaire.Migration]
(
	[ResourceIdentifier] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]


CREATE NONCLUSTERED INDEX [IX_H.Necessaire.Migration_VersionNumber] ON [dbo].[H.Necessaire.Migration]
(
	[VersionNumber] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]


CREATE NONCLUSTERED INDEX [IX_H.Necessaire.Migration_HappenedAt] ON [dbo].[H.Necessaire.Migration]
(
	[HappenedAt] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]


CREATE NONCLUSTERED INDEX [IX_H.Necessaire.Migration_HappenedAtTicks] ON [dbo].[H.Necessaire.Migration]
(
	[HappenedAtTicks] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
