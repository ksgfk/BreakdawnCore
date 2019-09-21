namespace Breakdawn.Core
{
    public abstract class FSMState
    {
        protected readonly FSMController controller;
        
        public string Id { get; }

        protected FSMState(FSMController controller, string id)
        {
            this.controller = controller;
            Id = id;
        }

        public abstract void OnLeave();

        public abstract void OnEnter();

        public abstract void Act();

        public abstract void Reason();
    }
}