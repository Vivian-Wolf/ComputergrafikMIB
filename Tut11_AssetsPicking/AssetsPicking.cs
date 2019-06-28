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
    public class AssetsPicking : RenderCanvas
    {
        private SceneContainer _scene;
        private SceneRenderer _sceneRenderer;
        private ScenePicker _scenePicker;
        private TransformComponent _leiterBack;
        private TransformComponent _leiterFront;
        private TransformComponent _DS;
        private ShaderEffectComponent _leiterBackShader;
        private ShaderEffectComponent _leiterFrontShader;
        private ShaderEffectComponent _DSShader;
        private PickResult _currentPick;
        private float3 _oldColor;
        private float3 _newColor;
        private float3 _oldColorLB;
        private float3 _oldColorLF;
        private float3 _oldColorDS;
        private float leiterBackRot;

        // Init is called on startup. 
        public override void Init()
        {
            // Set the clear color for the backbuffer to white (100% intensity in all color channels R, G, B, A).
            RC.ClearColor = new float4(0.2f, 0.3f, 1.0f, 1);

            _scene = AssetStorage.Get<SceneContainer>("fahrzeug.fus");

            // Create a scene renderer holding the scene above
            _sceneRenderer = new SceneRenderer(_scene);
            _scenePicker = new ScenePicker(_scene);
        }

        // RenderAFrame is called once a frame
        public override void RenderAFrame()
        {
            // _baseTransform.Rotation = new float3(0, M.MinAngle(TimeSinceStart), 0);

            //Rotation Drehsteuerung
            _DSShader = _scene.Children.FindNodes(node => node.Name == "Drehsteuerung")?.FirstOrDefault()?.GetComponent<ShaderEffectComponent>();
            _oldColorDS = (float3)_DSShader.Effect.GetEffectParam("DiffuseColor");
            if (_oldColorDS == _newColor)
            {

                _DS = _scene.Children.FindNodes(node => node.Name == "Drehsteuerung")?.FirstOrDefault()?.GetTransform();
                float DSRot = _DS.Rotation.y;
                DSRot += 0.4f * Keyboard.ADAxis * Time.DeltaTime;
                _DS.Rotation = new float3(0, DSRot, 0);

            }

            //Rotation hintere Leiter
            _leiterBackShader = _scene.Children.FindNodes(node => node.Name == "LeiterHinten")?.FirstOrDefault()?.GetComponent<ShaderEffectComponent>();
            _oldColorLB = (float3)_leiterBackShader.Effect.GetEffectParam("DiffuseColor");
            if (_oldColorLB == _newColor)
            {

                _leiterBack = _scene.Children.FindNodes(node => node.Name == "LeiterHinten")?.FirstOrDefault()?.GetTransform();
                leiterBackRot = _leiterBack.Rotation.x;
                leiterBackRot += 0.4f * Keyboard.WSAxis * Time.DeltaTime;

                if (leiterBackRot <= 0)
                {
                    _leiterBack.Rotation = new float3(0, 0, 0);
                }

                else if (leiterBackRot <= 60 * M.Pi / 180)
                {
                    _leiterBack.Rotation = new float3(leiterBackRot, 0, 0);
                }

                else if (leiterBackRot >= 60 * M.Pi / 180)
                {
                    leiterBackRot = 60 * M.Pi / 180;
                    _leiterBack.Rotation = new float3(leiterBackRot, 0, 0);
                }
            }

            //Rotation vordere Leiter
            _leiterFrontShader = _scene.Children.FindNodes(node => node.Name == "LeiterVorne")?.FirstOrDefault()?.GetComponent<ShaderEffectComponent>();
            _oldColorLF = (float3)_leiterFrontShader.Effect.GetEffectParam("DiffuseColor");
            if (_oldColorLF == _newColor)
            {
                _leiterFront = _scene.Children.FindNodes(node => node.Name == "LeiterVorne")?.FirstOrDefault()?.GetTransform();
                float leiterFrontRot = _leiterFront.Rotation.x;
                leiterFrontRot += 0.4f * Keyboard.WSAxis * Time.DeltaTime;

                if (leiterBackRot == 0)
                {
                    if (leiterFrontRot <= 0)
                    {
                        _leiterFront.Rotation = new float3(0, 0, 0);
                    }
                }

                else if (leiterFrontRot <= -30 * M.Pi / 180)
                {
                    leiterFrontRot = -30 * M.Pi / 180;
                    _leiterFront.Rotation = new float3(leiterFrontRot, 0, 0);
                }

                else if (leiterFrontRot <= 30 * M.Pi / 180)
                {
                    _leiterFront.Rotation = new float3(leiterFrontRot, 0, 0);
                }

                else if (leiterFrontRot >= 30 * M.Pi / 180)
                {
                    leiterFrontRot = 30 * M.Pi / 180;
                    _leiterFront.Rotation = new float3(leiterFrontRot, 0, 0);
                }
            }

            // Clear the backbuffer
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            // Setup the camera 
            RC.View = float4x4.CreateTranslation(0, 0, 20) * float4x4.CreateRotationX(-(float)Atan(10.0 / 40.0)) * float4x4.CreateRotationY(-(float)Acos(0));

            //Scene Picker
            if (Mouse.LeftButton)
            {
                float2 pickPosClip = Mouse.Position * new float2(2.0f / Width, -2.0f / Height) + new float2(-1, 1);
                _scenePicker.View = RC.View;
                _scenePicker.Projection = RC.Projection;

                List<PickResult> pickResults = _scenePicker.Pick(pickPosClip).ToList();
                PickResult newPick = null;
                if (pickResults.Count > 0)
                {
                    pickResults.Sort((a, b) => Sign(a.ClipPos.z - b.ClipPos.z));
                    newPick = pickResults[0];
                }

                if (newPick?.Node != _currentPick?.Node)
                {
                    if (_currentPick != null)
                    {
                        ShaderEffectComponent shaderEffectComponent = _currentPick.Node.GetComponent<ShaderEffectComponent>();
                        shaderEffectComponent.Effect.SetEffectParam("DiffuseColor", _oldColor);
                    }
                    if (newPick != null)
                    {
                        ShaderEffectComponent shaderEffectComponent = newPick.Node.GetComponent<ShaderEffectComponent>();
                        _oldColor = (float3)shaderEffectComponent.Effect.GetEffectParam("DiffuseColor");
                        shaderEffectComponent.Effect.SetEffectParam("DiffuseColor", new float3(1, 0.19f, 0.29f));
                        _newColor = (float3)shaderEffectComponent.Effect.GetEffectParam("DiffuseColor");
                    }
                    _currentPick = newPick;
                }
            }

            // Render the scene on the current render context
            _sceneRenderer.Render(RC);

            // Swap buffers: Show the contents of the backbuffer (containing the currently rendered frame) on the front buffer.
            Present();
        }


        // Is called when the window was resized
        public override void Resize()
        {
            // Set the new rendering area to the entire new windows size
            RC.Viewport(0, 0, Width, Height);

            // Create a new projection matrix generating undistorted images on the new aspect ratio.
            var aspectRatio = Width / (float)Height;

            // 0.25*PI Rad -> 45� Opening angle along the vertical direction. Horizontal opening angle is calculated based on the aspect ratio
            // Front clipping happens at 1 (Objects nearer than 1 world unit get clipped)
            // Back clipping happens at 2000 (Anything further away from the camera than 2000 world units gets clipped, polygons will be cut)
            var projection = float4x4.CreatePerspectiveFieldOfView(M.PiOver4, aspectRatio, 1, 20000);
            RC.Projection = projection;
        }
    }
}
