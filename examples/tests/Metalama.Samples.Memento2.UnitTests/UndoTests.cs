using Xunit;

namespace Metalama.Samples.Memento2.UnitTests;

public class UndoTests
{
    private sealed class StubFishGenerator : IFishGenerator
    {
        private int _counter;

        public string GetNewName() => $"Fish{++this._counter}";

        public string GetNewSpecies() => "Species";
    }

    [Fact]
    public void Undo_AfterRemove_RestoresCurrentFishFromMemento()
    {
        // Arrange: add two fish, select the second one.
        var caretaker = new Caretaker();
        var viewModel = new MainViewModel( new StubFishGenerator(), caretaker );

        viewModel.NewCommand.Execute();
        viewModel.NewCommand.Execute();

        Assert.Equal( 2, viewModel.Fishes.Count );
        var originalFirst = viewModel.Fishes[0];
        var originalSecond = viewModel.Fishes[1];
        viewModel.CurrentFish = originalSecond;

        // Act: remove the selected fish, then undo the removal.
        viewModel.RemoveCommand.Execute();
        Assert.Single( viewModel.Fishes );
        Assert.Same( originalFirst, viewModel.CurrentFish );

        viewModel.UndoCommand.Execute();

        // Assert: the memento restores both the list and the selection
        // that were current at the time the memento was captured.
        Assert.Equal( 2, viewModel.Fishes.Count );
        Assert.Same( originalFirst, viewModel.Fishes[0] );
        Assert.Same( originalSecond, viewModel.Fishes[1] );
        Assert.Same( originalSecond, viewModel.CurrentFish );
    }

    [Fact]
    public void Undo_AfterNew_RestoresNullSelectionFromMemento()
    {
        // Arrange: add two fish without selecting any, then select the second.
        var caretaker = new Caretaker();
        var viewModel = new MainViewModel( new StubFishGenerator(), caretaker );

        viewModel.NewCommand.Execute();
        viewModel.NewCommand.Execute();

        Assert.Equal( 2, viewModel.Fishes.Count );
        var originalFirst = viewModel.Fishes[0];
        var originalSecond = viewModel.Fishes[1];

        // User then navigates to the second fish before undoing.
        viewModel.CurrentFish = originalSecond;

        // Act: undo the second New.
        viewModel.UndoCommand.Execute();

        // Assert: the list is restored and CurrentFish is null, matching the
        // memento captured when the second New ran (no fish was selected yet).
        Assert.Single( viewModel.Fishes );
        Assert.Same( originalFirst, viewModel.Fishes[0] );
        Assert.Null( viewModel.CurrentFish );
    }
}
