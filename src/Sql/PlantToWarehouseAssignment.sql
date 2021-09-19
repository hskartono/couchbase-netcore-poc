if(not exists(select Id from FunctionInfos where Id = 'plant_to_warehouse_assignment')) begin
	insert into FunctionInfos (Id, Name, Uri, IconName, IsEnabled, CompanyId, CreatedBy, CreatedDate, IsDraftRecord, RecordActionDate, DraftFromUpload) values 
	('plant_to_warehouse_assignment','Plant To Warehouse Assignment','planttowarehouseassignment','',1,1,'admin',GETDATE(),0,GETDATE(),0);
end

if(not exists(select id from RoleDetails where FunctionInfoId = 'plant_to_warehouse_assignment')) begin
	insert into RoleDetails (RoleId, FunctionInfoId, AllowCreate, AllowRead, AllowUpdate, AllowDelete, ShowInMenu, AllowDownload, AllowPrint, AllowUpload,
	CompanyId, CreatedBy, CreatedDate, IsDraftRecord, RecordActionDate, DraftFromUpload) values
	(1,'plant_to_warehouse_assignment',1,1,1,1,1,1,1,1,1,'admin',GETDATE(),0,GETDATE(),0);
end
