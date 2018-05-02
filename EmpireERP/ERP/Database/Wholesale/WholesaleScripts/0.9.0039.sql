/*---------------------------------------------------------------------------------------
  ������ ��� ���������� ���� �� ������ 0.9.39

  ��� ������:
	+ ��������� ���� "�������" � ��������� ��������� �� ������
	+ ��������� ���� "�������" � ������������� ���������� ������ �� ������
	+ ��������� ���� "�������" � ��������� �������� ������
	* ���� "�������" � ��������� ���������� ������� ������������ ��� ����������
	+ ���������� �������� ���� "�������" ��� ���� ���� ������������� ���������
	
---------------------------------------------------------------------------------------*/
--SET NOEXEC OFF	-- ��������� ������ ������� � ������ ����������� ����������
SET DATEFORMAT DMY
SET NOCOUNT ON
SET ARITHABORT ON
SET XACT_ABORT ON
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
GO

DECLARE @PreviousVersion varchar(15),	-- ����� ���������� ������
		@CurrentVersion varchar(15),	-- ����� ������� ������ ���� ������
		@NewVersion varchar(15),		-- ����� ����� ������
		@DataBaseName varchar(256),		-- ������� ���� ������
		@CurrentDate nvarchar(10),		-- ������� ����
		@CurrentTime nvarchar(10),		-- ������� �����
		@BackupTarget nvarchar(100)		-- ���� ������ ����� ���� ������

SET @PreviousVersion = '0.9.38' 			-- ����� ���������� ������
SET @NewVersion     = '0.9.39'			-- ����� ����� ������

SELECT @CurrentVersion = DataBaseVersion FROM Setting
IF @@ERROR <> 0
BEGIN
	PRINT '�������� ���� ������'
END
ELSE
BEGIN
	-- ������� ����� ���� ������
	-- �������� ������� ����
	SET @CurrentDate = CONVERT(nvarchar(20), GETDATE(), 104)	--	dd.mm.yyyy
	SET @CurrentTime = REPLACE(CONVERT(nvarchar(20), GETDATE(), 108) , ':', '.') --	hh:mm:ss
	SET @DataBaseName = DB_NAME()

	SET @BackupTarget = N'D:\Bizpulse\Backup\Update\' + @DataBaseName + '(' + CAST(@CurrentVersion as nvarchar(20)) + ') ' + 
		@CurrentDate + ' ' + @CurrentTime + N'.bak'

	BACKUP DATABASE @DataBaseName TO DISK = @BackupTarget WITH  INIT,  NOUNLOAD,  NAME = N'wholesale', NOSKIP, DESCRIPTION = 
		N'���������� ������', NOFORMAT

	IF @@ERROR <> 0
	BEGIN
		PRINT '������ �������� backup''�. ����������� ���������� ����������.'
	END
	ELSE
		BEGIN

		IF (@CurrentVersion <> @PreviousVersion)
		BEGIN
			PRINT '�������� ���� ������ ' + @DataBaseName + ' �� ������ ' + @NewVersion + 
				' ����� ������ �� ������  ' + @PreviousVersion +
				'. ������� ������: ' + @CurrentVersion
		END
		ELSE
		BEGIN
			--�������� ����������
			BEGIN TRAN

			--��������� ������ ���� ������
			UPDATE Setting 
			SET DataBaseVersion = @NewVersion		
		END
	END
END
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ��������� ������ ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO


--***********************************************************************************************
--*********** ��������� ������� *****************************************************************
--***********************************************************************************************
    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_DealPaymentDocument_Team]') AND parent_object_id = OBJECT_ID('dbo.[DealPaymentDocument]'))
alter table dbo.[DealPaymentDocument]  drop constraint FK_DealPaymentDocument_Team

    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ReturnFromClientWaybill_Team]') AND parent_object_id = OBJECT_ID('dbo.[ReturnFromClientWaybill]'))
alter table dbo.[ReturnFromClientWaybill]  drop constraint FK_ReturnFromClientWaybill_Team

alter table DealPaymentDocument add TeamId SMALLINT null;
alter table ReturnFromClientWaybill add TeamId SMALLINT null;

alter table dbo.[DealPaymentDocument] 
    add constraint FK_DealPaymentDocument_Team 
    foreign key (TeamId) 
    references dbo.[Team]
    
