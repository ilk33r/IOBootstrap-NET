# Makefile for CLI

BUILD := release
VERSION := 1.0.0
SOURCE_ROOT_DIR=$(shell pwd)
BUILD_ROOT_DIR=$(SOURCE_ROOT_DIR)/bin

GOLANG = go
MODULE_NAME = iobootstrap-cli-build

GCFLAG.release =
GCFLAG.debug = '-gcflags=all=-N -l'
GCFLAG := $(GCFLAG.$(BUILD))

LD_FLAGS := -ldflags="-X 'main.Version=$(VERSION)'"

TAGS.release = -tags COMPILE_PROD
TAGS.debug = -tags COMPILE_DEV
TAGS := $(TAGS.$(BUILD))

OS.darwin = darwin
OS.linux = linux
OS.windows = windows

ARCH.amd64 = amd64
ARCH.arm = arm64
ARCH.i386 = 386

all: $(MODULE_NAME)

$(MODULE_NAME)-clean:
	@rm -rf $(BUILD_ROOT_DIR)
	@echo "_____________________________"
	@echo "|      ! Clean success      |"
	@echo "_____________________________"

clean: $(MODULE_NAME)-clean

$(MODULE_NAME): prepare $(MODULE_NAME)-$(OS.darwin)-$(ARCH.amd64) $(MODULE_NAME)-$(OS.darwin)-$(ARCH.arm) $(MODULE_NAME)-$(OS.linux)-$(ARCH.amd64) $(MODULE_NAME)-$(OS.linux)-$(ARCH.i386) $(MODULE_NAME)-$(OS.windows)-$(ARCH.amd64) $(MODULE_NAME)-$(OS.windows)-$(ARCH.i386)

$(MODULE_NAME)-$(OS.darwin)-$(ARCH.amd64):
	env GOOS=$(OS.darwin) GOARCH=$(ARCH.amd64) $(GOLANG) build $(GCFLAG) $(TAGS) $(LD_FLAGS) -o $(BUILD_ROOT_DIR)/$(OS.darwin)/$(ARCH.amd64)/$(MODULE_NAME)
	@echo "__________________________________________"
	@echo "|      ! Build darwin amd64 success      |"
	@echo "__________________________________________"

$(MODULE_NAME)-$(OS.darwin)-$(ARCH.arm):
	env GOOS=$(OS.darwin) GOARCH=$(ARCH.arm) $(GOLANG) build $(GCFLAG) $(TAGS) $(LD_FLAGS) -o $(BUILD_ROOT_DIR)/$(OS.darwin)/$(ARCH.arm)/$(MODULE_NAME)
	@echo "__________________________________________"
	@echo "|      ! Build darwin arm success        |"
	@echo "__________________________________________"

$(MODULE_NAME)-$(OS.linux)-$(ARCH.amd64):
	env GOOS=$(OS.linux) GOARCH=$(ARCH.amd64) $(GOLANG) build $(GCFLAG) $(TAGS) $(LD_FLAGS) -o $(BUILD_ROOT_DIR)/$(OS.linux)/$(ARCH.amd64)/$(MODULE_NAME)
	@echo "__________________________________________"
	@echo "|      ! Build linux amd64 success       |"
	@echo "__________________________________________"

$(MODULE_NAME)-$(OS.linux)-$(ARCH.i386):
	env GOOS=$(OS.linux) GOARCH=$(ARCH.i386) $(GOLANG) build $(GCFLAG) $(TAGS) $(LD_FLAGS) -o $(BUILD_ROOT_DIR)/$(OS.linux)/$(ARCH.i386)/$(MODULE_NAME)
	@echo "__________________________________________"
	@echo "|      ! Build linux i386 success        |"
	@echo "__________________________________________"

$(MODULE_NAME)-$(OS.windows)-$(ARCH.amd64):
	env GOOS=$(OS.windows) GOARCH=$(ARCH.amd64) $(GOLANG) build $(GCFLAG) $(TAGS) $(LD_FLAGS) -o $(BUILD_ROOT_DIR)/$(OS.windows)/$(ARCH.amd64)/$(MODULE_NAME).exe
	@echo "__________________________________________"
	@echo "|      ! Build windows amd64 success     |"
	@echo "__________________________________________"

$(MODULE_NAME)-$(OS.windows)-$(ARCH.i386):
	env GOOS=$(OS.windows) GOARCH=$(ARCH.i386) $(GOLANG) build $(GCFLAG) $(TAGS) $(LD_FLAGS) -o $(BUILD_ROOT_DIR)/$(OS.windows)/$(ARCH.i386)/$(MODULE_NAME).exe
	@echo "__________________________________________"
	@echo "|      ! Build windows i386 success      |"
	@echo "__________________________________________"

prepare:
	@mkdir -p $(BUILD_ROOT_DIR)
	@mkdir -p $(BUILD_ROOT_DIR)/$(OS.darwin)/$(ARCH.amd64)
	@mkdir -p $(BUILD_ROOT_DIR)/$(OS.darwin)/$(ARCH.arm)
	@mkdir -p $(BUILD_ROOT_DIR)/$(OS.linux)/$(ARCH.amd64)
	@mkdir -p $(BUILD_ROOT_DIR)/$(OS.linux)/$(ARCH.i386)
	@mkdir -p $(BUILD_ROOT_DIR)/$(OS.windows)/$(ARCH.amd64)
	@mkdir -p $(BUILD_ROOT_DIR)/$(OS.windows)/$(ARCH.i386)
	@echo "Output directory OK"

.PHONY: all clean
