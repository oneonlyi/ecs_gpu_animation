using EcsAnimation.Components;
using Unity.Mathematics;
using UnityEngine;

namespace EcsAnimation.Tools
{
    public readonly struct AnimationClipBaker
    {
        private readonly float _textureOffset;
        private readonly float _textureRange;
        private readonly float _onePixelOffset;
        private readonly float _textureWidth;
        private readonly float _oneOverTextureWidth;
        private readonly float _oneOverPixelOffset;

        public readonly float AnimationLength;
        public readonly bool Looping;

        public AnimationClipBaker(AnimationTextures animTextures, AnimationTextureBaker.AnimationClipData clipData)
        {
            float onePixel = 1f / animTextures.Animation0.width;
            float start = (float)clipData.PixelStart / animTextures.Animation0.width;
            float end = (float)clipData.PixelEnd / animTextures.Animation0.width;

            _textureOffset = start;
            _textureRange = end - start;
            _onePixelOffset = onePixel;
            _textureWidth = animTextures.Animation0.width;
            _oneOverTextureWidth = 1.0F / _textureWidth;
            _oneOverPixelOffset = 1.0F / _onePixelOffset;

            AnimationLength = clipData.Clip.length;
            Looping = clipData.Clip.wrapMode == WrapMode.Loop;
        }

        public float3 ComputeCoordinate(float normalizedTime)
        {
            float texturePosition = normalizedTime * _textureRange + _textureOffset;
            float lowerPixelFloor = math.floor(texturePosition * _textureWidth);

            float lowerPixelCenter = lowerPixelFloor * _oneOverTextureWidth;
            float upperPixelCenter = lowerPixelCenter + _onePixelOffset;
            float lerpFactor = (texturePosition - lowerPixelCenter) * _oneOverPixelOffset;

            return new float3(lowerPixelCenter, upperPixelCenter, lerpFactor);
        }

        public float ComputeNormalizedTime(float time)
        {
            if (Looping)
                return Mathf.Repeat(time, AnimationLength) / AnimationLength;
            else
                return math.saturate(time / AnimationLength);
        }
    }
}