alter table dbo.[ReturnFromClientWaybill] 
    add constraint FK_ReturnFromClientWaybill_Team 
    foreign key (TeamId) 
    references dbo.[Team]
    
-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO


--***********************************************************************************************   
--*********** ��������� ������������ ������������� � �������� ������� ***************************    
--***********************************************************************************************
INSERT INTO UserTeam (UserId, TeamId)
SELECT 
	u.Id,
	1
FROM 
	dbo.[User] u
	left join UserTeam ut on ut.UserId = u.Id
WHERE
	ut.TeamId is NULL

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

--***********************************************************************************************
--********** ����������� ������� ����������� ****************************************************    
--***********************************************************************************************

-- ���������, ������ �� ������� ���������� ��� ������� � ���� �������. ���� ������ ��� � ����,
-- �� ������� ��������� �� ������
declare @TeamCountForSaleWaybill int;
declare @MessageForsaleWaybill varchar(8000);

set @TeamCountForSaleWaybill = 
(
	SELECT count(*)
	FROM 
		SaleWaybill sw
		join dbo.[User] u on u.Id = sw.SaleWaybillCuratorId
		join UserTeam ut on u.Id = ut.UserId
		join Team t on t.Id = ut.TeamId
	WHERE
		sw.TeamId is NULL
)

set @MessageForSaleWaybill = ''

SELECT @MessageForSaleWaybill = @MessageForSaleWaybill + case when @MessageForSaleWaybill <> '' then ', ' else ' ' end + sw.Number
FROM 
	SaleWaybill sw
	join dbo.[User] u on u.Id = sw.SaleWaybillCuratorId
	join UserTeam ut on u.Id = ut.UserId
	join Team t on t.Id = ut.TeamId
WHERE
	sw.TeamId is NULL
	
set @MessageForSaleWaybill = '���������� ����������, � ������� �� ������ ������� � ������� ������ ����� ��� � ���� �������. ������ ��������� :' + @MessageForsaleWaybill

IF @TeamCountForSaleWaybill > 1 RAISERROR(@MessageForSaleWaybill, 16, 1)

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO


-- ����������� ������� ��� ��������� ����������, ������� �� ����� ��
UPDATE SaleWaybill
SET
	TeamId = d.TeamId
FROM(
	SELECT sw.Id as SaleWaybillId
		,t.Id as TeamId
	FROM 
		SaleWaybill sw
		inner join dbo.[User] u on u.Id = sw.SaleWaybillCuratorId
		inner join UserTeam ut on u.Id = ut.UserId
		inner join Team t on t.Id = ut.TeamId
	WHERE
		sw.TeamId is NULL
) d
WHERE 
	Id = d.SaleWaybillId

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

--***********************************************************************************************
--*********** ����������� ������� ������� �� ������� ********************************************    
--***********************************************************************************************
CREATE TABLE #SaleWybillTeamDictionary(
	SaleWaybillId uniqueidentifier not null
	,TeamId smallint not null
)
-- ��������� ���������, ��� ������� ������� �����������
INSERT INTO #SaleWybillTeamDictionary
SELECT
	sw.Id as SaleWaybillId
	,sw.TeamId as TeamId
FROM 
	SaleWaybill sw
WHERE
	not(sw.TeamId is NULL);

-- ���������, ��������� �� ������ �� ��������� ������ ������
declare @TeamCountForPayment int;
declare @MessageTeamCountForPayment varchar(8000);

set @TeamCountForPayment = (
	SELECT COUNT(*)
	FROM
		DealPaymentDocument dp
		join DealPaymentFromClient dpfc on dpfc.Id = dp.Id
		join DealPaymentDocumentDistribution dpdd on dp.Id = dpdd.SourceDealPaymentDocumentId
		join DealPaymentDocumentDistributionToSaleWaybill swd on swd.Id = dpdd.Id
		join #SaleWybillTeamDictionary swtd on swtd.SaleWaybillId = swd.SaleWaybillId
	GROUP BY
		dp.Id
		,swtd.SaleWaybillId
	having count(distinct swtd.TeamId) > 1
)
set @MessageTeamCountForPayment = '';
SELECT @MessageTeamCountForPayment = @MessageTeamCountForPayment + case when @MessageTeamCountForPayment <> '' then ', ' else ' ' end + CONVERT(varchar(50), dp.Id)
	FROM
		DealPaymentDocument dp
		join DealPaymentFromClient dpfc on dpfc.Id = dp.Id
		join DealPaymentDocumentDistribution dpdd on dp.Id = dpdd.SourceDealPaymentDocumentId
		join DealPaymentDocumentDistributionToSaleWaybill swd on swd.Id = dpdd.Id
		join #SaleWybillTeamDictionary swtd on swtd.SaleWaybillId = swd.SaleWaybillId
	GROUP BY
		dp.Id
		,swtd.SaleWaybillId
	having count(distinct swtd.TeamId) > 1
