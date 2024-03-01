namespace Game.Shared.Interfaces {
    public interface IEntityId {
        int Id { get; }
        void DestroyComponent();
    }
}