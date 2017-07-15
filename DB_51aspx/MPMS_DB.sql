USE master
GO

IF EXISTS(SELECT * FROM SYSDATABASES WHERE NAME ='MPMS_DB')
DROP DATABASE MPMS_DB
GO
CREATE DATABASE MPMS_DB
GO

USE MPMS_DB
GO

--表名：基础数据类别(Categories)
IF EXISTS (SELECT * FROM SYSOBJECTS WHERE NAME='Categories')
DROP TABLE Categories
GO
CREATE TABLE Categories
(
	C_Category VARCHAR(20) PRIMARY KEY ,			--类别名字
	C_Illustration VARCHAR(20),						--类别说明
	C_IsShow BIT									--是否在界面上显示
)
go
--数据基本操作（增删查改） 
INSERT INTO Categories VALUES('S_Category','','0')
INSERT INTO Categories VALUES('U_Role','','0')
INSERT INTO Categories VALUES('MC_State','','0')
INSERT INTO Categories VALUES('CO_OrderType','','0')

DELETE Categories WHERE C_Category=''
SELECT * FROM Categories
UPDATE Categories SET C_Illustration='',C_IsShow='' WHERE C_Category=''

--表名：基础数据项(CategoryItems)
IF EXISTS (SELECT * FROM SYSOBJECTS WHERE NAME='CategoryItems')
DROP TABLE CategoryItems
GO
CREATE TABLE CategoryItems
(
	C_Category VARCHAR(20) REFERENCES Categories(C_Category) ,			--类别名字
	CI_ID INT,															--类别项编号
	CI_Name VARCHAR(20)													--类别项名称
)
GO
--数据基本操作（增删查改） 
INSERT INTO CategoryItems VALUES('S_Category','1','总部')
INSERT INTO CategoryItems VALUES('S_Category','2','加盟店')
INSERT INTO CategoryItems VALUES('S_Category','3','自营店')

INSERT INTO CategoryItems VALUES('U_Role','1','系统管理员')
INSERT INTO CategoryItems VALUES('U_Role','2','店长')
INSERT INTO CategoryItems VALUES('U_Role','3','业务员')

INSERT INTO CategoryItems VALUES('MC_State','1','正常')
INSERT INTO CategoryItems VALUES('MC_State','2','挂失')
INSERT INTO CategoryItems VALUES('MC_State','3','锁定')

INSERT INTO CategoryItems VALUES('CO_OrderType','1','兑换积分')
INSERT INTO CategoryItems VALUES('CO_OrderType','2','积分返现')
INSERT INTO CategoryItems VALUES('CO_OrderType','3','减积分')
INSERT INTO CategoryItems VALUES('CO_OrderType','4','转介绍积分')
INSERT INTO CategoryItems VALUES('CO_OrderType','5','快速消费')

SELECT * FROM CategoryItems


--表名：店铺(Shops)
IF EXISTS (SELECT * FROM SYSOBJECTS WHERE NAME='Shops')
DROP TABLE Shops
GO
CREATE TABLE Shops
(
	S_ID INT PRIMARY KEY IDENTITY(1,1),		--店铺编号
	S_Name VARCHAR(20),						--店铺名称
	S_Category INT ,						--店铺类别
	S_ContactName VARCHAR(20),				--联系人
	S_ContactTel VARCHAR(20),				--联系电话
	S_Address VARCHAR(50),					--地址
	S_Remark VARCHAR(100),					--备注
	S_IsHasSetAdmin BIT,					--是否已分配管理员
	S_CreateTime DATETIME,					--加盟时间
)
GO
--数据基本操作（增删查改） 
INSERT INTO Shops VALUES('良品铺子武昌总部','1','大白','15827791536','武汉市东湖风景区外语外事职业学院','概念店','1','1997-04-07')

INSERT INTO Shops VALUES('良品铺子北京昌平分店','2','好学习','10086','北京市昌平区（详细地址待定）','概念店','1','2015-05-03')
INSERT INTO Shops VALUES('良品铺子北京朝阳分店','2','天向上','10086','北京市朝阳区（详细地址待定）','概念店','0','2015-07-26')

