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
const Reset = "\033[0m"
const Red = "\033[31m"
const Green = "\033[32m"
const Yellow = "\033[33m"
const Blue = "\033[34m"
const Magenta = "\033[35m"
const Cyan = "\033[36m"
const Gray = "\033[37m"
const White = "\033[97m"

func InitializeLogger() {
	// Configure logging for a command-line program.
	log.SetFlags(0)
	log.SetPrefix("iobootstrap-cli: ")
}

func LogError(message string) {
	coloredFormat := "\n" + Red + message + Reset + "\n"
	fmt.Fprint(os.Stderr, coloredFormat)
	os.Exit(1)
}

func LogErrorf(format string, v ...any) {
	coloredFormat := "\n" + Red + format + Reset + "\n"
	logString := fmt.Sprintf(coloredFormat, v...)
	fmt.Fprint(os.Stderr, logString)
	os.Exit(1)
}

func LogInfo(message string) {
	coloredFormat := Magenta + message + Reset
	fmt.Fprint(os.Stdout, coloredFormat)
}

func LogInfof(format string, v ...any) {
	coloredFormat := Magenta + format + Reset
	logString := fmt.Sprintf(coloredFormat, v...)
	fmt.Fprint(os.Stdout, logString)
}

func LogInfoSeparator(len int) {
	var separatorString = "\t"
	for i := 0; i < len; i++ {
		separatorString += "-"
	}

	coloredFormat := "\n" + Cyan + separatorString + Reset + "\n"
	fmt.Fprint(os.Stdout, coloredFormat)
}

func LogSuccess(message string) {
	coloredFormat := "\t" + Green + message + Reset + "\n"
	fmt.Fprint(os.Stdout, coloredFormat)
}

func LogSuccessf(format string, v ...any) {
	coloredFormat := "\t" + Green + format + Reset + "\n"
	logString := fmt.Sprintf(coloredFormat, v...)
	fmt.Fprint(os.Stdout, logString)
}

func LogWarning(message string) {
	coloredFormat := Yellow + message + Reset + "\n"
	fmt.Fprint(os.Stdout, coloredFormat)
}

func LogWarningf(format string, v ...any) {
	coloredFormat := Yellow + format + Reset + "\n"
	logString := fmt.Sprintf(coloredFormat, v...)
	fmt.Fprint(os.Stdout, logString)
}

func LogVerbose(message string) {
	configuration := common.GetConfiguration()
	if configuration.Verbose() {
		coloredFormat := "\n" + Gray + message + Reset + "\n"
		fmt.Fprint(os.Stdout, coloredFormat)
	}
}

func LogVerbosef(format string, v ...any) {
	configuration := common.GetConfiguration()
	if configuration.Verbose() {
		coloredFormat := "\n" + Gray + format + Reset + "\n"
		logString := fmt.Sprintf(coloredFormat, v...)
		fmt.Fprint(os.Stdout, logString)
	}
}

func LogMessage(message string) {
	coloredFormat := message
	fmt.Fprint(os.Stdout, coloredFormat)
}
