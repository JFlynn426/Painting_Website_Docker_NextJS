namespace ServerApp.Shared.Domain
{
    public abstract class AggregateRoot<T>
    {
        public T Id { get; protected set; }
        public int Version { get; protected set; }

        private readonly List<IDomainEvent> _events = new();
        public IEnumerable<IDomainEvent> Events => _events;

        protected void AddEvent(IDomainEvent @event)
        {
            if (!_events.Any() && !_versionIncremented)
            {
                Version++;
                _versionIncremented = true;
            }
            _events.Add(@event);
        }

        public void ClearEvents()
        {
            _events.Clear();
        }

        private bool _versionIncremented;

        public void IncrementVersion()
        {
            if (_versionIncremented)
            {
                return;
            }
            _versionIncremented = true;
            Version++;
        }
    }
}