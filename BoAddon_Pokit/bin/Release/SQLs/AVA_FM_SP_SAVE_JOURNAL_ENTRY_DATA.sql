if exists ( select "name" from "sysobjects" 
	where "name" = 'AVA_FM_SP_SAVE_JOURNAL_ENTRY_DATA'
	and "type" in ( N'P' ) )
    drop procedure "AVA_FM_SP_SAVE_JOURNAL_ENTRY_DATA"
go

/*=============================================================================
Create Date:2017-08-01
Create Programmer:Harold.Duan
Create Reason:Form [JournalEntry_Code_Generate] save journal entry data.
=============================================================================*/

create procedure "AVA_FM_SP_SAVE_JOURNAL_ENTRY_DATA"
	@TransId int,               --journal entry's TransId
	@VoucherCode nvarchar(20)   --journal entry's VoucherCode
as
begin
	-- Return values
	declare @error  int				-- Result (0 for no error)
	declare @error_message nvarchar (200) 		-- Error string to be displayed
	select @error = 0
	select @error_message = N'Ok'

	--------------------------------------------------------------------------------------------------------------------------------

	--	ADD	YOUR	CODE	HERE

	update t0 set t0."U_VoucherNum" = @VoucherCode,t0."U_Branch" = t1."BPLName"
	from "OJDT" t0 
	left join 
	(select distinct t0."TransId",t0."BPLId",t0."BPLName" from "JDT1" t0) t1 
	on t0."TransId" = t1."TransId"
	where t0."TransId" = @TransId

	--------------------------------------------------------------------------------------------------------------------------------

	-- Select the return values
	select @error, @error_message
end
go