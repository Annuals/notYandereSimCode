﻿using System;
using UnityEngine.Rendering;

namespace UnityEngine.PostProcessing
{
	// Token: 0x020004C3 RID: 1219
	public sealed class FogComponent : PostProcessingComponentCommandBuffer<FogModel>
	{
		// Token: 0x1700049F RID: 1183
		// (get) Token: 0x06001ECB RID: 7883 RVA: 0x00181DF1 File Offset: 0x0017FFF1
		public override bool active
		{
			get
			{
				return base.model.enabled && this.context.isGBufferAvailable && RenderSettings.fog && !this.context.interrupted;
			}
		}

		// Token: 0x06001ECC RID: 7884 RVA: 0x00181E24 File Offset: 0x00180024
		public override string GetName()
		{
			return "Fog";
		}

		// Token: 0x06001ECD RID: 7885 RVA: 0x00022944 File Offset: 0x00020B44
		public override DepthTextureMode GetCameraFlags()
		{
			return DepthTextureMode.Depth;
		}

		// Token: 0x06001ECE RID: 7886 RVA: 0x00181E2B File Offset: 0x0018002B
		public override CameraEvent GetCameraEvent()
		{
			return CameraEvent.AfterImageEffectsOpaque;
		}

		// Token: 0x06001ECF RID: 7887 RVA: 0x00181E30 File Offset: 0x00180030
		public override void PopulateCommandBuffer(CommandBuffer cb)
		{
			FogModel.Settings settings = base.model.settings;
			Material material = this.context.materialFactory.Get("Hidden/Post FX/Fog");
			material.shaderKeywords = null;
			Color value = GraphicsUtils.isLinearColorSpace ? RenderSettings.fogColor.linear : RenderSettings.fogColor;
			material.SetColor(FogComponent.Uniforms._FogColor, value);
			material.SetFloat(FogComponent.Uniforms._Density, RenderSettings.fogDensity);
			material.SetFloat(FogComponent.Uniforms._Start, RenderSettings.fogStartDistance);
			material.SetFloat(FogComponent.Uniforms._End, RenderSettings.fogEndDistance);
			switch (RenderSettings.fogMode)
			{
			case FogMode.Linear:
				material.EnableKeyword("FOG_LINEAR");
				break;
			case FogMode.Exponential:
				material.EnableKeyword("FOG_EXP");
				break;
			case FogMode.ExponentialSquared:
				material.EnableKeyword("FOG_EXP2");
				break;
			}
			RenderTextureFormat format = this.context.isHdr ? RenderTextureFormat.DefaultHDR : RenderTextureFormat.Default;
			cb.GetTemporaryRT(FogComponent.Uniforms._TempRT, this.context.width, this.context.height, 24, FilterMode.Bilinear, format);
			cb.Blit(BuiltinRenderTextureType.CameraTarget, FogComponent.Uniforms._TempRT);
			cb.Blit(FogComponent.Uniforms._TempRT, BuiltinRenderTextureType.CameraTarget, material, settings.excludeSkybox ? 1 : 0);
			cb.ReleaseTemporaryRT(FogComponent.Uniforms._TempRT);
		}

		// Token: 0x04003D17 RID: 15639
		private const string k_ShaderString = "Hidden/Post FX/Fog";

		// Token: 0x020006D9 RID: 1753
		private static class Uniforms
		{
			// Token: 0x0400480A RID: 18442
			internal static readonly int _FogColor = Shader.PropertyToID("_FogColor");

			// Token: 0x0400480B RID: 18443
			internal static readonly int _Density = Shader.PropertyToID("_Density");

			// Token: 0x0400480C RID: 18444
			internal static readonly int _Start = Shader.PropertyToID("_Start");

			// Token: 0x0400480D RID: 18445
			internal static readonly int _End = Shader.PropertyToID("_End");

			// Token: 0x0400480E RID: 18446
			internal static readonly int _TempRT = Shader.PropertyToID("_TempRT");
		}
	}
}
