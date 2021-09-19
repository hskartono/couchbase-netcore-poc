if(not exists(select Id from FunctionInfos where Id = 'stock_condition_remarks')) begin
	insert into FunctionInfos (Id, Name, Uri, IconName, IsEnabled, CompanyId, CreatedBy, CreatedDate, IsDraftRecord, RecordActionDate, DraftFromUpload) values 
	('stock_condition_remarks','Stock Condition Remarks','stockconditionremarks','',1,1,'admin',GETDATE(),0,GETDATE(),0);
end

if(not exists(select id from RoleDetails where FunctionInfoId = 'stock_condition_remarks')) begin
	insert into RoleDetails (RoleId, FunctionInfoId, AllowCreate, AllowRead, AllowUpdate, AllowDelete, ShowInMenu, AllowDownload, AllowPrint, AllowUpload,
	CompanyId, CreatedBy, CreatedDate, IsDraftRecord, RecordActionDate, DraftFromUpload) values
	(1,'stock_condition_remarks',1,1,1,1,1,1,1,1,1,'admin',GETDATE(),0,GETDATE(),0);
end
