namespace Core.Coordinates
{
    /// <summary>
    /// 2D区域定义和检测
    /// </summary>
    public abstract class Area2D
    {
        /// <summary>
        /// 检测点是否在区域内
        /// </summary>
        public abstract bool Contains(Vector2D point);

        /// <summary>
        /// 检测与另一个区域是否相交
        /// </summary>
        public abstract bool Intersects(Area2D other);

        /// <summary>
        /// 获取区域的边界框
        /// </summary>
        public abstract Bounds2D GetBounds();
    }

    /// <summary>
    /// 2D边界框
    /// </summary>
    public struct Bounds2D
    {
        public Vector2D Min;
        public Vector2D Max;

        public Bounds2D(Vector2D min, Vector2D max)
        {
            this.Min = min;
            this.Max = max;
        }

        public Vector2D Center => (Min + Max) * 0.5f;
        public Vector2D Size => Max - Min;

        public bool Contains(Vector2D point)
        {
            return point.x >= Min.x && point.x <= Max.x &&
                   point.y >= Min.y && point.y <= Max.y;
        }

        public bool Intersects(Bounds2D other)
        {
            return Min.x <= other.Max.x && Max.x >= other.Min.x &&
                   Min.y <= other.Max.y && Max.y >= other.Min.y;
        }
    }

    /// <summary>
    /// 圆形区域
    /// </summary>
    public class CircleArea2D : Area2D
    {
        public Vector2D Center;
        public float Radius;

        public CircleArea2D(Vector2D center, float radius)
        {
            this.Center = center;
            this.Radius = radius;
        }

        public override bool Contains(Vector2D point)
        {
            return Vector2D.SqrDistance(Center, point) <= Radius * Radius;
        }

        public override bool Intersects(Area2D other)
        {
            if (other is CircleArea2D circle)
            {
                float distance = Vector2D.Distance(Center, circle.Center);
                return distance <= Radius + circle.Radius;
            }
            else if (other is RectArea2D rect)
            {
                return rect.Intersects(this);
            }
            
            return false;
        }

        public override Bounds2D GetBounds()
        {
            return new Bounds2D(
                Center - Vector2D.One * Radius,
                Center + Vector2D.One * Radius
            );
        }

        /// <summary>
        /// 检测与矩形区域的相交
        /// </summary>
        public bool IntersectsRect(RectArea2D rect)
        {
            Vector2D clampedPoint = MathUtils2D.Clamp(Center, rect.Min, rect.Max);
            return Vector2D.SqrDistance(Center, clampedPoint) <= Radius * Radius;
        }
    }

    /// <summary>
    /// 矩形区域
    /// </summary>
    public class RectArea2D : Area2D
    {
        public Vector2D Min;
        public Vector2D Max;

        public RectArea2D(Vector2D min, Vector2D max)
        {
            this.Min = min;
            this.Max = max;
        }

        public static RectArea2D CreateFromCenterAndSize(Vector2D center, Vector2D size)
        {
            Vector2D halfSize = size * 0.5f;
            return new RectArea2D(center - halfSize, center + halfSize);
        }

        public Vector2D Center => (Min + Max) * 0.5f;
        public Vector2D Size => Max - Min;

        public override bool Contains(Vector2D point)
        {
            return point.x >= Min.x && point.x <= Max.x &&
                   point.y >= Min.y && point.y <= Max.y;
        }

        public override bool Intersects(Area2D other)
        {
            if (other is RectArea2D rect)
            {
                return Min.x <= rect.Max.x && Max.x >= rect.Min.x &&
                       Min.y <= rect.Max.y && Max.y >= rect.Min.y;
            }
            else if (other is CircleArea2D circle)
            {
                return circle.IntersectsRect(this);
            }
            
            return false;
        }

        public override Bounds2D GetBounds()
        {
            return new Bounds2D(Min, Max);
        }
    }

    /// <summary>
    /// 多边形区域
    /// </summary>
    public class PolygonArea2D : Area2D
    {
        public Vector2D[] Vertices;

        public PolygonArea2D(Vector2D[] vertices)
        {
            this.Vertices = vertices;
        }

        public override bool Contains(Vector2D point)
        {
            if (Vertices == null || Vertices.Length < 3) return false;

            bool inside = false;
            int j = Vertices.Length - 1;

            for (int i = 0; i < Vertices.Length; i++)
            {
                Vector2D vi = Vertices[i];
                Vector2D vj = Vertices[j];

                if (((vi.y > point.y) != (vj.y > point.y)) &&
                    (point.x < (vj.x - vi.x) * (point.y - vi.y) / (vj.y - vi.y) + vi.x))
                {
                    inside = !inside;
                }

                j = i;
            }

            return inside;
        }

        public override bool Intersects(Area2D other)
        {
            // 简化实现：检查边界框相交
            Bounds2D bounds = GetBounds();
            Bounds2D otherBounds = other.GetBounds();
            
            if (!bounds.Intersects(otherBounds)) return false;

            // 更精确的检测可以在需要时实现
            return true;
        }

        public override Bounds2D GetBounds()
        {
            if (Vertices == null || Vertices.Length == 0)
                return new Bounds2D(Vector2D.Zero, Vector2D.Zero);

            Vector2D min = Vertices[0];
            Vector2D max = Vertices[0];

            for (int i = 1; i < Vertices.Length; i++)
            {
                if (Vertices[i].x < min.x) min.x = Vertices[i].x;
                if (Vertices[i].x > max.x) max.x = Vertices[i].x;
                if (Vertices[i].y < min.y) min.y = Vertices[i].y;
                if (Vertices[i].y > max.y) max.y = Vertices[i].y;
            }

            return new Bounds2D(min, max);
        }
    }
}