set @MessageTeamCountForPayment = '��������� ������, ������� ��������� �� ��������� ������ ������. ���� �����:' + @MessageTeamCountForPayment

IF @TeamCountForPayment > 1 RAISERROR(@MessageTeamCountForPayment, 16, 1)

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

-- ������ ���, ����������� ������� ������� �� �������
UPDATE DealPaymentDocument
SET
	TeamId = d.TeamId
FROM( 
	SELECT 
		dp.Id as Id
		,swtd.TeamId as TeamId
	FROM
		DealPaymentDocument dp
		join DealPaymentFromClient dpfc on dpfc.Id = dp.Id
		join DealPaymentDocumentDistribution dpdd on dp.Id = dpdd.SourceDealPaymentDocumentId
		join DealPaymentDocumentDistributionToSaleWaybill swd on swd.Id = dpdd.Id
		join #SaleWybillTeamDictionary swtd on swtd.SaleWaybillId = swd.SaleWaybillId
	GROUP BY
		dp.Id
		,swtd.TeamId) d
WHERE
	DealPaymentDocument.Id = d.Id
	
drop table #SaleWybillTeamDictionary;
GO

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO      
        
--***********************************************************************************************        
--************ ����������� ������� ���������� �������������� ************************************
--***********************************************************************************************
CREATE TABLE #SaleWybillTeamDictionary(
	SaleWaybillId uniqueidentifier not null
	,TeamId smallint not null
)
-- ��������� ���������, ��� ������� ������� �����������
INSERT INTO #SaleWybillTeamDictionary
SELECT
	sw.Id as SaleWaybillId
	,sw.TeamId as TeamId
FROM 
	SaleWaybill sw
WHERE
	not(sw.TeamId is NULL);

-- ���������, ��������� �� ������������� �� ��������� ������ ������
declare @TeamCountForCreditCorrection int;
declare @MessageTeamCountForCreditCorrection varchar(8000);

set @TeamCountForCreditCorrection = (
	SELECT COUNT(*)
	FROM
		DealPaymentDocument dp
		join DealInitialBalanceCorrection dibc on dibc.Id = dp.Id
		join DealPaymentDocumentDistribution dpdd on dp.Id = dpdd.SourceDealPaymentDocumentId
		join DealPaymentDocumentDistributionToSaleWaybill swd on swd.Id = dpdd.Id
		join #SaleWybillTeamDictionary swtd on swtd.SaleWaybillId = swd.SaleWaybillId
	GROUP BY
		dp.Id
		,swtd.SaleWaybillId
	having count(distinct swtd.TeamId) > 1
)
set @MessageTeamCountForCreditCorrection = '';
SELECT @MessageTeamCountForCreditCorrection = @MessageTeamCountForCreditCorrection + case when @MessageTeamCountForCreditCorrection <> '' then ', ' else ' ' end + CONVERT(varchar(50), dp.Id)
	FROM
		DealPaymentDocument dp
		join DealInitialBalanceCorrection dibc on dibc.Id = dp.Id
		join DealPaymentDocumentDistribution dpdd on dp.Id = dpdd.SourceDealPaymentDocumentId
		join DealPaymentDocumentDistributionToSaleWaybill swd on swd.Id = dpdd.Id
		join #SaleWybillTeamDictionary swtd on swtd.SaleWaybillId = swd.SaleWaybillId
	GROUP BY
		dp.Id
		,swtd.SaleWaybillId
	having count(distinct swtd.TeamId) > 1
set @MessageTeamCountForCreditCorrection = '��������� ���������� �������������, ������� ��������� �� ��������� ������ ������. ���� �������������:' + @MessageTeamCountForCreditCorrection

IF @TeamCountForCreditCorrection > 1 RAISERROR(@MessageTeamCountForCreditCorrection, 16, 1)

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

-- ������ ���, ����������� ������� ���������� ��������������
UPDATE DealPaymentDocument
SET
	TeamId = d.TeamId
