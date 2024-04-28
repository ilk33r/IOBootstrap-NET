package core

import (
	"fmt"
	"time"
)

type ICLIStep interface {
	StartStep(name string)
	EndStep()
	Summary()
}

type CLIStep struct {
	logger           *Logger
	startTime        int64
	steps            *[]string
	stepStartTimes   *[]int64
	stepEndTimes     *[]int64
	currentStepIndex *int
}

func NewCliStep(logger *Logger) CLIStep {
	steps := []string{}
	stepStartTimes := []int64{}
	stepEndTimes := []int64{}
	currentStepIndex := 0

	return CLIStep{
		logger:           logger,
		startTime:        time.Now().Unix(),
		steps:            &steps,
		stepStartTimes:   &stepStartTimes,
		stepEndTimes:     &stepEndTimes,
		currentStepIndex: &currentStepIndex,
	}
}

func (step *CLIStep) StartStep(name string) {
	newSteps := append(*step.steps, name)
	stepStartTimes := append(*step.stepStartTimes, time.Now().Unix())

	step.steps = &newSteps
	step.stepStartTimes = &stepStartTimes
	step.logStartStep(name)
}

func (step *CLIStep) EndStep() {
	stepEndTime := time.Now().Unix()
	currentStepIndex := *step.currentStepIndex
	stepName := (*step.steps)[currentStepIndex]
	stepStartTime := (*step.stepStartTimes)[currentStepIndex]

	stepEndTimes := append(*step.stepEndTimes, stepEndTime)
	step.stepEndTimes = &stepEndTimes

	step.logEndStep(stepName, stepEndTime, stepStartTime)

	nextStepIndex := (*step.currentStepIndex) + 1
	step.currentStepIndex = &nextStepIndex
}

func (step *CLIStep) Summary() {
	totalTime := time.Now().Unix() - step.startTime

	var formattedNames = []string{}
	var formattedTimes = []string{}
	var maxNameLength = 0
	var maxTimeLength = 0

	totalStep := (*step.currentStepIndex) - 1
	for i := 0; i < totalStep; i++ {
		currentStepName := (*step.steps)[i]
		currentStepTime := (*step.stepStartTimes)[i]
		currentStepEndTime := (*step.stepEndTimes)[i]

		stepSeconds := currentStepEndTime - currentStepTime
		formattedName := fmt.Sprintf(" %s ", currentStepName)
		formattedTime := fmt.Sprintf(" %d (s) ", stepSeconds)

		if len(formattedName) > maxNameLength {
			maxNameLength = len(formattedName)
		}

		if len(formattedTime) > maxTimeLength {
			maxTimeLength = len(formattedTime)
		}

		formattedNames = append(formattedNames, formattedName)
		formattedTimes = append(formattedTimes, formattedTime)
	}

	var stepNameHeader = " Name"
	stepNameHeader = step.appendSpace(&stepNameHeader, maxNameLength)

	var stepTimeHeader = " Time"
	stepTimeHeader = step.appendSpace(&stepTimeHeader, maxTimeLength)

	tableHeader := fmt.Sprintf("%s+%s", step.separatorString(maxNameLength), step.separatorString(maxTimeLength))
	header := step.separatorString(len(tableHeader))

	var headerTitle = " Summary"
	headerTitle = step.appendSpace(&headerTitle, len(header)-2)

	step.logger.LogMessagef("\n+%s+\n|%s %s %s|", header, LogGreen, headerTitle, LogReset)
	step.logger.LogMessagef("\n+%s+", tableHeader)
	step.logger.LogMessagef("\n|%s|%s|", stepNameHeader, stepTimeHeader)
	step.logger.LogMessagef("\n+%s+", tableHeader)

	for i := 0; i < totalStep; i++ {
		var currentFormattedName = formattedNames[i]
		currentFormattedName = step.appendSpace(&currentFormattedName, maxNameLength)

		var currentFormattedTime = formattedTimes[i]
		currentFormattedTime = step.appendSpace(&currentFormattedTime, maxTimeLength)

		step.logger.LogMessagef("\n|%s|%s|", currentFormattedName, currentFormattedTime)
		step.logger.LogMessagef("\n+%s+", tableHeader)
	}

	var stepNameFooter = " Total"
	stepNameFooter = step.appendSpace(&stepNameFooter, maxNameLength)

	var stepTimeFooter = fmt.Sprintf(" %d (s)", totalTime)
	stepTimeFooter = step.appendSpace(&stepTimeFooter, maxTimeLength)

	step.logger.LogMessagef("\n|%s%s%s|%s|", LogMagenta, stepNameFooter, LogReset, stepTimeFooter)
	step.logger.LogMessagef("\n+%s+\n", tableHeader)
}

func (step *CLIStep) logStartStep(name string) {
	formattedStepName := fmt.Sprintf("--- %s ---", name)
	separatorString := step.separatorString(len(formattedStepName))
	step.logger.LogMessage("\nStep\n")
	step.logger.LogSuccess(separatorString)
	step.logger.LogSuccess(formattedStepName)
	step.logger.LogSuccess(separatorString)
}

func (step *CLIStep) logEndStep(name string, stepEndTime int64, stepStartTime int64) {
	stepSeconds := stepEndTime - stepStartTime
	formattedName := fmt.Sprintf(" %s ", name)
	formattedTime := fmt.Sprintf(" %d (s) ", stepSeconds)

	var stepNameHeader = " Name"
	stepNameHeader = step.appendSpace(&stepNameHeader, len(formattedName))

	var stepTimeHeader = " Time"
	stepTimeHeader = step.appendSpace(&stepTimeHeader, len(formattedTime))

	tableHeader := fmt.Sprintf("%s+%s", step.separatorString(len(formattedName)), step.separatorString(len(formattedTime)))
	header := step.separatorString(len(tableHeader))

	var headerTitle = " Step Summary"
	headerTitle = step.appendSpace(&headerTitle, len(header)-2)

	step.logger.LogMessagef("\n+%s+\n|%s %s %s|", header, LogGreen, headerTitle, LogReset)
	step.logger.LogMessagef("\n+%s+", tableHeader)
	step.logger.LogMessagef("\n|%s|%s|", stepNameHeader, stepTimeHeader)
	step.logger.LogMessagef("\n+%s+", tableHeader)
	step.logger.LogMessagef("\n|%s|%s|", formattedName, formattedTime)
	step.logger.LogMessagef("\n+%s+\n", tableHeader)
}

func (step *CLIStep) separatorString(len int) string {
	var separatorString = ""
	for i := 0; i < len; i++ {
		separatorString += "-"
	}

	return separatorString
}

func (step *CLIStep) appendSpace(input *string, length int) string {
	stepNameAppendCount := length - len(*input)
	var newStepName = *input

	for i := 0; i < stepNameAppendCount; i++ {
		newStepName += " "
	}

	return newStepName
}
