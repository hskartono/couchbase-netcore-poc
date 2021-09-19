if(not exists(select Id from FunctionInfos where Id = 'warehouse_role_configuration')) begin
	insert into FunctionInfos (Id, Name, Uri, IconName, IsEnabled, CompanyId, CreatedBy, CreatedDate, IsDraftRecord, RecordActionDate, DraftFromUpload) values 
	('warehouse_role_configuration','Warehouse Role Configuration','warehouseroleconfiguration','',1,1,'admin',GETDATE(),0,GETDATE(),0);
end

if(not exists(select id from RoleDetails where FunctionInfoId = 'warehouse_role_configuration')) begin
	insert into RoleDetails (RoleId, FunctionInfoId, AllowCreate, AllowRead, AllowUpdate, AllowDelete, ShowInMenu, AllowDownload, AllowPrint, AllowUpload,
	CompanyId, CreatedBy, CreatedDate, IsDraftRecord, RecordActionDate, DraftFromUpload) values
	(1,'warehouse_role_configuration',1,1,1,1,1,1,1,1,1,'admin',GETDATE(),0,GETDATE(),0);
end
if(not exists(select Id from FunctionInfos where Id = 'warehouse_role_configuration')) begin
	insert into FunctionInfos (Id, Name, Uri, IconName, IsEnabled, CompanyId, CreatedBy, CreatedDate, IsDraftRecord, RecordActionDate, DraftFromUpload) values 
	('warehouse_role_configuration','Warehouse Role Configuration','warehouseroleconfiguration','',1,1,'admin',GETDATE(),0,GETDATE(),0);
end

if(not exists(select id from RoleDetails where FunctionInfoId = 'warehouse_role_configuration')) begin
	insert into RoleDetails (RoleId, FunctionInfoId, AllowCreate, AllowRead, AllowUpdate, AllowDelete, ShowInMenu, AllowDownload, AllowPrint, AllowUpload,
	CompanyId, CreatedBy, CreatedDate, IsDraftRecord, RecordActionDate, DraftFromUpload) values
	(1,'warehouse_role_configuration',1,1,1,1,1,1,1,1,1,'admin',GETDATE(),0,GETDATE(),0);
end

if(not exists(select Id from FunctionInfos where Id = 'warehouse_role_configuration_detail')) begin
	insert into FunctionInfos (Id, Name, Uri, IconName, IsEnabled, CompanyId, CreatedBy, CreatedDate, IsDraftRecord, RecordActionDate, DraftFromUpload) values 
	('warehouse_role_configuration_detail','Warehouse Role Configuration Detail','warehouseroleconfigurationdetail','',1,1,'admin',GETDATE(),0,GETDATE(),0);
end

if(not exists(select id from RoleDetails where FunctionInfoId = 'warehouse_role_configuration_detail')) begin
	insert into RoleDetails (RoleId, FunctionInfoId, AllowCreate, AllowRead, AllowUpdate, AllowDelete, ShowInMenu, AllowDownload, AllowPrint, AllowUpload,
	CompanyId, CreatedBy, CreatedDate, IsDraftRecord, RecordActionDate, DraftFromUpload) values
	(1,'warehouse_role_configuration_detail',1,1,1,1,1,1,1,1,1,'admin',GETDATE(),0,GETDATE(),0);
end
