if(not exists(select Id from FunctionInfos where Id = 'cycle_supplier_time_table')) begin
	insert into FunctionInfos (Id, Name, Uri, IconName, IsEnabled, CompanyId, CreatedBy, CreatedDate, IsDraftRecord, RecordActionDate, DraftFromUpload) values 
	('cycle_supplier_time_table','Cycle Supplier Time Table','cyclesuppliertimetable','',1,1,'admin',GETDATE(),0,GETDATE(),0);
end

if(not exists(select id from RoleDetails where FunctionInfoId = 'cycle_supplier_time_table')) begin
	insert into RoleDetails (RoleId, FunctionInfoId, AllowCreate, AllowRead, AllowUpdate, AllowDelete, ShowInMenu, AllowDownload, AllowPrint, AllowUpload,
	CompanyId, CreatedBy, CreatedDate, IsDraftRecord, RecordActionDate, DraftFromUpload) values
	(1,'cycle_supplier_time_table',1,1,1,1,1,1,1,1,1,'admin',GETDATE(),0,GETDATE(),0);
end
