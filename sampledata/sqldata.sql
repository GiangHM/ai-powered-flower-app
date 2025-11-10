USE [FlowerShopDb]
GO
/****** Object:  Table [dbo].[FlowerCategories]    Script Date: 11/10/2025 5:25:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FlowerCategories](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[Description] [nvarchar](max) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[CreationDate] [datetime2](7) NOT NULL,
	[ModifiedDate] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_FlowerCategories] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FlowerPrices]    Script Date: 11/10/2025 5:25:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FlowerPrices](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[FlowerId] [bigint] NOT NULL,
	[UnitPrice] [decimal](18, 2) NOT NULL,
	[UnitPriceCurrency] [nvarchar](3) NOT NULL,
	[FromDate] [datetime2](7) NOT NULL,
	[ToDate] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_FlowerPrices] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Flowers]    Script Date: 11/10/2025 5:25:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Flowers](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[CategoryId] [bigint] NOT NULL,
	[FlowerName] [nvarchar](max) NOT NULL,
	[FlowerImageUrl] [nvarchar](max) NOT NULL,
	[FlowerDescription] [nvarchar](max) NOT NULL,
	[CreationDate] [datetime2](7) NOT NULL,
	[ModifiedDate] [datetime2](7) NOT NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_Flowers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FlowerStocks]    Script Date: 11/10/2025 5:25:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FlowerStocks](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[FlowerId] [bigint] NOT NULL,
	[ImportedDate] [datetime2](7) NOT NULL,
	[LastModifiedDate] [datetime2](7) NOT NULL,
	[Quantity] [int] NOT NULL,
	[QuantityUnit] [int] NOT NULL,
 CONSTRAINT [PK_FlowerStocks] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[FlowerCategories] ON 
GO
INSERT [dbo].[FlowerCategories] ([Id], [Name], [Description], [IsActive], [CreationDate], [ModifiedDate]) VALUES (1, N'Beautiful bouquet of flowers', N'Beautiful bouquet of flowers', 1, CAST(N'2025-10-06T08:01:46.9600000' AS DateTime2), CAST(N'2025-10-06T08:01:46.9600000' AS DateTime2))
GO
INSERT [dbo].[FlowerCategories] ([Id], [Name], [Description], [IsActive], [CreationDate], [ModifiedDate]) VALUES (2, N'Garden style', N'Garden style', 1, CAST(N'2025-10-06T08:01:46.9600000' AS DateTime2), CAST(N'2025-10-06T08:01:46.9600000' AS DateTime2))
GO
INSERT [dbo].[FlowerCategories] ([Id], [Name], [Description], [IsActive], [CreationDate], [ModifiedDate]) VALUES (3, N'Tulip flowers', N'Tulip flowers', 1, CAST(N'2025-10-07T04:53:10.5600000' AS DateTime2), CAST(N'2025-10-07T04:53:10.5600000' AS DateTime2))
GO
SET IDENTITY_INSERT [dbo].[FlowerCategories] OFF
GO
SET IDENTITY_INSERT [dbo].[FlowerPrices] ON 
GO
INSERT [dbo].[FlowerPrices] ([Id], [FlowerId], [UnitPrice], [UnitPriceCurrency], [FromDate], [ToDate]) VALUES (1, 1, CAST(550000.00 AS Decimal(18, 2)), N'VND', CAST(N'2025-10-06T08:07:16.8866667' AS DateTime2), CAST(N'2025-10-06T08:07:16.8866667' AS DateTime2))
GO
INSERT [dbo].[FlowerPrices] ([Id], [FlowerId], [UnitPrice], [UnitPriceCurrency], [FromDate], [ToDate]) VALUES (2, 2, CAST(550000.00 AS Decimal(18, 2)), N'VND', CAST(N'2025-10-06T08:07:16.8866667' AS DateTime2), CAST(N'2025-10-06T08:07:16.8866667' AS DateTime2))
GO
INSERT [dbo].[FlowerPrices] ([Id], [FlowerId], [UnitPrice], [UnitPriceCurrency], [FromDate], [ToDate]) VALUES (3, 3, CAST(550000.00 AS Decimal(18, 2)), N'VND', CAST(N'2025-10-07T07:00:09.6900000' AS DateTime2), CAST(N'2025-10-07T07:00:09.6900000' AS DateTime2))
GO
INSERT [dbo].[FlowerPrices] ([Id], [FlowerId], [UnitPrice], [UnitPriceCurrency], [FromDate], [ToDate]) VALUES (4, 4, CAST(650000.00 AS Decimal(18, 2)), N'VND', CAST(N'2025-10-07T07:14:42.2300000' AS DateTime2), CAST(N'2025-10-07T07:14:42.2300000' AS DateTime2))
GO
INSERT [dbo].[FlowerPrices] ([Id], [FlowerId], [UnitPrice], [UnitPriceCurrency], [FromDate], [ToDate]) VALUES (5, 5, CAST(550000.00 AS Decimal(18, 2)), N'VND', CAST(N'2025-10-07T07:18:08.9866667' AS DateTime2), CAST(N'2025-10-07T07:18:08.9866667' AS DateTime2))
GO
INSERT [dbo].[FlowerPrices] ([Id], [FlowerId], [UnitPrice], [UnitPriceCurrency], [FromDate], [ToDate]) VALUES (6, 6, CAST(650000.00 AS Decimal(18, 2)), N'VND', CAST(N'2025-10-07T07:26:50.7233333' AS DateTime2), CAST(N'2025-10-07T07:26:50.7233333' AS DateTime2))
GO
INSERT [dbo].[FlowerPrices] ([Id], [FlowerId], [UnitPrice], [UnitPriceCurrency], [FromDate], [ToDate]) VALUES (7, 7, CAST(650000.00 AS Decimal(18, 2)), N'VND', CAST(N'2025-10-07T07:30:05.4500000' AS DateTime2), CAST(N'2025-10-07T07:30:05.4500000' AS DateTime2))
GO
INSERT [dbo].[FlowerPrices] ([Id], [FlowerId], [UnitPrice], [UnitPriceCurrency], [FromDate], [ToDate]) VALUES (8, 8, CAST(1350000.00 AS Decimal(18, 2)), N'VND', CAST(N'2025-10-07T07:34:06.0333333' AS DateTime2), CAST(N'2025-10-07T07:34:06.0333333' AS DateTime2))
GO
INSERT [dbo].[FlowerPrices] ([Id], [FlowerId], [UnitPrice], [UnitPriceCurrency], [FromDate], [ToDate]) VALUES (9, 9, CAST(800000.00 AS Decimal(18, 2)), N'VND', CAST(N'2025-10-07T07:36:49.9533333' AS DateTime2), CAST(N'2025-10-07T07:36:49.9533333' AS DateTime2))
GO
INSERT [dbo].[FlowerPrices] ([Id], [FlowerId], [UnitPrice], [UnitPriceCurrency], [FromDate], [ToDate]) VALUES (10, 10, CAST(800000.00 AS Decimal(18, 2)), N'VND', CAST(N'2025-10-07T07:39:23.9500000' AS DateTime2), CAST(N'2025-10-07T07:39:23.9500000' AS DateTime2))
GO
INSERT [dbo].[FlowerPrices] ([Id], [FlowerId], [UnitPrice], [UnitPriceCurrency], [FromDate], [ToDate]) VALUES (10012, 10014, CAST(109900.00 AS Decimal(18, 2)), N'VND', CAST(N'2025-11-02T09:57:20.0163009' AS DateTime2), CAST(N'9999-12-31T23:59:59.9999999' AS DateTime2))
GO
INSERT [dbo].[FlowerPrices] ([Id], [FlowerId], [UnitPrice], [UnitPriceCurrency], [FromDate], [ToDate]) VALUES (10013, 10015, CAST(100000.00 AS Decimal(18, 2)), N'VND', CAST(N'2025-11-02T10:00:39.5574489' AS DateTime2), CAST(N'9999-12-31T23:59:59.9999999' AS DateTime2))
GO
SET IDENTITY_INSERT [dbo].[FlowerPrices] OFF
GO
SET IDENTITY_INSERT [dbo].[Flowers] ON 
GO
INSERT [dbo].[Flowers] ([Id], [CategoryId], [FlowerName], [FlowerImageUrl], [FlowerDescription], [CreationDate], [ModifiedDate], [IsActive]) VALUES (1, 1, N'Rose Pink Tweedia', N'Rose-Pink-Tweedia.webp', N'Rose Pink Tweedia Bouquet
Suitable for birthdays, dates, confessions, and new day wishes
Height: 50cm – Width: 40cm
Main flower: Cream Rose
Further flowers: Delphinium or Blue Star, Pink Carnation, Pink Gerbera
 Suitable for Everyone, Passionate for Any Love
With its multi-dimensional beauty, the Rose Pink Tweedia Bouquet is suitable for many special occasions:

Confession: Are you harboring feelings but do not know how to say them? This bouquet will express your feelings in the most sincere way, "I love you with a very gentle love."

Birthday: This birthday gift will replace your wishes for a new year filled with happiness, luck and the sweetest things.
Dating: A date becomes more romantic than ever when you suddenly give her/him a delicate bouquet like this.
Happy New Day: Sometimes, love is simply sincere wishes every morning. Giving this bouquet on a random day will surely make that person happy all day long.
More Suggestions for the Season of Love
In addition to Rose Pink Tweedia, if you want to find more unique options, check out:

Simple Sunflower Bouquet: For those who want to convey optimism, hope and strong vitality.
Classic Red Rose Basket: A classic choice that never goes out of style, expressing passionate and eternal love.
Pure White Baby is Breath Flower Box: Simple but meaningful, symbolizing pure, romantic love.

 ', CAST(N'2025-10-06T08:04:28.7233333' AS DateTime2), CAST(N'2025-10-06T08:04:28.7233333' AS DateTime2), 1)
GO
INSERT [dbo].[Flowers] ([Id], [CategoryId], [FlowerName], [FlowerImageUrl], [FlowerDescription], [CreationDate], [ModifiedDate], [IsActive]) VALUES (2, 1, N'Sophie Pink Rose', N'Sophie-Pink-Rose.webp', N'Sophie Pink Rose Bouquet
Suitable for birthdays, dates, confessions, and new day wishes
Height: 50cm - Width: 40cm
Main flower: Sophie
Furniture: Mini Carnation

Meaning of the bouquet and the message sent
The Sophie Pink Rose bouquet is not only beautiful but also contains many profound meanings:

Pure and sweet feelings: The pink color of Sophie Pink Rose is a symbol of pure love, admiration and sincere gratitude. Giving this bouquet is a delicate way for you to express your feelings without being too ostentatious.
Wishes for a bright future: Silver leaves and pure white baby flowers symbolize purity, new beginnings and hope. They bring wishes to the recipient for a bright future, a peaceful and happy life.
Femininity and grace: This bouquet is very suitable for gentle, romantic girls who love delicate beauty. This is a great gift to honor the beauty and femininity of the recipient on occasions such as birthdays, anniversaries, or simply want to surprise them.
Do not hesitate to contact Vuon Hoa Tuoi for advice and design unique flower products that bear your personal mark. We are always willing to listen and help you convey your love in the most complete way!', CAST(N'2025-10-06T08:04:28.7233333' AS DateTime2), CAST(N'2025-10-06T08:04:28.7233333' AS DateTime2), 1)
GO
INSERT [dbo].[Flowers] ([Id], [CategoryId], [FlowerName], [FlowerImageUrl], [FlowerDescription], [CreationDate], [ModifiedDate], [IsActive]) VALUES (3, 1, N'Sophie Roses for Birthday', N'Sophie-Roses-for-Birthday.webp', N'Sophie Rose Bouquet for Birthday
Suitable for birthdays, dates, confessions, and new day wishes
Size: Medium
Height: 45cm – Width: 35cm
Main flower: Sophie
Sophia Roses symbolize gentleness and femininity; the small flowers cluster together like a message of respect and gratitude. The warm pink color and delicate fragrance evoke sweet feelings, lasting attachment and delicate romantic beauty. Sophie Roses are used in vase arrangements, home decoration, wedding flower designs and are an ideal gift for birthdays, anniversaries, graduations or congratulations.
', CAST(N'2025-10-07T07:00:09.6666667' AS DateTime2), CAST(N'2025-10-07T07:00:09.6666667' AS DateTime2), 1)
GO
INSERT [dbo].[Flowers] ([Id], [CategoryId], [FlowerName], [FlowerImageUrl], [FlowerDescription], [CreationDate], [ModifiedDate], [IsActive]) VALUES (4, 1, N'Sophie LoveHeart', N'Sophie-LoveHeart.webp', N'Sophie LoveHeart Rose Bouquet
Suitable for birthdays, dates, confessions, and new day wishes
Height: 50cm - Width: 40cm
Main flower: Sophie
Furniture: Mini Carnation

Sending Love In Every Moment
The Sophie LoveHeart bouquet is the ideal choice for many special occasions:

Birthday Gift: Send this bouquet as a meaningful birthday wish.
Dating: Bring the bouquet on a date to create a romantic, warm impression.
Confession: Use this bouquet to express sincere feelings, in lieu of words.
Happy New Day: Give this bouquet to start a new day with lots of joy and freshness.', CAST(N'2025-10-07T07:14:42.0966667' AS DateTime2), CAST(N'2025-10-07T07:14:42.0966667' AS DateTime2), 1)
GO
INSERT [dbo].[Flowers] ([Id], [CategoryId], [FlowerName], [FlowerImageUrl], [FlowerDescription], [CreationDate], [ModifiedDate], [IsActive]) VALUES (5, 2, N'Blushing Meadow', N'Blushing-Meadow.webp', N'Flower Blushing Meadow Bouquet
Suitable for: All occasions, birthday gifts, dating, confession
Height: 45cm - Width: 35cm
Main flower: Sophie Rose
Secondary flower: Pink gerbera, yellow wild sunflower, white rose, pink sandalwood

Flower Blushing Meadow is a combination of flowers with both rustic and delicate beauty, creating a harmonious, romantic whole.

Sophie Rose: Standing out and sweet like a confession, Sophie Rose is the main character of this bouquet. The gentle, shy pink color of Sophie Rose not only represents gentle, romantic love but also symbolizes admiration and sincere gratitude. Giving this bouquet to your girlfriend, you have said what you want, affirming that your feelings for her are unique and most sincere.

Pink Gerbera: The flower of resilience and joy. The lovely pink gerbera flowers add a bit of freshness to the bouquet, representing endless joy and optimism in love. Pink also shows the femininity and gentleness of the girl you love.
Yellow Wild Sunflower: The wild, rustic beauty of yellow wild sunflower brings a feeling of closeness and sincerity. It symbolizes warm friendship, joy and wishes for a bright future.
White Rose: White roses are indispensable - a symbol of innocence, purity and a respectful love. The presence of white roses helps the bouquet become more balanced, romantic and delicate.
Pink Sand Flower: Gentle and soft, pink sand is an indispensable secondary flower, skillfully connecting the main flowers, creating an overall harmonious and emotional beauty.', CAST(N'2025-10-07T07:18:08.9666667' AS DateTime2), CAST(N'2025-10-07T07:18:08.9666667' AS DateTime2), 1)
GO
INSERT [dbo].[Flowers] ([Id], [CategoryId], [FlowerName], [FlowerImageUrl], [FlowerDescription], [CreationDate], [ModifiedDate], [IsActive]) VALUES (6, 2, N'Morning Blossom', N'Morning-Blossom.webp', N'Morning Blossom Flower Bouquet
Suitable for: All occasions, birthday gifts, dating, confession
Height: 55cm - Width: 45cm
Main flower: Cream Rose
Secondary flower: Sophie Rose, Pink Carnation, White Sand, White Wolf, Delphinium
Why Morning Blossom is a great choice
At Fresh Flower Garden, we believe that to create a perfect work, each flower must be carefully selected. The “Morning Blossom” bouquet is a combination of:

Cream Rose: The main flower, symbolizing pure, sweet love and respect. The cream roses are carefully selected, plump and fresh, promising to bring lasting beauty.
Sophie Rose: Helps the bouquet to be more feminine, soft and graceful, expressing the femininity and gentleness of the recipient.
Pink Carnation: Represents love, respect and sincere gratitude.
White Sand, White Wolf, Delphinium: These secondary flowers not only fill the gap but also create highlights, helping the overall bouquet become more airy, natural and unique.', CAST(N'2025-10-07T07:26:50.7066667' AS DateTime2), CAST(N'2025-10-07T07:26:50.7066667' AS DateTime2), 1)
GO
INSERT [dbo].[Flowers] ([Id], [CategoryId], [FlowerName], [FlowerImageUrl], [FlowerDescription], [CreationDate], [ModifiedDate], [IsActive]) VALUES (7, 2, N'Garden Felling Style', N'Garden-Felling-Style.webp', N'Garden Felling Style Fresh Flower Bouquet
Suitable for: All occasions, birthday gifts, dating, confession
Height: 55cm - Width: 45cm
Main flower: & Secondary flower: Mix 10 types of beautiful fresh flowers according to the sample

Garden Feeling Style – Miniature Home Garden Within Reach
Imagine, in the middle of a noisy city, you suddenly receive a bouquet of flowers as if they were just picked from a fairy garden. That is the message that “Garden Feeling Style” wants to convey. It is not just simply a bunch of flowers, it is also a green, vibrant space, bringing a familiar, peaceful feeling like walking in your grandmother’s or mother’s garden in the past.', CAST(N'2025-10-07T07:30:05.4466667' AS DateTime2), CAST(N'2025-10-07T07:30:05.4466667' AS DateTime2), 1)
GO
INSERT [dbo].[Flowers] ([Id], [CategoryId], [FlowerName], [FlowerImageUrl], [FlowerDescription], [CreationDate], [ModifiedDate], [IsActive]) VALUES (8, 3, N'Ohara Tulips White', N'Ohara-Tulips-White.webp', N'Ohara Tulips White Bouquet
Suitable for: birthday, dating, confession
Height: 55cm - Width: 45cm
Main flower: Ohara Rose
Secondary flower: Tulip, White Ping Pong

Ohara Tulips White - the flower of eternal promise
If you are dating, confessing your love, or simply want to give her a surprise gift on her birthday, the Ohara Tulips White bouquet will convey all your messages. Each flower in the bouquet has its own meaning, together creating a poetic love story.

Ohara Rose Perfect and passionate love: Not a fiery red rose, Ohara rose with its pure white color and charming beauty symbolizes a sincere, deep and perfect love. The petals bloom softly like a promise of a long-lasting, cherished relationship
White Tulip The perfect confession: White tulip symbolizes perfect love and forgiveness. Giving her a white tulip is also a way for you to show that in your eyes, she is the only one, the most perfect "muse".

White Ping Pong Flowers Symbol of Purity: The plump, pretty white Ping Pong flowers have a pure, noble beauty, helping the bouquet stand out, expressing your pure and sincere feelings.', CAST(N'2025-10-07T07:34:06.0200000' AS DateTime2), CAST(N'2025-10-07T07:34:06.0200000' AS DateTime2), 1)
GO
INSERT [dbo].[Flowers] ([Id], [CategoryId], [FlowerName], [FlowerImageUrl], [FlowerDescription], [CreationDate], [ModifiedDate], [IsActive]) VALUES (9, 3, N'Tulip Flower Sweet Blossom', N'Tulip-Flower-Sweet-Blossom-.webp', N'White Love Tulip Bouquet
Suitable for: Birthdays, Anniversaries

White Tulips
Additional Flowers
Pretty Grasses
Leaves and Accessories

Tulip Flower Sweet Blossom
The tulip bouquet Tulip Flower Sweet Blossom is suitable as a gift for many different occasions such as:

Birthday
Valentine Day
International Women Day
Mother Day
Wedding Anniversary
Other Holidays', CAST(N'2025-10-07T07:36:49.9466667' AS DateTime2), CAST(N'2025-10-07T07:36:49.9466667' AS DateTime2), 1)
GO
INSERT [dbo].[Flowers] ([Id], [CategoryId], [FlowerName], [FlowerImageUrl], [FlowerDescription], [CreationDate], [ModifiedDate], [IsActive]) VALUES (10, 3, N'Tulip Bouquet Forever Love', N'Tulip-Bouquet-Forever-Love.webp', N'Forever Love Tulip Bouquet
Suitable for: Birthday, Anniversary, Date, Confession, Sorry

White Tulips Dyed Green
Tana Daisy
Voile
Flowers
Pretty Grass
Leaves and Accessories', CAST(N'2025-10-07T07:39:23.9300000' AS DateTime2), CAST(N'2025-10-07T07:39:23.9300000' AS DateTime2), 1)
GO
INSERT [dbo].[Flowers] ([Id], [CategoryId], [FlowerName], [FlowerImageUrl], [FlowerDescription], [CreationDate], [ModifiedDate], [IsActive]) VALUES (10014, 1, N'Testing creation new flower', N'bo-hoa-hong-lac-than-my-girl.jpg.webp', N'Testing creation new flower', CAST(N'2025-11-02T16:57:20.1266667' AS DateTime2), CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), 0)
GO
INSERT [dbo].[Flowers] ([Id], [CategoryId], [FlowerName], [FlowerImageUrl], [FlowerDescription], [CreationDate], [ModifiedDate], [IsActive]) VALUES (10015, 2, N'test creating new flower 3', N'gb26-graduation-bouquet.jpg', N'Testing create new flower 3', CAST(N'2025-11-02T17:00:39.7466667' AS DateTime2), CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), 0)
GO
SET IDENTITY_INSERT [dbo].[Flowers] OFF
GO
SET IDENTITY_INSERT [dbo].[FlowerStocks] ON 
GO
INSERT [dbo].[FlowerStocks] ([Id], [FlowerId], [ImportedDate], [LastModifiedDate], [Quantity], [QuantityUnit]) VALUES (1, 1, CAST(N'2025-10-06T08:08:32.7533333' AS DateTime2), CAST(N'2025-10-06T08:08:32.7533333' AS DateTime2), 100, 1)
GO
INSERT [dbo].[FlowerStocks] ([Id], [FlowerId], [ImportedDate], [LastModifiedDate], [Quantity], [QuantityUnit]) VALUES (2, 2, CAST(N'2025-10-06T08:08:32.7533333' AS DateTime2), CAST(N'2025-10-06T08:08:32.7533333' AS DateTime2), 150, 1)
GO
INSERT [dbo].[FlowerStocks] ([Id], [FlowerId], [ImportedDate], [LastModifiedDate], [Quantity], [QuantityUnit]) VALUES (3, 3, CAST(N'2025-10-07T07:00:09.7000000' AS DateTime2), CAST(N'2025-10-07T07:00:09.7000000' AS DateTime2), 10, 1)
GO
INSERT [dbo].[FlowerStocks] ([Id], [FlowerId], [ImportedDate], [LastModifiedDate], [Quantity], [QuantityUnit]) VALUES (4, 4, CAST(N'2025-10-07T07:14:42.2366667' AS DateTime2), CAST(N'2025-10-07T07:14:42.2366667' AS DateTime2), 10, 1)
GO
INSERT [dbo].[FlowerStocks] ([Id], [FlowerId], [ImportedDate], [LastModifiedDate], [Quantity], [QuantityUnit]) VALUES (5, 5, CAST(N'2025-10-07T07:18:08.9900000' AS DateTime2), CAST(N'2025-10-07T07:18:08.9900000' AS DateTime2), 10, 1)
GO
INSERT [dbo].[FlowerStocks] ([Id], [FlowerId], [ImportedDate], [LastModifiedDate], [Quantity], [QuantityUnit]) VALUES (6, 6, CAST(N'2025-10-07T07:26:50.7333333' AS DateTime2), CAST(N'2025-10-07T07:26:50.7333333' AS DateTime2), 10, 1)
GO
INSERT [dbo].[FlowerStocks] ([Id], [FlowerId], [ImportedDate], [LastModifiedDate], [Quantity], [QuantityUnit]) VALUES (7, 7, CAST(N'2025-10-07T07:30:05.4700000' AS DateTime2), CAST(N'2025-10-07T07:30:05.4700000' AS DateTime2), 10, 1)
GO
INSERT [dbo].[FlowerStocks] ([Id], [FlowerId], [ImportedDate], [LastModifiedDate], [Quantity], [QuantityUnit]) VALUES (8, 8, CAST(N'2025-10-07T07:34:06.0400000' AS DateTime2), CAST(N'2025-10-07T07:34:06.0400000' AS DateTime2), 10, 1)
GO
INSERT [dbo].[FlowerStocks] ([Id], [FlowerId], [ImportedDate], [LastModifiedDate], [Quantity], [QuantityUnit]) VALUES (9, 9, CAST(N'2025-10-07T07:36:49.9700000' AS DateTime2), CAST(N'2025-10-07T07:36:49.9700000' AS DateTime2), 10, 1)
GO
INSERT [dbo].[FlowerStocks] ([Id], [FlowerId], [ImportedDate], [LastModifiedDate], [Quantity], [QuantityUnit]) VALUES (10, 10, CAST(N'2025-10-07T07:39:23.9566667' AS DateTime2), CAST(N'2025-10-07T07:39:23.9566667' AS DateTime2), 10, 1)
GO
SET IDENTITY_INSERT [dbo].[FlowerStocks] OFF
GO
ALTER TABLE [dbo].[FlowerCategories] ADD  CONSTRAINT [DF__FlowerCat__Creat__36B12243]  DEFAULT (getdate()) FOR [CreationDate]
GO
ALTER TABLE [dbo].[Flowers] ADD  CONSTRAINT [DF__Flowers__Creatio__398D8EEE]  DEFAULT (getdate()) FOR [CreationDate]
GO
ALTER TABLE [dbo].[FlowerStocks] ADD  CONSTRAINT [DF__FlowerSto__Impor__403A8C7D]  DEFAULT (getdate()) FOR [ImportedDate]
GO
ALTER TABLE [dbo].[FlowerStocks] ADD  CONSTRAINT [DF__FlowerSto__LastM__412EB0B6]  DEFAULT (getdate()) FOR [LastModifiedDate]
GO
ALTER TABLE [dbo].[FlowerPrices]  WITH CHECK ADD  CONSTRAINT [FK_FlowerPrices_Flowers_FlowerId] FOREIGN KEY([FlowerId])
REFERENCES [dbo].[Flowers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[FlowerPrices] CHECK CONSTRAINT [FK_FlowerPrices_Flowers_FlowerId]
GO
ALTER TABLE [dbo].[Flowers]  WITH CHECK ADD  CONSTRAINT [FK_Flowers_FlowerCategories_CategoryId] FOREIGN KEY([CategoryId])
REFERENCES [dbo].[FlowerCategories] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Flowers] CHECK CONSTRAINT [FK_Flowers_FlowerCategories_CategoryId]
GO
ALTER TABLE [dbo].[FlowerStocks]  WITH CHECK ADD  CONSTRAINT [FK_FlowerStocks_Flowers_FlowerId] FOREIGN KEY([FlowerId])
REFERENCES [dbo].[Flowers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[FlowerStocks] CHECK CONSTRAINT [FK_FlowerStocks_Flowers_FlowerId]
GO