INSERT INTO Shops VALUES('良品铺子武昌光谷分店','2','和春风','10086','北京市朝阳区（详细地址待定）','概念店','0','2015-07-26')
INSERT INTO Shops VALUES('良品铺子软件园分店','3','肃秋霜','10086','武汉光谷软件园分店','概念店','0','2015-07-26')

INSERT INTO Shops VALUES('良品铺子华夏分店','3','取象钱','10086','武汉光谷软件园分店','概念店','0','2015-07-27')

INSERT INTO Shops VALUES('测试1','2','测试111','10086','测试111111111','概念店','0','2015-05-03')
INSERT INTO Shops VALUES('测试2','2','测试222','10086','测试222222222','概念店','0','2015-07-26')
INSERT INTO Shops VALUES('测试3','3','测试333','10086','测试333333333','概念店','0','2015-07-26')
INSERT INTO Shops VALUES('测试4','3','测试444','10086','测试444444444','概念店','0','2015-07-26')
INSERT INTO Shops VALUES('测试5','2','测试555','10086','测试555555555','概念店','0','2015-05-03')
INSERT INTO Shops VALUES('测试6','2','测试666','10086','测试666666666','概念店','0','2015-07-26')
INSERT INTO Shops VALUES('测试7','3','测试777','10086','测试777777777','概念店','0','2015-07-26')
INSERT INTO Shops VALUES('测试8','3','测试888','10086','测试888888888','概念店','0','2015-07-26')
INSERT INTO Shops VALUES('测试9','3','测试999','10086','测试999999999','概念店','0','2015-07-26')


select * from Shops

--表名：用户(Users)
IF EXISTS (SELECT * FROM SYSOBJECTS WHERE NAME='Users')
DROP TABLE Users
GO
CREATE TABLE Users
(
	U_ID INT PRIMARY KEY IDENTITY(1,1),		--用户编号
	S_ID INT REFERENCES Shops(S_ID) ,		--店铺编号
	U_LoginName NVARCHAR(20),				--用户登录名
	U_Password NVARCHAR(50) ,				--密码
	U_RealName NVARCHAR(20),				--真实姓名
	U_Sex NVARCHAR(10),						--性别
	U_Telephone NVARCHAR(20),				--电话
	U_Role INT,								--角色
	U_CanDelete BIT,						--是否可以删除
)
GO
--数据基本操作（增删查改） 
INSERT INTO Users VALUES('1','bear','123','白志雄','男','15827791536','1','0')
INSERT INTO Users VALUES('1','admin001','123','吉瓦','男','1008611','2','0')
INSERT INTO Users VALUES('1','S001','123','还有谁','男','1008611','3','1')
INSERT INTO Users VALUES('1','S002','123','还有我','男','1008611','3','1')

INSERT INTO Users VALUES('2','admin002','123','谁井','男','1008611','2','0')
INSERT INTO Users VALUES('2','S003','123','水煮牛肉','男','1008611','2','1')
INSERT INTO Users VALUES('2','S004','123','麻婆豆腐','女','1008611','2','1')

INSERT INTO Users VALUES('3','admin003','123','杰快瑞','男','1008611','3','0')
INSERT INTO Users VALUES('3','S005','123','沔阳三蒸','男','1008611','3','1')
INSERT INTO Users VALUES('3','S006','123','红烧鱼块','女','1008611','3','1')

INSERT INTO Users VALUES('4','admin004','123','差迈尔','男','1008611','2','0')
INSERT INTO Users VALUES('4','S007','123','清炒三丝','男','1008611','3','1')
INSERT INTO Users VALUES('4','S008','123','蚂蚁上树','女','1008611','3','1')

INSERT INTO Users VALUES('5','admin005','123','欧瑞克','男','1008611','3','0')
INSERT INTO Users VALUES('5','S009','123','北京烤鸭','男','1008611','3','1')
INSERT INTO Users VALUES('5','S010','123','沁心莲子','女','1008611','3','1')

