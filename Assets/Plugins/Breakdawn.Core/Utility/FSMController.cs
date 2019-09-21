using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Breakdawn.Core
{
    public class FSMController
    {
        private readonly Dictionary<string, FSMState> _states = new Dictionary<string, FSMState>();

        public FSMState CurrentState { get; private set; }

        public void AddState([NotNull] FSMState state)
        {
            if (state == null)
            {
                throw new ArgumentNullException(nameof(state));
            }

            if (_states.ContainsKey(state.Id))
            {
                throw new ArgumentException($"已经添加过该状态:{state.Id}");
            }

            _states.Add(state.Id, state);
        }

        public bool RemoveState(string id)
        {
            if (!_states.TryGetValue(id, out var state))
            {
                return false;
            }

            if (CurrentState == state)
            {
                throw new InvalidOperationException($"当前状态为{CurrentState.Id},不可删除当前的状态");
            }

            return _states.Remove(id);
        }

        public void SwitchState(string id)
        {
            if (!_states.TryGetValue(id, out var state))
            {
                throw new ArgumentException($"没有该状态:{id}");
            }

            CurrentState?.OnLeave();
            state.OnEnter();
            CurrentState = state;
        }

        public void OnUpdate()
        {
            CurrentState.Act();
            CurrentState.Reason();
        }
    }
}