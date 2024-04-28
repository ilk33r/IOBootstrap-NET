package core

import (
	"fmt"
	"iobootstrap-cli-shared/common"
	"log"
	"os"
)

// Attribute defines a single SGR Code
type Attribute int

// Foreground text colors
const LogReset = "\033[0m"
const LogRed = "\033[31m"
const LogGreen = "\033[32m"
const LogYellow = "\033[33m"
const LogBlue = "\033[34m"
const LogMagenta = "\033[35m"
const LogCyan = "\033[36m"
const LogGray = "\033[37m"
const LogWhite = "\033[97m"

type ILogger interface {
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

type Logger struct {
}

func InitializeLogger() Logger {
	// Configure logging for a command-line program.
	log.SetFlags(0)
	log.SetPrefix("iobootstrap-cli: ")

	return Logger{}
}

func (logger Logger) LogError(message string) {
	coloredFormat := "\n" + LogRed + message + LogReset + "\n"
	fmt.Fprint(os.Stderr, coloredFormat)
	os.Exit(1)
}

func (logger Logger) LogErrorf(format string, v ...any) {
	coloredFormat := "\n" + LogRed + format + LogReset + "\n"
	logString := fmt.Sprintf(coloredFormat, v...)
	fmt.Fprint(os.Stderr, logString)
	os.Exit(1)
}

func (logger Logger) LogInfo(message string) {
	coloredFormat := LogMagenta + message + LogReset
	fmt.Fprint(os.Stdout, coloredFormat)
}

func (logger Logger) LogInfof(format string, v ...any) {
	coloredFormat := LogMagenta + format + LogReset
	logString := fmt.Sprintf(coloredFormat, v...)
	fmt.Fprint(os.Stdout, logString)
}

func (logger Logger) LogInfoSeparator(len int) {
	var separatorString = "\t"
	for i := 0; i < len; i++ {
		separatorString += "-"
	}

	coloredFormat := "\n" + LogCyan + separatorString + LogReset + "\n"
	fmt.Fprint(os.Stdout, coloredFormat)
}

func (logger Logger) LogSuccess(message string) {
	coloredFormat := "\t" + LogGreen + message + LogReset + "\n"
	fmt.Fprint(os.Stdout, coloredFormat)
}

func (logger Logger) LogSuccessf(format string, v ...any) {
	coloredFormat := "\t" + LogGreen + format + LogReset + "\n"
	logString := fmt.Sprintf(coloredFormat, v...)
	fmt.Fprint(os.Stdout, logString)
}

func (logger Logger) LogWarning(message string) {
	coloredFormat := LogYellow + message + LogReset + "\n"
	fmt.Fprint(os.Stdout, coloredFormat)
}

func (logger Logger) LogWarningf(format string, v ...any) {
	coloredFormat := LogYellow + format + LogReset + "\n"
	logString := fmt.Sprintf(coloredFormat, v...)
	fmt.Fprint(os.Stdout, logString)
}

func (logger Logger) LogVerbose(message string) {
	configuration := common.GetConfiguration()
	if configuration.Verbose() {
		coloredFormat := "\n" + LogGray + message + LogReset + "\n"
		fmt.Fprint(os.Stdout, coloredFormat)
	}
}

func (logger Logger) LogVerbosef(format string, v ...any) {
	configuration := common.GetConfiguration()
	if configuration.Verbose() {
		coloredFormat := "\n" + LogGray + format + LogReset + "\n"
		logString := fmt.Sprintf(coloredFormat, v...)
		fmt.Fprint(os.Stdout, logString)
	}
}

func (logger Logger) LogMessage(message string) {
	coloredFormat := message
	fmt.Fprint(os.Stdout, coloredFormat)
}

func (logger Logger) LogMessagef(message string, v ...any) {
	logString := fmt.Sprintf(message, v...)
	fmt.Fprint(os.Stdout, logString)
}
