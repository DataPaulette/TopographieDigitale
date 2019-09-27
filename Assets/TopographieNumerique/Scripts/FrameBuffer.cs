using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameBuffer
{
	private RenderTexture[] frames;
	private int currentFrame = 0;

	public FrameBuffer (int count, int width, int height, int depth, RenderTextureFormat format, RenderTextureReadWrite readwrite) {
		frames = new RenderTexture[count];
		for (int i = 0; i < count; ++i) {
			frames[i] = new RenderTexture(width, height, depth, format, readwrite);
		}
	}

	public void Blit (Material material) {
		Graphics.Blit(frames[currentFrame], frames[(currentFrame+1)%frames.Length], material);
		currentFrame = (currentFrame+1)%frames.Length;
	}

	public RenderTexture GetTexture () {
		return frames[currentFrame];
	}
}
