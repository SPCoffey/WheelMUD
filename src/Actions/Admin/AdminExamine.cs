//-----------------------------------------------------------------------------
// <copyright file="Examine.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   A script that allows a player to look at something.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Actions
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using WheelMUD.Core;
    using WheelMUD.Core.Attributes;
    using WheelMUD.Interfaces;

    /// <summary>A command that allows a player to look at something.</summary>
    [ExportGameAction]
    [ActionPrimaryAlias("adminexamine", CommandCategory.Inform)]
    [ActionAlias("adex", CommandCategory.Inform)]
    [ActionAlias("admexa", CommandCategory.Inform)]
    [ActionAlias("admininfo", CommandCategory.Inform)]
    [ActionDescription("Examine something VERY closely.")]
    [ActionSecurity(SecurityRole.fullAdmin)]
    public class AdminExamine : GameAction
    {
        /// <summary>List of reusable guards which must be passed before action requests may proceed to execution.</summary>
        private static readonly List<CommonGuards> ActionGuards = new List<CommonGuards>
        {
            CommonGuards.InitiatorMustBeAlive, 
            CommonGuards.InitiatorMustBeConscious,
            CommonGuards.RequiresAtLeastOneArgument
        };

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            IController sender = actionInput.Controller;
            Thing parent = sender.Thing.Parent;
            string searchString = actionInput.Tail.Trim().ToLower();

            if (string.IsNullOrEmpty(searchString))
            {
                sender.Write("You must specify something to search for.");
                return;
            }

            // Unique case. Use 'here' to list the contents of the room.
            if (searchString == "here")
            {
                var sb = new StringBuilder();

                sb.Append("ID: " + parent.Id);
                sb.Append(Environment.NewLine);

                foreach (Behavior behavior in parent.Behaviors.AllBehaviors)
                {
                    sb.Append("Behavior: " + behavior.GetType().Name);
                    sb.Append(Environment.NewLine);
                }

                foreach (Thing child in parent.Children)
                {
                    sb.Append("Child: " + child.Name);
                    sb.Append(Environment.NewLine);
                }

                sender.Write(sb.ToString().Trim());

                return;
            }

            Thing thing = FindTarget(sender.Thing, searchString);
            if (thing != null)
            {
                // @@@ TODO: Send a SensoryEvent?
                var sb = new StringBuilder();

                sb.Append("ID: " + thing.Id);
                sb.Append(Environment.NewLine);

                foreach (Behavior behavior in thing.Behaviors.AllBehaviors)
                {
                    sb.Append("Behavior: " + behavior.GetType().Name);
                    sb.Append(Environment.NewLine);
                }

                foreach (Thing child in thing.Children)
                {
                    sb.Append("Child: " + child.Name);
                    sb.Append(Environment.NewLine);
                }

                sender.Write(sb.ToString().Trim());
            }
            else
            {
                sender.Write("You cannot find " + searchString + ".");
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

        /// <summary>Builds a string listing the items that are in the specified room.</summary>
        /// <param name="room">The room whose items we care about.</param>
        /// <returns>A string listing the items that are in the specified room.</returns>
        private string ListRoomItems(Thing room)
        {
            StringBuilder sb = new StringBuilder();

            foreach (Thing thing in room.Children)
            {
                // @@@ TODO: Only list Items here...? ItemBehavior? CarryableBehavior?
                sb.Append(thing.Id.ToString().PadRight(20));
                sb.AppendLine(thing.FullName);
            }
            
            return sb.ToString();
        }
    }
}