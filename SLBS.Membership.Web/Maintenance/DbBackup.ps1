#Database to export
$DatabaseName = "SLSBS"
$ResourceGroupName = "Common-AusSouthEast"
$ServerName = "tnuaussoutheast"
$ServerAdmin = "tnu"
$ServerPassword = "!Carp147"
$SecurePassword = ConvertTo-SecureString -String $ServerPassword -AsPlainText -Force
$Creds = New-Object -TypeName System.Management.Automation.PSCredential -ArgumentList $ServerAdmin, $SecurePassword

#Genereate unique filename for BACPAC
$BacpacName = $DatabaseName + (Get-Date).ToString("yyyy-MM-dd HH:mm") + ".bacpac"

#storage account info
$BaseStorageUri = "https://tnu.blob.core.windows.net/tnucontainer/"
$BacpacUri = $BaseStorageUri + $BacpacName
$StorageKeyType = "StorageAccessKey"
$StorageKey = "+kyotH0MbhAw9/MTMQ1wip4zx9SlBBxQYq2HGDXdZw1gVYwI4SMLF/eCRAJtaLpPTi0f4LO9zzX/e+7BHNd/ew=="

$exportRequest = New-AzureRmSqlDatabaseExport -ResourceGroupName $ResourceGroupName -ServerName $ServerName `
    -DatabaseName $DatabaseName -StorageKeyType $StorageKeyType -StorageKey $StorageKey -StorageUri $BacpacUri `
    -AdministratorLogin $Creds.UserName -AdministratorLoginPassword $Creds.Password

#check status
Get-AzureRmSqlDatabaseImportExportStatus -OperationStatusLink $exportRequest.OperationStatusLink

# https://management.azure.com/subscriptions/d77b3c4e-688b-4a63-bfec-f3d04c2a3dd6/resourceGroups/Default-Web-AustraliaEast/providers/Microsoft.Sql/servers/tnu/databases/SLSBS/importExportOperationResults/ae78bd8d-b4b5-4b57-8691-3491c432d4c5?api-version=2014-04-01-Preview