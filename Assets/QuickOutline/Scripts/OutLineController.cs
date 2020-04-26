/// <summary>
/// OutLineController
/// </summary>
/// <remarks>
///2019/9/9 星期一 下午 6:28:45: 创建. 周凯强 <br/>
/// Info<br/>
/// </remarks>
using UnityEngine;

namespace ARdEZ.Render
{
    public class OutLineController
    {
        private static OutLineController instance;

        public static OutLineController Instance 
        {
            get 
            {
                if (instance == null) 
                {
                    instance = new OutLineController();
                }
                return instance;
            }
        }

        private Material outlineMaskMaterial;
        private Material outlineFillMaterial;

        public Material OutlineMaskMaterial
        {
            get 
            {
                if (outlineMaskMaterial == null)
                {
                    lock (this)
                    {
                        outlineMaskMaterial = Resources.Load<Material>(@"Materials/OutlineMask");
                    }
                }
                return outlineMaskMaterial;
            }
        }

        public Material OutlineFillMaterial
        {
            get
            {
                if (outlineFillMaterial == null)
                {
                    lock (this)
                    {
                        outlineFillMaterial = Resources.Load<Material>(@"Materials/OutlineFill");
                    }
                }
                return outlineFillMaterial;
            }
        }

    }
}
