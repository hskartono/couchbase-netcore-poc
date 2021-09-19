if(not exists(select Id from FunctionInfos where Id = 'rack_configuration_flat')) begin
	insert into FunctionInfos (Id, Name, Uri, IconName, IsEnabled, CompanyId, CreatedBy, CreatedDate, IsDraftRecord, RecordActionDate, DraftFromUpload) values 
	('rack_configuration_flat','Rack Configuration Flat','rackconfigurationflat','',1,1,'admin',GETDATE(),0,GETDATE(),0);
end

if(not exists(select id from RoleDetails where FunctionInfoId = 'rack_configuration_flat')) begin
	insert into RoleDetails (RoleId, FunctionInfoId, AllowCreate, AllowRead, AllowUpdate, AllowDelete, ShowInMenu, AllowDownload, AllowPrint, AllowUpload,
	CompanyId, CreatedBy, CreatedDate, IsDraftRecord, RecordActionDate, DraftFromUpload) values
	(1,'rack_configuration_flat',1,1,1,1,1,1,1,1,1,'admin',GETDATE(),0,GETDATE(),0);
end