select * from Users
--表名：会员等级(CardLevels)
IF EXISTS (SELECT * FROM SYSOBJECTS WHERE NAME='CardLevels')
DROP TABLE CardLevels
GO
CREATE TABLE CardLevels
(
	CL_ID INT PRIMARY KEY IDENTITY(1,1),--等级编号
	CL_LevelName NVARCHAR(20),			--等级名称
	CL_NeedPoint NVARCHAR(50),			--所需最大积分
	CL_Point FLOAT,						--积分比例
	CL_Percent FLOAT,					--折扣比例
)
GO
--数据基本操作（增删查改） 
INSERT INTO CardLevels VALUES('英勇黄铜会员','100','0.5','9.5')
INSERT INTO CardLevels VALUES('不屈白银会员','500','0.6','9.0')
INSERT INTO CardLevels VALUES('荣耀黄金会员','2000','0.7','8.0')
INSERT INTO CardLevels VALUES('尊贵白金会员','5000','0.8','7.0')
INSERT INTO CardLevels VALUES('璀璨钻石会员','20000','0.9','6.0')
INSERT INTO CardLevels VALUES('牛嗒嗒王者会员','50000','1','5.0')
select * from CardLevels
--表名：会员(MemCards)
IF EXISTS (SELECT * FROM SYSOBJECTS WHERE NAME='MemCards')
DROP TABLE MemCards
GO
CREATE TABLE MemCards
(
	MC_ID INT PRIMARY KEY IDENTITY(1,1),	--会员编号
	CL_ID INT REFERENCES CardLevels(CL_ID),	--等级编号
	S_ID INT  REFERENCES Shops(S_ID) ,		--店铺编号
	MC_CardID NVARCHAR(50),					--会员卡号（自增长 待解决）
	MC_Password NVARCHAR(20),				--卡片密码
	MC_Name NVARCHAR(20),					--会员姓名
	MC_Sex BIT,								--会员性别
	MC_Mobile NVARCHAR(50),					--手机号码
	MC_Photo VARCHAR(200),					--靓照
	MC_Birthday_Month INT,					--会员生日--月
	MC_Birthday_Day INT,					--会员生日--日
	MC_BirthdayType BIT,				--会员生日类型
	MC_IsPast BIT,						--是否设置卡片过期时间
	MC_PastTime DATETIME,					--卡片过期时间
	MC_Point	INT,						--当前积分
	MC_Money REAL,							--卡片付费
	MC_TotalMoney REAL,						--累计消费
	MC_TotalCount INT,						--累计消费次数
	MC_State INT,							--卡片状态
	MC_IsPointAuto BIT,					--积分是否可以自动换成等级
	MC_RefererID INT,						--推荐人ID
	MC_RefererCard NVARCHAR(50),			--推荐人卡号
	MC_RefererName NVARCHAR(20),			--推荐人姓名
	MC_CreateTime DATETIME					--登记日期
)
GO
--INSERT INTO Users(CL_ID,S_ID,MC_Password,MC_Name) VALUES('5','1','123','9.5')
--创建触发器（MC_CardID自增长）
--create trigger MC_CardIDAutoUp			--创建触发器 MC_CardIDAutoUp
--on MemCards								--触发器的表对象
--instead of insert						--用触发语句来代替插入语句.
--as
--declare @MC_CardID nvarchar(10)			--声明变量@MC_CardID

