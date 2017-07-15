USE master
GO

IF EXISTS(SELECT * FROM SYSDATABASES WHERE NAME ='MPMS_DB')
DROP DATABASE MPMS_DB
GO
CREATE DATABASE MPMS_DB
GO

USE MPMS_DB
GO

--�����������������(Categories)
IF EXISTS (SELECT * FROM SYSOBJECTS WHERE NAME='Categories')
DROP TABLE Categories
GO
CREATE TABLE Categories
(
	C_Category VARCHAR(20) PRIMARY KEY ,			--�������
	C_Illustration VARCHAR(20),						--���˵��
	C_IsShow BIT									--�Ƿ��ڽ�������ʾ
)
go
--���ݻ�����������ɾ��ģ� 
INSERT INTO Categories VALUES('S_Category','','0')
INSERT INTO Categories VALUES('U_Role','','0')
INSERT INTO Categories VALUES('MC_State','','0')
INSERT INTO Categories VALUES('CO_OrderType','','0')

DELETE Categories WHERE C_Category=''
SELECT * FROM Categories
UPDATE Categories SET C_Illustration='',C_IsShow='' WHERE C_Category=''

--����������������(CategoryItems)
IF EXISTS (SELECT * FROM SYSOBJECTS WHERE NAME='CategoryItems')
DROP TABLE CategoryItems
GO
CREATE TABLE CategoryItems
(
	C_Category VARCHAR(20) REFERENCES Categories(C_Category) ,			--�������
	CI_ID INT,															--�������
	CI_Name VARCHAR(20)													--���������
)
GO
--���ݻ�����������ɾ��ģ� 
INSERT INTO CategoryItems VALUES('S_Category','1','�ܲ�')
INSERT INTO CategoryItems VALUES('S_Category','2','���˵�')
INSERT INTO CategoryItems VALUES('S_Category','3','��Ӫ��')

INSERT INTO CategoryItems VALUES('U_Role','1','ϵͳ����Ա')
INSERT INTO CategoryItems VALUES('U_Role','2','�곤')
INSERT INTO CategoryItems VALUES('U_Role','3','ҵ��Ա')

INSERT INTO CategoryItems VALUES('MC_State','1','����')
INSERT INTO CategoryItems VALUES('MC_State','2','��ʧ')
INSERT INTO CategoryItems VALUES('MC_State','3','����')

INSERT INTO CategoryItems VALUES('CO_OrderType','1','�һ�����')
INSERT INTO CategoryItems VALUES('CO_OrderType','2','���ַ���')
INSERT INTO CategoryItems VALUES('CO_OrderType','3','������')
INSERT INTO CategoryItems VALUES('CO_OrderType','4','ת���ܻ���')
INSERT INTO CategoryItems VALUES('CO_OrderType','5','��������')

SELECT * FROM CategoryItems


--����������(Shops)
IF EXISTS (SELECT * FROM SYSOBJECTS WHERE NAME='Shops')
DROP TABLE Shops
GO
CREATE TABLE Shops
(
	S_ID INT PRIMARY KEY IDENTITY(1,1),		--���̱��
	S_Name VARCHAR(20),						--��������
	S_Category INT ,						--�������
	S_ContactName VARCHAR(20),				--��ϵ��
	S_ContactTel VARCHAR(20),				--��ϵ�绰
	S_Address VARCHAR(50),					--��ַ
	S_Remark VARCHAR(100),					--��ע
	S_IsHasSetAdmin BIT,					--�Ƿ��ѷ������Ա
	S_CreateTime DATETIME,					--����ʱ��
)
GO
--���ݻ�����������ɾ��ģ� 
INSERT INTO Shops VALUES('��Ʒ��������ܲ�','1','���','15827791536','�人�ж����羰����������ְҵѧԺ','�����','1','1997-04-07')

INSERT INTO Shops VALUES('��Ʒ���ӱ�����ƽ�ֵ�','2','��ѧϰ','10086','�����в�ƽ������ϸ��ַ������','�����','1','2015-05-03')
INSERT INTO Shops VALUES('��Ʒ���ӱ��������ֵ�','2','������','10086','�����г���������ϸ��ַ������','�����','0','2015-07-26')

