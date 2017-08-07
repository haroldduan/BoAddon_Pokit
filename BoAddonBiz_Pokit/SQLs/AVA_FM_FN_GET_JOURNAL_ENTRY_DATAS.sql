if exists ( select "name" from "sysobjects" 
	where "name" = 'AVA_FM_FN_GET_JOURNAL_ENTRY_DATAS'
	and "type" in ( N'TF' ) )
    drop function "AVA_FM_FN_GET_JOURNAL_ENTRY_DATAS"
go

/*=============================================================================
Create Date:2017-07-25
Create Programmer:Harold.Duan
Create Reason:Form [JournalEntry_Code_Generate] get journal entry datas.
=============================================================================*/

create function "AVA_FM_FN_GET_JOURNAL_ENTRY_DATAS"
(
)
returns @JournalEntrys table
(
--******************UI 关键字段不允许改变*********************--
	[TransId] int,
	[VoucherCode] nvarchar(20),
	[TransType] nvarchar(20),
	[TaxDate] datetime,
	[Ref1] nvarchar(100),
	[Ref2] nvarchar(100),
	[BPLId] int,
	[BPLName] nvarchar(100),
	[Memo] nvarchar(50),
	[BaseRef] nvarchar(11),
	[Project] nvarchar(20),
	[LocTotal] numeric(19,6)
--************************************************************--
)
as
begin
	insert into @JournalEntrys ("TransId","TransType","TaxDate","Ref1","Ref2","BPLId","BPLName",
		"Memo","BaseRef","Project","LocTotal","VoucherCode")
		select t0."TransId",t0."TransType",t0."RefDate",t0."Ref1",t0."Ref2",t1."BPLId",t1."BPLName",
			t0."Memo",t0."BaseRef",t0."Project",t0."LocTotal",t0."U_VoucherNum" "VoucherCode" 
		from "OJDT" t0
		left join 
		(
			select distinct t0."TransId",t0."BPLId",t0."BPLName" from "JDT1" t0
		) t1 on t0."TransId" = t1."TransId"
		
	return
end
go