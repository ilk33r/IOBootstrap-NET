describe Fastlane::Actions::ReactAction do
  describe '#run' do
    it 'prints a message' do
      expect(Fastlane::UI).to receive(:message).with("The react plugin is working!")

      Fastlane::Actions::ReactAction.run(nil)
    end
  end
end
