using Si.GameEngine.Core.Debug;
using Si.GameEngine.Core.Debug._Superclass;
using Si.GameEngine.Core.NativeRendering;
using Si.GameEngine.Sprites._Superclass;
using Si.Library;
using Si.Library.ExtensionMethods;
using Si.Library.Types.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Si.GameEngine.Core.Managers
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
            "Display-Framerate||Gets the currently configured target framerate.",
            "Display-Adapters||List available video adapters.",

            "Display-RenderWindowPosition-Get||Gets the current background offset.",
            "Display-RenderWindowPosition-Set|x:Required:Numeric,y:Required:Numeric|Sets the current background offset.",
            "Display-RenderWindowPosition-CenterOn|spriteUID:Required:Numeric|Centers the background offset on a given sprite.",

            "Engine-HighlightAll|state:Required:Boolean|Highlights all visible sprites.",
            "Engine-Pause|state:Required:Boolean|Pauses and unpauses the engine.",

            "Sprite-Create|typeName:Required:String,x:Required:Numeric,y:Required:Numeric|Creates a sprite at the given position.",
            "Sprite-ListTypes||Lists all sprite types.",

            "Sprite-Watch|uid:Required:Numeric|Starts a watch form to monitor various sprite metrics.",

            "Sprite-Enemies-DeleteAll||Deletes all enemy sprites.",
            "Sprite-Enemies-ExplodeAll||Explodes all enemy sprites.",

            "Sprite-Player-Reflect-List||Displays all available properties of the player sprite.",
            "Sprite-Player-Reflect-Set|property:Required:String,value:Optional:String|Sets the given property for the player sprite.",

            "Sprite-Sprite-Reflect-List|uid:Required:Numeric|Displays all available properties of the given sprite.",
            "Sprite-Sprite-Reflect-Set|uid:Required:Numeric,property:Required:String,value:Optional:String|Sets the given property for the given sprite.",

            "Sprite-Player-Explode||Explodes the player sprite",
            "Sprite-Player-Inspect||Returns various metrics about the player sprite",

            "Sprite-AngleInDegrees|uid:Required:Numeric,value:Required:Numeric|Gets the angle (in degrees) from one sprite to another.",
            "Sprite-AngleTo|baseSpriteUID:Required:Numeric,targetSpriteUID:Required:Numeric|Gets the angle (in degrees) that a sprite is pointing.",
            "Sprite-BoostThrottle|uid:Required:Numeric,value:Required:Numeric|Gets the current boost percentage for a sprite.",
            "Sprite-DistanceTo|baseSpriteUID:Required:Numeric,targetSpriteUID:Required:Numeric|Returns the distance from one sprite to another.",

            "Sprite-Explode|uid:Required:Numeric|Causes a sprite to explode.",
            "Sprite-Highlight|uid:Required:Numeric,state:Required:Boolean|Highlights a given sprite.",
            "Sprite-Inspect|uid:Required:Numeric|Returns various metrics about a given sprite.",
            "Sprite-IsPointingAt|baseSpriteUID:Required:Numeric,targetSpriteUID:Required:Numeric,toleranceDegrees:Optional=10:Numeric,maxDistance:Optional=10000:Numeric|Determines if one sprite is pointing at another.",
            "Sprite-IsPointingAway|baseSpriteUID:Required:Numeric,targetSpriteUID:Required:Numeric,toleranceDegrees:Optional=10:Numeric,maxDistance:Optional=10000:Numeric|Determines if one sprite is pointing aways from another",
            "Sprite-List|typeFilter:Optional:Criterion|Lists all sprites given an optional filter. Filter is a LIKE using !% and _.",
            "Sprite-Boost|uid:Required:Numeric,value:Required:Numeric|Displays a sprites configured max boost speed.",
            "Sprite-Speed|uid:Required:Numeric,value:Required:Numeric|Displays a sprites configured native boost speed.",
            "Sprite-Move|uid:Required:Numeric,x:Required:Numeric,y:Required:Numeric|Sets a new local position for a given sprite.",
            "Sprite-Move-Center|uid:Required:Numeric|Moves a given sprite to the center of the screen.",
            "Sprite-SpeedThrottle|uid:Required:Numeric,value:Required:Numeric|Gets the current throttle percentage for a sprite.",
            "Sprite-Visible|uid:Required:Numeric,state:Required:Boolean|Displays whether a given sprite is visible or not.",
        };

        private readonly GameEngineCore _gameEngine;
        private readonly Stack<string> _commandStack = new();
        private IDebugForm _debugForm;
        public DebugCommandParser CommandParser { get; } = new(_commandPrototypes);
        private readonly List<MethodInfo> _hardDebugMethods;
        public bool IsVisible { get; private set; } = false;

        public EngineDebugManager(GameEngineCore gameEngine, IDebugForm debugForm)
        {
            _gameEngine = gameEngine;
            _debugForm = debugForm;
            _hardDebugMethods = GetMethodsWithOnlyDebugCommandParameter();
        }

        public void EnqueueCommand(string command) => _commandStack.Push(command);

        public void ToggleVisibility()
        {
            IsVisible = !IsVisible;

            if (IsVisible)
            {
                _debugForm.Show();
            }
            else
            {
                _debugForm.Hide();
            }
        }

        public void ProcessCommand()
        {
            if (_commandStack.TryPop(out var command))
            {
                try
                {
                    _debugForm.WriteLine($"Executing(\"{command}\") {{:", System.Drawing.Color.Blue);

                    var parsedCommand = CommandParser.Parse(command);

                    var methodToExecute = _hardDebugMethods
                        .Where(o => o.Name.ToLower() == parsedCommand.PhysicalFunctionKey).FirstOrDefault()
                        ?? throw new Exception($"Physical function '{parsedCommand.PhysicalFunctionKey}' is not implemented.");

                    methodToExecute?.Invoke(this, new object[] { parsedCommand });
                }
                catch (Exception ex)
                {
                    _debugForm.WriteLine(ex.Message, System.Drawing.Color.DarkRed);
                }
                _debugForm.WriteLine($"}} ← Complete", System.Drawing.Color.Blue);
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

        public void CommandHandler_Display_RenderWindowPosition_Get(DebugCommand command)
        {
            _debugForm.WriteLine($"{_gameEngine.Display.RenderWindowPosition}", System.Drawing.Color.Black);
        }

        public void CommandHandler_Display_RenderWindowPosition_Set(DebugCommand command)
        {
            _gameEngine.Display.RenderWindowPosition.X = command.ParameterValue<double>("x");
            _gameEngine.Display.RenderWindowPosition.Y = command.ParameterValue<double>("y");
        }

        public void CommandHandler_Display_RenderWindowPosition_CenterOn(DebugCommand command)
        {
            _gameEngine.Sprites.Use(o =>
            {
                var spriteUID = command.ParameterValue<double>("spriteUID");
                var baseSprite = o.Where(o => o.UID == spriteUID).FirstOrDefault();
                if (baseSprite != null)
                {
                    _gameEngine.Display.RenderWindowPosition.X = baseSprite.X;
                    _gameEngine.Display.RenderWindowPosition.Y = baseSprite.Y;
                }
            });
        }

        public void CommandHandler_Display_Adapters(DebugCommand command)
        {
            var text = GraphicsUtility.GetGraphicsAdaptersDescriptions();
            _debugForm.Write(text, System.Drawing.Color.Black);
        }

        public void CommandHandler_Sprite_Enemies_DeleteAll(DebugCommand command)
        {
            foreach (var sprite in _gameEngine.Sprites.Enemies.All())
            {
                sprite.QueueForDelete();
            }
        }

        public void CommandHandler_Sprite_Enemies_ExplodeAll(DebugCommand command)
        {
            foreach (var sprite in _gameEngine.Sprites.Enemies.All())
            {
                sprite.Explode();
            }
        }

        public void CommandHandler_Sprite_Player_Explode(DebugCommand command)
        {
            _gameEngine.Player.Sprite.Explode();
        }

        public void CommandHandler_Cls(DebugCommand command)
        {
            _debugForm.ClearText();
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

                _debugForm.Write(text, System.Drawing.Color.Black);
            }
        }

        public void CommandHandler_Display_Metrics(DebugCommand command)
        {
            var infoText =
                  $"          Background Offset: {_gameEngine.Display.RenderWindowPosition}\r\n"
                + $"            Base Draw Scale: {_gameEngine.Display.BaseDrawScale:n4}\r\n"
                + $"              Overdraw Size: {_gameEngine.Display.OverdrawSize}\r\n"
                + $"        Natrual Screen Size: {_gameEngine.Display.NatrualScreenSize}\r\n"
                + $"          Total Canvas Size: {_gameEngine.Display.TotalCanvasSize}\r\n"
                + $"        Total Canvas Bounds: {_gameEngine.Display.TotalCanvasBounds}\r\n"
                + $"      Natrual Screen Bounds: {_gameEngine.Display.NatrualScreenBounds}\r\n"
                + $" Speed Frame Scaling Factor: {_gameEngine.Display.SpeedOrientedFrameScalingFactor():n4}";
            _debugForm.WriteLine(infoText, System.Drawing.Color.Black);
        }

        public void CommandHandler_Engine_HighlightAll(DebugCommand command)
        {
            var state = command.ParameterValue<bool>("state");
            _gameEngine.Settings.HighlightAllSprites = state;
        }

        public void CommandHandler_Display_Zoom_Reset(DebugCommand command)
        {
            _gameEngine.Display.OverrideSpeedOrientedFrameScalingFactor = double.NaN;
        }

        public void CommandHandler_Display_Zoom_Override(DebugCommand command)
        {
            var level = command.ParameterValue<double>("level");
            _gameEngine.Display.OverrideSpeedOrientedFrameScalingFactor = level.Clamp(-1, 1);
        }

        public void CommandHandler_Display_Zoom_Get(DebugCommand command)
        {
            _debugForm.WriteLine($"{_gameEngine.Display.SpeedOrientedFrameScalingFactor():n4}", System.Drawing.Color.Black);
        }

        public void CommandHandler_Engine_Pause(DebugCommand command)
        {
            var state = command.ParameterValue<bool>("state");

            if (state == true && _gameEngine.IsPaused() == false)
            {
                _gameEngine.Pause();
            }
            else if (state == false && _gameEngine.IsPaused() == true)
            {
                _gameEngine.Resume();
            }
        }

        public void CommandHandler_Display_Framerate(DebugCommand command)
        {
            var infoText =
                  $" Target: {_gameEngine.Settings.TargetFrameRate:n4}\r\n"
                + $"Average: {_gameEngine.Display.FrameCounter.AverageFrameRate:n4}\r\n"
                + $"Minimum: {_gameEngine.Display.FrameCounter.MinimumFrameRate:n4}\r\n"
                + $"Maximum: {_gameEngine.Display.FrameCounter.MaximumFrameRate:n4}";
            _debugForm.WriteLine(infoText, System.Drawing.Color.Black);
        }

        public void CommandHandler_Sprite_ListTypes(DebugCommand command)
        {
            var spriteTypes = SiReflection.GetSubClassesOf<SpriteBase>();

            StringBuilder text = new();

            foreach (var item in spriteTypes)
            {
                text.AppendLine(item.Name.ToString());
            }

            _debugForm.WriteLine(text.ToString(), System.Drawing.Color.Black);
        }

        public void CommandHandler_Sprite_Create(DebugCommand command)
        {
            var typeName = command.ParameterValue<string>("typeName");
            var x = command.ParameterValue<uint>("x");
            var y = command.ParameterValue<uint>("y");

            var sprite = SiReflection.CreateInstanceFromTypeName<SpriteBase>(typeName, new[] { _gameEngine });
            sprite.Location = new SiPoint(x, y);
            sprite.Visable = true;

            _gameEngine.Sprites.Add(sprite);

            _debugForm.WriteLine($"CreatedUID: {sprite.UID}, MultiplayUID: {sprite.MultiplayUID}", System.Drawing.Color.Black);
        }

        public void CommandHandler_Sprite_Player_Reflect_List(DebugCommand command)
        {
            var reflectionType = _gameEngine.Player.Sprite.GetType();
            var properties = reflectionType.GetProperties().OrderBy(o => o.Name).ToList();

            foreach (PropertyInfo property in properties)
            {
                _debugForm.WriteLine("    >[" + property.Name + "] : [" + property.PropertyType + "] = '" + property.GetValue(_gameEngine.Player.Sprite)?.ToString() + "'", System.Drawing.Color.Black);
            }
        }

        public void CommandHandler_Sprite_Player_Reflect_Set(DebugCommand command)
        {
            var propertyName = command.ParameterValue<string>("property");
            var propertyValue = command.ParameterValue<string>("value", string.Empty);

            var reflectionType = _gameEngine.Player.Sprite.GetType();
            var property = reflectionType.GetProperty(propertyName);

            if (property != null)
            {
                var convertedValue = Convert.ChangeType(propertyValue, property.PropertyType);
                property.SetValue(_gameEngine.Player.Sprite, convertedValue);

                _debugForm.WriteLine("    New value: " + property.GetValue(_gameEngine.Player.Sprite), System.Drawing.Color.Black);
            }
            else
            {
                _debugForm.WriteLine("    Property not found: " + propertyName, System.Drawing.Color.Black);
            }
        }

        public void CommandHandler_Sprite_Reflect_List(DebugCommand command)
        {
            _gameEngine.Sprites.Use(o =>
            {
                var uid = command.ParameterValue<uint>("uid");
                var sprite = o.Where(o => o.UID == uid).FirstOrDefault();
                if (sprite != null)
                {
                    var reflectionType = sprite.GetType();
                    var properties = reflectionType.GetProperties().OrderBy(o => o.Name).ToList();

                    foreach (PropertyInfo property in properties)
                    {
                        _debugForm.WriteLine("    >[" + property.Name + "] : [" + property.PropertyType + "] = '" + property.GetValue(sprite)?.ToString() + "'", System.Drawing.Color.Black);
                    }
                }
            });
        }

        public void CommandHandler_Sprite_Reflect_Set(DebugCommand command)
        {
            var propertyName = command.ParameterValue<string>("property");
            var propertyValue = command.ParameterValue<string>("value", string.Empty);

            _gameEngine.Sprites.Use(o =>
            {
                var uid = command.ParameterValue<uint>("uid");
                var sprite = o.Where(o => o.UID == uid).FirstOrDefault();
                if (sprite != null)
                {
                    var reflectionType = sprite.GetType();
                    var property = reflectionType.GetProperty(propertyName);

                    if (property != null)
                    {
                        var convertedValue = Convert.ChangeType(propertyValue, property.PropertyType);
                        property.SetValue(sprite, convertedValue);

                        _debugForm.WriteLine("    New value: " + property.GetValue(sprite), System.Drawing.Color.Black);
                    }
                    else
                    {
                        _debugForm.WriteLine("    Property not found: " + propertyName, System.Drawing.Color.Black);
                    }
                }
            });
        }

        public void CommandHandler_Sprite_Player_Inspect(DebugCommand command)
        {
            _debugForm.WriteLine(_gameEngine.Player.Sprite.GetInspectionText(), System.Drawing.Color.Black);
        }

        public void CommandHandler_Sprite_Inspect(DebugCommand command)
        {
            _gameEngine.Sprites.Use(o =>
            {
                var uid = command.ParameterValue<uint>("uid");
                var sprite = o.Where(o => o.UID == uid).FirstOrDefault();
                if (sprite != null)
                {
                    _debugForm.WriteLine(sprite.GetInspectionText(), System.Drawing.Color.Black);
                }
            });
        }

        public void CommandHandler_Sprite_Explode(DebugCommand command)
        {
            _gameEngine.Sprites.Use(o =>
            {
                var uid = command.ParameterValue<uint>("uid");
                var sprite = o.Where(o => o.UID == uid).FirstOrDefault();
                if (sprite != null)
                {
                    sprite.Explode();
                }
            });
        }

        public void CommandHandler_Sprite_IsPointingAt(DebugCommand command)
        {
            var baseSpriteUID = command.ParameterValue<uint>("baseSpriteUID");
            var targetSpriteUID = command.ParameterValue<uint>("targetSpriteUID");
            var toleranceDegrees = command.ParameterValue<double>("toleranceDegrees");
            var maxDistance = command.ParameterValue<double>("maxDistance");

            _gameEngine.Sprites.Use(o =>
            {
                var baseSprite = o.Where(o => o.UID == baseSpriteUID).FirstOrDefault();
                var targetSprite = o.Where(o => o.UID == targetSpriteUID).FirstOrDefault();

                if (baseSprite != null && targetSprite != null)
                {
                    var result = baseSprite.IsPointingAt(targetSprite, toleranceDegrees, maxDistance);
                    _debugForm.WriteLine($"IsPointingAt: {result}", System.Drawing.Color.Black);
                }
            });
        }

        public void CommandHandler_Sprite_IsPointingAway(DebugCommand command)
        {
            var baseSpriteUID = command.ParameterValue<uint>("baseSpriteUID");
            var targetSpriteUID = command.ParameterValue<uint>("targetSpriteUID");
            var toleranceDegrees = command.ParameterValue<double>("toleranceDegrees", 10);
            var maxDistance = command.ParameterValue<double>("maxDistance", 1000);

            _gameEngine.Sprites.Use(o =>
            {
                var baseSprite = o.Where(o => o.UID == baseSpriteUID).FirstOrDefault();
                var targetSprite = o.Where(o => o.UID == targetSpriteUID).FirstOrDefault();

                if (baseSprite != null && targetSprite != null)
                {
                    var result = baseSprite.IsPointingAway(targetSprite, toleranceDegrees, maxDistance);
                    _debugForm.WriteLine($"IsPointingAt: {result}", System.Drawing.Color.Black);
                }
            });
        }


        public void CommandHandler_Sprite_DistanceTo(DebugCommand command)
        {
            var baseSpriteUID = command.ParameterValue<uint>("baseSpriteUID");
            var targetSpriteUID = command.ParameterValue<uint>("targetSpriteUID");

            _gameEngine.Sprites.Use(o =>
            {
                var baseSprite = o.Where(o => o.UID == baseSpriteUID).FirstOrDefault();
                var targetSprite = o.Where(o => o.UID == targetSpriteUID).FirstOrDefault();

                if (baseSprite != null && targetSprite != null)
                {
                    var result = baseSprite.DistanceTo(targetSprite);
                    _debugForm.WriteLine($"DistanceTo: {result:n4}", System.Drawing.Color.Black);
                }
            });
        }

        public void CommandHandler_Sprite_AngleTo(DebugCommand command)
        {
            var baseSpriteUID = command.ParameterValue<uint>("baseSpriteUID");
            var targetSpriteUID = command.ParameterValue<uint>("targetSpriteUID");

            _gameEngine.Sprites.Use(o =>
            {
                var baseSprite = o.Where(o => o.UID == baseSpriteUID).FirstOrDefault();
                var targetSprite = o.Where(o => o.UID == targetSpriteUID).FirstOrDefault();

                if (baseSprite != null && targetSprite != null)
                {
                    var result = baseSprite.AngleTo360(targetSprite);
                    _debugForm.WriteLine($"AngleTo: {result:n4}", System.Drawing.Color.Black);
                }
            });
        }

        public void CommandHandler_Sprite_Watch(DebugCommand command)
        {
            var uid = command.ParameterValue<uint>("uid");
            _gameEngine.Sprites.Use(o =>
            {
                var sprite = o.Where(o => o.UID == uid).FirstOrDefault();
                if (sprite != null)
                {
                    _debugForm.StartWatch(_gameEngine, sprite);
                }
                else
                {
                    _debugForm.WriteLine($"Sprite not found: {uid}.", System.Drawing.Color.Red);
                }
            });
        }

        public void CommandHandler_Sprite_AngleInDegrees(DebugCommand command)
        {
            var uid = command.ParameterValue<uint>("uid");
            _gameEngine.Sprites.Use(o =>
            {
                var sprite = o.Where(o => o.UID == uid).FirstOrDefault();
                if (sprite != null)
                {
                    sprite.Velocity.Angle.Degrees = command.ParameterValue<double>("value");
                }
            });
        }

        public void CommandHandler_Sprite_Boost(DebugCommand command)
        {
            var uid = command.ParameterValue<uint>("uid");

            _gameEngine.Sprites.Use(o =>
            {
                var sprite = o.Where(o => o.UID == uid).FirstOrDefault();
                if (sprite != null)
                {
                    sprite.Velocity.BoostPercentage = command.ParameterValue<double>("value");
                }
            });
        }

        public void CommandHandler_Sprite_SpeedThrottle(DebugCommand command)
        {
            var uid = command.ParameterValue<uint>("uid");

            _gameEngine.Sprites.Use(o =>
            {
                var sprite = o.Where(o => o.UID == uid).FirstOrDefault();
                if (sprite != null)
                {
                    sprite.Velocity.ThrottlePercentage = command.ParameterValue<double>("value");
                }
            });
        }

        public void CommandHandler_Sprite_BoostThrottle(DebugCommand command)
        {
            var uid = command.ParameterValue<uint>("uid");

            _gameEngine.Sprites.Use(o =>
            {
                var sprite = o.Where(o => o.UID == uid).FirstOrDefault();
                if (sprite != null)
                {
                    sprite.Velocity.Boost = command.ParameterValue<double>("value");
                }
            });
        }

        public void CommandHandler_Sprite_Speed(DebugCommand command)
        {
            var uid = command.ParameterValue<uint>("uid");

            _gameEngine.Sprites.Use(o =>
            {
                var sprite = o.Where(o => o.UID == uid).FirstOrDefault();
                if (sprite != null)
                {
                    sprite.Velocity.Speed = command.ParameterValue<double>("value");
                }
            });
        }

        public void CommandHandler_Sprite_Highlight(DebugCommand command)
        {
            var uid = command.ParameterValue<uint>("uid");

            _gameEngine.Sprites.Use(o =>
            {
                var sprite = o.Where(o => o.UID == uid).FirstOrDefault();
                if (sprite != null)
                {
                    sprite.IsHighlighted = command.ParameterValue<bool>("state");
                }
            });
        }

        public void CommandHandler_Sprite_Visible(DebugCommand command)
        {
            var uid = command.ParameterValue<uint>("uid");

            _gameEngine.Sprites.Use(o =>
            {
                var sprite = o.Where(o => o.UID == uid).FirstOrDefault();
                if (sprite != null)
                {
                    sprite.Visable = command.ParameterValue<bool>("state");
                }
            });
        }

        public void CommandHandler_Sprite_Move(DebugCommand command)
        {
            var uid = command.ParameterValue<uint>("uid");

            _gameEngine.Sprites.Use(o =>
            {
                var sprite = o.Where(o => o.UID == uid).FirstOrDefault();
                if (sprite != null)
                {
                    sprite.X = command.ParameterValue<double>("x");
                    sprite.Y = command.ParameterValue<double>("y");
                }
            });
        }

        public void CommandHandler_Sprite_Move_Center(DebugCommand command)
        {
            var uid = command.ParameterValue<uint>("uid");

            _gameEngine.Sprites.Use(o =>
            {
                var sprite = o.Where(o => o.UID == uid).FirstOrDefault();
                if (sprite != null)
                {
                    sprite.X = _gameEngine.Display.TotalCanvasSize.Width / 2;
                    sprite.Y = _gameEngine.Display.TotalCanvasSize.Height / 2;
                }
            });
        }

        public void CommandHandler_Sprite_List(DebugCommand command)
        {
            _gameEngine.Sprites.Use(o =>
            {
                var sprites = o.ToList();

                var typeFilter = command.ParameterValue<DebugCommandParameterCriterion>("typeFilter");
                if (typeFilter != null)
                {
                    sprites = sprites.Where(o => DebugCommandParameter.IsMatchLike(o.GetType().Name, typeFilter.Value, typeFilter.IsNotCriteria)).ToList();
                }

                foreach (var sprite in sprites)
                {
                    _debugForm.WriteLine($"Type: {sprite.GetType().Name}, UID: {sprite.UID}, Position: {sprite.Location}", System.Drawing.Color.Black);
                }
            });
        }

        #endregion
    }
}
