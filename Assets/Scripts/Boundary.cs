using UnityEngine;

struct Boundary
{
    public float upBound, downBound, leftBound, rightBound;
    public static Vector2 visibleWorldCentre
    {
        get
        {
            return new Vector2(visibleWorldWidth/2, visibleWorldHeight/2) + visibleWorldMin;
        }
    }
    public static float visibleWorldHeight
    {
        get
        {
            return visibleWorldMax.y - visibleWorldMin.y;
        }
    }
    public static float visibleWorldWidth
    {
        get
        {
            return visibleWorldMax.x - visibleWorldMin.x;
        }
    }
    public static Vector2 visibleWorldExtents
    {
        get
        {
            return visibleWorldSize/2;
        }
    }
    public static Vector2 visibleWorldMax
    {
        get
        {
            return Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth, Camera.main.pixelHeight, 0)); ;
        }
    }
    public static Vector2 visibleWorldMin
    {
        get
        {
            return Camera.main.ScreenToWorldPoint(Vector3.zero);
        }
    }
    public static Vector2 visibleWorldSize
    {
        get
        {
            return new Vector2(visibleWorldWidth, visibleWorldHeight);
        }
    }
    public Boundary(float up, float down, float left, float right)
    {
        upBound = up;
        downBound = down;
        leftBound = left;
        rightBound = right;
    }


    // Summary:
    //  Determines playerMovementBoundaries based on screen size and playerSize
    public static Boundary ScreenBoundary(Vector2 playerSize)
    {
        Boundary screenBoundary = new Boundary();

        Bounds screenBounds = new Bounds(visibleWorldCentre, visibleWorldSize);

        screenBoundary.upBound = screenBounds.max.y - playerSize.y;
        screenBoundary.downBound = screenBounds.min.y + playerSize.y;
        screenBoundary.leftBound = screenBounds.min.x + playerSize.x;
        screenBoundary.rightBound = screenBounds.max.x - playerSize.x;

        return screenBoundary;
    }
}
