﻿IF (NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES  WHERE TABLE_SCHEMA = 'dbo' AND  TABLE_NAME = 'WebHookProcessingResult'))
BEGIN

	CREATE TABLE [dbo].[WebHookProcessingResult](

		[ID] [uniqueidentifier] NOT NULL,
		[WebHookRequestID] [uniqueidentifier] NOT NULL,
		[HappenedAt] [datetime2](7) NOT NULL,
		[IsSuccessful] [bit] NOT NULL,
		[Reason] [ntext] NULL,
		[CommentsJson] [ntext] NULL,

	CONSTRAINT [PK_WebHookProcessingResult] PRIMARY KEY CLUSTERED 
	(
		[ID] ASC
	)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

	CREATE NONCLUSTERED INDEX [IX_WebHookProcessingResult_WebHookRequestID] ON [dbo].[WebHookProcessingResult]
	(
		[WebHookRequestID] ASC
	)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]

	CREATE NONCLUSTERED INDEX [IX_WebHookProcessingResult_HappenedAt] ON [dbo].[WebHookProcessingResult]
	(
		[HappenedAt] ASC
	)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]

	CREATE NONCLUSTERED INDEX [IX_WebHookProcessingResult_IsSuccessful] ON [dbo].[WebHookProcessingResult]
	(
		[IsSuccessful] ASC
	)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]

END