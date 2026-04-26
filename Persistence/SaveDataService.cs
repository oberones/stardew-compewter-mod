namespace ComPewter.Persistence;

public sealed class SaveDataService
{
    public void Load()
    {
        // v1 intentionally stores no chat history, provider secrets, or custom save state.
    }

    public void Save()
    {
        // Keeping v1 stateless makes add/remove behavior safe for existing saves.
    }
}
