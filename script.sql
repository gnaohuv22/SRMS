USE [FormDB]
GO
SET IDENTITY_INSERT [dbo].[Users] ON 

INSERT [dbo].[Users] ([UserId], [Email], [Role], [CreatedAt]) VALUES (4, N'student1@university.edu.vn', 0, CAST(N'2024-10-18T13:47:42.6366667' AS DateTime2))
INSERT [dbo].[Users] ([UserId], [Email], [Role], [CreatedAt]) VALUES (5, N'student2@university.edu.vn', 0, CAST(N'2024-10-18T13:47:42.6366667' AS DateTime2))
INSERT [dbo].[Users] ([UserId], [Email], [Role], [CreatedAt]) VALUES (6, N'student3@university.edu.vn', 0, CAST(N'2024-10-18T13:47:42.6366667' AS DateTime2))
INSERT [dbo].[Users] ([UserId], [Email], [Role], [CreatedAt]) VALUES (7, N'admin@example.com', 2, CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2))
INSERT [dbo].[Users] ([UserId], [Email], [Role], [CreatedAt]) VALUES (8, N'hoangmeo1905@gmail.com', 0, CAST(N'2024-10-26T06:52:40.6207515' AS DateTime2))
INSERT [dbo].[Users] ([UserId], [Email], [Role], [CreatedAt]) VALUES (9, N'HoangVHHE176169@fpt.edu.vn', 0, CAST(N'2024-10-26T07:25:15.9337266' AS DateTime2))
INSERT [dbo].[Users] ([UserId], [Email], [Role], [CreatedAt]) VALUES (11, N'daotao@university.edu.vn', 1, CAST(N'2024-10-27T01:08:16.8700000' AS DateTime2))
INSERT [dbo].[Users] ([UserId], [Email], [Role], [CreatedAt]) VALUES (12, N'khaothi@university.edu.vn', 1, CAST(N'2024-10-27T01:09:07.1100000' AS DateTime2))
INSERT [dbo].[Users] ([UserId], [Email], [Role], [CreatedAt]) VALUES (13, N'ctsv@university.edu.vn', 1, CAST(N'2024-10-27T01:09:13.9666667' AS DateTime2))
INSERT [dbo].[Users] ([UserId], [Email], [Role], [CreatedAt]) VALUES (14, N'doantn@university.edu.vn', 1, CAST(N'2024-10-27T01:09:18.8000000' AS DateTime2))
INSERT [dbo].[Users] ([UserId], [Email], [Role], [CreatedAt]) VALUES (15, N'ketoan@university.edu.vn', 1, CAST(N'2024-10-27T01:09:21.0766667' AS DateTime2))
SET IDENTITY_INSERT [dbo].[Users] OFF
GO
SET IDENTITY_INSERT [dbo].[Categories] ON 

INSERT [dbo].[Categories] ([CategoryId], [Name], [DepartmentUserId]) VALUES (1, N'Xin đăng ký học', 11)
INSERT [dbo].[Categories] ([CategoryId], [Name], [DepartmentUserId]) VALUES (2, N'Xin giấy xác nhận', 11)
INSERT [dbo].[Categories] ([CategoryId], [Name], [DepartmentUserId]) VALUES (3, N'Xin học lại', 11)
INSERT [dbo].[Categories] ([CategoryId], [Name], [DepartmentUserId]) VALUES (4, N'Phúc khảo bài thi', 12)
INSERT [dbo].[Categories] ([CategoryId], [Name], [DepartmentUserId]) VALUES (5, N'Xin giấy chứng nhận điểm', 12)
INSERT [dbo].[Categories] ([CategoryId], [Name], [DepartmentUserId]) VALUES (6, N'Học bổng', 13)
INSERT [dbo].[Categories] ([CategoryId], [Name], [DepartmentUserId]) VALUES (7, N'Ký luật', 13)
INSERT [dbo].[Categories] ([CategoryId], [Name], [DepartmentUserId]) VALUES (8, N'Ðăng ký họat động', 14)
INSERT [dbo].[Categories] ([CategoryId], [Name], [DepartmentUserId]) VALUES (9, N'Xin điểm rèn luyện', 14)
INSERT [dbo].[Categories] ([CategoryId], [Name], [DepartmentUserId]) VALUES (10, N'Học phí', 15)
INSERT [dbo].[Categories] ([CategoryId], [Name], [DepartmentUserId]) VALUES (11, N'Hoàn học phí', 15)
SET IDENTITY_INSERT [dbo].[Categories] OFF
GO
SET IDENTITY_INSERT [dbo].[Forms] ON 

