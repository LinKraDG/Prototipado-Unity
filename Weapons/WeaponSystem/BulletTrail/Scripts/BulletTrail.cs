using UnityEngine;
using DG.Tweening;

public class BulletTrail : MonoBehaviour
{
    [SerializeField]
    float duration = 0.25f;
    LineRenderer lineRenderer;

    const int numberOfPositions = 10;

    Tween tween;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    public void InitBullet(Vector3 startPosition, Vector3 endPosition)
    {
        Vector3[] positions = new Vector3[numberOfPositions];
        for (int i = 0; i < numberOfPositions; i++)
        {
            float t = (float)i / (float)numberOfPositions;
            positions[i] = Vector3.Lerp(startPosition, endPosition, t);
        }
        lineRenderer.SetPositions(positions);

        tween = DOTween.To(
            () => lineRenderer.widthMultiplier,
            (x) => lineRenderer.widthMultiplier = x,
            0f,
            duration);

        tween.OnComplete(() => Destroy(gameObject));
    }
}