INSERT INTO Shops VALUES('��Ʒ���������ȷֵ�','2','�ʹ���','10086','�����г���������ϸ��ַ������','�����','0','2015-07-26')
INSERT INTO Shops VALUES('��Ʒ�������԰�ֵ�','3','����˪','10086','�人������԰�ֵ�','�����','0','2015-07-26')

INSERT INTO Shops VALUES('��Ʒ���ӻ��ķֵ�','3','ȡ��Ǯ','10086','�人������԰�ֵ�','�����','0','2015-07-27')

INSERT INTO Shops VALUES('����1','2','����111','10086','����111111111','�����','0','2015-05-03')
INSERT INTO Shops VALUES('����2','2','����222','10086','����222222222','�����','0','2015-07-26')
INSERT INTO Shops VALUES('����3','3','����333','10086','����333333333','�����','0','2015-07-26')
INSERT INTO Shops VALUES('����4','3','����444','10086','����444444444','�����','0','2015-07-26')
INSERT INTO Shops VALUES('����5','2','����555','10086','����555555555','�����','0','2015-05-03')
INSERT INTO Shops VALUES('����6','2','����666','10086','����666666666','�����','0','2015-07-26')
INSERT INTO Shops VALUES('����7','3','����777','10086','����777777777','�����','0','2015-07-26')
INSERT INTO Shops VALUES('����8','3','����888','10086','����888888888','�����','0','2015-07-26')
INSERT INTO Shops VALUES('����9','3','����999','10086','����999999999','�����','0','2015-07-26')


select * from Shops

--�������û�(Users)
IF EXISTS (SELECT * FROM SYSOBJECTS WHERE NAME='Users')
DROP TABLE Users
GO
CREATE TABLE Users
(
	U_ID INT PRIMARY KEY IDENTITY(1,1),		--�û����
	S_ID INT REFERENCES Shops(S_ID) ,		--���̱��
	U_LoginName NVARCHAR(20),				--�û���¼��
	U_Password NVARCHAR(50) ,				--����
	U_RealName NVARCHAR(20),				--��ʵ����
	U_Sex NVARCHAR(10),						--�Ա�
	U_Telephone NVARCHAR(20),				--�绰
	U_Role INT,								--��ɫ
	U_CanDelete BIT,						--�Ƿ����ɾ��
)
GO
--���ݻ�����������ɾ��ģ� 
INSERT INTO Users VALUES('1','bear','123','��־��','��','15827791536','1','0')
INSERT INTO Users VALUES('1','admin001','123','����','��','1008611','2','0')
INSERT INTO Users VALUES('1','S001','123','����˭','��','1008611','3','1')
INSERT INTO Users VALUES('1','S002','123','������','��','1008611','3','1')

INSERT INTO Users VALUES('2','admin002','123','˭��','��','1008611','2','0')
INSERT INTO Users VALUES('2','S003','123','ˮ��ţ��','��','1008611','2','1')
INSERT INTO Users VALUES('2','S004','123','���Ŷ���','Ů','1008611','2','1')

INSERT INTO Users VALUES('3','admin003','123','�ܿ���','��','1008611','3','0')
INSERT INTO Users VALUES('3','S005','123','��������','��','1008611','3','1')
INSERT INTO Users VALUES('3','S006','123','�������','Ů','1008611','3','1')

INSERT INTO Users VALUES('4','admin004','123','������','��','1008611','2','0')
INSERT INTO Users VALUES('4','S007','123','�峴��˿','��','1008611','3','1')
INSERT INTO Users VALUES('4','S008','123','��������','Ů','1008611','3','1')

INSERT INTO Users VALUES('5','admin005','123','ŷ���','��','1008611','3','0')
INSERT INTO Users VALUES('5','S009','123','������Ѽ','��','1008611','3','1')
INSERT INTO Users VALUES('5','S010','123','��������','Ů','1008611','3','1')

