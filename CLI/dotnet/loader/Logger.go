package loader

type Logger interface {
	LogError(message string)
	LogErrorf(format string, v ...any)
	LogInfo(message string)
	LogInfof(format string, v ...any)
	LogInfoSeparator(len int)
	LogSuccess(message string)
	LogSuccessf(format string, v ...any)
	LogWarning(message string)
	LogWarningf(format string, v ...any)
	LogVerbose(message string)
	LogVerbosef(format string, v ...any)
	LogMessage(message string)
	LogMessagef(message string, v ...any)
}
