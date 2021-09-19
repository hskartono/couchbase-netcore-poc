if(not exists(select Id from FunctionInfos where Id = 'report_update_stock_condition')) begin
	insert into FunctionInfos (Id, Name, Uri, IconName, IsEnabled, CompanyId, CreatedBy, CreatedDate, IsDraftRecord, RecordActionDate, DraftFromUpload) values 
	('report_update_stock_condition','Report Update Stock Condition','reportupdatestockcondition','',1,1,'admin',GETDATE(),0,GETDATE(),0);
end

if(not exists(select id from RoleDetails where FunctionInfoId = 'report_update_stock_condition')) begin
	insert into RoleDetails (RoleId, FunctionInfoId, AllowCreate, AllowRead, AllowUpdate, AllowDelete, ShowInMenu, AllowDownload, AllowPrint, AllowUpload,
	CompanyId, CreatedBy, CreatedDate, IsDraftRecord, RecordActionDate, DraftFromUpload) values
	(1,'report_update_stock_condition',1,1,1,1,1,1,1,1,1,'admin',GETDATE(),0,GETDATE(),0);
end
