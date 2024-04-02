require 'fastlane/action'
require_relative '../helper/react_helper'

module Fastlane
  module Actions
    class ReactAction < Action
      def self.run(params)
        Helper::ReactHelper.start(
          sources_directory: params[:sources_directory],
          environment: params[:environment],
          print_output: params[:print_output],
          output_directory: params[:output_directory]
        )
      end

      def self.description
        "React.js fastlane plugin"
      end

      def self.authors
        ["Ilker OZCAN"]
      end

      def self.return_value
        # If your method provides a return value, you can describe here what it does
      end

      def self.details
        # Optional:
        "React.js fastlane plugin"
      end

      def self.available_options
        [
          FastlaneCore::ConfigItem.new(key: :sources_directory,
                                  env_name: "REACT_SOURCE",
                                description: "Source directory",
                                   optional: false,
                                       type: String),
          FastlaneCore::ConfigItem.new(key: :environment,
                                  env_name: "REACT_ENVIRONMENT",
                                description: "Environment",
                                   optional: false,
                                       type: String),
          FastlaneCore::ConfigItem.new(key: :print_output,
                                  env_name: "REACT_PRINT_OUTPUT",
                                description: "Print STDOUT",
                                  optional: true,
                                      type: Boolean),
          FastlaneCore::ConfigItem.new(key: :output_directory,
                                  env_name: "REACT_Output",
                                description: "Output directory",
                                  optional: true,
                                      type: String)
        ]
      end

      def self.is_supported?(platform)
        # Adjust this if your plugin only works for a particular platform (iOS vs. Android, for example)
        # See: https://docs.fastlane.tools/advanced/#control-configuration-by-lane-and-by-platform
        #
        # [:ios, :mac, :android].include?(platform)
        true
      end
    end
  end
end
