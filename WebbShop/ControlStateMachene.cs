using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebbShop
{
    public class ControlStateMachine
    {
        public enum ControlState
        {
            Menu,
            Page,
            Window,
            Editor
        }
        public enum ControlTrigger
        {
            MenuToPage,
            PageToWindow,
            WindowToPage,
            PageToMenu,
            WindowToEditor,
            EditorToWindow
        }
        private Dictionary<(ControlState, ControlTrigger), ControlState> StateTransitions;
        public ControlState CurrentState {  get; private set; }
        public ControlStateMachine()
        {
            CurrentState = ControlState.Menu;
            StateTransitions = new Dictionary<(ControlState, ControlTrigger), ControlState>
            {
                {(ControlState.Menu,ControlTrigger.MenuToPage), ControlState.Page },
                {(ControlState.Page,ControlTrigger.PageToWindow), ControlState.Window },
                {(ControlState.Window,ControlTrigger.WindowToEditor), ControlState.Editor },
                {(ControlState.Editor,ControlTrigger.EditorToWindow), ControlState.Window },
                {(ControlState.Window,ControlTrigger.WindowToPage), ControlState.Page },
                {(ControlState.Page,ControlTrigger.PageToMenu), ControlState.Menu }
                
            };
        }
        public void ControlTransition(ControlTrigger trigger) 
        {

            if (StateTransitions.TryGetValue((CurrentState, trigger), out ControlState newState))
            {
                CurrentState = newState;
            }
            else
            {
                Console.WriteLine($"invalid transition: {CurrentState} cannot handle {trigger}");
            }
        }
        
        
    }
}
