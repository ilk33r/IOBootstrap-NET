package loader

type CLIStep interface {
	StartStep(name string)
	EndStep()
	Summary()
}
