using Si.GameEngine.Engine;
using Si.GameEngine.Engine.Debug;
using Si.GameEngine.Forms;
using Si.Shared.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Si.GameEngine.Managers
{
    /// <summary>
    /// Handles keyboard debugging inquiries.
    /// </summary>
    public class EngineDebugManager
    {
        private static string[] _commandPrototypes = {
            "Cls||Clears the debug screen.",
            "Help|command:Optional:Criterion|Displays help for all or a given command.",

            "Display-Zoom-Reset||Resets the global zoom level to auto.",
            "Display-Zoom-Override|level:Required:Numeric|Sets the global zoom level.",
            "Display-Zoom-Get||Gets the current global zoom level.",
            "Display-Metrics||Displays various display metrics.",
            "Display-Framerate-Set|rate:Required:Numeric|Sets the target framerate.",
            "Display-Framerate-Get||Gets the currently configured target framerate.",
            "Display-Adapters||List available video adapters.",

            "Display-BackgroundOffset-Get||Gets the current background offset.",
            "Display-BackgroundOffset-Set|x:Required:Numeric,y:Required:Numeric|Sets the current background offset.",
            "Display-BackgroundOffset-CenterOn|spriteUID:Required:Numeric|Centers the background offset on a given sprite.",

            "Engine-HighlightAll|state:Required:Boolean|Highlights all visible sprites.",
            "Engine-Pause|state:Required:Boolean|Pauses and unpauses the engine.",

            "Sprite-Enemies-DeleteAll||Deletes all enemy sprites.",
            "Sprite-Enemies-ExplodeAll||Explodes all enemy sprites.",

            "Sprite-Player-Explode||Explodes the player sprite",
            "Sprite-Player-Inspect||Returns various metrics about the player sprite",

            "Sprite-AngleInDegrees|uid:Required:Numeric,value:Required:Numeric|Gets the angle (in degrees) from one sprite to another.",
            "Sprite-AngleTo|baseSpriteUID:Required:Numeric,targetSpriteUID:Required:Numeric|Gets the angle (in degrees) that a sprite is pointing.",
            "Sprite-Boost|uid:Required:Numeric,value:Required:Numeric|Gets the current boost percentage for a sprite.",
            "Sprite-DistanceTo|baseSpriteUID:Required:Numeric,targetSpriteUID:Required:Numeric|Returns the distance from one sprite to another.",

            "Sprite-Explode|uid:Required:Numeric|Causes a sprite to explode.",
            "Sprite-Highlight|uid:Required:Numeric,state:Required:Boolean|Highlights a given sprite.",
            "Sprite-Inspect|uid:Required:Numeric|Returns various metrics about a given sprite.",
            "Sprite-IsPointingAt|baseSpriteUID:Required:Numeric,targetSpriteUID:Required:Numeric,toleranceDegrees:Optional=10:Numeric,maxDistance:Optional=10000:Numeric|Determines if one sprite is pointing at another.",
            "Sprite-IsPointingAway|baseSpriteUID:Required:Numeric,targetSpriteUID:Required:Numeric,toleranceDegrees:Optional=10:Numeric,maxDistance:Optional=10000:Numeric|Determines if one sprite is pointing aways from another",
            "Sprite-List|typeFilter:Optional:Criterion|Lists all sprites given an optional filter. Filter is a LIKE using !% and _.",
            "Sprite-MaxBoost|uid:Required:Numeric,value:Required:Numeric|Displays a sprites configured max boost speed.",
            "Sprite-MaxSpeed|uid:Required:Numeric,value:Required:Numeric|Displays a sprites configured native boost speed.",
            "Sprite-Move|uid:Required:Numeric,x:Required:Numeric,y:Required:Numeric|Sets a new position for a given sprite.",
            "Sprite-Move-Center|uid:Required:Numeric|Moves a given sprite to the center of the screen.",
            "Sprite-Throttle|uid:Required:Numeric,value:Required:Numeric|Gets the current throttle percentage for a sprite.",
            "Sprite-Visible|uid:Required:Numeric,state:Required:Boolean|Displays whether a given sprite is visible or not.",
        };

        private readonly EngineCore _gameCore;
        private readonly Stack<string> _commandStack = new();
        private readonly FormDebug formDebug;
        public DebugCommandParser CommandParser { get; } = new(_commandPrototypes);
        private readonly List<MethodInfo> _hardDebugMethods;

        public bool IsVisible { get; private set; } = false;

        public EngineDebugManager(EngineCore gameCore)
        {
            _gameCore = gameCore;
            formDebug = new FormDebug(_gameCore);
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
                    formDebug.WriteLine($"Executing ↓ \"{command}\":", System.Drawing.Color.Blue);

                    var parsedCommand = CommandParser.Parse(command);

                    var methodToExecute = _hardDebugMethods
                        .Where(o => o.Name.ToLower() == parsedCommand.PhysicalFunctionKey).FirstOrDefault()
                        ?? throw new Exception($"Physical function '{parsedCommand.PhysicalFunctionKey}' is not implemented.");

                    methodToExecute?.Invoke(this, new object[] { parsedCommand });
                }
                catch (Exception ex)
                {
                    formDebug.WriteLine(ex.Message, System.Drawing.Color.DarkRed);
                }
                formDebug.WriteLine($"Complete  ↑", System.Drawing.Color.Blue);
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

        public void CommandHandler_Display_BackgroundOffset_Get(DebugCommand command)
        {
            formDebug.WriteLine($"{_gameCore.Display.BackgroundOffset}", System.Drawing.Color.Black);
        }

        public void CommandHandler_Display_BackgroundOffset_Set(DebugCommand command)
        {
            var x = command.ParameterValue<double>("x");
            var y = command.ParameterValue<double>("y");

            var deltaX = _gameCore.Display.BackgroundOffset.X - x;
            var deltaY = _gameCore.Display.BackgroundOffset.Y - y;

            foreach (var sprite in _gameCore.Sprites.Collection)
            {
                if (sprite.IsFixedPosition == false)
                {
                    sprite.X += deltaX;
                    sprite.Y += deltaY;
                }
            }

            _gameCore.Display.BackgroundOffset.X = x;
            _gameCore.Display.BackgroundOffset.Y = y;
        }

        public void CommandHandler_Display_BackgroundOffset_CenterOn(DebugCommand command)
        {
            var spriteUID = command.ParameterValue<double>("spriteUID");
            var baseSprite = _gameCore.Sprites.Collection.Where(o => o.UID == spriteUID).FirstOrDefault();
            if (baseSprite != null)
            {
                var deltaX = (_gameCore.Display.TotalCanvasSize.Width / 2) - baseSprite.X;
                var deltaY = (_gameCore.Display.TotalCanvasSize.Height / 2) - baseSprite.Y;

                foreach (var sprite in _gameCore.Sprites.Collection)
                {
                    if (sprite.IsFixedPosition == false)
                    {
                        sprite.X += deltaX;
                        sprite.Y += deltaY;
                    }
                }

                _gameCore.Display.BackgroundOffset.X = baseSprite.X;
                _gameCore.Display.BackgroundOffset.Y = baseSprite.Y;
            }
        }

        public void CommandHandler_Display_Adapters(DebugCommand command)
        {
            var text = _gameCore.Rendering.GetGraphicsAdaptersInfo();
            formDebug.Write(text, System.Drawing.Color.Black);
        }

        public void CommandHandler_Sprite_Enemies_DeleteAll(DebugCommand command)
        {
            foreach (var sprite in _gameCore.Sprites.Enemies.All())
            {
                sprite.QueueForDelete();
            }
        }

        public void CommandHandler_Sprite_Enemies_ExplodeAll(DebugCommand command)
        {
            foreach (var sprite in _gameCore.Sprites.Enemies.All())
            {
                sprite.Explode();
            }
        }

        public void CommandHandler_Sprite_Player_Explode(DebugCommand command)
        {
            _gameCore.Player.Sprite.Explode();
        }

        public void CommandHandler_Cls(DebugCommand command)
        {
            formDebug.ClearText();
        }

        public void CommandHandler_Help(DebugCommand command)
        {
            var commands = CommandParser.Commands.OrderBy(o => o.Name).ToList();

            var typeFilter = command.ParameterValue<DebugCommandParameterCriterion>("command");
            if (typeFilter != null)
            {
                commands = commands.Where(o => DebugCommandParameter.IsMatchLike(o.Name, typeFilter.Value, typeFilter.IsNotCriteria)).ToList();
            }

            foreach (var cmd in commands)
            {
                string text = $" > {cmd.Name} - {cmd.Description}\r\n";

                if (cmd.Parameters.Count > 0)
                {
                    text += "    Parameters: { \r\n";
                }

                foreach (var cmdParam in cmd.Parameters)
                {
                    string optionalText = "";

                    if (cmdParam.IsRequired == false)
                    {
                        optionalText = " (optional";
                        if (cmdParam.DefaultValue == null)
                        {
                            optionalText += "=null";
                        }
                        else optionalText += $"={cmdParam.DefaultValue}";
                        optionalText += ")";
                    }

                    text += $"        {cmdParam.Name}, {cmdParam.CommandParameterType}{optionalText}\r\n";
                }

                if (cmd.Parameters.Count > 0)
                {
                    text += "    }\r\n";
                }

                formDebug.Write(text, System.Drawing.Color.Black);
            }
        }

        public void CommandHandler_Display_Metrics(DebugCommand command)
        {
            var infoText =
                  $"          Background Offset: {_gameCore.Display.BackgroundOffset}\r\n"
                + $"            Base Draw Scale: {_gameCore.Display.BaseDrawScale:n4}\r\n"
                + $"              Overdraw Size: {_gameCore.Display.OverdrawSize}\r\n"
                + $"        Natrual Screen Size: {_gameCore.Display.NatrualScreenSize}\r\n"
                + $"          Total Canvas Size: {_gameCore.Display.TotalCanvasSize}\r\n"
                + $"        Total Canvas Bounds: {_gameCore.Display.TotalCanvasBounds}\r\n"
                + $"      Natrual Screen Bounds: {_gameCore.Display.NatrualScreenBounds}\r\n"
                + $" Speed Frame Scaling Factor: {_gameCore.Display.SpeedOrientedFrameScalingFactor():n4}";
            formDebug.WriteLine(infoText, System.Drawing.Color.Black);
        }

        public void CommandHandler_Engine_HighlightAll(DebugCommand command)
        {
            var state = command.ParameterValue<bool>("state");
            _gameCore.Settings.HighlightAllSprites = state;
        }

        public void CommandHandler_Display_Zoom_Reset(DebugCommand command)
        {
            _gameCore.Display.OverrideSpeedOrientedFrameScalingFactor = double.NaN;
        }

        public void CommandHandler_Display_Zoom_Override(DebugCommand command)
        {
            var level = command.ParameterValue<double>("level");
            _gameCore.Display.OverrideSpeedOrientedFrameScalingFactor = level.Box(-1, 1);
        }

        public void CommandHandler_Display_Zoom_Get(DebugCommand command)
        {
            formDebug.WriteLine($"{_gameCore.Display.SpeedOrientedFrameScalingFactor():n4}", System.Drawing.Color.Black);
        }

        public void CommandHandler_Engine_Pause(DebugCommand command)
        {
            var state = command.ParameterValue<bool>("state");

            if (state == true && _gameCore.IsPaused() == false)
            {
                _gameCore.Pause();
            }
            else if (state == false && _gameCore.IsPaused() == true)
            {
                _gameCore.Resume();
            }
        }

        public void CommandHandler_Display_Framerate_Set(DebugCommand command)
        {
            var rate = command.ParameterValue<double>("rate");
            _gameCore.Settings.FrameLimiter = rate;
        }

        public void CommandHandler_Display_Framerate_Get(DebugCommand command)
        {
            var infoText =
                  $"Limit: {_gameCore.Settings.FrameLimiter:n4}\r\n"
                + $"  Avg: {_gameCore.Display.GameLoopCounter.AverageFrameRate:n4}\r\n"
                + $"  Min: {_gameCore.Display.GameLoopCounter.FrameRateMin:n4}\r\n"
                + $"  Max: {_gameCore.Display.GameLoopCounter.FrameRateMax:n4}";
            formDebug.WriteLine(infoText, System.Drawing.Color.Black);
        }

        public void CommandHandler_Sprite_Player_Inspect(DebugCommand command)
        {
            formDebug.WriteLine(_gameCore.Player.Sprite.GetInspectionText(), System.Drawing.Color.Black);
        }

        public void CommandHandler_Sprite_Inspect(DebugCommand command)
        {
            var uid = command.ParameterValue<uint>("uid");
            var sprite = _gameCore.Sprites.Collection.Where(o => o.UID == uid).FirstOrDefault();
            if (sprite != null)
            {
                formDebug.WriteLine(sprite.GetInspectionText(), System.Drawing.Color.Black);
            }
        }

        public void CommandHandler_Sprite_Explode(DebugCommand command)
        {
            var uid = command.ParameterValue<uint>("uid");
            var sprite = _gameCore.Sprites.Collection.Where(o => o.UID == uid).FirstOrDefault();
            if (sprite != null)
            {
                sprite.Explode();
            }
        }

        public void CommandHandler_Sprite_IsPointingAt(DebugCommand command)
        {
            var baseSpriteUID = command.ParameterValue<uint>("baseSpriteUID");
            var targetSpriteUID = command.ParameterValue<uint>("targetSpriteUID");
            var toleranceDegrees = command.ParameterValue<double>("toleranceDegrees");
            var maxDistance = command.ParameterValue<double>("maxDistance");

            var baseSprite = _gameCore.Sprites.Collection.Where(o => o.UID == baseSpriteUID).FirstOrDefault();
            var targetSprite = _gameCore.Sprites.Collection.Where(o => o.UID == targetSpriteUID).FirstOrDefault();

            if (baseSprite != null && targetSprite != null)
            {
                var result = baseSprite.IsPointingAt(targetSprite, toleranceDegrees, maxDistance);
                formDebug.WriteLine($"IsPointingAt: {result}", System.Drawing.Color.Black);
            }
        }

        public void CommandHandler_Sprite_IsPointingAway(DebugCommand command)
        {
            var baseSpriteUID = command.ParameterValue<uint>("baseSpriteUID");
            var targetSpriteUID = command.ParameterValue<uint>("targetSpriteUID");
            var toleranceDegrees = command.ParameterValue<double>("toleranceDegrees", 10);
            var maxDistance = command.ParameterValue<double>("maxDistance", 1000);

            var baseSprite = _gameCore.Sprites.Collection.Where(o => o.UID == baseSpriteUID).FirstOrDefault();
            var targetSprite = _gameCore.Sprites.Collection.Where(o => o.UID == targetSpriteUID).FirstOrDefault();

            if (baseSprite != null && targetSprite != null)
            {
                var result = baseSprite.IsPointingAway(targetSprite, toleranceDegrees, maxDistance);
                formDebug.WriteLine($"IsPointingAt: {result}", System.Drawing.Color.Black);
            }
        }


        public void CommandHandler_Sprite_DistanceTo(DebugCommand command)
        {
            var baseSpriteUID = command.ParameterValue<uint>("baseSpriteUID");
            var targetSpriteUID = command.ParameterValue<uint>("targetSpriteUID");

            var baseSprite = _gameCore.Sprites.Collection.Where(o => o.UID == baseSpriteUID).FirstOrDefault();
            var targetSprite = _gameCore.Sprites.Collection.Where(o => o.UID == targetSpriteUID).FirstOrDefault();

            if (baseSprite != null && targetSprite != null)
            {
                var result = baseSprite.DistanceTo(targetSprite);
                formDebug.WriteLine($"DistanceTo: {result:n4}", System.Drawing.Color.Black);
            }
        }

        public void CommandHandler_Sprite_AngleTo(DebugCommand command)
        {
            var baseSpriteUID = command.ParameterValue<uint>("baseSpriteUID");
            var targetSpriteUID = command.ParameterValue<uint>("targetSpriteUID");

            var baseSprite = _gameCore.Sprites.Collection.Where(o => o.UID == baseSpriteUID).FirstOrDefault();
            var targetSprite = _gameCore.Sprites.Collection.Where(o => o.UID == targetSpriteUID).FirstOrDefault();

            if (baseSprite != null && targetSprite != null)
            {
                var result = baseSprite.AngleTo360(targetSprite);
                formDebug.WriteLine($"AngleTo: {result:n4}", System.Drawing.Color.Black);
            }
        }

        public void CommandHandler_Sprite_AngleInDegrees(DebugCommand command)
        {
            var uid = command.ParameterValue<uint>("uid");
            var sprite = _gameCore.Sprites.Collection.Where(o => o.UID == uid).FirstOrDefault();
            if (sprite != null)
            {
                sprite.Velocity.Angle.Degrees = command.ParameterValue<double>("value");
            }
        }

        public void CommandHandler_Sprite_Boost(DebugCommand command)
        {
            var uid = command.ParameterValue<uint>("uid");
            var sprite = _gameCore.Sprites.Collection.Where(o => o.UID == uid).FirstOrDefault();
            if (sprite != null)
            {
                sprite.Velocity.BoostPercentage = command.ParameterValue<double>("value");
            }
        }

        public void CommandHandler_Sprite_Throttle(DebugCommand command)
        {
            var uid = command.ParameterValue<uint>("uid");
            var sprite = _gameCore.Sprites.Collection.Where(o => o.UID == uid).FirstOrDefault();
            if (sprite != null)
            {
                sprite.Velocity.ThrottlePercentage = command.ParameterValue<double>("value");
            }
        }

        public void CommandHandler_Sprite_MaxBoost(DebugCommand command)
        {
            var uid = command.ParameterValue<uint>("uid");
            var sprite = _gameCore.Sprites.Collection.Where(o => o.UID == uid).FirstOrDefault();
            if (sprite != null)
            {
                sprite.Velocity.MaxBoost = command.ParameterValue<double>("value");
            }
        }

        public void CommandHandler_Sprite_MaxSpeed(DebugCommand command)
        {
            var uid = command.ParameterValue<uint>("uid");
            var sprite = _gameCore.Sprites.Collection.Where(o => o.UID == uid).FirstOrDefault();
            if (sprite != null)
            {
                sprite.Velocity.MaxSpeed = command.ParameterValue<double>("value");
            }
        }

        public void CommandHandler_Sprite_Highlight(DebugCommand command)
        {
            var uid = command.ParameterValue<uint>("uid");
            var sprite = _gameCore.Sprites.Collection.Where(o => o.UID == uid).FirstOrDefault();
            if (sprite != null)
            {
                sprite.Highlight = command.ParameterValue<bool>("state");
            }
        }

        public void CommandHandler_Sprite_Visible(DebugCommand command)
        {
            var uid = command.ParameterValue<uint>("uid");
            var sprite = _gameCore.Sprites.Collection.Where(o => o.UID == uid).FirstOrDefault();
            if (sprite != null)
            {
                sprite.Visable = command.ParameterValue<bool>("state");
            }
        }

        public void CommandHandler_Sprite_Move(DebugCommand command)
        {
            var uid = command.ParameterValue<uint>("uid");
            var sprite = _gameCore.Sprites.Collection.Where(o => o.UID == uid).FirstOrDefault();
            if (sprite != null)
            {
                sprite.X = command.ParameterValue<double>("x");
                sprite.Y = command.ParameterValue<double>("y");
            }
        }

        public void CommandHandler_Sprite_Move_Center(DebugCommand command)
        {
            var uid = command.ParameterValue<uint>("uid");
            var sprite = _gameCore.Sprites.Collection.Where(o => o.UID == uid).FirstOrDefault();
            if (sprite != null)
            {
                sprite.X = _gameCore.Display.TotalCanvasSize.Width / 2;
                sprite.Y = _gameCore.Display.TotalCanvasSize.Height / 2;
            }
        }

        public void CommandHandler_Sprite_List(DebugCommand command)
        {
            var sprites = _gameCore.Sprites.Collection.ToList();

            var typeFilter = command.ParameterValue<DebugCommandParameterCriterion>("typeFilter");
            if (typeFilter != null)
            {
                sprites = sprites.Where(o => DebugCommandParameter.IsMatchLike(o.GetType().Name, typeFilter.Value, typeFilter.IsNotCriteria)).ToList();
            }

            foreach (var sprite in sprites)
            {
                formDebug.WriteLine($"Type: {sprite.GetType().Name}, UID: {sprite.UID}, Position: {sprite.Location}", System.Drawing.Color.Black);
            }
        }

        #endregion
    }
}