--select @MC_CardID= @MC_CardID from inserted 
--insert into MemCards(MC_CardID)
--select cast(isnull(max(MC_CardID),'0') as int)+1
--from MemCards
go
--数据基本操作（增删查改） 
--INSERT INTO MemCards
--VALUES(CL_ID,S_ID,MC_CardID,MC_Password,MC_Name,MC_Sex,MC_Mobile,MC_Photo,MC_Birthday_Month,MC_Birthday_Day,MC_BirthdayType,MC_IsPast,MC_PastTime,MC_Point,MC_Money,MC_TotalMoney,MC_TotalCount,MC_State,MC_IsPointAuto,MC_RefererID,MC_RefererCard,MC_RefererName,MC_CreateTime)
INSERT INTO MemCards
VALUES(1,1,'80000','123','郑安涛',0,'13487132603',null,7,9,1,1,'2019-7-9',2000,0,500,2,1,0,1,'1111','aaaa','2016-6-5')
INSERT INTO MemCards
VALUES(2,1,'80001','123','王雨',1,'15171496056',null,6,9,1,1,'2019-2-9',1000,0,100,2,0,1,2,'100','uuuu','2016-6-5')
INSERT INTO MemCards
VALUES(3,1,'80002','123','安安',0,'12345678901',null,3,13,1,1,'2019-5-19',2000,0,500,2,1,0,3,'1999','qqqq','2016-6-5')
INSERT INTO MemCards
VALUES(4,1,'80003','123','夏夏',1,'18177965112',null,6,19,1,1,'2018-6-13',2500,0,200,2,2,1,4,'1000','dddd','2016-6-5')
INSERT INTO MemCards
VALUES(1,1,'80004','123','彬彬',0,'13425645673',null,7,5,1,1,'2017-7-5',2000,0,1000,2,2,0,1,'1','ccc','2016-6-5')
INSERT INTO MemCards
VALUES(3,2,'80005','123','阿潘',1,'13487132070',null,3,8,1,1,'2017-7-1',1000,0,500,2,1,1,2,'222','bbb','2016-6-5')
select m.*, c.CL_LevelName, s.S_Name from MemCards m, CardLevels c, Shops s where m.CL_ID = c.CL_ID and m.S_ID = s.S_ID
select m.*,c.CL_LevelName,s.S_Name from MemCards m,CardLevels c,Shops s where m.CL_ID=c.CL_ID and m.S_ID=s.S_ID
select * from MemCards left join CardLevels on MemCards.CL_ID = CardLevels.CL_ID  where S_ID =1 AND MC_State = 1

--表名：礼品(ExchangGifts)
IF EXISTS (SELECT * FROM SYSOBJECTS WHERE NAME='ExchangGifts')
DROP TABLE ExchangGifts
GO
CREATE TABLE ExchangGifts
(
	EG_ID INT PRIMARY KEY IDENTITY(1,1),	--礼品编号
	S_ID INT REFERENCES Shops(S_ID),		--店铺编号
	EG_GiftCode VARCHAR(255),				--礼品编码
	EG_GiftName VARCHAR(255),				--礼品名称
	EG_Photo VARCHAR(255),					--礼品图片
	EG_Point INT,							--所需积分
	EG_Number INT,							--总数量
	EG_ExchangNum INT,						--已兑换的数量
	EG_Remark VARCHAR(255),					--备注
)
GO
--数据基本操作（增删查改） 
 select * from ExchangGifts
INSERT INTO ExchangGifts VALUES(1,'150101','老北京锅巴','','5','100','30','食品')
INSERT INTO ExchangGifts VALUES(1,'150103','众望麻花','','5','100','15','食品')
INSERT INTO ExchangGifts VALUES(1,'150108','午时金银花露','','5','100','10','食品')

INSERT INTO ExchangGifts VALUES(1,'150102','台湾松糕','','10','100','20','食品')
INSERT INTO ExchangGifts VALUES(1,'150104','乐事薯片','','10','100','35','食品')
INSERT INTO ExchangGifts VALUES(1,'150105','趣多多','','10','100','17','食品')

INSERT INTO ExchangGifts VALUES(1,'150106','南方黑芝麻','','20','80','10','食品')
INSERT INTO ExchangGifts VALUES(1,'150107','南方黑芝麻(无糖)','','40','50','40','食品')
INSERT INTO ExchangGifts VALUES(1,'150108','冠生园峰蜜','','40','50','40','食品')

