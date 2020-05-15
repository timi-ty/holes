using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingingObstacle : MonoBehaviour
{
    private List<LineRenderer> chainRenderers = new List<LineRenderer>();
    LinkNode[] chainNodes;
    int chainResilience;
    public Sprite obstacleSprite;
    public Sprite hingeSprite;
    public float chainLength;
    public float chainWidth;
    public float obstacleSize;
    private const float linkLength = 0.25f;
    private bool isSpinning;
    private bool isCut;
    Transform obstacle;
    Transform hinge;
    void Start()
    {
        isCut = false;
        chainRenderers.Add(GetComponentInChildren<LineRenderer>());
        CreateObstacle(transform.position);
    }

    
    void Update()
    {
        if (isSpinning && !isCut) obstacle.rotation *= Quaternion.Euler(0, 0, 450 * Time.deltaTime);

        DrawChain();

        if(hinge.position.x < Boundary.visibleWorldMin.x - (Boundary.visibleWorldSize.x * 0.1f) && 
            obstacle.position.x < Boundary.visibleWorldMin.x - (Boundary.visibleWorldSize.x * 0.1f))
        {
            Destroy(gameObject);
        }
    }

    public void Modify(Vector2? hingePoint, Sprite obstacleSprite, float? chainLength, float? chainWidth, int? chainResilience,
        float? obstacleSize, Sprite hingeSprite, bool? isSpinning)
    {
        if (hingePoint.HasValue)
        {
            transform.position = hingePoint.Value;
        }
        if (chainLength.HasValue)
        {
            this.chainLength = chainLength.Value;
        }
        if (chainWidth.HasValue)
        {
            this.chainWidth = chainWidth.Value;
        }
        if (chainResilience.HasValue)
        {
            this.chainResilience = chainResilience.Value;
        }
        if (obstacleSize.HasValue)
        {
            this.obstacleSize = obstacleSize.Value;
        }
        if (isSpinning.HasValue)
        {
            this.isSpinning = isSpinning.Value;
        }
        if (obstacleSprite) this.obstacleSprite = obstacleSprite;
        if (hingeSprite) this.hingeSprite = hingeSprite;
    }

    public void CreateObstacle(Vector2 hingePoint)
    {
        transform.position = hingePoint;
        int linkCount = (int)(chainLength / linkLength);
        chainNodes = new LinkNode[linkCount + 1];
        chainRenderers[0].positionCount = chainNodes.Length;
        chainRenderers[0].startWidth = chainRenderers[0].endWidth = chainWidth;
        HingeJoint2D parentLinkJoint = null;
        for (int i = 0; i < linkCount; i++)
        {
            GameObject link = new GameObject();
            link.transform.SetParent(transform);
            link.name = "Link " + (i + 1);
            Rigidbody2D linkBody = link.AddComponent<Rigidbody2D>();
            linkBody.mass = 0.1f;
            HingeJoint2D linkJoint;
            linkJoint = link.AddComponent<HingeJoint2D>();
            chainNodes[i] = link.AddComponent<LinkNode>();
            chainNodes[i].resilience = chainResilience;
            if (i > 0)
            {
                BoxCollider2D collider = link.AddComponent<BoxCollider2D>();
                collider.size = new Vector2(chainWidth, linkLength);
                link.transform.position = hingePoint;
                parentLinkJoint.connectedBody = linkBody;
                if (i == 1) link.transform.position = hingePoint;
            }
            else
            {
                SpriteRenderer hingeRenderer = link.AddComponent<SpriteRenderer>();
                hingeRenderer.sprite = hingeSprite;
                hingeRenderer.sortingOrder = chainRenderers[0].sortingOrder + 1;
                linkBody.transform.position = hingePoint + hingeRenderer.bounds.extents.y * Vector2.up;
                linkBody.bodyType = RigidbodyType2D.Kinematic;
                hinge = linkBody.transform;
            }

            parentLinkJoint = linkJoint;
            hingePoint = new Vector2(hingePoint.x, hingePoint.y - linkLength);
        }

        GameObject obstacleObject = new GameObject();
        obstacle = obstacleObject.transform;
        obstacle.SetParent(transform);
        obstacle.name = "Obstacle";
        obstacle.tag = "Obstacle";
        SpriteRenderer obstacleRenderer = obstacleObject.AddComponent<SpriteRenderer>();
        obstacleRenderer.sprite = obstacleSprite;
        obstacleRenderer.sortingOrder = chainRenderers[0].sortingOrder + 1;
        float scale = obstacleSize / obstacleRenderer.bounds.size.y;
        obstacleObject.transform.localScale = Vector3.one * scale;
        obstacleObject.AddComponent<PolygonCollider2D>();
        chainNodes[linkCount] = obstacleObject.AddComponent<LinkNode>();
        Rigidbody2D obstacleBody = obstacleObject.AddComponent<Rigidbody2D>();
        obstacleBody.mass = 1f;
        obstacleObject.transform.position = (hingePoint - new Vector2(0, obstacleRenderer.bounds.extents.y));
        parentLinkJoint.connectedBody = obstacleBody;

        obstacleBody.AddForce(Vector2.left * 10, ForceMode2D.Impulse);
    }

    private void DrawChain()
    {
        List<List<Vector3>> nodePositions = new List<List<Vector3>>();
        nodePositions.Add(new List<Vector3>());
        for (int i = 0; i < chainNodes.Length; i++)
        {
            nodePositions[nodePositions.Count - 1].Add(chainNodes[i].transform.position);
            if (chainNodes[i].isBroken) nodePositions.Add(new List<Vector3>());
        }
        
        for (int i = 0; i < nodePositions.Count; i++)
        {
            chainRenderers[i].positionCount = nodePositions[i].Count;
            chainRenderers[i].SetPositions(nodePositions[i].ToArray());
        }
    }

    public void BreakChain()
    {
        LineRenderer segmentRenderer = Instantiate(chainRenderers[0], transform);
        chainRenderers.Add(segmentRenderer);
        isCut = true;
    }
}