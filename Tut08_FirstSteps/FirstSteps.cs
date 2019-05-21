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
    public class FirstSteps : RenderCanvas
    {   
        private SceneContainer _scene;
        private SceneRenderer _sceneRenderer;
        private float _camAngle = 0;
        private TransformComponent _cube1Transform;
        private TransformComponent _cube2Transform;
        private TransformComponent _cube3Transform;
        private ShaderEffectComponent cube3Shader;

        // Init is called on startup. 
        public override void Init()
        {
            // Set the clear color for the backbuffer to custom color (100% intentsity in all color channels R, G, B, A).
            RC.ClearColor = new float4(0.58f, 0, 0.7f, 1);

            // Create a scene with 3 cubes
            // The three components: one XForm, one Shader and the Mesh
            
            //Würfel Nr.1
            _cube1Transform = new TransformComponent {Scale = new float3(1, 1, 1), Translation = new float3(0, 0, 0)}; //Translation = Position im Raum
            var cube1Shader = new ShaderEffectComponent
            { 
                Effect = SimpleMeshes.MakeShaderEffect(new float3 (0.3f, 0.88f, 1), new float3 (1, 1, 1),  4)
            };
            var cube1Mesh = SimpleMeshes.CreateCuboid(new float3(10, 10, 10));

            //Würfel Nr.2
            _cube2Transform = new TransformComponent {Scale = new float3(1,1,1), Translation = new float3(20,0,0)};
            var cube2Shader = new ShaderEffectComponent
            {
                Effect = SimpleMeshes.MakeShaderEffect(new float3(1, 0, 0.67f), new float3(1,1,1), 4)
            };
            var cube2Mesh = SimpleMeshes.CreateCuboid(new float3(12,12,12)); 

            //Würfel Nr.3
            _cube3Transform = new TransformComponent {Scale = new float3(1,1,1), Translation = new float3(-20,5,0)};
             cube3Shader = new ShaderEffectComponent
            {
                Effect = SimpleMeshes.MakeShaderEffect(new float3(0.8f, 0, 0.13f), new float3(1,1,1), 4)
            };
            var cube3Mesh = SimpleMeshes.CreateCuboid(new float3(10,10,10)); 

            // Assemble the cube node containing the three components
            var cube1Node = new SceneNodeContainer();
            cube1Node.Components = new List<SceneComponentContainer>();
            cube1Node.Components.Add(_cube1Transform);
            cube1Node.Components.Add(cube1Shader);
            cube1Node.Components.Add(cube1Mesh);

            var cube2Node = new SceneNodeContainer();
            cube2Node.Components = new List<SceneComponentContainer>();
            cube2Node.Components.Add(_cube2Transform);
            cube2Node.Components.Add(cube2Shader);
            cube2Node.Components.Add(cube2Mesh);

            var cube3Node = new SceneNodeContainer();
            cube3Node.Components = new List<SceneComponentContainer>();
            cube3Node.Components.Add(_cube3Transform);
            cube3Node.Components.Add(cube3Shader);
            cube3Node.Components.Add(cube3Mesh);

            // Create the scene containing the cube as the only object
            _scene = new SceneContainer();
            _scene.Children = new List<SceneNodeContainer>();
            _scene.Children.Add(cube1Node);
            _scene.Children.Add(cube2Node);
            _scene.Children.Add(cube3Node);

            // Create a scene renderer holding the scene above
            _sceneRenderer = new SceneRenderer(_scene);
        }

        // RenderAFrame is called once a frame
        public override void RenderAFrame()
        {
            // Clear the backbuffer
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            // Animate the camera angle
             _camAngle = _camAngle + 20.0f * M.Pi/180.0f * DeltaTime;    //Drehung von 90° pro Sekunde (bei 90*M.Pi/180), egal wie mit vielen Frames die Animation läuft

            // Setup the camera 
            RC.View = float4x4.CreateTranslation(0, 0, 50) * float4x4.CreateRotationY(_camAngle);

            // Animate the cube
            _cube1Transform.Translation = new float3(0, 2 * M.Sin(1.75f * TimeSinceStart), 0);
            _cube2Transform.Scale = new float3(0.5f * M.Sin(4 * TimeSinceStart)+1,1,1);
            _cube3Transform.Translation = new float3(-20,5, 5 * M.Sin(1.5f*TimeSinceStart));
            _cube3Transform.Rotation = new float3(0, 0, (-M.Pi/4 * TimeSinceStart));
            cube3Shader.Effect = SimpleMeshes.MakeShaderEffect (new float3(0.5f * M.Sin(2*TimeSinceStart)+0.5f, 0.09f, 0.3f), new float3(1,1,1), 4);

            // Render the scene on the current render context
            _sceneRenderer.Render(RC);

            Diagnostics.Log(Time.DeltaTime);
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
    }
}