if(not exists(select Id from FunctionInfos where Id = 'single_level_where_used_list')) begin
	insert into FunctionInfos (Id, Name, Uri, IconName, IsEnabled, CompanyId, CreatedBy, CreatedDate, IsDraftRecord, RecordActionDate, DraftFromUpload) values 
	('single_level_where_used_list','Single Level Where Used List','singlelevelwhereusedlist','',1,1,'admin',GETDATE(),0,GETDATE(),0);
end

if(not exists(select id from RoleDetails where FunctionInfoId = 'single_level_where_used_list')) begin
	insert into RoleDetails (RoleId, FunctionInfoId, AllowCreate, AllowRead, AllowUpdate, AllowDelete, ShowInMenu, AllowDownload, AllowPrint, AllowUpload,
	CompanyId, CreatedBy, CreatedDate, IsDraftRecord, RecordActionDate, DraftFromUpload) values
	(1,'single_level_where_used_list',1,1,1,1,1,1,1,1,1,'admin',GETDATE(),0,GETDATE(),0);
end
