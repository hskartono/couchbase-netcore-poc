if(not exists(select Id from FunctionInfos where Id = 'scheduler_cron_interval')) begin
	insert into FunctionInfos (Id, Name, Uri, IconName, IsEnabled, CompanyId, CreatedBy, CreatedDate, IsDraftRecord, RecordActionDate, DraftFromUpload) values 
	('scheduler_cron_interval','Scheduler Cron Interval','schedulercroninterval','',1,1,'admin',GETDATE(),0,GETDATE(),0);
end

if(not exists(select id from RoleDetails where FunctionInfoId = 'scheduler_cron_interval')) begin
	insert into RoleDetails (RoleId, FunctionInfoId, AllowCreate, AllowRead, AllowUpdate, AllowDelete, ShowInMenu, AllowDownload, AllowPrint, AllowUpload,
	CompanyId, CreatedBy, CreatedDate, IsDraftRecord, RecordActionDate, DraftFromUpload) values
	(1,'scheduler_cron_interval',1,1,1,1,1,1,1,1,1,'admin',GETDATE(),0,GETDATE(),0);
end
