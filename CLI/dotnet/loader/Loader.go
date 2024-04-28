package loader

import (
	"fmt"
	"os"
	"plugin"
)

func LoadPlugin(path string) *plugin.Plugin {
	sharedPlugin, err := plugin.Open(path)
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error while opening shared object file %v", err)
		os.Exit(1)
	}

	return sharedPlugin
}

func InitializeCLIStep(plugin *plugin.Plugin) CLIStep {
	cliStep, err := plugin.Lookup("CLIStep")
	if err != nil {
		fmt.Fprintf(os.Stderr, "%v", err)
		os.Exit(1)
	}

	var cliStepInstance CLIStep
	cliStepInstance, ok := cliStep.(CLIStep)
	if !ok {
		fmt.Fprint(os.Stderr, "Unexpected type from module symbol")
		os.Exit(1)
	}

	return cliStepInstance
}
