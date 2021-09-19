if(not exists(select Id from FunctionInfos where Id = 'delivery_instruction_detail_case')) begin
	insert into FunctionInfos (Id, Name, Uri, IconName, IsEnabled, CompanyId, CreatedBy, CreatedDate, IsDraftRecord, RecordActionDate, DraftFromUpload) values 
	('delivery_instruction_detail_case','Delivery Instruction Detail Case','deliveryinstructiondetailcase','',1,1,'admin',GETDATE(),0,GETDATE(),0);
end

if(not exists(select id from RoleDetails where FunctionInfoId = 'delivery_instruction_detail_case')) begin
	insert into RoleDetails (RoleId, FunctionInfoId, AllowCreate, AllowRead, AllowUpdate, AllowDelete, ShowInMenu, AllowDownload, AllowPrint, AllowUpload,
	CompanyId, CreatedBy, CreatedDate, IsDraftRecord, RecordActionDate, DraftFromUpload) values
	(1,'delivery_instruction_detail_case',1,1,1,1,1,1,1,1,1,'admin',GETDATE(),0,GETDATE(),0);
end