FROM( 
	SELECT 
		dp.Id as Id
		,swtd.TeamId as TeamId
	FROM
		DealPaymentDocument dp
		join DealInitialBalanceCorrection dibc on dibc.Id = dp.Id
		join DealPaymentDocumentDistribution dpdd on dp.Id = dpdd.SourceDealPaymentDocumentId
		join DealPaymentDocumentDistributionToSaleWaybill swd on swd.Id = dpdd.Id
		join #SaleWybillTeamDictionary swtd on swtd.SaleWaybillId = swd.SaleWaybillId
	GROUP BY
		dp.Id
		,swtd.TeamId) d
WHERE
	DealPaymentDocument.Id = d.Id
	
drop table #SaleWybillTeamDictionary;
GO

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO      

--***********************************************************************************************
--************* ����������� ������� ��������� ������ �������� ***********************************    
--***********************************************************************************************
-- ���������, �� ������� �� ������� ������ ����������� ������ ������
declare @TeamCountForPaymentToClient int;
declare @MessageTeamCountForPaymentToClient varchar(8000);

set @TeamCountForPaymentToClient = (
	SELECT COUNT(*)
	FROM 
		DealPaymentDocument dpd
		join DealPaymentDocumentDistribution dpdd on dpdd.SourceDealPaymentDocumentId = dpd.Id
		join DealPaymentDocumentDistributionToDealPaymentDocument dpddtdpd on dpddtdpd.Id = dpdd.Id
		join DealPaymentToClient dptc on dptc.Id = dpddtdpd.DestinationDealPaymentDocumentId
	
	GROUP BY dptc.Id
	having count(distinct dpd.TeamId) > 1
)
set @MessageTeamCountForPaymentToClient = '';
SELECT @MessageTeamCountForPaymentToClient = @MessageTeamCountForPaymentToClient + case when @MessageTeamCountForPaymentToClient <> '' then ', ' else ' ' end + CONVERT(varchar(50), dptc.Id)
	FROM 
		DealPaymentDocument dpd
		join DealPaymentDocumentDistribution dpdd on dpdd.SourceDealPaymentDocumentId = dpd.Id
		join DealPaymentDocumentDistributionToDealPaymentDocument dpddtdpd on dpddtdpd.Id = dpdd.Id
		join DealPaymentToClient dptc on dptc.Id = dpddtdpd.DestinationDealPaymentDocumentId
	GROUP BY dptc.Id
	having count(distinct dpd.TeamId) > 1
set @MessageTeamCountForPaymentToClient = '���������� �������� ������ �������, �� ������� ��������� ��������� ������ ������. ���� ��������� ������:' + @MessageTeamCountForPaymentToClient

IF @TeamCountForPaymentToClient > 1 RAISERROR(@MessageTeamCountForPaymentToClient, 16, 1)

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

-- ������ ���. ����������� ������� ��� �������� ������
UPDATE DealPaymentDocument
SET TeamId = d.TeamId
FROM(
	SELECT
		dptc.Id
		,dpd.TeamId
	FROM 
		DealPaymentDocument dpd
		join DealPaymentDocumentDistribution dpdd on dpdd.SourceDealPaymentDocumentId = dpd.Id
		join DealPaymentDocumentDistributionToDealPaymentDocument dpddtdpd on dpddtdpd.Id = dpdd.Id
		join DealPaymentToClient dptc on dptc.Id = dpddtdpd.DestinationDealPaymentDocumentId
	) d
WHERE
	DealPaymentDocument.Id = d.Id

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

--***********************************************************************************************
--************* ����������� ������� ��������� ������������� *************************************
--***********************************************************************************************    

-- ���������, �� �������� �� ������������� ����������� ������ ������
declare @TeamCountForDebitCorrection int;
declare @MessageTeamCountForDebitCorrection varchar(8000);

