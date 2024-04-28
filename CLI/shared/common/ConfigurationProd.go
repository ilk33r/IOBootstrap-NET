//go:build !COMPILE_DEV

package common

func GetConfiguration() Configuration {
	return Configuration{
		false,
	}
}
