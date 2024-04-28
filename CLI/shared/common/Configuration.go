package common

type Configuration struct {
	verbose bool
}

func (config Configuration) Verbose() bool {
	return config.verbose
}
