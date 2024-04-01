require 'fileutils'
require 'fastlane_core/ui/ui'

module Fastlane
  UI = FastlaneCore::UI unless Fastlane.const_defined?(:UI)

  module Helper
    class DotnetHelper
      def self.start(solution_directory:, environment:, output_directory:)
        self.prepare_output(output_directory: output_directory)
        self.restore(solution_directory: solution_directory)
        self.clean(solution_directory: solution_directory)
        self.publish(
          solution_directory: solution_directory, 
          environment: environment,
          output_directory: output_directory
        )
        self.create_www(output_directory: output_directory)
      end
      def self.prepare_output(output_directory:)
        UI.message("Preparing output directory")
        if Dir.exist?(output_directory)
          FileUtils.rm_rf(output_directory)
        end
        UI.success("Output directory is ready")
      end
      def self.restore(solution_directory:)
        UI.message("Restore solution")
        Dir.chdir(solution_directory) do
          command = "dotnet restore"
          UI.message("Execute command:\n#{command}")
          FastlaneCore::CommandExecutor.execute(command: command,
                                              print_all: true,
                                          print_command: true,
                                                  error: proc do |output|
                                                    ErrorHandler.handle_build_error(output)
                                                  end)
          UI.success("Solution was restored")
        end
      end
      def self.clean(solution_directory:)
        UI.message("Clean solution")
        Dir.chdir(solution_directory) do
          command = "dotnet clean"
          UI.message("Execute command:\n#{command}")
          FastlaneCore::CommandExecutor.execute(command: command,
                                              print_all: true,
                                          print_command: true,
                                                  error: proc do |output|
                                                    ErrorHandler.handle_build_error(output)
                                                  end)
          UI.success("Solution was cleaned")
        end
      end
      def self.publish(solution_directory:, environment:, output_directory:)
        UI.message("Publish application")
        Dir.chdir(solution_directory) do
          command = "dotnet publish --configuration #{environment} --output #{output_directory}"
          UI.message("Execute command:\n#{command}")
          FastlaneCore::CommandExecutor.execute(command: command,
                                              print_all: true,
                                          print_command: true,
                                                  error: proc do |output|
                                                    ErrorHandler.handle_build_error(output)
                                                  end)
          UI.success("Application was published")
        end
      end
      def self.create_www(output_directory:)
        UI.message("Creating wwwroot directory if exists")
        wwwroot_directory = "#{output_directory}/wwwroot"
        Dir.mkdir(wwwroot_directory) unless Dir.exist?(wwwroot_directory)
        UI.success("wwwroot directory is ready")
      end
    end
  end
end
