if(not exists(select Id from FunctionInfos where Id = 'display_accessories_cycle_time_of_installation')) begin
	insert into FunctionInfos (Id, Name, Uri, IconName, IsEnabled, CompanyId, CreatedBy, CreatedDate, IsDraftRecord, RecordActionDate, DraftFromUpload) values 
	('display_accessories_cycle_time_of_installation','Display Accessories Cycle Time Of Installation','displayaccessoriescycletimeofinstallation','',1,1,'admin',GETDATE(),0,GETDATE(),0);
end

if(not exists(select id from RoleDetails where FunctionInfoId = 'display_accessories_cycle_time_of_installation')) begin
	insert into RoleDetails (RoleId, FunctionInfoId, AllowCreate, AllowRead, AllowUpdate, AllowDelete, ShowInMenu, AllowDownload, AllowPrint, AllowUpload,
	CompanyId, CreatedBy, CreatedDate, IsDraftRecord, RecordActionDate, DraftFromUpload) values
	(1,'display_accessories_cycle_time_of_installation',1,1,1,1,1,1,1,1,1,'admin',GETDATE(),0,GETDATE(),0);
end
