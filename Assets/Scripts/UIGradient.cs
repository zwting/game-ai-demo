using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [AddComponentMenu("UI Extend/Effects/Gradient")]
    public class UIGradient : BaseMeshEffect
    {
        public enum EColorMode
        {
            Override,       // 颜色覆盖
            Additive,       // 颜色相加
            Multiply        // 颜色相乘
        }

        public enum EOrigin
        {
            Bottom,
            Top,
            Left,
            Right,
        }

        public enum EDirection
        {
            Vertical,
            Horizontal
        }

        [SerializeField]
        private EColorMode color_mode;                  // 颜色模式

        [UnityEngine.Serialization.FormerlySerializedAs("topColor"), SerializeField]
        private Color32 start_color = Color.white;

        [UnityEngine.Serialization.FormerlySerializedAs("bottomColor"), SerializeField]
        private Color32 end_color = Color.black;

        [SerializeField]
        private EOrigin origin = EOrigin.Bottom;        // 渐变起始方位

        [SerializeField]
        private EDirection direction;                  // 方向

        [SerializeField]
        private bool use_graphic_alpha = false;

        [SerializeField]
        private AnimationCurve curve = new AnimationCurve(new Keyframe(0, 0, 1, 1), new Keyframe(1, 1, 1, 1));

        [System.NonSerialized]
        private int graphi_type = 0;                  // 顶点类型(内部使用)

        /// <summary>
        /// 渐变的位置依据的尺寸,（0, 0）默认值是根据当前的mesh的尺寸
        /// </summary>
        [SerializeField]
        private Vector2 gradientSize = Vector2.zero;

        private Vector2 _size = Vector2.zero;

        public AnimationCurve Curve
        {
            get
            {
                return curve;
            }
            set
            {
                curve = value;
            }
        }

        protected UIGradient()
        { }

        public Color StartColor
        {
            get { return start_color; }
            set
            {
                start_color = value;
                if (graphic != null)
                {
                    graphic.SetVerticesDirty();
                }
            }
        }

        public Color EndColor
        {
            get { return end_color; }
            set
            {
                end_color = value;
                if (graphic != null)
                {
                    graphic.SetVerticesDirty();
                }
            }
        }

        public EColorMode ColorMode
        {
            get
            {
                return color_mode;
            }
            set
            {
                color_mode = value;
                if (graphic != null)
                {
                    graphic.SetVerticesDirty();
                }
            }
        }

        public EOrigin Origin
        {
            get { return origin; }
            set
            {
                origin = value;
                if (graphic != null)
                    graphic.SetVerticesDirty();
            }
        }

        public EDirection Direction
        {
            get
            {
                return direction;
            }
            set
            {
                direction = value;
                if (graphic != null)
                {
                    graphic.SetVerticesDirty();
                }
            }
        }

        public bool UseGraphicAlpha
        {
            get
            {
                return use_graphic_alpha;
            }
            set
            {
                use_graphic_alpha = value;
                if (graphic != null)
                {
                    graphic.SetVerticesDirty();
                }
            }
        }

        private int GraphicType
        {
            get
            {
                if (graphi_type == 0)
                {
                    if (graphic is Text)
                    {
                        graphi_type = 1;
                    }
                    else if (graphic is Image || graphic is RawImage)
                    {
                        graphi_type = 2;
                    }
                    else
                    {
                        graphi_type = -1;
                    }
                }
                return graphi_type;
            }
        }

        public override void ModifyMesh(VertexHelper _vh)
        {
            if (!IsActive())
            {
                return;
            }

            if (graphi_type == -1)
            {
                // 只接受指定类型
                return;
            }

            if (_vh.currentVertCount == 0)
            {
                return;
            }


            UIVertex _vertex = UIVertex.simpleVert;
            // 找出全局左下和右上的点位置
            _vh.PopulateUIVertex(ref _vertex, 0);
            Vector2 posLB = _vertex.position;
            _vh.PopulateUIVertex(ref _vertex, _vh.currentVertCount - 1);
            Vector2 posRT = _vertex.position;
            for (int i = 0; i < _vh.currentVertCount; ++i)
            {
                _vh.PopulateUIVertex(ref _vertex, i);
                if (_vertex.position.x < posLB.x)
                {
                    posLB.x = _vertex.position.x;
                }
                if (_vertex.position.y > posLB.y)
                {
                    posLB.y = _vertex.position.y;
                }
                if (_vertex.position.x > posRT.x)
                {
                    posRT.x = _vertex.position.x;
                }
                if (_vertex.position.y < posRT.y)
                {
                    posRT.y = _vertex.position.y;
                }
            }
            // 总高度和宽度
            float _total_height = gradientSize.y <= 0? posLB.y - posRT.y:gradientSize.y;
            float _total_width = gradientSize.x <= 0? posRT.x - posLB.x:gradientSize.x;
            Color _cur_color;
            Color32 _new_color = Color.white;
            for (int i = 0; i < _vh.currentVertCount; ++i)
            {
                _vh.PopulateUIVertex(ref _vertex, i);
                if (direction == EDirection.Vertical)
                {
                    _cur_color = Color.Lerp(start_color, end_color,
                        curve.Evaluate((posLB.y - _vertex.position.y) / _total_height));
                }
                else
                {
                    _cur_color = Color.Lerp(start_color, end_color,
                        curve.Evaluate((_vertex.position.x - posLB.x) / _total_width));
                }
                CalculateColor(ref _new_color, _vertex.color, _cur_color, color_mode);
                if (use_graphic_alpha)
                {
                    _new_color.a = (byte)((_vertex.color.a * _new_color.a) / 0xff);
                }
                _vertex.color = _new_color;
                _vh.SetUIVertex(_vertex, i);
            }
        }

        private void CalculateColor(ref Color32 _resultColor, Color32 _vertexColor, Color _newColor, EColorMode _colorMode)
        {
            if (_colorMode != EColorMode.Override)
            {
                if (_colorMode == EColorMode.Additive)
                {
                    _resultColor = (_vertexColor + _newColor);
                }
                if (_colorMode == EColorMode.Multiply)
                {
                    _resultColor = (_vertexColor * _newColor);
                }
            }
            _resultColor = _newColor;
        }
    }
}