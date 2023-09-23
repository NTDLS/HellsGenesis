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
        private static string[] _commandPrototypes = {
            "Sprite-List|typeFilter:Optional:Criterion",
            "Sprite-Player-Inspect|typeFilter:Optional:Criterion",
            "Sprite-Visible|uid:Required:Numeric,state:Required:Boolean",
            "Sprite-Highlight|uid:Required:Numeric,state:Required:Boolean",
            "Cls|",
            "Help|",
            "Display-Metrics|",
            "Display-Framerate-Get|",
            "Display-Framerate-Set|rate:Required:Numeric",
            "Sprite-Move|uid:Required:Numeric,x:Required:Numeric,y:Required:Numeric",
            "Sprite-Move-Center|uid:Required:Numeric",
            "Sprite-Explode|uid:Required:Numeric",
            "Sprite-Inspect|uid:Required:Numeric",
            "Sprite-MaxSpeed|uid:Required:Numeric,value:Required:Numeric",
            "Sprite-MaxBoost|uid:Required:Numeric,value:Required:Numeric",
            "Sprite-Throttle|uid:Required:Numeric,value:Required:Numeric",
            "Sprite-Boost|uid:Required:Numeric,value:Required:Numeric",
            "Sprite-AngleDegrees|uid:Required:Numeric,value:Required:Numeric",
        };

        private readonly EngineCore _core;
        private readonly Stack<string> _commandStack = new();
        private readonly FormDebug formDebug;
        public DebugCommandParser CommandParser { get; } = new(_commandPrototypes);
        private readonly List<MethodInfo> _hardDebugMethods;

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
                        .Where(o => o.Name.ToLower() == parsedCommand.PhysicalFunctionKey).FirstOrDefault() ?? throw new Exception($"Physical function '{parsedCommand.PhysicalFunctionKey}' is not implemented.");

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

        public void DebugHandler_Display_Metrics(DebugCommand command)
        {
            var infoText =
                  $"          BackgroundOffset: X:{_core.Display.BackgroundOffset.X:n2}, Y:{_core.Display.BackgroundOffset.X:n2}\r\n"
                + $"             BaseDrawScale: {_core.Display.BaseDrawScale:n4}\r\n"
                + $"              OverdrawSize: W:{_core.Display.OverdrawSize.Width:n0}, H{_core.Display.OverdrawSize.Height:n0}\r\n"
                + $"         NatrualScreenSize: W:{_core.Display.NatrualScreenSize.Width:n0}, H{_core.Display.NatrualScreenSize.Height:n0}\r\n"
                + $"           TotalCanvasSize: W:{_core.Display.TotalCanvasSize.Width:n0}, H{_core.Display.TotalCanvasSize.Height:n0}\r\n"
                + $"         TotalCanvasBounds: X:{_core.Display.TotalCanvasBounds.X:n0}, Y:{_core.Display.TotalCanvasBounds.Y:n0} / W:{_core.Display.TotalCanvasBounds.Width:n0}, H:{_core.Display.TotalCanvasBounds.Height:n0}\r\n"
                + $"       NatrualScreenBounds: X:{_core.Display.NatrualScreenBounds.X:n0}, Y:{_core.Display.NatrualScreenBounds.Y:n0} / W:{_core.Display.NatrualScreenBounds.Width:n0}, H:{_core.Display.NatrualScreenBounds.Height:n0}\r\n"
                + $"   SpeedFrameScalingFactor: {_core.Display.SpeedOrientedFrameScalingFactor:n4}\r\n"
                + $"           CurrentQuadrant: {_core.Display.CurrentQuadrant.Key}";
            formDebug.WriteLine(infoText);
        }

        public void DebugHandler_Display_Framerate_Set(DebugCommand command)
        {
            var rate = command.ParameterValue<double>("rate");
            _core.Settings.FrameLimiter = rate;
        }

        public void DebugHandler_Display_Framerate_Get(DebugCommand command)
        {
            var infoText =
                  $"Limit: {_core.Settings.FrameLimiter:n4}\r\n"
                + $"  Avg: {_core.Display.GameLoopCounter.AverageFrameRate:n4}\r\n"
                + $"  Min: {_core.Display.GameLoopCounter.FrameRateMin:n4}\r\n"
                + $"  Max: {_core.Display.GameLoopCounter.FrameRateMax:n4}";
            formDebug.WriteLine(infoText);
        }

        public void DebugHandler_Sprite_Player_Inspect(DebugCommand command)
        {
            /*
                $"Frame Rate: Avg: {_core.Display.GameLoopCounter.AverageFrameRate:n2}, "
                + $"Min: {_core.Display.GameLoopCounter.FrameRateMin:n2}, "
                + $"Max: {_core.Display.GameLoopCounter.FrameRateMax:n2}\r\n"
                + $"Quadrant: {_core.Display.CurrentQuadrant.Key.X}:{_core.Display.CurrentQuadrant.Key.Y}\r\n"
                //+ $"  Delta BG Offset: {displacementVector.X:#0.00}x, {displacementVector.Y:#0.00}y\r\n"
             */

            var infoText =
                  $">  Sprite UID: {_core.Player.Sprite.UID}y\r\n"
                + $"   Display XY: {_core.Player.Sprite.X:#0.00}x, {_core.Player.Sprite.Y:#0.00}y\r\n"
                + $"        Angle: {_core.Player.Sprite.Velocity.Angle.X:#0.00}x, {_core.Player.Sprite.Velocity.Angle.Y:#0.00}y, "
                                    + $"{_core.Player.Sprite.Velocity.Angle.Degrees:#0.00}deg, "
                                    + $" {_core.Player.Sprite.Velocity.Angle.Radians:#0.00}rad, "
                                    + $" {_core.Player.Sprite.Velocity.Angle.RadiansUnadjusted:#0.00}rad unadjusted\r\n"
                + $"   Virtual XY: {_core.Player.Sprite.X + _core.Display.BackgroundOffset.X:#0.00}x,"
                                    + $" {_core.Player.Sprite.Y + _core.Display.BackgroundOffset.Y:#0.00}y\r\n"
                + $"    BG Offset: {_core.Display.BackgroundOffset.X:#0.00}x, {_core.Display.BackgroundOffset.Y:#0.00}y\r\n"
                + $"       Thrust: {(_core.Player.Sprite.Velocity.ThrottlePercentage * 100):#0.00}\r\n"
                + $"        Boost: {(_core.Player.Sprite.Velocity.BoostPercentage * 100):#0.00}\r\n"
                + $"       Recoil: {(_core.Player.Sprite.Velocity.RecoilPercentage * 100):#0.00}\r\n";

            formDebug.WriteLine(infoText);
        }

        public void DebugHandler_Sprite_Inspect(DebugCommand command)
        {
            var uid = command.ParameterValue<uint>("uid");
            var sprite = _core.Sprites.Collection.Where(o => o.UID == uid).FirstOrDefault();
            if (sprite != null)
            {
                var infoText =
                      $">  Sprite UID: {sprite.UID}y\r\n"
                    + $"   Display XY: {sprite.X:#0.00}x, {sprite.Y:#0.00}y\r\n"
                    + $"        Angle: {sprite.Velocity.Angle.X:#0.00}x, {sprite.Velocity.Angle.Y:#0.00}y, "
                                        + $"{sprite.Velocity.Angle.Degrees:#0.00}deg, "
                                        + $" {sprite.Velocity.Angle.Radians:#0.00}rad, "
                                        + $" {sprite.Velocity.Angle.RadiansUnadjusted:#0.00}rad unadjusted\r\n"
                    + $"   Virtual XY: {sprite.X + _core.Display.BackgroundOffset.X:#0.00}x,"
                                        + $" {sprite.Y + _core.Display.BackgroundOffset.Y:#0.00}y\r\n"
                    + $"    BG Offset: {_core.Display.BackgroundOffset.X:#0.00}x, {_core.Display.BackgroundOffset.Y:#0.00}y\r\n"
                    + $"       Thrust: {(sprite.Velocity.ThrottlePercentage * 100):#0.00}\r\n"
                    + $"        Boost: {(sprite.Velocity.BoostPercentage * 100):#0.00}\r\n"
                    + $"       Recoil: {(sprite.Velocity.RecoilPercentage * 100):#0.00}\r\n";

                //TODO: Add these items here and to DebugHandler_Sprite_Player_Inspect()
                //sprite.HullHealth
                //sprite.ShieldHealth
                //sprite.RotationMode
                //sprite.Attachments(...)
                //sprite.Bounds
                //sprite.Highlight
                //sprite.IsDead
                //sprite.IsFixedPosition
                //sprite.IsLockedOn
                //sprite.IsLockedOnSoft
                //sprite.IsWithinCurrentScaledScreenBounds
                //sprite.OwnerUID
                //sprite.ReadyForDeletion
                //sprite.Size
                //sprite.SpriteTag
                //sprite.Visable
                //sprite.VisibleBounds

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

        public void DebugHandler_Sprite_AngleDegrees(DebugCommand command)
        {
            var uid = command.ParameterValue<uint>("uid");
            var sprite = _core.Sprites.Collection.Where(o => o.UID == uid).FirstOrDefault();
            if (sprite != null)
            {
                sprite.Velocity.Angle.Degrees = command.ParameterValue<double>("value");
            }
        }

        public void DebugHandler_Sprite_Boost(DebugCommand command)
        {
            var uid = command.ParameterValue<uint>("uid");
            var sprite = _core.Sprites.Collection.Where(o => o.UID == uid).FirstOrDefault();
            if (sprite != null)
            {
                sprite.Velocity.BoostPercentage = command.ParameterValue<double>("value");
            }
        }

        public void DebugHandler_Sprite_Throttle(DebugCommand command)
        {
            var uid = command.ParameterValue<uint>("uid");
            var sprite = _core.Sprites.Collection.Where(o => o.UID == uid).FirstOrDefault();
            if (sprite != null)
            {
                sprite.Velocity.ThrottlePercentage = command.ParameterValue<double>("value");
            }
        }

        public void DebugHandler_Sprite_MaxBoost(DebugCommand command)
        {
            var uid = command.ParameterValue<uint>("uid");
            var sprite = _core.Sprites.Collection.Where(o => o.UID == uid).FirstOrDefault();
            if (sprite != null)
            {
                sprite.Velocity.MaxBoost = command.ParameterValue<double>("value");
            }
        }

        public void DebugHandler_Sprite_MaxSpeed(DebugCommand command)
        {
            var uid = command.ParameterValue<uint>("uid");
            var sprite = _core.Sprites.Collection.Where(o => o.UID == uid).FirstOrDefault();
            if (sprite != null)
            {
                sprite.Velocity.MaxSpeed = command.ParameterValue<double>("value");
            }
        }

        public void DebugHandler_Sprite_Highlight(DebugCommand command)
        {
            var uid = command.ParameterValue<uint>("uid");
            var sprite = _core.Sprites.Collection.Where(o => o.UID == uid).FirstOrDefault();
            if (sprite != null)
            {
                sprite.Highlight = command.ParameterValue<bool>("state");
            }
        }

        public void DebugHandler_Sprite_Visible(DebugCommand command)
        {
            var uid = command.ParameterValue<uint>("uid");
            var sprite = _core.Sprites.Collection.Where(o => o.UID == uid).FirstOrDefault();
            if (sprite != null)
            {
                sprite.Visable = command.ParameterValue<bool>("state");
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

        public void DebugHandler_Sprite_Move_Center(DebugCommand command)
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
