using System.Numerics;
using Point = SixLabors.ImageSharp.Point;
using Rectangle = SixLabors.ImageSharp.Rectangle;

namespace Sharp.Platform.Video;

public static class RectangleHelpers
{
    public static Rectangle ToVeldridRectangle(this Rectangle rectangle)
        => new(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);

    public static Point ToPoint(this Rectangle rect) => new(rect.X, rect.Y);
    public static Vector2 ToVector2(this Point point) => new(point.X, point.Y);
}
