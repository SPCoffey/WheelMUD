using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WheelMUD.Actions.Admin
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using WheelMUD.Core;
    using WheelMUD.Core.Attributes;
    using WheelMUD.Interfaces;

    /// <summary>An action to ban a player or IP address range for a time.</summary>
    [ExportGameAction]
    [ActionPrimaryAlias("dig", CommandCategory.Admin)]
    [ActionDescription("Create a room")]
    [ActionSecurity(SecurityRole.fullAdmin)]
    public class Dig : GameAction
    {
        /// <summary>List of reusable guards which must be passed before action requests may proceed to execution.</summary>
        private static readonly List<CommonGuards> ActionGuards = new List<CommonGuards>
        {
            CommonGuards.RequiresAtLeastOneArgument
        };

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            IController sender = actionInput.Controller;

            string direction = actionInput.Tail.Trim();

            var newRoom = new Thing();
            newRoom.Id = "1";
            newRoom.Name = "New Room";
            newRoom.Behaviors.Add(new RoomBehavior());

            var thing = new Thing();
            thing.Id = "0";
            thing.Name = direction;
            thing.Parent = sender.Thing.Parent;
            thing.Behaviors.Add(new ExitBehavior());
            thing.Behaviors.FindFirst<ExitBehavior>().AddDestination(direction, newRoom.Id);

            sender.Thing.Parent.Add(thing);
        }

        /// <summary>Checks against the guards for the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        /// <returns>A string with the error message for the user upon guard failure, else null.</returns>
        public override string Guards(ActionInput actionInput)
        {
            string commonFailure = VerifyCommonGuards(actionInput, ActionGuards);
            if (commonFailure != null)
            {
                return commonFailure;
            }

            return null;
        }
    }
}
