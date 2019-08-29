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
    [ActionPrimaryAlias("admindescription", CommandCategory.Admin)]
    [ActionAlias("adesc", CommandCategory.Admin)]
    [ActionDescription("Edit a description")]
    [ActionSecurity(SecurityRole.fullAdmin)]
    public class AdminDescription : GameAction
    {
        /// <summary>List of reusable guards which must be passed before action requests may proceed to execution.</summary>
        private static readonly List<CommonGuards> ActionGuards = new List<CommonGuards>
        {
            CommonGuards.RequiresAtLeastTwoArguments
        };

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            IController sender = actionInput.Controller;
            Thing parent = sender.Thing.Parent;

            string targetString = actionInput.Params[0].Trim().ToLower();
            string newDesciption = actionInput.Tail.Substring(actionInput.Params[0].Length).Trim();

            if (string.IsNullOrEmpty(targetString))
            {
                sender.Write("You must specify something to change the description of.");
                return;
            }

            // Unique case. Use 'here' to list the contents of the room.
            if (targetString == "here" || targetString == "room")
            {
                parent.Description = newDesciption;
                return;
            }

            Thing thing = FindTarget(sender.Thing, targetString);
            if (thing != null)
            {
                thing.Description = newDesciption;
            }
            else
            {
                sender.Write("There is no " + targetString + " here.");
            }
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
