/****** Object:  Job [Load_eCheckIn_from_Ministry_Platform]    Script Date: 1/4/2016 3:42:52 PM ******/
BEGIN TRANSACTION
DECLARE @ReturnCode INT
SELECT @ReturnCode = 0
/****** Object:  JobCategory [[Uncategorized (Local)]]    Script Date: 1/4/2016 3:42:52 PM ******/
IF NOT EXISTS (SELECT name FROM msdb.dbo.syscategories WHERE name=N'[Uncategorized (Local)]' AND category_class=1)
BEGIN
EXEC @ReturnCode = msdb.dbo.sp_add_category @class=N'JOB', @type=N'LOCAL', @name=N'[Uncategorized (Local)]'
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
END

DECLARE @jobId BINARY(16)

-- Cleanup existing job
select @jobId = job_id from msdb.dbo.sysjobs where (name = N'Load_eCheckIn_from_Ministry_Platform')
IF @jobID IS NOT NULL
EXEC @ReturnCode = msdb.dbo.sp_delete_job @job_id = @jobId

-- Create new job
SET @jobId = null
EXEC @ReturnCode =  msdb.dbo.sp_add_job @job_name=N'Load_eCheckIn_from_Ministry_Platform', 
-- TODO: Should we disable so we don't run in PROD before it's really PROD, and make that a seperate step of implementation plan?
		@enabled=1, 
		@notify_level_eventlog=0, 
		@notify_level_email=0, 
		@notify_level_netsend=0, 
		@notify_level_page=0, 
		@delete_level=0, 
		@description=N'Ministry Platform to eCheckIn ETL load process', 
		@category_name=N'[Uncategorized (Local)]', 
		@owner_login_name=N'MinistryPlatform', @job_id = @jobId OUTPUT
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback

EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'crds_Echeck_ETL_Load', 
		@step_id=1, 
		@cmdexec_success_code=0, 
		@on_success_action=1, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'crds_Echeck_ETL_Load', 
		@database_name=N'eCheckIn', 
		@flags=0
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback

-- Notify via email on failures
EXEC msdb.dbo.sp_update_job @job_id=@jobId, 
		@notify_level_email=2,
		@notify_email_operator_name=N'Server Alerts'

-- Add Schedules
DECLARE @schedule_id INT

EXEC @ReturnCode = msdb.dbo.sp_add_jobschedule @job_id=@jobId, @name=N'Monday to Saturday 7AM', 
		@enabled=1, 
		@freq_type=8, 
		@freq_interval=126, 
		@freq_subday_type=1, 
		@freq_subday_interval=0, 
		@freq_relative_interval=0, 
		@freq_recurrence_factor=1, 
		@active_start_date=20160104, 
		@active_end_date=99991231, 
		@active_start_time=70000, 
		@active_end_time=235959, @schedule_id = @schedule_id OUTPUT
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback

EXEC @ReturnCode = msdb.dbo.sp_add_jobschedule @job_id=@jobId, @name=N'Monday to Thursday 1PM', 
		@enabled=1, 
		@freq_type=8, 
		@freq_interval=30, 
		@freq_subday_type=1, 
		@freq_subday_interval=0, 
		@freq_relative_interval=0, 
		@freq_recurrence_factor=1, 
		@active_start_date=20160104, 
		@active_end_date=99991231, 
		@active_start_time=130000, 
		@active_end_time=235959, @schedule_id = @schedule_id OUTPUT
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback

EXEC @ReturnCode = msdb.dbo.sp_add_jobschedule @job_id=@jobId, @name=N'Friday 4 PM', 
		@enabled=1, 
		@freq_type=8, 
		@freq_interval=32, 
		@freq_subday_type=1, 
		@freq_subday_interval=0, 
		@freq_relative_interval=0, 
		@freq_recurrence_factor=1, 
		@active_start_date=20160104, 
		@active_end_date=99991231, 
		@active_start_time=160000, 
		@active_end_time=235959, @schedule_id = @schedule_id OUTPUT
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback


EXEC @ReturnCode = msdb.dbo.sp_update_job @job_id = @jobId, @start_step_id = 1
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
EXEC @ReturnCode = msdb.dbo.sp_add_jobserver @job_id = @jobId, @server_name = N'(local)'
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
COMMIT TRANSACTION
GOTO EndSave
QuitWithRollback:
    IF (@@TRANCOUNT > 0) ROLLBACK TRANSACTION
EndSave:

GO