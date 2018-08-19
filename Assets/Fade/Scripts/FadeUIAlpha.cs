/*
 The MIT License (MIT)

Copyright (c) 2013 yamamura tatsuhiko

Permission is hereby granted, free of charge, to any person obtaining a copy of
this software and associated documentation files (the "Software"), to deal in
the Software without restriction, including without limitation the rights to
use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
the Software, and to permit persons to whom the Software is furnished to do so,
subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

public class FadeUIAlpha : MonoBehaviour, IMaterialModifier, IMeshModifier
{
    public Graphic target
    {
        get
        {
            if(_target == null)
            {
                _target = this.GetComponent<Graphic>();
            }
            return _target;
        }
        set
        {
            _target = value;
            _material = _target.material;
            _target.SetMaterialDirty();
        }
    }

    Graphic _target;
    Material _material;

    [SerializeField]
    private Texture maskTexture = null;

    [SerializeField, Range(0, 1)]
    private float cutoutRange;

#if UNITY_EDITOR
    void OnValidate()
    {
        _material = target.material;
        target.SetMaterialDirty();
    }
#endif

    protected  void Start()
    {

    }

    public float Range
    {
        get
        {
            return cutoutRange;
        }
        set
        {
            cutoutRange = value;
            target.SetMaterialDirty();
        }
    }

    public void ModifyMesh(Mesh mesh)
    {
        using (var vh = new VertexHelper(mesh))
        {
            ModifyMesh(vh);
            vh.FillMesh(mesh);
        }
    }

    public void ModifyMesh(VertexHelper vh)
    {
        if (!enabled)
            return;

        Rect r = target.rectTransform.rect;
        Vector2 pivot = target.rectTransform.pivot;
        float w = r.width;
        float h = r.height;
        UIVertex vert = new UIVertex();
        for (int i = 0; i < vh.currentVertCount; i++)
        {
            vh.PopulateUIVertex(ref vert, i);
            vert.uv1 = new Vector2(vert.position.x / w + pivot.x, vert.position.y / h + pivot.y);
            vh.SetUIVertex(vert, i);
        }
    }

    public Material GetModifiedMaterial(Material baseMaterial)
    {
        Texture srcTexture = target.mainTexture;
        _material.SetTexture("_MaskTex", maskTexture);
        _material.SetTexture("_MainTex", srcTexture);
        //        _material.SetColor("_Color", color);
        _material.SetFloat("_Range", Range);
/*
        if (IsPremultipliedAlpha)
        {
            baseMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            baseMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);

            baseMaterial.EnableKeyword("PREMULTIPLIED_ALPHA");
        }

        else
        {
            baseMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            baseMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            baseMaterial.DisableKeyword("PREMULTIPLIED_ALPHA");
        }
*/
        return baseMaterial;
    }
}