select * from Users
--��������Ա�ȼ�(CardLevels)
IF EXISTS (SELECT * FROM SYSOBJECTS WHERE NAME='CardLevels')
DROP TABLE CardLevels
GO
CREATE TABLE CardLevels
(
	CL_ID INT PRIMARY KEY IDENTITY(1,1),--�ȼ����
	CL_LevelName NVARCHAR(20),			--�ȼ�����
	CL_NeedPoint NVARCHAR(50),			--����������
	CL_Point FLOAT,						--���ֱ���
	CL_Percent FLOAT,					--�ۿ۱���
)
GO
--���ݻ�����������ɾ��ģ� 
INSERT INTO CardLevels VALUES('Ӣ�»�ͭ��Ա','100','0.5','9.5')
INSERT INTO CardLevels VALUES('����������Ա','500','0.6','9.0')
INSERT INTO CardLevels VALUES('��ҫ�ƽ��Ա','2000','0.7','8.0')
INSERT INTO CardLevels VALUES('���׽��Ա','5000','0.8','7.0')
INSERT INTO CardLevels VALUES('����ʯ��Ա','20000','0.9','6.0')
INSERT INTO CardLevels VALUES('ţ�����߻�Ա','50000','1','5.0')
select * from CardLevels
--��������Ա(MemCards)
IF EXISTS (SELECT * FROM SYSOBJECTS WHERE NAME='MemCards')
DROP TABLE MemCards
GO
CREATE TABLE MemCards
(
	MC_ID INT PRIMARY KEY IDENTITY(1,1),	--��Ա���
	CL_ID INT REFERENCES CardLevels(CL_ID),	--�ȼ����
	S_ID INT  REFERENCES Shops(S_ID) ,		--���̱��
	MC_CardID NVARCHAR(50),					--��Ա���ţ������� �������
	MC_Password NVARCHAR(20),				--��Ƭ����
	MC_Name NVARCHAR(20),					--��Ա����
	MC_Sex BIT,								--��Ա�Ա�
	MC_Mobile NVARCHAR(50),					--�ֻ�����
	MC_Photo VARCHAR(200),					--����
	MC_Birthday_Month INT,					--��Ա����--��
	MC_Birthday_Day INT,					--��Ա����--��
	MC_BirthdayType BIT,				--��Ա��������
	MC_IsPast BIT,						--�Ƿ����ÿ�Ƭ����ʱ��
	MC_PastTime DATETIME,					--��Ƭ����ʱ��
	MC_Point	INT,						--��ǰ����
	MC_Money REAL,							--��Ƭ����
	MC_TotalMoney REAL,						--�ۼ�����
	MC_TotalCount INT,						--�ۼ����Ѵ���
	MC_State INT,							--��Ƭ״̬
	MC_IsPointAuto BIT,					--�����Ƿ�����Զ����ɵȼ�
	MC_RefererID INT,						--�Ƽ���ID
	MC_RefererCard NVARCHAR(50),			--�Ƽ��˿���
	MC_RefererName NVARCHAR(20),			--�Ƽ�������
	MC_CreateTime DATETIME					--�Ǽ�����
)
GO
--INSERT INTO Users(CL_ID,S_ID,MC_Password,MC_Name) VALUES('5','1','123','9.5')
--������������MC_CardID��������
--create trigger MC_CardIDAutoUp			--���������� MC_CardIDAutoUp
--on MemCards								--�������ı����
--instead of insert						--�ô������������������.
--as
--declare @MC_CardID nvarchar(10)			--��������@MC_CardID

