if(not exists(select Id from FunctionInfos where Id = 'accumulation_accuracy_and_delivery_accuracy')) begin
	insert into FunctionInfos (Id, Name, Uri, IconName, IsEnabled, CompanyId, CreatedBy, CreatedDate, IsDraftRecord, RecordActionDate, DraftFromUpload) values 
	('accumulation_accuracy_and_delivery_accuracy','Accumulation Accuracy And Delivery Accuracy','accumulationaccuracyanddeliveryaccuracy','',1,1,'admin',GETDATE(),0,GETDATE(),0);
end

if(not exists(select id from RoleDetails where FunctionInfoId = 'accumulation_accuracy_and_delivery_accuracy')) begin
	insert into RoleDetails (RoleId, FunctionInfoId, AllowCreate, AllowRead, AllowUpdate, AllowDelete, ShowInMenu, AllowDownload, AllowPrint, AllowUpload,
	CompanyId, CreatedBy, CreatedDate, IsDraftRecord, RecordActionDate, DraftFromUpload) values
	(1,'accumulation_accuracy_and_delivery_accuracy',1,1,1,1,1,1,1,1,1,'admin',GETDATE(),0,GETDATE(),0);
end
