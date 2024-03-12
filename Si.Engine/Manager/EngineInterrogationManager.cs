using Si.Engine.Interrogation;
using Si.Engine.Interrogation._Superclass;
using Si.Engine.Sprite._Superclass;
using Si.Library;
using Si.Library.Mathematics.Geometry;
using Si.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Si.Engine.Manager
{
    /// <summary>
    /// Handles keyboard debugging inquiries.
    /// </summary>
    public class EngineInterrogationManager
    {
        private static string[] _commandPrototypes = {
            "Cls||Clears the debug screen.",
            "Help|command:Optional:Criterion|Displays help for all or a given command.",

            "Display-Zoom-Reset||Resets the global zoom level to auto.",
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

        private readonly EngineCore _engine;
        private readonly Stack<string> _commandStack = new();
        private IInterrogationForm _debugForm;
        public InterrogationCommandParser CommandParser { get; } = new(_commandPrototypes);
        private readonly List<MethodInfo> _hardDebugMethods;
        public bool IsVisible { get; private set; } = false;

        public EngineInterrogationManager(EngineCore engine, IInterrogationForm debugForm)
        {
            _engine = engine;
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
            var allMethods = typeof(EngineInterrogationManager)
                .GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var method in allMethods)
            {
                var parameters = method.GetParameters();
                if (parameters.Length == 1 && parameters[0].ParameterType == typeof(InterrogationCommand))
                {
                    methods.Add(method);
                }
            }

            return methods;
        }

        #region Physical debug command handlers.

        public void CommandHandler_Display_RenderWindowPosition_Get(InterrogationCommand command)
        {
            _debugForm.WriteLine($"{_engine.Display.RenderWindowPosition}", System.Drawing.Color.Black);
        }

        public void CommandHandler_Display_RenderWindowPosition_Set(InterrogationCommand command)
        {
            _engine.Display.RenderWindowPosition.X = command.ParameterValue<float>("x");
            _engine.Display.RenderWindowPosition.Y = command.ParameterValue<float>("y");
        }

        public void CommandHandler_Display_RenderWindowPosition_CenterOn(InterrogationCommand command)
        {
            _engine.Sprites.DebugOnlyAccess(o =>
            {
                var spriteUID = command.ParameterValue<float>("spriteUID");
                var baseSprite = o.Where(o => o.UID == spriteUID).FirstOrDefault();
                if (baseSprite != null)
                {
                    _engine.Display.RenderWindowPosition.X = baseSprite.X;
                    _engine.Display.RenderWindowPosition.Y = baseSprite.Y;
                }
            });
        }

        public void CommandHandler_Display_Adapters(InterrogationCommand command)
        {
            var text = SiRenderingUtility.GetGraphicsAdaptersDescriptions();
            _debugForm.Write(text, System.Drawing.Color.Black);
        }

        public void CommandHandler_Sprite_Enemies_DeleteAll(InterrogationCommand command)
        {
            foreach (var sprite in _engine.Sprites.Enemies.All())
            {
                sprite.QueueForDelete();
            }
        }

        public void CommandHandler_Sprite_Enemies_ExplodeAll(InterrogationCommand command)
        {
            foreach (var sprite in _engine.Sprites.Enemies.All())
            {
                sprite.Explode();
            }
        }

        public void CommandHandler_Sprite_Player_Explode(InterrogationCommand command)
        {
            _engine.Player.Sprite.Explode();
        }

        public void CommandHandler_Cls(InterrogationCommand command)
        {
            _debugForm.ClearText();
        }

        public void CommandHandler_Help(InterrogationCommand command)
        {
            var commands = CommandParser.Commands.OrderBy(o => o.Name).ToList();

            var typeFilter = command.ParameterValue<InterrogationCommandParameterCriterion>("command");
            if (typeFilter != null)
            {
                commands = commands.Where(o => InterrogationCommandParameter.IsMatchLike(o.Name, typeFilter.Value, typeFilter.IsNotCriteria)).ToList();
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

        public void CommandHandler_Display_Metrics(InterrogationCommand command)
        {
            var infoText =
                  $"          Background Offset: {_engine.Display.RenderWindowPosition}\r\n"
                + $"            Base Draw Scale: {_engine.Display.BaseDrawScale:n4}\r\n"
                + $"              Overdraw Size: {_engine.Display.OverdrawSize}\r\n"
                + $"        Natrual Screen Size: {_engine.Display.NatrualScreenSize}\r\n"
                + $"          Total Canvas Size: {_engine.Display.TotalCanvasSize}\r\n"
                + $"        Total Canvas Bounds: {_engine.Display.TotalCanvasBounds}\r\n"
                + $"      Natrual Screen Bounds: {_engine.Display.NatrualScreenBounds}\r\n"
                + $" Speed Frame Scaling Factor: {_engine.Display.SpeedOrientedFrameScalingFactor():n4}";
            _debugForm.WriteLine(infoText, System.Drawing.Color.Black);
        }

        public void CommandHandler_Engine_HighlightAll(InterrogationCommand command)
        {
            var state = command.ParameterValue<bool>("state");
            _engine.Settings.HighlightAllSprites = state;
        }

        public void CommandHandler_Display_Zoom_Get(InterrogationCommand command)
        {
            _debugForm.WriteLine($"{_engine.Display.SpeedOrientedFrameScalingFactor():n4}", System.Drawing.Color.Black);
        }

        public void CommandHandler_Engine_Pause(InterrogationCommand command)
        {
            var state = command.ParameterValue<bool>("state");

            if (state == true && _engine.IsPaused() == false)
            {
                _engine.Pause();
            }
            else if (state == false && _engine.IsPaused() == true)
            {
                _engine.Resume();
            }
        }

        public void CommandHandler_Display_Framerate(InterrogationCommand command)
        {
            var infoText =
                  $" Target: {_engine.Settings.TargetFrameRate:n4}\r\n"
                + $"Average: {_engine.Display.FrameCounter.AverageFrameRate:n4}\r\n"
                + $"Minimum: {_engine.Display.FrameCounter.MinimumFrameRate:n4}\r\n"
                + $"Maximum: {_engine.Display.FrameCounter.MaximumFrameRate:n4}";
            _debugForm.WriteLine(infoText, System.Drawing.Color.Black);
        }

        public void CommandHandler_Sprite_ListTypes(InterrogationCommand command)
        {
            var spriteTypes = SiReflection.GetSubClassesOf<SpriteBase>();

            StringBuilder text = new();

            foreach (var item in spriteTypes)
            {
                text.AppendLine(item.Name.ToString());
            }

            _debugForm.WriteLine(text.ToString(), System.Drawing.Color.Black);
        }

        public void CommandHandler_Sprite_Create(InterrogationCommand command)
        {
            var typeName = command.ParameterValue<string>("typeName");
            var x = command.ParameterValue<uint>("x");
            var y = command.ParameterValue<uint>("y");

            var sprite = SiReflection.CreateInstanceFromTypeName<SpriteBase>(typeName, new[] { _engine });
            sprite.Location = new SiPoint(x, y);
            sprite.Visable = true;

            _engine.Sprites.Add(sprite);

            _debugForm.WriteLine($"CreatedUID: {sprite.UID}", System.Drawing.Color.Black);
        }

        public void CommandHandler_Sprite_Player_Reflect_List(InterrogationCommand command)
        {
            var reflectionType = _engine.Player.Sprite.GetType();
            var properties = reflectionType.GetProperties().OrderBy(o => o.Name).ToList();

            foreach (PropertyInfo property in properties)
            {
                _debugForm.WriteLine("    >[" + property.Name + "] : [" + property.PropertyType + "] = '" + property.GetValue(_engine.Player.Sprite)?.ToString() + "'", System.Drawing.Color.Black);
            }
        }

        public void CommandHandler_Sprite_Player_Reflect_Set(InterrogationCommand command)
        {
            var propertyName = command.ParameterValue<string>("property");
            var propertyValue = command.ParameterValue("value", string.Empty);

            var reflectionType = _engine.Player.Sprite.GetType();
            var property = reflectionType.GetProperty(propertyName);

            if (property != null)
            {
                var convertedValue = Convert.ChangeType(propertyValue, property.PropertyType);
                property.SetValue(_engine.Player.Sprite, convertedValue);

                _debugForm.WriteLine("    New value: " + property.GetValue(_engine.Player.Sprite), System.Drawing.Color.Black);
            }
            else
            {
                _debugForm.WriteLine("    Property not found: " + propertyName, System.Drawing.Color.Black);
            }
        }

        public void CommandHandler_Sprite_Reflect_List(InterrogationCommand command)
        {
            _engine.Sprites.DebugOnlyAccess(o =>
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

        public void CommandHandler_Sprite_Reflect_Set(InterrogationCommand command)
        {
            var propertyName = command.ParameterValue<string>("property");
            var propertyValue = command.ParameterValue("value", string.Empty);

            _engine.Sprites.DebugOnlyAccess(o =>
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

        public void CommandHandler_Sprite_Player_Inspect(InterrogationCommand command)
        {
            _debugForm.WriteLine(_engine.Player.Sprite.GetInspectionText(), System.Drawing.Color.Black);
        }

        public void CommandHandler_Sprite_Inspect(InterrogationCommand command)
        {
            _engine.Sprites.DebugOnlyAccess(o =>
            {
                var uid = command.ParameterValue<uint>("uid");
                var sprite = o.Where(o => o.UID == uid).FirstOrDefault();
                if (sprite != null)
                {
                    _debugForm.WriteLine(sprite.GetInspectionText(), System.Drawing.Color.Black);
                }
            });
        }

        public void CommandHandler_Sprite_Explode(InterrogationCommand command)
        {
            _engine.Sprites.DebugOnlyAccess(o =>
            {
                var uid = command.ParameterValue<uint>("uid");
                var sprite = o.Where(o => o.UID == uid).FirstOrDefault();
                if (sprite != null)
                {
                    sprite.Explode();
                }
            });
        }

        public void CommandHandler_Sprite_IsPointingAt(InterrogationCommand command)
        {
            var baseSpriteUID = command.ParameterValue<uint>("baseSpriteUID");
            var targetSpriteUID = command.ParameterValue<uint>("targetSpriteUID");
            var toleranceDegrees = command.ParameterValue<float>("toleranceDegrees");
            var maxDistance = command.ParameterValue<float>("maxDistance");

            _engine.Sprites.DebugOnlyAccess(o =>
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

        public void CommandHandler_Sprite_IsPointingAway(InterrogationCommand command)
        {
            var baseSpriteUID = command.ParameterValue<uint>("baseSpriteUID");
            var targetSpriteUID = command.ParameterValue<uint>("targetSpriteUID");
            var toleranceDegrees = command.ParameterValue<float>("toleranceDegrees", 10);
            var maxDistance = command.ParameterValue<float>("maxDistance", 1000);

            _engine.Sprites.DebugOnlyAccess(o =>
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


        public void CommandHandler_Sprite_DistanceTo(InterrogationCommand command)
        {
            var baseSpriteUID = command.ParameterValue<uint>("baseSpriteUID");
            var targetSpriteUID = command.ParameterValue<uint>("targetSpriteUID");

            _engine.Sprites.DebugOnlyAccess(o =>
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

        public void CommandHandler_Sprite_AngleTo(InterrogationCommand command)
        {
            var baseSpriteUID = command.ParameterValue<uint>("baseSpriteUID");
            var targetSpriteUID = command.ParameterValue<uint>("targetSpriteUID");

            _engine.Sprites.DebugOnlyAccess(o =>
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

        public void CommandHandler_Sprite_Watch(InterrogationCommand command)
        {
            var uid = command.ParameterValue<uint>("uid");
            _engine.Sprites.DebugOnlyAccess(o =>
            {
                var sprite = o.Where(o => o.UID == uid).FirstOrDefault();
                if (sprite != null)
                {
                    _debugForm.StartWatch(_engine, sprite);
                }
                else
                {
                    _debugForm.WriteLine($"Sprite not found: {uid}.", System.Drawing.Color.Red);
                }
            });
        }

        public void CommandHandler_Sprite_AngleInDegrees(InterrogationCommand command)
        {
            var uid = command.ParameterValue<uint>("uid");
            _engine.Sprites.DebugOnlyAccess(o =>
            {
                var sprite = o.Where(o => o.UID == uid).FirstOrDefault();
                if (sprite != null)
                {
                    sprite.Velocity.ForwardAngle.Degrees = command.ParameterValue<float>("value");
                }
            });
        }

        public void CommandHandler_Sprite_Boost(InterrogationCommand command)
        {
            var uid = command.ParameterValue<uint>("uid");

            _engine.Sprites.DebugOnlyAccess(o =>
            {
                var sprite = o.Where(o => o.UID == uid).FirstOrDefault();
                if (sprite != null)
                {
                    sprite.Velocity.ForwardBoostVelocity = command.ParameterValue<float>("value");
                }
            });
        }

        public void CommandHandler_Sprite_SpeedThrottle(InterrogationCommand command)
        {
            var uid = command.ParameterValue<uint>("uid");

            _engine.Sprites.DebugOnlyAccess(o =>
            {
                var sprite = o.Where(o => o.UID == uid).FirstOrDefault();
                if (sprite != null)
                {
                    sprite.Velocity.ForwardVelocity = command.ParameterValue<float>("value");
                }
            });
        }

        public void CommandHandler_Sprite_BoostThrottle(InterrogationCommand command)
        {
            var uid = command.ParameterValue<uint>("uid");

            _engine.Sprites.DebugOnlyAccess(o =>
            {
                var sprite = o.Where(o => o.UID == uid).FirstOrDefault();
                if (sprite != null)
                {
                    sprite.Velocity.MaximumBoostSpeed = command.ParameterValue<float>("value");
                }
            });
        }

        public void CommandHandler_Sprite_Speed(InterrogationCommand command)
        {
            var uid = command.ParameterValue<uint>("uid");

            _engine.Sprites.DebugOnlyAccess(o =>
            {
                var sprite = o.Where(o => o.UID == uid).FirstOrDefault();
                if (sprite != null)
                {
                    sprite.Velocity.MaximumSpeed = command.ParameterValue<float>("value");
                }
            });
        }

        public void CommandHandler_Sprite_Highlight(InterrogationCommand command)
        {
            var uid = command.ParameterValue<uint>("uid");

            _engine.Sprites.DebugOnlyAccess(o =>
            {
                var sprite = o.Where(o => o.UID == uid).FirstOrDefault();
                if (sprite != null)
                {
                    sprite.IsHighlighted = command.ParameterValue<bool>("state");
                }
            });
        }

        public void CommandHandler_Sprite_Visible(InterrogationCommand command)
        {
            var uid = command.ParameterValue<uint>("uid");

            _engine.Sprites.DebugOnlyAccess(o =>
            {
                var sprite = o.Where(o => o.UID == uid).FirstOrDefault();
                if (sprite != null)
                {
                    sprite.Visable = command.ParameterValue<bool>("state");
                }
            });
        }

        public void CommandHandler_Sprite_Move(InterrogationCommand command)
        {
            var uid = command.ParameterValue<uint>("uid");

            _engine.Sprites.DebugOnlyAccess(o =>
            {
                var sprite = o.Where(o => o.UID == uid).FirstOrDefault();
                if (sprite != null)
                {
                    sprite.X = command.ParameterValue<float>("x");
                    sprite.Y = command.ParameterValue<float>("y");
                }
            });
        }

        public void CommandHandler_Sprite_Move_Center(InterrogationCommand command)
        {
            var uid = command.ParameterValue<uint>("uid");

            _engine.Sprites.DebugOnlyAccess(o =>
            {
                var sprite = o.Where(o => o.UID == uid).FirstOrDefault();
                if (sprite != null)
                {
                    sprite.X = _engine.Display.TotalCanvasSize.Width / 2;
                    sprite.Y = _engine.Display.TotalCanvasSize.Height / 2;
                }
            });
        }

        public void CommandHandler_Sprite_List(InterrogationCommand command)
        {
            _engine.Sprites.DebugOnlyAccess(o =>
            {
                var sprites = o.ToList();

                var typeFilter = command.ParameterValue<InterrogationCommandParameterCriterion>("typeFilter");
                if (typeFilter != null)
                {
                    sprites = sprites.Where(o => InterrogationCommandParameter.IsMatchLike(o.GetType().Name, typeFilter.Value, typeFilter.IsNotCriteria)).ToList();
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
