package main

import "iobootstrap-cli-shared/core"

var Logger = core.InitializeLogger()
var CLIStep = core.NewCliStep(&Logger)
var ExecutorInitializer = core.NewExecutorInitializer(&Logger)