INSERT [dbo].[Forms] ([FormId], [StudentId], [CategoryId], [Subject], [Content], [Status], [CreatedAt], [UpdatedAt]) VALUES (3, 4, 1, N'Ðăng ký môn Toán cao cấp (MAD101)', N'Em muốn đăng ký môn MAD101 cho học kỳ SP25', 1, CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2))
INSERT [dbo].[Forms] ([FormId], [StudentId], [CategoryId], [Subject], [Content], [Status], [CreatedAt], [UpdatedAt]) VALUES (4, 4, 5, N'Xin giấy chứng nhận điểm học kỳ 1', N'Em cần giấy chứng nhận điểm để xin học bổng', 1, CAST(N'2024-10-18T13:49:11.4600000' AS DateTime2), CAST(N'2024-10-18T13:49:11.4600000' AS DateTime2))
INSERT [dbo].[Forms] ([FormId], [StudentId], [CategoryId], [Subject], [Content], [Status], [CreatedAt], [UpdatedAt]) VALUES (5, 5, 3, N'Xin học lại môn PRN231', N'Em muốn đăng ký học lại môn PRN231', 2, CAST(N'2024-10-18T13:49:11.4600000' AS DateTime2), CAST(N'2024-10-18T13:49:11.4600000' AS DateTime2))
INSERT [dbo].[Forms] ([FormId], [StudentId], [CategoryId], [Subject], [Content], [Status], [CreatedAt], [UpdatedAt]) VALUES (6, 5, 6, N'Ðăng ký xét học bổng khuyến khích', N'Em muốn đăng ký xét học bổng học kỳ này', 1, CAST(N'2024-10-18T13:49:11.4600000' AS DateTime2), CAST(N'2024-10-18T13:49:11.4600000' AS DateTime2))
INSERT [dbo].[Forms] ([FormId], [StudentId], [CategoryId], [Subject], [Content], [Status], [CreatedAt], [UpdatedAt]) VALUES (7, 6, 8, N'Ðăng ký tham gia công tác Đoàn', N'Em muốn tham gia họat động tình nguyện', 0, CAST(N'2024-10-18T13:49:11.4600000' AS DateTime2), CAST(N'2024-10-18T13:49:11.4600000' AS DateTime2))
INSERT [dbo].[Forms] ([FormId], [StudentId], [CategoryId], [Subject], [Content], [Status], [CreatedAt], [UpdatedAt]) VALUES (8, 6, 10, N'Xác nhận đóng học phí', N'Em muốn xác nhận việc đóng học phí học kỳ FA24', 2, CAST(N'2024-10-18T13:49:11.4600000' AS DateTime2), CAST(N'2024-10-18T13:49:11.4600000' AS DateTime2))
SET IDENTITY_INSERT [dbo].[Forms] OFF
GO
SET IDENTITY_INSERT [dbo].[Responses] ON 

INSERT [dbo].[Responses] ([ResponseId], [FormId], [StaffId], [Content], [CreatedAt]) VALUES (1, 3, 11, N'123', CAST(N'2024-10-30T15:42:34.4266667' AS DateTime2))
SET IDENTITY_INSERT [dbo].[Responses] OFF
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20241026183501_InitialCreate', N'8.0.10')
GO
