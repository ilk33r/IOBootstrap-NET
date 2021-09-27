# https://github.com/rmbolger/Posh-ACME/blob/main/docs/Tutorial.md

Set-PAServer LE_PROD

New-PACertificate 'api.hopdedi.com','backoffice.hopdedi.com','app.hopdedi.com','www.hopdedi.com','hopdedi.com' -AcceptTOS -Contact 'iletisim@ilkerozcan.com.tr'

Get-PACertificate | fl

# The password on the PFX files is poshacme 