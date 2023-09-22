using NebulaSiege.Engine;
using NebulaSiege.Engine.Debug;
using NebulaSiege.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NebulaSiege.Managers
{
    /// <summary>
    /// Handles keyboard debugging inquiries.
    /// </summary>
    internal class EngineDebugManager
    {
        private static string[] commandPrototypes = {
            "Sprite|List|typeFilter:Optional:Criterion",
            "Sprite|Visible|uid:Required:Numeric,visible:Required:Boolean",
            "|Cls|",
            "|Help|",
            "Sprite|Move|uid:Required:Numeric,x:Required:Numeric,y:Required:Numeric",
            "Sprite|Center|uid:Required:Numeric",
            "Sprite|Explode|uid:Required:Numeric",
            "Sprite|Info|uid:Required:Numeric"
        };

        private readonly EngineCore _core;
        private readonly Stack<string> _commandStack = new();
        private readonly FormDebug formDebug;
        public DebugCommandParser CommandParser { get; } = new(commandPrototypes);
        private List<MethodInfo> _hardDebugMethods;

        public bool IsVisible { get; private set; } = false;

        public EngineDebugManager(EngineCore core)
        {
            _core = core;
            formDebug = new FormDebug(_core);
            _hardDebugMethods = GetMethodsWithOnlyDebugCommandParameter();
        }

        public void EnqueueCommand(string command) => _commandStack.Push(command);

        public void ToggleVisibility()
        {
            IsVisible = !IsVisible;

            if (IsVisible)
            {
                formDebug.Show();
            }
            else
            {
                formDebug.Hide();
            }
        }

        public void ProcessCommand()
        {
            if (_commandStack.TryPop(out var command))
            {
                try
                {
                    var parsedCommand = CommandParser.Parse(command);

                    var methodToExecute = _hardDebugMethods
                        .Where(o => o.Name.ToLower() == parsedCommand.PhysicalFunctionKey).FirstOrDefault();

                    if (methodToExecute == null)
                    {
                        throw new Exception($"Physical function '{parsedCommand.PhysicalFunctionKey}' is not implemented.");
                    }

                    methodToExecute?.Invoke(this, new object[] { parsedCommand });
                }
                catch (Exception ex)
                {
                    formDebug.WriteLine(ex.Message);
                }
            }
        }

        public List<MethodInfo> GetMethodsWithOnlyDebugCommandParameter()
        {
            var methods = new List<MethodInfo>();
            var allMethods = typeof(EngineDebugManager)
                .GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var method in allMethods)
            {
                var parameters = method.GetParameters();
                if (parameters.Length == 1 && parameters[0].ParameterType == typeof(DebugCommand))
                {
                    methods.Add(method);
                }
            }

            return methods;
        }

        #region Physical debug command handlers.

        public void DebugHandler_Cls(DebugCommand command)
        {
            formDebug.ClearText();
        }

        public void DebugHandler_Help(DebugCommand command)
        {
            foreach (var cmd in CommandParser.Commands.OrderBy(o => o.Name))
            {
                string text = $"> {cmd.Name}\r\n";

                foreach (var cmdParam in cmd.Parameters)
                {
                    text += $"\t{cmdParam.Name}, {cmdParam.CommandParameterType}" + (cmdParam.IsRequired ? "" : " (optional)") + "\r\n";
                }
                formDebug.Write(text);
            }
        }

        public void DebugHandler_Sprite_Info(DebugCommand command)
        {
            var uid = command.ParameterValue<uint>("uid");
            var sprite = _core.Sprites.Collection.Where(o => o.UID == uid).FirstOrDefault();
            if (sprite != null)
            {
                /*
                    $"Frame Rate: Avg: {_core.Display.GameLoopCounter.AverageFrameRate.ToString("0.0")}, "
                    + $"Min: {_core.Display.GameLoopCounter.FrameRateMin.ToString("0.0")}, "
                    + $"Max: {_core.Display.GameLoopCounter.FrameRateMax.ToString("0.0")}\r\n"
                    + $"Quadrant: {_core.Display.CurrentQuadrant.Key.X}:{_core.Display.CurrentQuadrant.Key.Y}\r\n"
                    //+ $"  Delta BG Offset: {displacementVector.X:#0.00}x, {displacementVector.Y:#0.00}y\r\n"
                 */

                var infoText =
                      $">  Sprite UID: {sprite.UID}y\r\n"
                    + $"   Display XY: {sprite.X:#0.00}x, {sprite.Y:#0.00}y\r\n"
                    + $" Player Angle: {sprite.Velocity.Angle.X:#0.00}x, {sprite.Velocity.Angle.Y:#0.00}y, "
                                        + $"{sprite.Velocity.Angle.Degrees:#0.00}deg, "
                                        + $" {sprite.Velocity.Angle.Radians:#0.00}rad, "
                                        + $" {sprite.Velocity.Angle.RadiansUnadjusted:#0.00}rad unadjusted\r\n"
                    + $"   Virtual XY: {sprite.X + _core.Display.BackgroundOffset.X:#0.00}x,"
                                        + $" {sprite.Y + _core.Display.BackgroundOffset.Y:#0.00}y\r\n"
                    + $"    BG Offset: {_core.Display.BackgroundOffset.X:#0.00}x, {_core.Display.BackgroundOffset.Y:#0.00}y\r\n"
                    + $"       Thrust: {(sprite.Velocity.ThrottlePercentage * 100):#0.00}\r\n"
                    + $"        Boost: {(sprite.Velocity.BoostPercentage * 100):#0.00}\r\n"
                    + $"       Recoil: {(sprite.Velocity.RecoilPercentage * 100):#0.00}\r\n";

                formDebug.WriteLine(infoText);
            }
        }

        public void DebugHandler_Sprite_Explode(DebugCommand command)
        {
            var uid = command.ParameterValue<uint>("uid");
            var sprite = _core.Sprites.Collection.Where(o => o.UID == uid).FirstOrDefault();
            if (sprite != null)
            {
                sprite.Explode();
            }
        }

        public void DebugHandler_Sprite_Visible(DebugCommand command)
        {
            var uid = command.ParameterValue<uint>("uid");
            var sprite = _core.Sprites.Collection.Where(o => o.UID == uid).FirstOrDefault();
            if (sprite != null)
            {
                sprite.Visable = command.ParameterValue<bool>("visible");
            }
        }

        public void DebugHandler_Sprite_Move(DebugCommand command)
        {
            var uid = command.ParameterValue<uint>("uid");
            var sprite = _core.Sprites.Collection.Where(o => o.UID == uid).FirstOrDefault();
            if (sprite != null)
            {
                sprite.X = command.ParameterValue<double>("x");
                sprite.Y = command.ParameterValue<double>("y");
            }
        }

        public void DebugHandler_Sprite_Center(DebugCommand command)
        {
            var uid = command.ParameterValue<uint>("uid");
            var sprite = _core.Sprites.Collection.Where(o => o.UID == uid).FirstOrDefault();
            if (sprite != null)
            {
                sprite.X = _core.Display.TotalCanvasSize.Width / 2;
                sprite.Y = _core.Display.TotalCanvasSize.Height / 2;
            }
        }

        public void DebugHandler_Sprite_List(DebugCommand command)
        {
            var sprites = _core.Sprites.Collection.ToList();

            var typeFilter = command.ParameterValue<DebugCommandParameterCriterion>("typeFilter");
            if (typeFilter != null)
            {
                sprites = sprites.Where(o => DebugCommandParameter.IsMatchLike(o.GetType().Name, typeFilter.Value, typeFilter.IsNotCriteria)).ToList();
            }

            foreach (var sprite in sprites)
            {
                formDebug.WriteLine($"Type: {sprite.GetType().Name}, UID: {sprite.UID}");
            }
        }

        #endregion
    }
}
