describe Fastlane::Actions::DotnetAction do
  describe '#run' do
    it 'prints a message' do
      expect(Fastlane::UI).to receive(:message).with("The dotnet plugin is working!")

      Fastlane::Actions::DotnetAction.run(nil)
    end
  end
end
