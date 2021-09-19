if(not exists(select Id from FunctionInfos where Id = 'physical_inventory_document_status')) begin
	insert into FunctionInfos (Id, Name, Uri, IconName, IsEnabled, CompanyId, CreatedBy, CreatedDate, IsDraftRecord, RecordActionDate, DraftFromUpload) values 
	('physical_inventory_document_status','Physical Inventory Document Status','physicalinventorydocumentstatus','',1,1,'admin',GETDATE(),0,GETDATE(),0);
end

if(not exists(select id from RoleDetails where FunctionInfoId = 'physical_inventory_document_status')) begin
	insert into RoleDetails (RoleId, FunctionInfoId, AllowCreate, AllowRead, AllowUpdate, AllowDelete, ShowInMenu, AllowDownload, AllowPrint, AllowUpload,
	CompanyId, CreatedBy, CreatedDate, IsDraftRecord, RecordActionDate, DraftFromUpload) values
	(1,'physical_inventory_document_status',1,1,1,1,1,1,1,1,1,'admin',GETDATE(),0,GETDATE(),0);
end
