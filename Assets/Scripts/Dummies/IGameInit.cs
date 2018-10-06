public interface IGameInit
{

    void AddGameStartedListener(EmptyEventHandler handler);
    event EmptyEventHandler AllObjectsCreated;
}
