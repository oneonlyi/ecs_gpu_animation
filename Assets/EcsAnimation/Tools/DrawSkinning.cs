using System;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Rendering;

namespace EcsAnimation.Tools
{
    public class DrawSkinning : IDisposable
    {
        private static readonly int TextureCoordinatesBuffer = Shader.PropertyToID("textureCoordinatesBuffer");
        private static readonly int ObjectToWorldBuffer = Shader.PropertyToID("objectToWorldBuffer");
        private static readonly int AnimationTexture0 = Shader.PropertyToID("_AnimationTexture0");
        private static readonly int AnimationTexture1 = Shader.PropertyToID("_AnimationTexture1");
        private static readonly int AnimationTexture2 = Shader.PropertyToID("_AnimationTexture2");

        private const int PreallocatedBufferSize = 1024;

        private readonly ComputeBuffer _argsBuffer;

        private readonly uint[] _indirectArgs = new uint[] { 0, 0, 0, 0, 0 };

        private ComputeBuffer _textureCoordinatesBuffer;
        private ComputeBuffer _objectToWorldBuffer;

        private readonly Mesh _mesh;
        private readonly Material _material;

        public unsafe DrawSkinning(Material srcMaterial, Mesh meshToDraw, AnimationTextures animTexture)
        {
            this._mesh = meshToDraw;
            this._material = new Material(srcMaterial);

            _argsBuffer =
                new ComputeBuffer(1, _indirectArgs.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
            _indirectArgs[0] = _mesh.GetIndexCount(0);
            _indirectArgs[1] = (uint)0;
            _argsBuffer.SetData(_indirectArgs);

            _objectToWorldBuffer = new ComputeBuffer(PreallocatedBufferSize, 16 * sizeof(float));
            _textureCoordinatesBuffer = new ComputeBuffer(PreallocatedBufferSize, 3 * sizeof(float));

            this._material.SetBuffer(TextureCoordinatesBuffer, _textureCoordinatesBuffer);
            this._material.SetBuffer(ObjectToWorldBuffer, _objectToWorldBuffer);
            this._material.SetTexture(AnimationTexture0, animTexture.Animation0);
            this._material.SetTexture(AnimationTexture1, animTexture.Animation1);
            this._material.SetTexture(AnimationTexture2, animTexture.Animation2);
        }

        public void Dispose()
        {
            UnityEngine.Object.DestroyImmediate(_material);

            _argsBuffer?.Dispose();
            _objectToWorldBuffer?.Dispose();
            _textureCoordinatesBuffer?.Dispose();
        }

        public void Draw(NativeArray<float3> textureCoordinates, NativeArray<float4x4> objectToWorld,
            ShadowCastingMode shadowCastingMode, bool receiveShadows)
        {
            // CHECK: Systems seem to be called when exiting playmode once things start getting destroyed, such as the mesh here.
            if (_mesh == null || _material == null)
                return;

            var count = textureCoordinates.Length;
            if (count == 0)
                return;

            if (count > _objectToWorldBuffer.count)
            {
                _objectToWorldBuffer.Dispose();
                _textureCoordinatesBuffer.Dispose();

                _objectToWorldBuffer = new ComputeBuffer(textureCoordinates.Length, 16 * sizeof(float));
                _textureCoordinatesBuffer = new ComputeBuffer(textureCoordinates.Length, 3 * sizeof(float));
            }

            this._material.SetBuffer(TextureCoordinatesBuffer, _textureCoordinatesBuffer);
            this._material.SetBuffer(ObjectToWorldBuffer, _objectToWorldBuffer);

            Profiler.BeginSample("Modify compute buffers");
            Profiler.BeginSample("Shader set data");

            _objectToWorldBuffer.SetData(objectToWorld, 0, 0, count);
            _textureCoordinatesBuffer.SetData(textureCoordinates, 0, 0, count);

            Profiler.EndSample();
            Profiler.EndSample();

            _indirectArgs[1] = (uint)count;
            _argsBuffer.SetData(_indirectArgs);

            Graphics.DrawMeshInstancedIndirect(_mesh, 0, _material, new Bounds(Vector3.zero, 1000000 * Vector3.one),
                _argsBuffer, 0, new MaterialPropertyBlock(), shadowCastingMode, receiveShadows);
        }
    }
}