--select @MC_CardID= @MC_CardID from inserted 
--insert into MemCards(MC_CardID)
--select cast(isnull(max(MC_CardID),'0') as int)+1
--from MemCards
go
--���ݻ�����������ɾ��ģ� 
--INSERT INTO MemCards
--VALUES(CL_ID,S_ID,MC_CardID,MC_Password,MC_Name,MC_Sex,MC_Mobile,MC_Photo,MC_Birthday_Month,MC_Birthday_Day,MC_BirthdayType,MC_IsPast,MC_PastTime,MC_Point,MC_Money,MC_TotalMoney,MC_TotalCount,MC_State,MC_IsPointAuto,MC_RefererID,MC_RefererCard,MC_RefererName,MC_CreateTime)
INSERT INTO MemCards
VALUES(1,1,'80000','123','֣����',0,'13487132603',null,7,9,1,1,'2019-7-9',2000,0,500,2,1,0,1,'1111','aaaa','2016-6-5')
INSERT INTO MemCards
VALUES(2,1,'80001','123','����',1,'15171496056',null,6,9,1,1,'2019-2-9',1000,0,100,2,0,1,2,'100','uuuu','2016-6-5')
INSERT INTO MemCards
VALUES(3,1,'80002','123','����',0,'12345678901',null,3,13,1,1,'2019-5-19',2000,0,500,2,1,0,3,'1999','qqqq','2016-6-5')
INSERT INTO MemCards
VALUES(4,1,'80003','123','����',1,'18177965112',null,6,19,1,1,'2018-6-13',2500,0,200,2,2,1,4,'1000','dddd','2016-6-5')
INSERT INTO MemCards
VALUES(1,1,'80004','123','���',0,'13425645673',null,7,5,1,1,'2017-7-5',2000,0,1000,2,2,0,1,'1','ccc','2016-6-5')
INSERT INTO MemCards
VALUES(3,2,'80005','123','����',1,'13487132070',null,3,8,1,1,'2017-7-1',1000,0,500,2,1,1,2,'222','bbb','2016-6-5')
select m.*, c.CL_LevelName, s.S_Name from MemCards m, CardLevels c, Shops s where m.CL_ID = c.CL_ID and m.S_ID = s.S_ID
select m.*,c.CL_LevelName,s.S_Name from MemCards m,CardLevels c,Shops s where m.CL_ID=c.CL_ID and m.S_ID=s.S_ID
select * from MemCards left join CardLevels on MemCards.CL_ID = CardLevels.CL_ID  where S_ID =1 AND MC_State = 1

--��������Ʒ(ExchangGifts)
IF EXISTS (SELECT * FROM SYSOBJECTS WHERE NAME='ExchangGifts')
DROP TABLE ExchangGifts
GO
CREATE TABLE ExchangGifts
(
	EG_ID INT PRIMARY KEY IDENTITY(1,1),	--��Ʒ���
	S_ID INT REFERENCES Shops(S_ID),		--���̱��
	EG_GiftCode VARCHAR(255),				--��Ʒ����
	EG_GiftName VARCHAR(255),				--��Ʒ����
	EG_Photo VARCHAR(255),					--��ƷͼƬ
	EG_Point INT,							--�������
	EG_Number INT,							--������
	EG_ExchangNum INT,						--�Ѷһ�������
	EG_Remark VARCHAR(255),					--��ע
)
GO
--���ݻ�����������ɾ��ģ� 
 select * from ExchangGifts
INSERT INTO ExchangGifts VALUES(1,'150101','�ϱ�������','','5','100','30','ʳƷ')
INSERT INTO ExchangGifts VALUES(1,'150103','�����黨','','5','100','15','ʳƷ')
INSERT INTO ExchangGifts VALUES(1,'150108','��ʱ������¶','','5','100','10','ʳƷ')

INSERT INTO ExchangGifts VALUES(1,'150102','̨���ɸ�','','10','100','20','ʳƷ')
INSERT INTO ExchangGifts VALUES(1,'150104','������Ƭ','','10','100','35','ʳƷ')
INSERT INTO ExchangGifts VALUES(1,'150105','Ȥ���','','10','100','17','ʳƷ')

INSERT INTO ExchangGifts VALUES(1,'150106','�Ϸ���֥��','','20','80','10','ʳƷ')
INSERT INTO ExchangGifts VALUES(1,'150107','�Ϸ���֥��(����)','','40','50','40','ʳƷ')
INSERT INTO ExchangGifts VALUES(1,'150108','����԰����','','40','50','40','ʳƷ')

