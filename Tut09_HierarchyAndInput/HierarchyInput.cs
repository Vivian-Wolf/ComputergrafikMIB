using System;
using System.Collections.Generic;
using System.Linq;
using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Math.Core;
using Fusee.Serialization;
using Fusee.Xene;
using static System.Math;
using static Fusee.Engine.Core.Input;
using static Fusee.Engine.Core.Time;

namespace Fusee.Tutorial.Core
{
    public class HierarchyInput : RenderCanvas
    {
        private SceneContainer _scene;
        private SceneRenderer _sceneRenderer;
        private float _camAngle = 0;
        private TransformComponent _baseTransform;
        private TransformComponent _bodyTransform;
        private TransformComponent _upperArmTransform;
        private TransformComponent _foreArmTransform;
        private TransformComponent _greifLeftTransform;
        private TransformComponent _greifRightTransform;

        SceneContainer CreateScene()
        {
            // Initialize transform components that need to be changed inside "RenderAFrame"
            _baseTransform = new TransformComponent
            {
                Rotation = new float3(0, 0, 0),
                Scale = new float3(1, 1, 1),
                Translation = new float3(0, 0, 0)
            };
            _bodyTransform = new TransformComponent
            {
               Rotation = new float3(0, -45 * M.Pi / 180, 0), 
               Scale = new float3(1, 1, 1),
               Translation = new float3(0, 6, 0)
            };
            _upperArmTransform = new TransformComponent
            {
               Rotation = new float3(38 * M.Pi / 180, 0, 0), 
               Scale = new float3(1, 1, 1),
               Translation = new float3(2, 4, 0)
            };
            _foreArmTransform = new TransformComponent
            {
               Rotation = new float3(90 * M.Pi / 180, 0 , 0),
               Scale = new float3(1, 1, 1),
               Translation = new float3(-2, 4, 0)
            };
            _greifLeftTransform = new TransformComponent
            {
               Rotation = new float3(0, 0, 45 * M.Pi / 180), 
               Scale = new float3(1, 1, 1),
               Translation = new float3(-1, 5, 0)
            };
            _greifRightTransform = new TransformComponent
            {
                Rotation = new float3(0, 0, -45 * M.Pi / 180),
                Scale = new float3(1, 1, 1),
                Translation = new float3(1, 5, 0)
            };
            

            // Setup the scene graph
            return new SceneContainer
            {
                Children = new List<SceneNodeContainer>
                {
                    new SceneNodeContainer
                    {
                        Components = new List<SceneComponentContainer>
                        {
                            // TRANSFROM COMPONENT
                            _baseTransform,

                            // SHADER EFFECT COMPONENT
                            new ShaderEffectComponent
                            {
                                Effect = SimpleMeshes.MakeShaderEffect(new float3(0.7f, 0.7f, 0.7f), new float3(0.7f, 0.7f, 0.7f), 5)
                            },

                            // MESH COMPONENT
                            SimpleMeshes.CreateCuboid(new float3(10, 2, 10))
                        },
                        
                        Children = new List<SceneNodeContainer>
                        {
                            //roter Arm/ roter Body
                            new SceneNodeContainer
                            {
                                Components = new List<SceneComponentContainer>
                                {
                                    _bodyTransform,

                                    new ShaderEffectComponent
                                    {
                                        Effect = SimpleMeshes.MakeShaderEffect(new float3(0.83f, 0.13f, 0.18f), new float3(0.7f, 0.7f, 0.7f), 5)
                                    },

                                    SimpleMeshes.CreateCuboid(new float3(2, 10, 2))
                                },

                                Children = new List<SceneNodeContainer>
                                {
                                    //grüner Arm
                                    new SceneNodeContainer
                                    {
                                        Components = new List<SceneComponentContainer>
                                        {
                                            _upperArmTransform //extra Koordinatensystem für Rotation um bestimmten Punkt eingefügt als extra Child
                                        },

                                        Children = new List<SceneNodeContainer>
                                        {
                                            new SceneNodeContainer
                                            {
                                                Components = new List<SceneComponentContainer>
                                                {
                                                    new TransformComponent
                                                    {
                                                        Rotation = new float3(0, 0, 0), 
                                                        Scale = new float3(1, 1, 1),
                                                        Translation = new float3(0, 4, 0)
                                                    },

                                                    new ShaderEffectComponent
                                                    {
                                                        Effect = SimpleMeshes.MakeShaderEffect(new float3(0.23f, 0.64f, 0.34f), new float3(0.7f, 0.7f, 0.7f), 5)
                                                    },

                                                    SimpleMeshes.CreateCuboid(new float3(2, 10, 2))
                                                },

                                                Children = new List<SceneNodeContainer>
                                                {
                                                    //blauer Arm
                                                    new SceneNodeContainer
                                                    {
                                                        Components = new List<SceneComponentContainer>
                                                        {
                                                            _foreArmTransform //extra Koordinatensystem für Rotation um bestimmten Punkt eingefügt als extra Child
                                                        },

                                                        Children = new List<SceneNodeContainer>
                                                        {
                                                            new SceneNodeContainer
                                                            {
                                                                Components = new List<SceneComponentContainer>
                                                                {
                                                                    new TransformComponent
                                                                    {
                                                                        Rotation = new float3(0, 0, 0), 
                                                                        Scale = new float3(1, 1, 1),
                                                                        Translation = new float3(0, 4, 0)
                                                                    },

                                                                    new ShaderEffectComponent
                                                                    {
                                                                        Effect = SimpleMeshes.MakeShaderEffect(new float3(0.13f, 0.21f, 0.61f), new float3(0.7f, 0.7f, 0.7f), 5)
                                                                    },

                                                                    SimpleMeshes.CreateCuboid(new float3(2, 10, 2))
                                                                },

                                                                //Greifarme
                                                                Children = new List<SceneNodeContainer>
                                                                {
                                                                    //Linker Arm
                                                                    new SceneNodeContainer
                                                                    {
                                                                        Components = new List<SceneComponentContainer>
                                                                        {
                                                                            _greifLeftTransform
                                                                        },

                                                                        Children = new List<SceneNodeContainer>
                                                                        {
                                                                            new SceneNodeContainer
                                                                            {
                                                                                Components = new List<SceneComponentContainer>
                                                                                {
                                                                                    new TransformComponent
                                                                                    {
                                                                                        Rotation = new float3(0, 0, 0),
                                                                                        Scale = new float3(1, 1, 1),
                                                                                        Translation = new float3(0, 1.25f, 0)
                                                                                    },

                                                                                    new ShaderEffectComponent
                                                                                    {
                                                                                        Effect = SimpleMeshes.MakeShaderEffect(new float3(0, 0.84f, 1.0f), new float3(0.7f, 0.7f, 0.7f),5)
                                                                                    },

                                                                                    SimpleMeshes.CreateCuboid(new float3(1, 2.5f, 1))
                                                                                }
                                                                            }
                                                                        }
                                                                    },
                                                                    
                                                                    //Rechter Arm
                                                                    new SceneNodeContainer
                                                                    {
                                                                        Components = new List<SceneComponentContainer>
                                                                        {
                                                                            _greifRightTransform
                                                                        },

                                                                        Children = new List<SceneNodeContainer>
                                                                        {
                                                                            new SceneNodeContainer
                                                                            {
                                                                                Components = new List<SceneComponentContainer>
                                                                                {
                                                                                    new TransformComponent
                                                                                    {
                                                                                        Rotation = new float3(0, 0, 0),
                                                                                        Scale = new float3(1, 1, 1),
                                                                                        Translation = new float3(0, 1.25f, 0)
                                                                                    },

                                                                                    new ShaderEffectComponent
                                                                                    {
                                                                                        Effect = SimpleMeshes.MakeShaderEffect(new float3(0, 0.84f, 1.0f), new float3(0.7f, 0.7f, 0.7f),5)
                                                                                    },

                                                                                    SimpleMeshes.CreateCuboid(new float3(1, 2.5f, 1))
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };
        }

        // Init is called on startup. 
        public override void Init()
        {
            // Set the clear color for the backbuffer to white (100% intensity in all color channels R, G, B, A).
            RC.ClearColor = new float4(0.0f, 0.2f, 0.2f, 1);

            _scene = CreateScene();

            // Create a scene renderer holding the scene above
            _sceneRenderer = new SceneRenderer(_scene);
        }

        // RenderAFrame is called once a frame
        public override void RenderAFrame()
        {
            //Roboter mit Tastatur steuern können
            //Body
            float bodyRot = _bodyTransform.Rotation.y;
            bodyRot += 0.4f * Keyboard.LeftRightAxis * Time.DeltaTime;
            _bodyTransform.Rotation = new float3(0, bodyRot, 0);
            //UpperArm
            float upperArmRot = _upperArmTransform.Rotation.x;
            upperArmRot += 0.4f * Keyboard.UpDownAxis * Time.DeltaTime;
            _upperArmTransform.Rotation = new float3(upperArmRot, 0, 0);
            //ForeArm
            float foreArmRot = _foreArmTransform.Rotation.x;
            foreArmRot += 0.4f * Keyboard.WSAxis * Time.DeltaTime;
            _foreArmTransform.Rotation = new float3(foreArmRot, 0, 0);

            //Greifarme
            //GreifarmBegrenzungLinks();
            //GreifarmBegrenzungRechts();
            GreifarmBegrenzungLinks();
            GreifarmBegrenzungRechts();
                

            //Maussteuerung
            if(Mouse.LeftButton)
              _camAngle += 0.01f * Mouse.Velocity.x * Time.DeltaTime;
            
            // Clear the backbuffer
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            // Setup the camera 
            RC.View = float4x4.CreateTranslation(0, -10, 50) * float4x4.CreateRotationY(_camAngle);

            // Render the scene on the current render context
            _sceneRenderer.Render(RC);

            // Swap buffers: Show the contents of the backbuffer (containing the currently rendered farame) on the front buffer.
            Present();
        }


        // Is called when the window was resized
        public override void Resize()
        {
            // Set the new rendering area to the entire new windows size
            RC.Viewport(0, 0, Width, Height);

            // Create a new projection matrix generating undistorted images on the new aspect ratio.
            var aspectRatio = Width / (float)Height;

            // 0.25*PI Rad -> 45° Opening angle along the vertical direction. Horizontal opening angle is calculated based on the aspect ratio
            // Front clipping happens at 1 (Objects nearer than 1 world unit get clipped)
            // Back clipping happens at 2000 (Anything further away from the camera than 2000 world units gets clipped, polygons will be cut)
            var projection = float4x4.CreatePerspectiveFieldOfView(M.PiOver4, aspectRatio, 1, 20000);
            RC.Projection = projection;
        }

        public void GreifarmBegrenzungLinks()
        {
            float leftArmRot = _greifLeftTransform.Rotation.z;
            leftArmRot += 0.4f * Keyboard.ADAxis * Time.DeltaTime;
            //außen
            if(leftArmRot >= 45 * M.Pi / 180)
            {
                leftArmRot = 45 * M.Pi / 180;
                _greifLeftTransform.Rotation = new float3(0, 0, leftArmRot);
            }    
            //innen
            else if(leftArmRot <= -11.26f * M.Pi / 180)
            {
                leftArmRot = -11.26f * M.Pi / 180;
                _greifLeftTransform.Rotation = new float3(0, 0, leftArmRot);
            }else
            {
                _greifLeftTransform.Rotation = new float3(0, 0, leftArmRot);
            }  
        }
        public void GreifarmBegrenzungRechts()
        {
            float rightArmRot = _greifRightTransform.Rotation.z;
            rightArmRot += -0.4f * Keyboard.ADAxis * Time.DeltaTime;
            //außen
            if(rightArmRot <= -45 * M.Pi / 180)
            {
                rightArmRot = -45 * M.Pi / 180;
                _greifRightTransform.Rotation = new float3(0, 0, rightArmRot);
            }    
            //innen
            else if(rightArmRot >= 11.26f * M.Pi / 180)
            {
                rightArmRot = 11.26f * M.Pi / 180;
                _greifRightTransform.Rotation = new float3(0, 0, rightArmRot);
            }else
            {
                _greifRightTransform.Rotation = new float3(0, 0, rightArmRot);
            }  
        }
    }
}