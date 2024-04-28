package core

import (
	"fmt"
	"os/exec"
)

type Executor struct {
	command     *exec.Cmd
	commandName *string
}

func NewExecutor(name string, args ...string) Executor {
	var commandString = name

	for argIndex := range args {
		commandString += fmt.Sprintf(" %v", args[argIndex])
	}

	LogInfo("Starting command:\n")
	LogSuccess(commandString)
	LogInfoSeparator(len(commandString))

	return Executor{
		command:     exec.Command(name, args...),
		commandName: &name,
	}
}

func (shellExecuter Executor) WorkingDirectory(dir string) {
	shellExecuter.command.Dir = dir
}

func (shellExecuter Executor) Run() {
	stdout, err := shellExecuter.command.StdoutPipe()
	if err != nil {
		LogErrorf("%v", err)
	}

	stderr, err := shellExecuter.command.StderrPipe()
	if err != nil {
		LogErrorf("%v", err)
	}

	if err = shellExecuter.command.Start(); err != nil {
		LogErrorf("%v", err)
	}

	for {
		stdOutBuffer := make([]byte, 1024)
		stdOutSize, stdOutErr := stdout.Read(stdOutBuffer)

		if stdOutSize > 0 {
			stdOutString := string(stdOutBuffer)
			LogMessage(stdOutString)
		}

		stdErrBuffer := make([]byte, 1024)
		stdErrSize, stdErrErr := stderr.Read(stdErrBuffer)

		if stdErrSize > 0 {
			stdErrString := string(stdErrBuffer)
			LogError(stdErrString)
		}

		if stdOutErr != nil && stdErrErr != nil {
			break
		}
	}

	LogInfof("Command %s", *shellExecuter.commandName)
	LogSuccess("Success")
}