--表名：消费订单(ConsumeOrders)
IF EXISTS (SELECT * FROM SYSOBJECTS WHERE NAME='ConsumeOrders')
DROP TABLE ConsumeOrders
GO
CREATE TABLE ConsumeOrders
(
	CO_ID INT PRIMARY KEY IDENTITY(1,1),--消费编号
	S_ID INT REFERENCES Shops(S_ID),	--店铺编号
	U_ID INT REFERENCES Users(U_ID),	--用户编号
	CO_OrderCode NVARCHAR(20),			--订单号
	CO_OrderType TINYINT,				--订单类型
	MC_ID INT,							--会员编号
	MC_CardID NVARCHAR(50),				--会员卡号
	EG_ID INT,							--礼品编号
	CO_TotalMoney MONEY,				--额度
	CO_DiscountMoney MONEY,				--实际支付
	CO_GavePoint INT,					--兑/减积分
	CO_Recash MONEY,					--积分返现
	CO_Remark VARCHAR(255),				--备注
	CO_CreateTime DATETIME				--消费时间
)
GO
--数据基本操作（增删改查）
insert into ConsumeOrders values(1,2,10001,1,1,80000,2,80,100,15,15,'备注','2016-6-5')
--表名：转帐记录(TransferLogs)
IF EXISTS (SELECT * FROM SYSOBJECTS WHERE NAME='TransferLogs')
DROP TABLE TransferLogs
GO
CREATE TABLE TransferLogs
(
	TL_ID INT PRIMARY KEY IDENTITY(1,1),--转帐编号
	S_ID INT  REFERENCES Shops(S_ID),	--店铺编号
	U_ID INT REFERENCES Users(U_ID),	--用户编号
	TL_FromMC_ID INT,					--转出会员编号
	TL_FromMC_CardID NVARCHAR(50),		--转出会员卡号
	TL_ToMC_ID INT,						--转入会员编号
	TL_ToMC_CardID NVARCHAR(50),		--转入会员卡号
	TL_TransferMoney MONEY,				--转帐金额
	TL_Remark VARCHAR(200),				--转帐备注
	TL_CreateTime DATETIME,				--转帐日期
)
GO
select * from TransferLogs
INSERT INTO TransferLogs VALUES(1,1,1,'80000',2,'80001',3000,'还钱~~~~','2016-07-09')
INSERT INTO TransferLogs VALUES(2,3,2,'80001',2,'80002',1000,'哈哈哈','2016-07-09')
INSERT INTO TransferLogs VALUES(3,4,3,'80001',2,'80001',100,'兮兮','2016-07-09')
INSERT INTO TransferLogs VALUES(4,5,4,'80000',2,'80003',7000,'啊啊啊','2016-07-09')

--表名：兑换记录(ExchangLogs)
IF EXISTS (SELECT * FROM SYSOBJECTS WHERE NAME='ExchangLogs')
DROP TABLE ExchangLogs
GO
CREATE TABLE ExchangLogs
(
	EL_ID INT PRIMARY KEY IDENTITY(1,1),	--兑换记录编号
	S_ID INT ,								--店铺编号
	U_ID INT,								--用户编号
	MC_ID INT,								--会员编号
	MC_CardID  NVARCHAR(50),				--会员卡号
	MC_Name NVARCHAR(20),					--会员姓名
	EG_ID INT,								--礼品编号
	EG_GiftCode NVARCHAR(50),				--礼品编码
	EG_GiftName NVARCHAR(50),				--礼品名称
	EL_Number INT,							--兑换数量
	EL_Point INT,							--所用积分
	EL_CreateTime DATETIME					--兑换时间
)
GO
select * from MemCards
 ExchangLogs
INSERT INTO ExchangLogs(S_ID, U_ID, MC_ID, MC_CardID, MC_Name, EG_ID, EG_GiftCode, EG_GiftName, EL_Number, EL_Point, EL_CreateTime)	
VALUES('1','1','100','80000','郑安涛','1','150101','荒古遗精光剑','1','20',GETDATE())	
INSERT INTO ExchangLogs(S_ID, U_ID, MC_ID, MC_CardID, MC_Name, EG_ID, EG_GiftCode, EG_GiftName, EL_Number, EL_Point, EL_CreateTime)	
VALUES('1','1','100','80001','王雨','2','150103','荒古遗精光剑','1','20',GETDATE())	
INSERT INTO ExchangLogs(S_ID, U_ID, MC_ID, MC_CardID, MC_Name, EG_ID, EG_GiftCode, EG_GiftName, EL_Number, EL_Point, EL_CreateTime)	
VALUES('2','2','101','80002','安安' ,'3','150108','荒古遗精太刀','1','20',GETDATE())		
INSERT INTO ExchangLogs(S_ID, U_ID, MC_ID, MC_CardID, MC_Name, EG_ID, EG_GiftCode, EG_GiftName, EL_Number, EL_Point, EL_CreateTime)	
VALUES('3','3','102','80003','夏夏','4','150102','荒古遗精短剑','1','20',GETDATE())