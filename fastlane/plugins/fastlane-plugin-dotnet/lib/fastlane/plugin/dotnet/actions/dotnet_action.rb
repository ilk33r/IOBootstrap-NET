require 'fastlane/action'
require_relative '../helper/dotnet_helper'

module Fastlane
  module Actions
    class DotnetAction < Action
      def self.run(params)
        Helper::DotnetHelper.start(
          solution_directory: params[:solution_directory],
          environment: params[:environment],
          output_directory: params[:output_directory]
        )
      end

      def self.description
        "Net.Core fastlane plugin"
      end

      def self.authors
        ["Ilker OZCAN"]
      end

      def self.return_value
        # If your method provides a return value, you can describe here what it does
      end

      def self.details
        # Optional:
        "Net.Core fastlane plugin"
      end

      def self.available_options
        [
          FastlaneCore::ConfigItem.new(key: :solution_directory,
                                  env_name: "DOTNET_SOLUTION",
                                description: "Solution directory",
                                   optional: false,
                                       type: String),
          FastlaneCore::ConfigItem.new(key: :environment,
                                  env_name: "DOTNET_ENVIRONMENT",
                                description: "Build environment",
                                   optional: false,
                                       type: String),
            FastlaneCore::ConfigItem.new(key: :output_directory,
                                       env_name: "DOTNET_OUTPUT",
                                     description: "Output directory",
                                        optional: false,
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
