require 'fastlane_core/ui/ui'

module Fastlane
  UI = FastlaneCore::UI unless Fastlane.const_defined?(:UI)

  module Helper
    class ReactHelper
      def self.start(sources_directory:, environment:, print_output:, output_directory:)
        self.install_dependencies(sources_directory: sources_directory)
        self.clean(sources_directory: sources_directory)
        
        print_all = print_output.nil? ? true : print_output
        self.build(
          sources_directory: sources_directory,
          environment: environment,
          print_output: print_all
        )

        unless output_directory.nil?
          self.prepare_output(output_directory: output_directory)
          self.copy_output(
            sources_directory: sources_directory,
            output_directory: output_directory
          )
        end
      end
      def self.install_dependencies(sources_directory:)
        UI.message("Install dependencies")
        Dir.chdir(sources_directory) do
          command = "npm install"
          UI.message("Execute command:\n#{command}")
          FastlaneCore::CommandExecutor.execute(command: command,
                                              print_all: true,
                                          print_command: true,
                                                  error: proc do |output|
                                                    ErrorHandler.handle_build_error(output)
                                                  end)
          UI.success("Dependencies were installed")
        end
      end
      def self.build(sources_directory:, environment:, print_output:)
        UI.message("Build application")
        Dir.chdir(sources_directory) do
          command = "npm run build:#{environment}"
          UI.message("Execute command:\n#{command}")
          FastlaneCore::CommandExecutor.execute(command: command,
                                              print_all: print_output,
                                          print_command: true,
                                                  error: proc do |output|
                                                    ErrorHandler.handle_build_error(output)
                                                  end)
          UI.success("Application was builded")
        end
      end
      def self.clean(sources_directory:)
        UI.message("Cleaning build")
        build_directory = "#{sources_directory}/build"
        if Dir.exist?(build_directory)
          FileUtils.rm_rf(build_directory)
        end

        UI.success("Build is cleaned")
      end
      def self.prepare_output(output_directory:)
        UI.message("Preparing output directory")
        if Dir.exist?(output_directory)
          FileUtils.rm_rf(output_directory)
        end

        Dir.mkdir(output_directory) unless Dir.exist?(output_directory)
        UI.success("Output directory is ready")
      end
      def self.copy_output(sources_directory:, output_directory:)
        UI.message("Copy output")
        Dir.chdir(sources_directory) do
          build_directory = "#{sources_directory}/build"
          FileUtils.copy_entry(build_directory, output_directory)
          UI.success("Output was copied.")
        end
      end
    end
  end
end