set @TeamCountForDebitCorrection = (
	SELECT COUNT(*)
	FROM 
		DealPaymentDocument dpd
		join DealPaymentDocumentDistribution dpdd on dpdd.SourceDealPaymentDocumentId = dpd.Id
		join DealPaymentDocumentDistributionToDealPaymentDocument dpddtdpd on dpddtdpd.Id = dpdd.Id
		join DealDebitInitialBalanceCorrection ddibc on ddibc.Id = dpddtdpd.DestinationDealPaymentDocumentId
	GROUP BY ddibc.Id
	having count(distinct dpd.TeamId) > 1
)
set @MessageTeamCountForDebitCorrection = '';
SELECT @MessageTeamCountForDebitCorrection = @MessageTeamCountForDebitCorrection + case when @MessageTeamCountForDebitCorrection <> '' then ', ' else ' ' end + CONVERT(varchar(50), ddibc.Id)
	FROM 
		DealPaymentDocument dpd
		join DealPaymentDocumentDistribution dpdd on dpdd.SourceDealPaymentDocumentId = dpd.Id
		join DealPaymentDocumentDistributionToDealPaymentDocument dpddtdpd on dpddtdpd.Id = dpdd.Id
		join DealDebitInitialBalanceCorrection ddibc on ddibc.Id = dpddtdpd.DestinationDealPaymentDocumentId
	GROUP BY ddibc.Id
	having count(distinct dpd.TeamId) > 1
set @MessageTeamCountForDebitCorrection = '��������� ��������� �������������, �� ������� ��������� ��������� ������ ������. ���� �������������:' + @MessageTeamCountForDebitCorrection

IF @TeamCountForDebitCorrection > 1 RAISERROR(@MessageTeamCountForDebitCorrection, 16, 1)

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

-- ������ ���, ����������� ������� ���������� ��������������
UPDATE DealPaymentDocument
SET
	TeamId = d.TeamId
FROM( 
	SELECT 
		ddibc.Id as Id
		,dpd.TeamId as TeamId
	FROM
		DealPaymentDocument dpd
		join DealPaymentDocumentDistribution dpdd on dpdd.SourceDealPaymentDocumentId = dpd.Id
		join DealPaymentDocumentDistributionToDealPaymentDocument dpddtdpd on dpddtdpd.Id = dpdd.Id
		join DealDebitInitialBalanceCorrection ddibc on ddibc.Id = dpddtdpd.DestinationDealPaymentDocumentId
	GROUP BY
		ddibc.Id
		,dpd.TeamId) d
WHERE
	DealPaymentDocument.Id = d.Id	


-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

--***********************************************************************************************
--******** ����������� ������� �������, ������� ��������� ������ �� ��������� ��������� *********
--***********************************************************************************************
-- ����������� ����� ���������� ������� �������� ������. ������ NULL ������ ������� ������ 
-- � ����� ����������

UPDATE DealPaymentDocument
SET
	TeamId = d.TeamId
FROM(
	SELECT
		dpd.Id as Id
		,MIN(t.Id) as TeamId
	FROM
		DealPaymentDocument dpd
		join Deal d on dpd.DealId = d.Id
		join dbo.[User] u on u.Id = d.CuratorId
		join UserTeam ut on ut.UserId = u.Id
		join Team t on t.Id = ut.TeamId
	GROUP BY dpd.Id
	)d
WHERE
	DealPaymentDocument.Id = d.Id 
	AND DealPaymentDocument.TeamId is NULL
    

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

--***********************************************************************************************
--********** ����������� ������� ��� ��������� ������� ******************************************
--***********************************************************************************************    

-- ���������, �� ������ �� ������� �� ���������� ������ ������
declare @TeamCountForRetrunFromClientWaybill int;
declare @MessageTeamCountForRetrunFromClientWaybill varchar(8000);

set @TeamCountForRetrunFromClientWaybill = (
	SELECT COUNT(*)
	FROM
		ReturnFromClientWaybill rfcw
		join ReturnFromClientWaybillRow rfcwr on rfcw.Id = rfcwr.ReturnFromClientWaybillId
		join SaleWaybillRow swr on rfcwr.SaleWaybillRowId = swr.Id
		join SaleWaybill sw on sw.Id = swr.SaleWaybillId
	GROUP BY rfcw.Id
	having count(distinct sw.TeamId) > 1
)
set @MessageTeamCountForRetrunFromClientWaybill = '';
SELECT @MessageTeamCountForRetrunFromClientWaybill = @MessageTeamCountForRetrunFromClientWaybill + case when @MessageTeamCountForRetrunFromClientWaybill <> '' then ', ' else ' ' end + CONVERT(varchar(50), rfcw.Id)
	FROM
		ReturnFromClientWaybill rfcw
		join ReturnFromClientWaybillRow rfcwr on rfcw.Id = rfcwr.ReturnFromClientWaybillId
		join SaleWaybillRow swr on rfcwr.SaleWaybillRowId = swr.Id
		join SaleWaybill sw on sw.Id = swr.SaleWaybillId
	GROUP BY rfcw.Id
	having count(distinct sw.TeamId) > 1