--���������Ѷ���(ConsumeOrders)
IF EXISTS (SELECT * FROM SYSOBJECTS WHERE NAME='ConsumeOrders')
DROP TABLE ConsumeOrders
GO
CREATE TABLE ConsumeOrders
(
	CO_ID INT PRIMARY KEY IDENTITY(1,1),--���ѱ��
	S_ID INT REFERENCES Shops(S_ID),	--���̱��
	U_ID INT REFERENCES Users(U_ID),	--�û����
	CO_OrderCode NVARCHAR(20),			--������
	CO_OrderType TINYINT,				--��������
	MC_ID INT,							--��Ա���
	MC_CardID NVARCHAR(50),				--��Ա����
	EG_ID INT,							--��Ʒ���
	CO_TotalMoney MONEY,				--���
	CO_DiscountMoney MONEY,				--ʵ��֧��
	CO_GavePoint INT,					--��/������
	CO_Recash MONEY,					--���ַ���
	CO_Remark VARCHAR(255),				--��ע
	CO_CreateTime DATETIME				--����ʱ��
)
GO
--���ݻ�����������ɾ�Ĳ飩
insert into ConsumeOrders values(1,2,10001,1,1,80000,2,80,100,15,15,'��ע','2016-6-5')
--������ת�ʼ�¼(TransferLogs)
IF EXISTS (SELECT * FROM SYSOBJECTS WHERE NAME='TransferLogs')
DROP TABLE TransferLogs
GO
CREATE TABLE TransferLogs
(
	TL_ID INT PRIMARY KEY IDENTITY(1,1),--ת�ʱ��
	S_ID INT  REFERENCES Shops(S_ID),	--���̱��
	U_ID INT REFERENCES Users(U_ID),	--�û����
	TL_FromMC_ID INT,					--ת����Ա���
	TL_FromMC_CardID NVARCHAR(50),		--ת����Ա����
	TL_ToMC_ID INT,						--ת���Ա���
	TL_ToMC_CardID NVARCHAR(50),		--ת���Ա����
	TL_TransferMoney MONEY,				--ת�ʽ��
	TL_Remark VARCHAR(200),				--ת�ʱ�ע
	TL_CreateTime DATETIME,				--ת������
)
GO
select * from TransferLogs
INSERT INTO TransferLogs VALUES(1,1,1,'80000',2,'80001',3000,'��Ǯ~~~~','2016-07-09')
INSERT INTO TransferLogs VALUES(2,3,2,'80001',2,'80002',1000,'������','2016-07-09')
INSERT INTO TransferLogs VALUES(3,4,3,'80001',2,'80001',100,'����','2016-07-09')
INSERT INTO TransferLogs VALUES(4,5,4,'80000',2,'80003',7000,'������','2016-07-09')

--�������һ���¼(ExchangLogs)
IF EXISTS (SELECT * FROM SYSOBJECTS WHERE NAME='ExchangLogs')
DROP TABLE ExchangLogs
GO
CREATE TABLE ExchangLogs
(
	EL_ID INT PRIMARY KEY IDENTITY(1,1),	--�һ���¼���
	S_ID INT ,								--���̱��
	U_ID INT,								--�û����
	MC_ID INT,								--��Ա���
	MC_CardID  NVARCHAR(50),				--��Ա����
	MC_Name NVARCHAR(20),					--��Ա����
	EG_ID INT,								--��Ʒ���
	EG_GiftCode NVARCHAR(50),				--��Ʒ����
	EG_GiftName NVARCHAR(50),				--��Ʒ����
	EL_Number INT,							--�һ�����
	EL_Point INT,							--���û���
	EL_CreateTime DATETIME					--�һ�ʱ��
)
GO
select * from MemCards
 ExchangLogs
INSERT INTO ExchangLogs(S_ID, U_ID, MC_ID, MC_CardID, MC_Name, EG_ID, EG_GiftCode, EG_GiftName, EL_Number, EL_Point, EL_CreateTime)	
VALUES('1','1','100','80000','֣����','1','150101','�Ĺ��ž��⽣','1','20',GETDATE())	
INSERT INTO ExchangLogs(S_ID, U_ID, MC_ID, MC_CardID, MC_Name, EG_ID, EG_GiftCode, EG_GiftName, EL_Number, EL_Point, EL_CreateTime)	
VALUES('1','1','100','80001','����','2','150103','�Ĺ��ž��⽣','1','20',GETDATE())	
INSERT INTO ExchangLogs(S_ID, U_ID, MC_ID, MC_CardID, MC_Name, EG_ID, EG_GiftCode, EG_GiftName, EL_Number, EL_Point, EL_CreateTime)	
VALUES('2','2','101','80002','����' ,'3','150108','�Ĺ��ž�̫��','1','20',GETDATE())		
INSERT INTO ExchangLogs(S_ID, U_ID, MC_ID, MC_CardID, MC_Name, EG_ID, EG_GiftCode, EG_GiftName, EL_Number, EL_Point, EL_CreateTime)	
VALUES('3','3','102','80003','����','4','150102','�Ĺ��ž��̽�','1','20',GETDATE())