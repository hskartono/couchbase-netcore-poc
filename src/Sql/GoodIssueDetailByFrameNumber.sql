if(not exists(select Id from FunctionInfos where Id = 'good_issue_detail_by_frame_number')) begin
	insert into FunctionInfos (Id, Name, Uri, IconName, IsEnabled, CompanyId, CreatedBy, CreatedDate, IsDraftRecord, RecordActionDate, DraftFromUpload) values 
	('good_issue_detail_by_frame_number','Good Issue Detail By Frame Number','goodissuedetailbyframenumber','',1,1,'admin',GETDATE(),0,GETDATE(),0);
end

if(not exists(select id from RoleDetails where FunctionInfoId = 'good_issue_detail_by_frame_number')) begin
	insert into RoleDetails (RoleId, FunctionInfoId, AllowCreate, AllowRead, AllowUpdate, AllowDelete, ShowInMenu, AllowDownload, AllowPrint, AllowUpload,
	CompanyId, CreatedBy, CreatedDate, IsDraftRecord, RecordActionDate, DraftFromUpload) values
	(1,'good_issue_detail_by_frame_number',1,1,1,1,1,1,1,1,1,'admin',GETDATE(),0,GETDATE(),0);
end