set @MessageTeamCountForRetrunFromClientWaybill = '���������� �������� ������, ������� ������� �� ����������� ������ ������. ���� ���������:' + @MessageTeamCountForRetrunFromClientWaybill

IF @TeamCountForRetrunFromClientWaybill > 1 RAISERROR(@MessageTeamCountForRetrunFromClientWaybill, 16, 1)

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

-- ������ ���, ����������� ������� ���������
UPDATE ReturnFromClientWaybill
SET
	TeamId = d.TeamId
FROM(
	SELECT rfcw.Id as Id
		,Min(sw.TeamId) as TeamId
	FROM
		ReturnFromClientWaybill rfcw
		join ReturnFromClientWaybillRow rfcwr on rfcw.Id = rfcwr.ReturnFromClientWaybillId
		join SaleWaybillRow swr on rfcwr.SaleWaybillRowId = swr.Id
		join SaleWaybill sw on sw.Id = swr.SaleWaybillId
	GROUP BY rfcw.Id
	) d
WHERE
	ReturnFromClientWaybill.Id = d.Id


-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

--***********************************************************************************************
--********** ����������� ������� ��� ��������� ������� ��� ������� ******************************
--*********************************************************************************************** 
-- ���������� ������� �� �������� ���������

UPDATE ReturnFromClientWaybill
SET
	TeamId = d.TeamId
FROM(
	SELECT
		rfcw.Id as Id
		,MIN(t.Id) as TeamId
	FROM
		ReturnFromClientWaybill rfcw
		join dbo.[User] u on u.Id = rfcw.ReturnFromClientWaybillCuratorId
		join UserTeam ut on ut.UserId = u.Id
		join Team t on t.Id = ut.TeamId
	GROUP BY rfcw.Id
	)d
WHERE
	ReturnFromClientWaybill.Id = d.Id 
	AND ReturnFromClientWaybill.TeamId is NULL

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

--***********************************************************************************************
--************ ������ ���� ������� ������������ ��� ���������� **********************************
--***********************************************************************************************
alter table DealPaymentDocument alter column TeamId SMALLINT NOT null;
alter table ReturnFromClientWaybill alter column TeamId SMALLINT NOT null;
alter table SaleWaybill alter column TeamId SMALLINT NOT null;

IF EXISTS(SELECT * FROM sys.sysindexes WHERE name = 'IX_AcceptedSaleIndicator_StorageId_UserId_TeamId_DealId')
DROP INDEX IX_AcceptedSaleIndicator_StorageId_UserId_TeamId_DealId ON [dbo].[AcceptedSaleIndicator]
GO
alter table AcceptedSaleIndicator alter column TeamId SMALLINT NOT null;
GO
CREATE INDEX IX_AcceptedSaleIndicator_StorageId_UserId_TeamId_DealId ON [AcceptedSaleIndicator] ([StorageId], [UserId], [TeamId], [DealId])
GO
IF EXISTS(SELECT * FROM sys.sysindexes WHERE name = 'IX_ShippedSaleIndicator_StorageId_UserId_TeamId_DealId')
DROP INDEX IX_ShippedSaleIndicator_StorageId_UserId_TeamId_DealId ON [dbo].[ShippedSaleIndicator]
GO
alter table ShippedSaleIndicator alter column TeamId SMALLINT NOT null;
GO
CREATE INDEX IX_ShippedSaleIndicator_StorageId_UserId_TeamId_DealId ON [ShippedSaleIndicator] ([StorageId], [UserId], [TeamId], [DealId])
GO

alter table AcceptedReturnFromClientIndicator alter column TeamId SMALLINT NOT null;
alter table ReceiptedReturnFromClientIndicator alter column TeamId SMALLINT NOT null;
alter table ReturnFromClientBySaleAcceptanceDateIndicator alter column TeamId SMALLINT NOT null;
alter table ReturnFromClientBySaleShippingDateIndicator alter column TeamId SMALLINT NOT null;

-- � ����� �����
IF @@TRANCOUNT > 0 BEGIN COMMIT TRAN PRINT '���������� ��������� �������' END
GO