Set-PAServer LE_PROD

New-PACertificate 'api.comigofit.com','backoffice.comigofit.com','jobs.comigofit.com','app.comigofit.com','www.comigofit.com','comigofit.com' -AcceptTOS -Contact 'iletisim@ilkerozcan.com.tr'

Get-PACertificate | fl