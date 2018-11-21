using UnityEngine;
using System.Collections;

public class PostEffect : MonoBehaviour
{
    [SerializeField]
    public Material effect;

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if(effect != null)
            Graphics.Blit(src, dest, effect);
        else
            Graphics.Blit(src, dest);
    }
}