if(not exists(select Id from FunctionInfos where Id = 'combine_case_config_flat')) begin
	insert into FunctionInfos (Id, Name, Uri, IconName, IsEnabled, CompanyId, CreatedBy, CreatedDate, IsDraftRecord, RecordActionDate, DraftFromUpload) values 
	('combine_case_config_flat','Combine Case Config Flat','combinecaseconfigflat','',1,1,'admin',GETDATE(),0,GETDATE(),0);
end

if(not exists(select id from RoleDetails where FunctionInfoId = 'combine_case_config_flat')) begin
	insert into RoleDetails (RoleId, FunctionInfoId, AllowCreate, AllowRead, AllowUpdate, AllowDelete, ShowInMenu, AllowDownload, AllowPrint, AllowUpload,
	CompanyId, CreatedBy, CreatedDate, IsDraftRecord, RecordActionDate, DraftFromUpload) values
	(1,'combine_case_config_flat',1,1,1,1,1,1,1,1,1,'admin',GETDATE(),0,GETDATE(),0);
end
