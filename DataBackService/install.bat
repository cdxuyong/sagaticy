set name="hr_databack"
nssm install %name% DataBackService.exe
nssm set %name% AppDirectory %cd%
nssm start %name%